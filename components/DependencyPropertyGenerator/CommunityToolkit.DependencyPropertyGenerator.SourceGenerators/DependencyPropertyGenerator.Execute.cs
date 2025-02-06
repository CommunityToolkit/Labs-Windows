// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using CommunityToolkit.GeneratedDependencyProperty.Helpers;
using CommunityToolkit.GeneratedDependencyProperty.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <inheritdoc/>
partial class DependencyPropertyGenerator
{
    /// <summary>
    /// A container for all the logic for <see cref="DependencyPropertyGenerator"/>.
    /// </summary>
    private static partial class Execute
    {
        /// <summary>
        /// Generates the sources for the embedded types, for <c>PrivateAssets="all"</c> scenarios.
        /// </summary>
        /// <param name="context">The input <see cref="IncrementalGeneratorPostInitializationContext"/> value to use to emit sources.</param>
        public static void GeneratePostInitializationSources(IncrementalGeneratorPostInitializationContext context)
        {
            void GenerateSource(string typeName)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                string fileName = $"{typeName}.g.cs";
                string sourceText;

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
                using (StreamReader reader = new(stream))
                {
                    sourceText = reader.ReadToEnd();
                }

                context.CancellationToken.ThrowIfCancellationRequested();

                string updatedSourceText = sourceText
                    .Replace("<GENERATOR_NAME>", GeneratorName)
                    .Replace("<ASSEMBLY_VERSION>", typeof(Execute).Assembly.GetName().Version.ToString());

                context.CancellationToken.ThrowIfCancellationRequested();

                context.AddSource(fileName, updatedSourceText);
            }

            GenerateSource("GeneratedDependencyProperty");
            GenerateSource("GeneratedDependencyPropertyAttribute");
        }

        /// <summary>
        /// Checks whether an input syntax node is a candidate property declaration for the generator.
        /// </summary>
        /// <param name="node">The input syntax node to check.</param>
        /// <param name="token">The <see cref="CancellationToken"/> used to cancel the operation, if needed.</param>
        /// <returns>Whether <paramref name="node"/> is a candidate property declaration.</returns>
        public static bool IsCandidateSyntaxValid(SyntaxNode node, CancellationToken token)
        {
            // Initial check that's identical to the analyzer
            if (!InvalidPropertySyntaxDeclarationAnalyzer.IsValidPropertyDeclaration(node))
            {
                return false;
            }

            // Make sure that all containing types are partial, otherwise declaring a partial property
            // would not be valid. We don't need to emit diagnostics here, the compiler will handle that.
            for (TypeDeclarationSyntax? parentNode = node.FirstAncestor<TypeDeclarationSyntax>();
                 parentNode is not null;
                 parentNode = parentNode.FirstAncestor<TypeDeclarationSyntax>())
            {
                if (!parentNode.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    return false;
                }
            }

            // Here we can also easily filter out ref-returning properties just using syntax
            if (((PropertyDeclarationSyntax)node).Type.IsKind(SyntaxKind.RefType))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether an input symbol is a candidate property declaration for the generator.
        /// </summary>
        /// <param name="propertySymbol">The input symbol to check.</param>
        /// <param name="useWindowsUIXaml">Whether to use the UWP XAML or WinUI 3 XAML namespaces.</param>
        /// <returns>Whether <paramref name="propertySymbol"/> is a candidate property declaration.</returns>
        public static bool IsCandidateSymbolValid(IPropertySymbol propertySymbol, bool useWindowsUIXaml)
        {
            // Ensure that the property declaration is a partial definition with no implementation
            if (propertySymbol is not { IsPartialDefinition: true, PartialImplementationPart: null })
            {
                return false;
            }

            // Also ignore all properties returning a byref-like value. We don't need to also
            // check for ref values here, as that's already validated by the syntax filter.
            if (propertySymbol.Type.IsRefLikeType)
            {
                return false;
            }

            // Pointer types are never allowed
            if (propertySymbol.Type.TypeKind is TypeKind.Pointer or TypeKind.FunctionPointer)
            {
                return false;
            }

            // Ensure we do have a valid containing
            if (propertySymbol.ContainingType is not { } typeSymbol)
            {
                return false;
            }

            // Ensure that the containing type derives from 'DependencyObject'
            if (!typeSymbol.InheritsFromFullyQualifiedMetadataName(WellKnownTypeNames.DependencyObject(useWindowsUIXaml)))
            {
                return false;
            }

            // If the generated property name is called "Property" and the type is either object or 'DependencyPropertyChangedEventArgs',
            // consider it invalid. This is needed because if such a property was generated, the partial 'On<PROPERTY_NAME>Changed'
            // methods would conflict.
            if (propertySymbol.Name == "Property")
            {
                bool propertyTypeWouldCauseConflicts =
                    propertySymbol.Type.SpecialType == SpecialType.System_Object ||
                    propertySymbol.Type.HasFullyQualifiedMetadataName(WellKnownTypeNames.DependencyPropertyChangedEventArgs(useWindowsUIXaml));

                return !propertyTypeWouldCauseConflicts;
            }

            return true;
        }

        /// <summary>
        /// Gathers all allowed property modifiers that should be forwarded to the generated property.
        /// </summary>
        /// <param name="node">The input <see cref="PropertyDeclarationSyntax"/> node.</param>
        /// <returns>The returned set of property modifiers, if any.</returns>
        public static ImmutableArray<SyntaxKind> GetPropertyModifiers(PropertyDeclarationSyntax node)
        {
            // We only allow a subset of all possible modifiers (aside from the accessibility modifiers)
            ReadOnlySpan<SyntaxKind> candidateKinds =
            [
                SyntaxKind.NewKeyword,
                SyntaxKind.VirtualKeyword,
                SyntaxKind.SealedKeyword,
                SyntaxKind.OverrideKeyword,
                SyntaxKind.RequiredKeyword
            ];

            using ImmutableArrayBuilder<SyntaxKind> builder = new();

            // Track all modifiers from the allowed set on the input property declaration
            foreach (SyntaxKind kind in candidateKinds)
            {
                if (node.Modifiers.Any(kind))
                {
                    builder.Add(kind);
                }
            }

            return builder.ToImmutable();
        }

        /// <summary>
        /// Tries to get the accessibility of the property and accessors, if possible.
        /// </summary>
        /// <param name="node">The input <see cref="PropertyDeclarationSyntax"/> node.</param>
        /// <param name="propertySymbol">The input <see cref="IPropertySymbol"/> instance.</param>
        /// <param name="declaredAccessibility">The accessibility of the property, if available.</param>
        /// <param name="getterAccessibility">The accessibility of the <see langword="get"/> accessor, if available.</param>
        /// <param name="setterAccessibility">The accessibility of the <see langword="set"/> accessor, if available.</param>
        /// <returns>Whether the property was valid and the accessibilities could be retrieved.</returns>
        public static bool TryGetAccessibilityModifiers(
            PropertyDeclarationSyntax node,
            IPropertySymbol propertySymbol,
            out Accessibility declaredAccessibility,
            out Accessibility getterAccessibility,
            out Accessibility setterAccessibility)
        {
            declaredAccessibility = Accessibility.NotApplicable;
            getterAccessibility = Accessibility.NotApplicable;
            setterAccessibility = Accessibility.NotApplicable;

            // Ensure that we have a getter and a setter, and that the setter is not init-only
            if (propertySymbol is not { GetMethod: { } getMethod, SetMethod: { IsInitOnly: false } setMethod })
            {
                return false;
            }

            // Track the property accessibility if explicitly set
            if (node.Modifiers.Count > 0)
            {
                declaredAccessibility = propertySymbol.DeclaredAccessibility;
            }

            // Track the accessors accessibility, if explicitly set
            foreach (AccessorDeclarationSyntax accessor in node.AccessorList?.Accessors ?? [])
            {
                if (accessor.Modifiers.Count == 0)
                {
                    continue;
                }

                switch (accessor.Kind())
                {
                    case SyntaxKind.GetAccessorDeclaration:
                        getterAccessibility = getMethod.DeclaredAccessibility;
                        break;
                    case SyntaxKind.SetAccessorDeclaration:
                        setterAccessibility = setMethod.DeclaredAccessibility;
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to get the accessibility of the property and accessors, if possible.
        /// </summary>
        /// <param name="propertySymbol">The input <see cref="IPropertySymbol"/> instance.</param>
        /// <param name="attributeData">The input <see cref="AttributeData"/> that triggered the annotation.</param>
        /// <param name="typeName">The type name for the generated property (without nullability annotations).</param>
        /// <param name="typeNameWithNullabilityAnnotations">The type name for the generated property, including nullability annotations.</param>
        /// <param name="metadataTypeName">The type name for the metadata declaration of the property, if explicitly set.</param>
        /// <param name="metadataTypeSymbol">The type symbol for the metadata declaration of the property, if explicitly set.</param>
        public static void GetPropertyTypes(
            IPropertySymbol propertySymbol,
            AttributeData attributeData,
            out string typeName,
            out string typeNameWithNullabilityAnnotations,
            out string? metadataTypeName,
            out ITypeSymbol? metadataTypeSymbol)
        {
            // These type names are always present and directly derived from the property type
            typeName = propertySymbol.Type.GetFullyQualifiedName();
            typeNameWithNullabilityAnnotations = propertySymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations();

            // Check if the user has specified an explicit property type to use in metadata
            if (attributeData.TryGetNamedArgument("PropertyType", out TypedConstant propertyType))
            {
                // Also make sure we do have a type. We don't need to perform additional validation here, since
                // the resulting code will always compile even if the type isn't actually compatible. We can do
                // that validation just from an analizer, and emit warnings if the requested type is incorrect.
                if (propertyType is { Kind: TypedConstantKind.Type, IsNull: false, Value: ITypeSymbol typeSymbol })
                {
                    metadataTypeName = typeSymbol.GetFullyQualifiedName();
                    metadataTypeSymbol = typeSymbol;

                    return;
                }
            }

            // By default, we'll just match the declared property type
            metadataTypeName = null;
            metadataTypeSymbol = null;
        }

        /// <summary>
        /// Gets the default value to use to initialize the generated property, if explicitly specified.
        /// </summary>
        /// <param name="attributeData">The input <see cref="AttributeData"/> that triggered the annotation.</param>
        /// <param name="propertySymbol">The input <see cref="IPropertySymbol"/> instance.</param>
        /// <param name="metadataTypeSymbol">The type symbol for the metadata declaration of the property, if explicitly set.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/> for the current compilation.</param>
        /// <param name="useWindowsUIXaml">Whether to use the UWP XAML or WinUI 3 XAML namespaces.</param>
        /// <param name="token">The <see cref="CancellationToken"/> used to cancel the operation, if needed.</param>
        /// <returns>The default value to use to initialize the generated property.</returns>
        public static DependencyPropertyDefaultValue GetDefaultValue(
            AttributeData attributeData,
            IPropertySymbol propertySymbol,
            ITypeSymbol? metadataTypeSymbol,
            SemanticModel semanticModel,
            bool useWindowsUIXaml,            
            CancellationToken token)
        {
            // First, check if we have a callback
            if (attributeData.TryGetNamedArgument("DefaultValueCallback", out TypedConstant defaultValueCallback))
            {
                // This must be a valid 'string' value
                if (defaultValueCallback is { Type.SpecialType: SpecialType.System_String, Value: string { Length: > 0 } methodName })
                {
                    // Check that we can find a potential candidate callback method
                    if (InvalidPropertyDefaultValueCallbackTypeAnalyzer.TryFindDefaultValueCallbackMethod(propertySymbol, methodName, out IMethodSymbol? methodSymbol))
                    {
                        // Validate the method has a valid signature as well
                        if (InvalidPropertyDefaultValueCallbackTypeAnalyzer.IsDefaultValueCallbackValid(propertySymbol, methodSymbol))
                        {
                            return new DependencyPropertyDefaultValue.Callback(methodName);
                        }
                    }
                }

                // Invalid callback, the analyzer will emit an error
                return DependencyPropertyDefaultValue.Null.Instance;
            }

            token.ThrowIfCancellationRequested();

            // Next, check whether the default value is explicitly set or not
            if (attributeData.TryGetNamedArgument("DefaultValue", out TypedConstant defaultValue))
            {
                // If the explicit value is anything other than 'null', we can return it directly
                if (!defaultValue.IsNull)
                {
                    return new DependencyPropertyDefaultValue.Constant(TypedConstantInfo.Create(defaultValue));
                }

                // If we do have a default value, we also want to check whether it's the special 'UnsetValue' placeholder.
                // To do so, we get the application syntax, find the argument, then get the operation and inspect it.
                if (attributeData.ApplicationSyntaxReference?.GetSyntax(token) is AttributeSyntax attributeSyntax)
                {
                    foreach (AttributeArgumentSyntax attributeArgumentSyntax in attributeSyntax.ArgumentList?.Arguments ?? [])
                    {
                        // Let's see whether the current argument is the one that set the 'DefaultValue' property
                        if (attributeArgumentSyntax.NameEquals?.Name.Identifier.Text is "DefaultValue")
                        {
                            IOperation? operation = semanticModel.GetOperation(attributeArgumentSyntax.Expression, token);

                            // Double check that it's a constant field reference (it could also be a literal of some kind, etc.)
                            if (operation is IFieldReferenceOperation { Field: { Name: "UnsetValue" } fieldSymbol })
                            {
                                // Last step: we want to validate that the reference is actually to the special placeholder
                                if (fieldSymbol.ContainingType!.HasFullyQualifiedMetadataName(WellKnownTypeNames.GeneratedDependencyProperty))
                                {
                                    return new DependencyPropertyDefaultValue.UnsetValue(useWindowsUIXaml);
                                }
                            }
                        }
                    }
                }

                // Otherwise, the value has been explicitly set to 'null', so let's respect that
                return DependencyPropertyDefaultValue.Null.Instance;
            }

            token.ThrowIfCancellationRequested();

            // In all other cases, we'll automatically use the default value of the type in question.
            // First we need to special case non nullable values, as for those we need 'default'.
            if (!propertySymbol.Type.IsDefaultValueNull())
            {
                // We need special logic to handle cases where the metadata type is different. For instance,
                // the XAML initialization won't work if the metadata type on a property is just 'object'.
                ITypeSymbol effectiveMetadataTypeSymbol = metadataTypeSymbol ?? propertySymbol.Type;

                // For non nullable types, we return 'default(T)', unless we can optimize for projected types
                return new DependencyPropertyDefaultValue.Default(
                    TypeName: propertySymbol.Type.GetFullyQualifiedName(),
                    IsProjectedType: effectiveMetadataTypeSymbol.IsWellKnownWinRTProjectedValueType(useWindowsUIXaml));
            }

            // If the property type is nullable, but the metadata type is not, and it's a projected WinRT value
            // type (meaning that XAML would initialize it to a value), we need to explicitly set it to 'null'.
            if (metadataTypeSymbol?.IsWellKnownWinRTProjectedValueType(useWindowsUIXaml) is true)
            {
                return DependencyPropertyDefaultValue.ExplicitNull.Instance;
            }

            // For all other ones, we can just use the 'null' placeholder again
            return DependencyPropertyDefaultValue.Null.Instance;
        }

        /// <summary>
        /// Checks whether the generated code has to register the property changed callback with WinRT.
        /// </summary>
        /// <param name="attributeData">The input <see cref="AttributeData"/> that triggered the annotation.</param>
        /// <returns>Whether the generated should register the property changed callback.</returns>
        public static bool IsLocalCachingEnabled(AttributeData attributeData)
        {
            return attributeData.GetNamedArgument("IsLocalCacheEnabled", defaultValue: false);
        }

        /// <summary>
        /// Checks whether the generated code has to register the property changed callback with WinRT.
        /// </summary>
        /// <param name="propertySymbol">The input <see cref="IPropertySymbol"/> instance to process.</param>
        /// <param name="useWindowsUIXaml">Whether to use the UWP XAML or WinUI 3 XAML namespaces.</param>
        /// <returns>Whether the generated should register the property changed callback.</returns>
        public static bool IsPropertyChangedCallbackImplemented(IPropertySymbol propertySymbol, bool useWindowsUIXaml)
        {
            // Check for any 'On<PROPERTY_NAME>Changed' methods
            foreach (ISymbol symbol in propertySymbol.ContainingType.GetMembers($"On{propertySymbol.Name}PropertyChanged"))
            {
                // We're looking for methods with one parameters, so filter on that first
                if (symbol is not IMethodSymbol { IsStatic: false, ReturnsVoid: true, Parameters: [{ Type: INamedTypeSymbol argsType }] })
                {
                    continue;
                }

                // There might be other property changed callback methods when field caching is enabled, or in other scenarios.
                // Because the callback method existing adds overhead (since we have to register it with WinRT), we want to
                // avoid false positives. To do that, we check that the parameter type is exactly the one we need.
                if (argsType.HasFullyQualifiedMetadataName(WellKnownTypeNames.DependencyPropertyChangedEventArgs(useWindowsUIXaml)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the generated code has to register the shared property changed callback with WinRT.
        /// </summary>
        /// <param name="propertySymbol">The input <see cref="IPropertySymbol"/> instance to process.</param>
        /// <param name="useWindowsUIXaml">Whether to use the UWP XAML or WinUI 3 XAML namespaces.</param>
        /// <returns>Whether the generated should register the shared property changed callback.</returns>
        public static bool IsSharedPropertyChangedCallbackImplemented(IPropertySymbol propertySymbol, bool useWindowsUIXaml)
        {
            // Check for any 'OnPropertyChanged' methods
            foreach (ISymbol symbol in propertySymbol.ContainingType.GetMembers("OnPropertyChanged"))
            {
                // Same filter as above
                if (symbol is not IMethodSymbol { IsStatic: false, ReturnsVoid: true, Parameters: [{ Type: INamedTypeSymbol argsType }] })
                {
                    continue;
                }

                // Also same actual check as above
                if (argsType.HasFullyQualifiedMetadataName(WellKnownTypeNames.DependencyPropertyChangedEventArgs(useWindowsUIXaml)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gathers all forwarded attributes for the generated property.
        /// </summary>
        ///<param name="node">The input <see cref="PropertyDeclarationSyntax"/> node.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current run.</param>
        /// <param name="forwardedAttributes">The collection of forwarded attributes to add new ones to.</param>
        /// <param name="diagnostics">The current collection of gathered diagnostics.</param>
        /// <param name="token">The cancellation token for the current operation.</param>
        public static void GetForwardedAttributes(
            PropertyDeclarationSyntax node,
            SemanticModel semanticModel,
            CancellationToken token,
            out ImmutableArray<AttributeInfo> staticFieldAttributes)
        {
            using ImmutableArrayBuilder<AttributeInfo> builder = new();

            // Gather explicit forwarded attributes info
            foreach (AttributeListSyntax attributeList in node.AttributeLists)
            {
                // Only look for the 'static' attribute target, which can be used to target the generated 'DependencyProperty' static field.
                // Roslyn will normally emit a 'CS0658' warning (invalid target), but that is automatically suppressed by a dedicated diagnostic
                // suppressor that recognizes uses of this target specifically to support '[GeneratedDependencyProperty]'. We can't use 'field'
                // as trigger, as that's used for the actual 'field' keyword, when local caching is enabled.
                if (attributeList.Target?.Identifier is not SyntaxToken(SyntaxKind.StaticKeyword))
                {
                    continue;
                }

                token.ThrowIfCancellationRequested();

                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    // Roslyn ignores attributes in an attribute list with an invalid target, so we can't get the 'AttributeData' as usual.
                    // To reconstruct all necessary attribute info to generate the serialized model, we use the following steps:
                    //   - We try to get the attribute symbol from the semantic model, for the current attribute syntax. In case this is not
                    //     available (in theory it shouldn't, but it can be), we try to get it from the candidate symbols list for the node.
                    //     If there are no candidates or more than one, we just issue a diagnostic and stop processing the current attribute.
                    //     The returned symbols might be method symbols (constructor attribute) so in that case we can get the declaring type.
                    //   - We then go over each attribute argument expression and get the operation for it. This will still be available even
                    //     though the rest of the attribute is not validated nor bound at all. From the operation we can still retrieve all
                    //     constant values to build the 'AttributeInfo' model. After all, attributes only support constant values, 'typeof(T)'
                    //     expressions, or arrays of either these two types, or of other arrays with the same rules, recursively.
                    //   - From the syntax, we can also determine the identifier names for named attribute arguments, if any.
                    //
                    // There is no need to validate anything here: the attribute will be forwarded as is, and then Roslyn will validate on the
                    // generated property. Users will get the same validation they'd have had directly over the field. The only drawback is the
                    // lack of IntelliSense when constructing attributes over the field, but this is the best we can do from this end anyway.
                    if (!semanticModel.GetSymbolInfo(attribute, token).TryGetAttributeTypeSymbol(out INamedTypeSymbol? attributeTypeSymbol))
                    {
                        continue;
                    }

                    IEnumerable<AttributeArgumentSyntax> attributeArguments = attribute.ArgumentList?.Arguments ?? [];

                    // Try to extract the forwarded attribute
                    if (!AttributeInfo.TryCreate(attributeTypeSymbol, semanticModel, attributeArguments, token, out AttributeInfo? attributeInfo))
                    {
                        continue;
                    }

                    builder.Add(attributeInfo);
                }
            }

            staticFieldAttributes = builder.ToImmutable();
        }

        /// <summary>
        /// Writes all implementations of partial dependency property declarations.
        /// </summary>
        /// <param name="propertyInfos">The input set of declared dependency properties.</param>
        /// <param name="writer">The <see cref="IndentedTextWriter"/> instance to write into.</param>
        public static void WritePropertyDeclarations(EquatableArray<DependencyPropertyInfo> propertyInfos, IndentedTextWriter writer)
        {
            // Helper to get the nullable type name for the initial property value
            static string GetOldValueTypeNameAsNullable(DependencyPropertyInfo propertyInfo)
            {
                // Prepare the nullable type for the previous property value. This is needed because if the type is a reference
                // type, the previous value might be null even if the property type is not nullable, as the first invocation would
                // happen when the property is first set to some value that is not null (but the backing field would still be so).
                // As a cheap way to check whether we need to add nullable, we can simply check whether the type name with nullability
                // annotations ends with a '?'. If it doesn't and the type is a reference type, we add it. Otherwise, we keep it.
                return propertyInfo.IsReferenceTypeOrUnconstraindTypeParameter switch
                {
                    true when !propertyInfo.TypeNameWithNullabilityAnnotations.EndsWith("?")
                        => $"{propertyInfo.TypeNameWithNullabilityAnnotations}?",
                    _ => propertyInfo.TypeNameWithNullabilityAnnotations
                };
            }

            // Helper to get the accessibility with a trailing space
            static string GetExpressionWithTrailingSpace(Accessibility accessibility)
            {
                return SyntaxFacts.GetText(accessibility) switch
                {
                    { Length: > 0 } expression => expression + " ",
                    _ => ""
                };
            }

            string typeQualifiedName = propertyInfos[0].Hierarchy.Hierarchy[0].QualifiedName;

            // First, generate all the actual dependency property fields
            foreach (DependencyPropertyInfo propertyInfo in propertyInfos)
            {
                string typeMetadata = propertyInfo switch
                {
                    // Shared codegen
                    { DefaultValue: DependencyPropertyDefaultValue.Null or DependencyPropertyDefaultValue.Default(_, true), IsPropertyChangedCallbackImplemented: false, IsSharedPropertyChangedCallbackImplemented: false }
                        => "null",
                    { DefaultValue: DependencyPropertyDefaultValue.Callback(string methodName), IsPropertyChangedCallbackImplemented: false, IsSharedPropertyChangedCallbackImplemented: false }
                        => $"""
                        global::{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}.Create(
                            createDefaultValueCallback: new {WellKnownTypeNames.CreateDefaultValueCallback(propertyInfo.UseWindowsUIXaml)}({methodName}))
                        """,
                    { DefaultValue: { } defaultValue, IsPropertyChangedCallbackImplemented: false, IsSharedPropertyChangedCallbackImplemented: false }
                        => $"new global::{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}({defaultValue})",

                    // Codegen for legacy UWP
                    { IsAdditionalTypesGenerationSupported: false } => propertyInfo switch
                    {
                        { DefaultValue: DependencyPropertyDefaultValue.Callback(string methodName), IsPropertyChangedCallbackImplemented: true, IsSharedPropertyChangedCallbackImplemented: false }
                            => $"""
                            global::{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}.Create(
                                 createDefaultValueCallback: new {WellKnownTypeNames.CreateDefaultValueCallback(propertyInfo.UseWindowsUIXaml)}({methodName}),
                                 propertyChangedCallback: static (d, e) => (({typeQualifiedName})d).On{propertyInfo.PropertyName}PropertyChanged(e))
                            """,
                        { DefaultValue: DependencyPropertyDefaultValue.Callback(string methodName), IsPropertyChangedCallbackImplemented: false, IsSharedPropertyChangedCallbackImplemented: true }
                            => $"""
                            global::{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}.Create(
                                createDefaultValueCallback: new {WellKnownTypeNames.CreateDefaultValueCallback(propertyInfo.UseWindowsUIXaml)}({methodName}),
                                propertyChangedCallback: static (d, e) => (({typeQualifiedName})d).OnPropertyChanged(e))
                            """,
                        { DefaultValue: DependencyPropertyDefaultValue.Callback(string methodName), IsPropertyChangedCallbackImplemented: true, IsSharedPropertyChangedCallbackImplemented: true }
                            => $$"""
                            global::{{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}}.Create(
                                createDefaultValueCallback: new {{WellKnownTypeNames.CreateDefaultValueCallback(propertyInfo.UseWindowsUIXaml)}}({{methodName}}),
                                propertyChangedCallback: static (d, e) => { (({{typeQualifiedName}})d).On{{propertyInfo.PropertyName}}PropertyChanged(e); (({{typeQualifiedName}})d).OnPropertyChanged(e); })
                            """,
                        { DefaultValue: { } defaultValue, IsPropertyChangedCallbackImplemented: true, IsSharedPropertyChangedCallbackImplemented: false }
                            => $"""
                            new global::{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}(
                                defaultValue: {defaultValue},
                                propertyChangedCallback: static (d, e) => (({typeQualifiedName})d).On{propertyInfo.PropertyName}PropertyChanged(e))
                            """,
                        { DefaultValue: { } defaultValue, IsPropertyChangedCallbackImplemented: false, IsSharedPropertyChangedCallbackImplemented: true }
                            => $"""
                            new global::{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}(
                                defaultValue: {defaultValue},
                                propertyChangedCallback: static (d, e) => (({typeQualifiedName})d).OnPropertyChanged(e))
                            """,
                        { DefaultValue: { } defaultValue, IsPropertyChangedCallbackImplemented: true, IsSharedPropertyChangedCallbackImplemented: true }
                            => $$"""
                            new global::{{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}}(
                                defaultValue: {{defaultValue}},
                                propertyChangedCallback: static (d, e) =>
                                {
                                    {{typeQualifiedName}} __this = ({{typeQualifiedName}})d;

                                    __this.On{{propertyInfo.PropertyName}}PropertyChanged(e);
                                    __this.OnPropertyChanged(e);
                                })
                            """,
                        _ => throw new ArgumentException($"Invalid default value '{propertyInfo.DefaultValue}'."),
                    },

                    // Codegen for .NET 8 or greater
                    { DefaultValue: DependencyPropertyDefaultValue.Null }
                        => $"""
                        new global::{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}(
                            defaultValue: null,
                            propertyChangedCallback: global::{GeneratorName}.PropertyChangedCallbacks.{propertyInfo.PropertyName}())
                        """,
                    { DefaultValue: DependencyPropertyDefaultValue.Callback(string methodName) }
                        => $"""
                        global::{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}.Create(
                            createDefaultValueCallback: new {WellKnownTypeNames.CreateDefaultValueCallback(propertyInfo.UseWindowsUIXaml)}({methodName}),
                            propertyChangedCallback: global::{GeneratorName}.PropertyChangedCallbacks.{propertyInfo.PropertyName}())
                        """,
                    { DefaultValue: { } defaultValue } and ({ IsPropertyChangedCallbackImplemented: true } or { IsSharedPropertyChangedCallbackImplemented: true })
                        => $"""
                        new global::{WellKnownTypeNames.PropertyMetadata(propertyInfo.UseWindowsUIXaml)}(
                            defaultValue: {defaultValue},
                            propertyChangedCallback: global::{GeneratorName}.PropertyChangedCallbacks.{propertyInfo.PropertyName}())
                        """,
                    _ => throw new ArgumentException($"Invalid default value '{propertyInfo.DefaultValue}'."),
                };

                writer.WriteLine($$"""
                    /// <summary>
                    /// The backing <see cref="global::{{WellKnownTypeNames.DependencyProperty(propertyInfo.UseWindowsUIXaml)}}"/> instance for <see cref="{{propertyInfo.PropertyName}}"/>.
                    /// </summary>
                    """, isMultiline: true);
                writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);

                // Write any forwarded attributes
                foreach (AttributeInfo attributeInfo in propertyInfo.StaticFieldAttributes)
                {
                    writer.WriteLine($"[{attributeInfo}]");
                }

                // Use the explicitly requested type name, if present, or the declared property type otherwise
                string propertyType = propertyInfo.MetadataTypeName ?? propertyInfo.TypeName;

                writer.Write($$"""
                    public static readonly global::{{WellKnownTypeNames.DependencyProperty(propertyInfo.UseWindowsUIXaml)}} {{propertyInfo.PropertyName}}Property = global::{{WellKnownTypeNames.DependencyProperty(propertyInfo.UseWindowsUIXaml)}}.Register(
                        name: "{{propertyInfo.PropertyName}}",
                        propertyType: typeof({{propertyType}}),
                        ownerType: typeof({{typeQualifiedName}}),
                        typeMetadata: 
                    """, isMultiline: true);
                writer.IncreaseIndent();
                writer.WriteLine($"{typeMetadata});", isMultiline: true);
                writer.DecreaseIndent();
                writer.WriteLine();
            }

            // After the properties, generate all partial property implementations at the top of the partial type declaration
            foreach (DependencyPropertyInfo propertyInfo in propertyInfos)
            {
                string oldValueTypeNameAsNullable = GetOldValueTypeNameAsNullable(propertyInfo);

                // Declare the property
                writer.WriteLine(skipIfPresent: true);
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteGeneratedAttributes(GeneratorName);
                writer.Write(GetExpressionWithTrailingSpace(propertyInfo.DeclaredAccessibility));

                // Add all gathered modifiers
                foreach (SyntaxKind modifier in propertyInfo.PropertyModifiers.AsImmutableArray().AsSyntaxKindArray())
                {
                    writer.Write($"{SyntaxFacts.GetText(modifier)} ");
                }

                // The 'partial' modifier always goes last, right before the property type and the property name.
                // We will never have the 'partial' modifier in the set of property modifiers processed above.
                writer.WriteLine($"partial {propertyInfo.TypeNameWithNullabilityAnnotations} {propertyInfo.PropertyName}");

                using (writer.WriteBlock())
                {
                    // We need very different codegen depending on whether local caching is enabled or not
                    if (propertyInfo.IsLocalCachingEnabled)
                    {
                        writer.WriteLine($$"""
                            {{GetExpressionWithTrailingSpace(propertyInfo.GetterAccessibility)}}get => field;
                            {{GetExpressionWithTrailingSpace(propertyInfo.SetterAccessibility)}}set
                            {
                                On{{propertyInfo.PropertyName}}Set(ref value);

                                if (global::System.Collections.Generic.EqualityComparer<{{oldValueTypeNameAsNullable}}>.Default.Equals(field, value))
                                {
                                    return;
                                }

                                {{oldValueTypeNameAsNullable}} __oldValue = field;

                                On{{propertyInfo.PropertyName}}Changing(value);
                                On{{propertyInfo.PropertyName}}Changing(__oldValue, value);

                                field = value;

                                object? __boxedValue = value;
                            """, isMultiline: true);
                        writer.WriteLineIf(propertyInfo.TypeName != "object", $"""

                                On{propertyInfo.PropertyName}Set(ref __boxedValue);
                            """, isMultiline: true);
                        writer.Write($$"""

                                SetValue({{propertyInfo.PropertyName}}Property, __boxedValue);

                                On{{propertyInfo.PropertyName}}Changed(value);
                                On{{propertyInfo.PropertyName}}Changed(__oldValue, value);
                            }
                            """, isMultiline: true);

                        // If the default value is not what the default field value would be, add an initializer
                        if (propertyInfo.DefaultValue is not (DependencyPropertyDefaultValue.Null or DependencyPropertyDefaultValue.Default or DependencyPropertyDefaultValue.Callback))
                        {
                            writer.Write($" = {propertyInfo.DefaultValue};");
                        }

                        // Always leave a newline after the end of the property declaration, in either case
                        writer.WriteLine();
                    }
                    else if (propertyInfo.TypeName == "object")
                    {
                        // If local caching is not enabled, we simply relay to the 'DependencyProperty' value. We cannot raise any methods
                        // to explicitly notify of changes that rely on the previous value. Retrieving it to conditionally invoke the methods
                        // would introduce a lot of overhead. If callers really do want to have a callback being invoked, they can implement
                        // the one wired up to the property metadata directly. We can still invoke the ones only using the new value, though.
                        writer.WriteLine($$"""
                            {{GetExpressionWithTrailingSpace(propertyInfo.GetterAccessibility)}}get
                            {
                                object? __boxedValue = GetValue({{propertyInfo.PropertyName}}Property);

                                On{{propertyInfo.PropertyName}}Get(ref __boxedValue);

                                return __boxedValue;
                            }
                            {{GetExpressionWithTrailingSpace(propertyInfo.SetterAccessibility)}}set
                            {
                                On{{propertyInfo.PropertyName}}Set(ref value);

                                SetValue({{propertyInfo.PropertyName}}Property, value);

                                On{{propertyInfo.PropertyName}}Changed(value);
                            }
                            """, isMultiline: true);
                    }
                    else
                    {
                        // Same as above but with the extra typed hook for both accessors
                        writer.WriteLine($$"""
                            {{GetExpressionWithTrailingSpace(propertyInfo.GetterAccessibility)}}get
                            {
                                object? __boxedValue = GetValue({{propertyInfo.PropertyName}}Property);

                                On{{propertyInfo.PropertyName}}Get(ref __boxedValue);

                                {{propertyInfo.TypeNameWithNullabilityAnnotations}} __unboxedValue = ({{propertyInfo.TypeNameWithNullabilityAnnotations}})__boxedValue;

                                On{{propertyInfo.PropertyName}}Get(ref __unboxedValue);

                                return __unboxedValue;
                            }
                            {{GetExpressionWithTrailingSpace(propertyInfo.SetterAccessibility)}}set
                            {
                                On{{propertyInfo.PropertyName}}Set(ref value);

                                object? __boxedValue = value;

                                On{{propertyInfo.PropertyName}}Set(ref __boxedValue);

                                SetValue({{propertyInfo.PropertyName}}Property, __boxedValue);

                                On{{propertyInfo.PropertyName}}Changed(value);
                            }
                            """, isMultiline: true);

                    }
                }
            }

            // Next, emit all partial method declarations at the bottom of the file
            foreach (DependencyPropertyInfo propertyInfo in propertyInfos)
            {
                string oldValueTypeNameAsNullable = GetOldValueTypeNameAsNullable(propertyInfo);
                string objectTypeNameWithNullabilityAnnotation = propertyInfo.TypeNameWithNullabilityAnnotations.EndsWith("?") ? "object?" : "object";

                if (!propertyInfo.IsLocalCachingEnabled)
                {
                    // On<PROPERTY_NAME>Get 'object' overload (only without local caching, as otherwise we just return the field value)
                    writer.WriteLine(skipIfPresent: true);
                    writer.WriteLine($"""
                        /// <summary>Executes the logic for when the <see langword="get"/> accessor <see cref="{propertyInfo.PropertyName}"/> is invoked</summary>
                        /// <param name="propertyValue">The raw property value that has been retrieved from <see cref="{propertyInfo.PropertyName}Property"/>.</param>
                        /// <remarks>This method is invoked on the boxed value retrieved via <see cref="GetValue"/> on <see cref="{propertyInfo.PropertyName}Property"/>.</remarks>
                        """, isMultiline: true);
                    writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                    writer.WriteLine($"partial void On{propertyInfo.PropertyName}Get(ref {objectTypeNameWithNullabilityAnnotation} propertyValue);");

                    // On<PROPERTY_NAME>Get typed overload
                    if (propertyInfo.TypeName != "object")
                    {
                        writer.WriteLine(skipIfPresent: true);
                        writer.WriteLine($"""
                            /// <summary>Executes the logic for when the <see langword="get"/> accessor <see cref="{propertyInfo.PropertyName}"/> is invoked</summary>
                            /// <param name="propertyValue">The unboxed property value that has been retrieved from <see cref="{propertyInfo.PropertyName}Property"/>.</param>
                            /// <remarks>This method is invoked on the unboxed value retrieved via <see cref="GetValue"/> on <see cref="{propertyInfo.PropertyName}Property"/>.</remarks>
                            """, isMultiline: true);
                        writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                        writer.WriteLine($"partial void On{propertyInfo.PropertyName}Get(ref {propertyInfo.TypeNameWithNullabilityAnnotations} propertyValue);");
                    }
                }

                // On<PROPERTY_NAME>Set 'object' overload
                writer.WriteLine(skipIfPresent: true);
                writer.WriteLine($"""
                    /// <summary>Executes the logic for when the <see langword="set"/> accessor <see cref="{propertyInfo.PropertyName}"/> is invoked</summary>
                    /// <param name="propertyValue">The boxed property value that has been produced before assigning to <see cref="{propertyInfo.PropertyName}Property"/>.</param>
                    /// <remarks>This method is invoked on the boxed value that is about to be passed to <see cref="SetValue"/> on <see cref="{propertyInfo.PropertyName}Property"/>.</remarks>
                    """, isMultiline: true);
                writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                writer.WriteLine($"partial void On{propertyInfo.PropertyName}Set(ref {objectTypeNameWithNullabilityAnnotation} propertyValue);");

                if (propertyInfo.TypeName != "object")
                {
                    // On<PROPERTY_NAME>Set typed overload
                    writer.WriteLine(skipIfPresent: true);
                    writer.WriteLine($"""
                        /// <summary>Executes the logic for when the <see langword="set"/> accessor <see cref="{propertyInfo.PropertyName}"/> is invoked</summary>
                        /// <param name="propertyValue">The property value that is being assigned to <see cref="{propertyInfo.PropertyName}"/>.</param>
                        /// <remarks>This method is invoked on the raw value being assigned to <see cref="{propertyInfo.PropertyName}"/>, before <see cref="SetValue"/> is used.</remarks>
                        """, isMultiline: true);
                    writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                    writer.WriteLine($"partial void On{propertyInfo.PropertyName}Set(ref {propertyInfo.TypeNameWithNullabilityAnnotations} propertyValue);");
                }

                // We can only generate the direct callback methods when using local caching (see notes above)
                if (propertyInfo.IsLocalCachingEnabled)
                {
                    // On<PROPERTY_NAME>Changing, only with new value
                    writer.WriteLine(skipIfPresent: true);
                    writer.WriteLine($"""
                        /// <summary>Executes the logic for when <see cref="{propertyInfo.PropertyName}"/> is changing.</summary>
                        /// <param name="value">The new property value being set.</param>
                        /// <remarks>This method is invoked right before the value of <see cref="{propertyInfo.PropertyName}"/> is changed.</remarks>
                        """, isMultiline: true);
                    writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                    writer.WriteLine($"partial void On{propertyInfo.PropertyName}Changing({propertyInfo.TypeNameWithNullabilityAnnotations} newValue);");

                    // On<PROPERTY_NAME>Changing, with both values
                    writer.WriteLine();
                    writer.WriteLine($"""
                        /// <summary>Executes the logic for when <see cref="{propertyInfo.PropertyName}"/> is changing.</summary>
                        /// <param name="oldValue">The previous property value that is being replaced.</param>
                        /// <param name="newValue">The new property value being set.</param>
                        /// <remarks>This method is invoked right before the value of <see cref="{propertyInfo.PropertyName}"/> is changed.</remarks>
                        """, isMultiline: true);
                    writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                    writer.WriteLine($"partial void On{propertyInfo.PropertyName}Changing({oldValueTypeNameAsNullable} oldValue, {propertyInfo.TypeNameWithNullabilityAnnotations} newValue);");
                }

                // On<PROPERTY_NAME>Changed, only with new value (this is always supported)
                writer.WriteLine(skipIfPresent: true);
                writer.WriteLine($"""
                    /// <summary>Executes the logic for when <see cref="{propertyInfo.PropertyName}"/> has just changed.</summary>
                    /// <param name="value">The new property value that has been set.</param>
                    /// <remarks>This method is invoked right after the value of <see cref="{propertyInfo.PropertyName}"/> is changed.</remarks>
                    """, isMultiline: true);
                writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                writer.WriteLine($"partial void On{propertyInfo.PropertyName}Changed({propertyInfo.TypeNameWithNullabilityAnnotations} newValue);");

                // On<PROPERTY_NAME>Changed, with both values (once again, this is only supported when local caching is enabled)
                if (propertyInfo.IsLocalCachingEnabled)
                {
                    writer.WriteLine();
                    writer.WriteLine($"""
                        /// <summary>Executes the logic for when <see cref="{propertyInfo.PropertyName}"/> has just changed.</summary>
                        /// <param name="oldValue">The previous property value that has been replaced.</param>
                        /// <param name="newValue">The new property value that has been set.</param>
                        /// <remarks>This method is invoked right after the value of <see cref="{propertyInfo.PropertyName}"/> is changed.</remarks>
                        """, isMultiline: true);
                    writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                    writer.WriteLine($"partial void On{propertyInfo.PropertyName}Changed({oldValueTypeNameAsNullable} oldValue, {propertyInfo.TypeNameWithNullabilityAnnotations} newValue);");
                }

                // On<PROPERTY_NAME>Changed, for the property metadata callback
                writer.WriteLine();
                writer.WriteLine($"""
                    /// <summary>Executes the logic for when <see cref="{propertyInfo.PropertyName}"/> has just changed.</summary>
                    /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
                    /// <remarks>This method is invoked by the <see cref="global::{WellKnownTypeNames.DependencyProperty(propertyInfo.UseWindowsUIXaml)}"/> infrastructure, after the value of <see cref="{propertyInfo.PropertyName}"/> is changed.</remarks>
                    """, isMultiline: true);
                writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                writer.WriteLine($"partial void On{propertyInfo.PropertyName}PropertyChanged(global::{WellKnownTypeNames.DependencyPropertyChangedEventArgs(propertyInfo.UseWindowsUIXaml)} e);");
            }

            // OnPropertyChanged, for the shared property metadata callback
            writer.WriteLine();
            writer.WriteLine($"""
                /// <summary>Executes the logic for when any dependency property has just changed.</summary>
                /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
                /// <remarks>This method is invoked by the <see cref="global::{WellKnownTypeNames.DependencyProperty(propertyInfos[0].UseWindowsUIXaml)}"/> infrastructure, after the value of any dependency property has just changed.</remarks>
                """, isMultiline: true);
            writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
            writer.WriteLine($"partial void OnPropertyChanged(global::{WellKnownTypeNames.DependencyPropertyChangedEventArgs(propertyInfos[0].UseWindowsUIXaml)} e);");
        }

        /// <summary>
        /// Checks whether additional types are required for the input set of properties.
        /// </summary>
        /// <param name="propertyInfos">The input set of declared dependency properties.</param>
        /// <returns>Whether additional types are required.</returns>
        public static bool RequiresAdditionalTypes(EquatableArray<DependencyPropertyInfo> propertyInfos)
        {
            // Check whether generating additional types is supported. This is a performance optimization for some
            // scenarios. We can only do this in some cases though. For instance, this is only supported on .NET 8
            // and above, as we need some additional types from the BCL (as '[UnsafeAccessor]'). Furthermore, if the
            // containing type is generic, we cannot generate these additional types either, as it's not really viable
            // to handle forwarding all type parameters to all generated accessors. In this case, we'll just use inline
            // lambda expressions. This results in marginally worse codegen, but it's better than not supporting this.
            if (!propertyInfos[0].IsAdditionalTypesGenerationSupported)
            {
                return false;
            }

            // We need the additional type holding the generated callbacks if at least one WinRT-based callback is present
            foreach (DependencyPropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.IsPropertyChangedCallbackImplemented || propertyInfo.IsSharedPropertyChangedCallbackImplemented)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Registers a callback to generate additional types, if needed.
        /// </summary>
        /// <param name="propertyInfos">The input set of declared dependency properties.</param>
        /// <param name="writer">The <see cref="IndentedTextWriter"/> instance to write into.</param>
        public static void WriteAdditionalTypes(EquatableArray<DependencyPropertyInfo> propertyInfos, IndentedTextWriter writer)
        {
            string fullyQualifiedTypeName = propertyInfos[0].Hierarchy.GetFullyQualifiedTypeName();

            // Define the 'PropertyChangedCallbacks' type
            writer.WriteLine("using global::System.Runtime.CompilerServices;");
            writer.WriteLine($"using global::{WellKnownTypeNames.XamlNamespace(propertyInfos[0].UseWindowsUIXaml)};");
            writer.WriteLine();
            writer.WriteLine($$"""
                /// <summary>
                /// Contains shared property changed callbacks for <see cref="{{propertyInfos[0].Hierarchy.Hierarchy[0].QualifiedName}}"/>.
                /// </summary>
                """, isMultiline: true);
            writer.WriteGeneratedAttributes(GeneratorName);
            writer.WriteLine("file sealed class PropertyChangedCallbacks");

            using (writer.WriteBlock())
            {
                // Shared dummy instance field (to make delegate invocations faster)
                writer.WriteLine("""
                    /// <summary>Shared <see cref="PropertyChangedCallbacks"/> instance, used to speedup delegate invocations (avoids the shuffle thunks).
                    private static readonly PropertyChangedCallbacks Instance = new();
                    """, isMultiline: true);

                int numberOfSharedPropertyCallbacks = propertyInfos.Count(static property => !property.IsPropertyChangedCallbackImplemented && property.IsSharedPropertyChangedCallbackImplemented);
                bool shouldCacheSharedPropertyChangedCallback = numberOfSharedPropertyCallbacks > 1;
                bool shouldGenerateSharedPropertyCallback = numberOfSharedPropertyCallbacks > 0;

                // If the shared callback should be cached, do that here
                if (shouldCacheSharedPropertyChangedCallback)
                {
                    writer.WriteLine();
                    writer.WriteLine("""
                        /// <summary>Shared <see cref="PropertyChangedCallback"/> instance, for all properties only using the shared callback.
                        private static readonly PropertyChangedCallback SharedPropertyChangedCallback = new(Instance.OnPropertyChanged);
                        """, isMultiline: true);
                }

                // Write the public accessors to use in property initializers
                foreach (DependencyPropertyInfo propertyInfo in propertyInfos)
                {
                    if (!propertyInfo.IsPropertyChangedCallbackImplemented && !propertyInfo.IsSharedPropertyChangedCallbackImplemented)
                    {
                        continue;
                    }

                    writer.WriteLine();
                    writer.WriteLine($$"""
                        /// <summary>
                        /// Gets a <see cref="PropertyChangedCallback"/> value for <see cref="{{fullyQualifiedTypeName}}.{{propertyInfo.PropertyName}}Property"/>.
                        /// </summary>
                        /// <returns>The <see cref="PropertyChangedCallback"/> value with the right callbacks.</returns>
                        public static PropertyChangedCallback {{propertyInfo.PropertyName}}()
                        {
                        """, isMultiline: true);
                    writer.IncreaseIndent();

                    // There are 3 possible scenarios to handle:
                    //   1) The property uses a dedicated property changed callback. In this case we always need a dedicated stub.
                    //   2) The property uses the shared callback only, and there's more than one property like this. Reuse the instance.
                    //   3) This is the only property using the shared callback only. In that case, create a new delegate over it.
                    if (propertyInfo.IsPropertyChangedCallbackImplemented)
                    {
                        writer.WriteLine($"return new(Instance.On{propertyInfo.PropertyName}PropertyChanged);");
                    }
                    else if (shouldCacheSharedPropertyChangedCallback)
                    {
                        writer.WriteLine("return SharedPropertyChangedCallback;");
                    }
                    else
                    {
                        writer.WriteLine("return new(Instance.OnPropertyChanged);");
                    }

                    writer.DecreaseIndent();
                    writer.WriteLine("}");
                }

                // Write the private combined 
                foreach (DependencyPropertyInfo propertyInfo in propertyInfos)
                {
                    if (!propertyInfo.IsPropertyChangedCallbackImplemented)
                    {
                        continue;
                    }

                    writer.WriteLine();
                    writer.WriteLine($$"""
                        /// <inheritdoc cref="cref="{{fullyQualifiedTypeName}}.OnPropertyChanged""/>
                        private void On{{propertyInfo.PropertyName}}PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                        {
                            {{fullyQualifiedTypeName}} __this = ({{fullyQualifiedTypeName}})d;

                            PropertyChangedUnsafeAccessors.On{{propertyInfo.PropertyName}}PropertyChanged(__this, e);
                        """, isMultiline: true);                    

                    // Shared callback, if needed
                    if (propertyInfo.IsSharedPropertyChangedCallbackImplemented)
                    {
                        writer.IncreaseIndent();
                        writer.WriteLine("PropertyChangedUnsafeAccessors.OnPropertyChanged(__this, e);");
                        writer.DecreaseIndent();
                    }

                    writer.WriteLine("}");
                }

                // If we need to generate the shared callback, let's also generate its target method
                if (shouldGenerateSharedPropertyCallback)
                {
                    writer.WriteLine();
                    writer.WriteLine($$"""
                        /// <inheritdoc cref="cref="{{fullyQualifiedTypeName}}.OnPropertyChanged""/>
                        private void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                        {
                            {{fullyQualifiedTypeName}} __this = ({{fullyQualifiedTypeName}})d;

                            PropertyChangedUnsafeAccessors.OnPropertyChanged(__this, e);
                        }
                        """, isMultiline: true);
                }
            }

            // Define the 'PropertyChangedAccessors' type
            writer.WriteLine();
            writer.WriteLine($"""
                /// <summary>
                /// Contains all unsafe accessors for <see cref="{propertyInfos[0].Hierarchy.Hierarchy[0].QualifiedName}"/>.
                /// </summary>
                """, isMultiline: true);
            writer.WriteGeneratedAttributes(GeneratorName);
            writer.WriteLine("file sealed class PropertyChangedUnsafeAccessors");

            using (writer.WriteBlock())
            {
                // Write the accessors for all WinRT-based callbacks (not the shared one)
                foreach (DependencyPropertyInfo propertyInfo in propertyInfos.Where(static property => property.IsPropertyChangedCallbackImplemented))
                {
                    writer.WriteLine(skipIfPresent: true);
                    writer.WriteLine($"""
                        /// <inheritdoc cref="{fullyQualifiedTypeName}.On{propertyInfo.PropertyName}PropertyChanged"/>
                        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "On{propertyInfo.PropertyName}PropertyChanged")]
                        public static extern void On{propertyInfo.PropertyName}PropertyChanged({fullyQualifiedTypeName} _, DependencyPropertyChangedEventArgs e);
                        """, isMultiline: true);
                }

                // Also emit one for the shared callback, if it's ever used
                if (propertyInfos.Any(static property => property.IsSharedPropertyChangedCallbackImplemented))
                {
                    writer.WriteLine(skipIfPresent: true);
                    writer.WriteLine($"""
                        /// <inheritdoc cref="{fullyQualifiedTypeName}.OnPropertyChanged"/>
                        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "OnPropertyChanged")]
                        public static extern void OnPropertyChanged({fullyQualifiedTypeName} _, DependencyPropertyChangedEventArgs e);
                        """, isMultiline: true);
                }
            }
        }
    }
}
