// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
        /// Placeholder for <see langword="null"/>.
        /// </summary>
        private static readonly TypedConstantInfo.Null NullInfo = new();

        /// <summary>
        /// Placeholder for the unset value of a given property type.
        /// </summary>
        private static readonly TypedConstantInfo.UnsetValue UnsetValueInfo = new();

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
        /// <returns>Whether <paramref name="propertySymbol"/> is a candidate property declaration.</returns>
        public static bool IsCandidateSymbolValid(IPropertySymbol propertySymbol)
        {
            // Ensure that the property declaration is a partial definition with no implementation
            if (propertySymbol is not { IsPartialDefinition: true, PartialImplementationPart: null })
            {
                return false;
            }

            // Also ignore all properties that have an invalid declaration
            if (propertySymbol.IsStatic || propertySymbol.ReturnsByRef || propertySymbol.ReturnsByRefReadonly || propertySymbol.Type.IsRefLikeType)
            {
                return false;
            }

            // Ensure we do have a valid containing
            if (propertySymbol.ContainingType is not { } typeSymbol)
            {
                return false;
            }

            // Ensure that the containing type derives from 'DependencyObject'
            if (!typeSymbol.InheritsFromFullyQualifiedMetadataName(WellKnownTypeNames.DependencyObject))
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
                    propertySymbol.Type.HasFullyQualifiedMetadataName(WellKnownTypeNames.DependencyPropertyChangedEventArgs);

                return !propertyTypeWouldCauseConflicts;
            }

            return true;
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
        /// Gets the default value to use to initialize the generated property, if explicitly specified.
        /// </summary>
        /// <param name="attributeData">The input <see cref="AttributeData"/> that triggered the annotation.</param>
        /// <param name="propertySymbol">The input <see cref="IPropertySymbol"/> instance.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/> for the current compilation.</param>
        /// <param name="token">The <see cref="CancellationToken"/> used to cancel the operation, if needed.</param>
        /// <returns>The default value to use to initialize the generated property.</returns>
        public static TypedConstantInfo GetDefaultValue(
            AttributeData attributeData,
            IPropertySymbol propertySymbol,
            SemanticModel semanticModel,
            CancellationToken token)
        {
            // First, check whether the default value is explicitly set or not
            if (attributeData.TryGetNamedArgument("DefaultValue", out TypedConstant defaultValue))
            {
                // If the explicit value is anything other than 'null', we can return it directly
                if (!defaultValue.IsNull)
                {
                    return TypedConstantInfo.Create(defaultValue);
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
                                    return UnsetValueInfo;
                                }
                            }
                        }
                    }
                }

                // Otherwise, the value has been explicitly set to 'null', so let's respect that
                return NullInfo;
            }

            // In all other cases, we'll automatically use the default value of the type in question.
            // First we need to special case non nullable values, as for those we need 'default'.
            if (propertySymbol.Type is { IsValueType: true } and not INamedTypeSymbol { IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T })
            {
                return new TypedConstantInfo.Default(propertySymbol.Type.GetFullyQualifiedName());
            }

            // For all other ones, we can just use the 'null' placeholder again
            return NullInfo;
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
        /// <returns>Whether the generated should register the property changed callback.</returns>
        public static bool IsPropertyChangedCallbackImplemented(IPropertySymbol propertySymbol)
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
                if (argsType.HasFullyQualifiedMetadataName(WellKnownTypeNames.DependencyPropertyChangedEventArgs))
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
        /// <returns>Whether the generated should register the shared property changed callback.</returns>
        public static bool IsSharedPropertyChangedCallbackImplemented(IPropertySymbol propertySymbol)
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
                if (argsType.HasFullyQualifiedMetadataName(WellKnownTypeNames.DependencyPropertyChangedEventArgs))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether an input property is required.
        /// </summary>
        /// <param name="propertySymbol">The input <see cref="IPropertySymbol"/> instance to process.</param>
        /// <returns>Whether <paramref name="propertySymbol"/> is required.</returns>
        public static bool IsRequiredProperty(IPropertySymbol propertySymbol)
        {
            return propertySymbol.IsRequired;
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
                return accessibility.GetExpression() switch
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
                    { DefaultValue: TypedConstantInfo.Null, IsPropertyChangedCallbackImplemented: false, IsSharedPropertyChangedCallbackImplemented: false }
                            => "null",
                    { DefaultValue: { } defaultValue, IsPropertyChangedCallbackImplemented: false, IsSharedPropertyChangedCallbackImplemented: false }
                        => $"new global::{WellKnownTypeNames.PropertyMetadata}({defaultValue})",

                    // Codegen for legacy UWP
                    { IsNet8OrGreater: false } => propertyInfo switch
                    {
                        { DefaultValue: { } defaultValue, IsPropertyChangedCallbackImplemented: true, IsSharedPropertyChangedCallbackImplemented: false }
                            => $"new global::{WellKnownTypeNames.PropertyMetadata}({defaultValue}, static (d, e) => (({typeQualifiedName})d).On{propertyInfo.PropertyName}PropertyChanged(e))",
                        { DefaultValue: { } defaultValue, IsPropertyChangedCallbackImplemented: false, IsSharedPropertyChangedCallbackImplemented: true }
                            => $"new global::{WellKnownTypeNames.PropertyMetadata}({defaultValue}, static (d, e) => (({typeQualifiedName})d).OnPropertyChanged(e))",
                        { DefaultValue: { } defaultValue, IsPropertyChangedCallbackImplemented: true, IsSharedPropertyChangedCallbackImplemented: true }
                            => $"new global::{WellKnownTypeNames.PropertyMetadata}({defaultValue}, static (d, e) => {{ (({typeQualifiedName})d).On{propertyInfo.PropertyName}PropertyChanged(e); (({typeQualifiedName})d).OnPropertyChanged(e); }})",
                        _ => throw new ArgumentException($"Invalid default value '{propertyInfo.DefaultValue}'."),
                    },

                    // Codegen for .NET 8 or greater
                    { DefaultValue: TypedConstantInfo.Null } and ({ IsPropertyChangedCallbackImplemented: true } or { IsSharedPropertyChangedCallbackImplemented: true })
                        => $"new global::{WellKnownTypeNames.PropertyMetadata}(null, global::{GeneratorName}.PropertyChangedCallbacks.{propertyInfo.PropertyName}())",
                    { DefaultValue: { } defaultValue } and ({ IsPropertyChangedCallbackImplemented: true } or { IsSharedPropertyChangedCallbackImplemented: true })
                        => $"new global::{WellKnownTypeNames.PropertyMetadata}({defaultValue}, global::{GeneratorName}.PropertyChangedCallbacks.{propertyInfo.PropertyName}())",
                    _ => throw new ArgumentException($"Invalid default value '{propertyInfo.DefaultValue}'."),
                };

                writer.WriteLine($$"""
                    /// <summary>
                    /// The backing <see cref="global::{{WellKnownTypeNames.DependencyProperty}}"/> instance for <see cref="{{propertyInfo.PropertyName}}"/>.
                    /// </summary>
                    """, isMultiline: true);
                writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                writer.WriteLine($$"""
                    public static readonly global::{{WellKnownTypeNames.DependencyProperty}} {{propertyInfo.PropertyName}}Property = global::{{WellKnownTypeNames.DependencyProperty}}.Register(
                        name: "{{propertyInfo.PropertyName}}",
                        propertyType: typeof({{propertyInfo.TypeName}}),
                        ownerType: typeof({{typeQualifiedName}}),
                        typeMetadata: {{typeMetadata}});
                    """, isMultiline: true);
                writer.WriteLine();
            }

            // After the properties, generate all partial property implementations at the top of the partial type declaration
            foreach (DependencyPropertyInfo propertyInfo in propertyInfos)
            {
                string oldValueTypeNameAsNullable = GetOldValueTypeNameAsNullable(propertyInfo);

                writer.WriteLine(skipIfPresent: true);
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteGeneratedAttributes(GeneratorName);
                writer.Write(GetExpressionWithTrailingSpace(propertyInfo.DeclaredAccessibility));
                writer.WriteIf(propertyInfo.IsRequired, "required ");
                writer.WriteLine($"partial {propertyInfo.TypeNameWithNullabilityAnnotations} {propertyInfo.PropertyName}");

                using (writer.WriteBlock())
                {
                    // We need very different codegen depending on whether local caching is enabled or not
                    if (propertyInfo.IsLocalCachingEnabled)
                    {
                        writer.WriteLine($$"""
                            get => field;
                            set
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
                        if (propertyInfo.DefaultValue is not (TypedConstantInfo.Null or TypedConstantInfo.Default))
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
                            get
                            {
                                object? __boxedValue = GetValue({{propertyInfo.PropertyName}}Property);

                                On{{propertyInfo.PropertyName}}Get(ref __boxedValue);

                                return __boxedValue;
                            }
                            set
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
                            get
                            {
                                object? __boxedValue = GetValue({{propertyInfo.PropertyName}}Property);

                                On{{propertyInfo.PropertyName}}Get(ref __boxedValue);

                                {{propertyInfo.TypeNameWithNullabilityAnnotations}} __unboxedValue = ({{propertyInfo.TypeNameWithNullabilityAnnotations}})__boxedValue;

                                On{{propertyInfo.PropertyName}}Get(ref __unboxedValue);

                                return __unboxedValue;
                            }
                            set
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
                    /// <remarks>This method is invoked by the <see cref="global::{WellKnownTypeNames.DependencyProperty}"/> infrastructure, after the value of <see cref="{propertyInfo.PropertyName}"/> is changed.</remarks>
                    """, isMultiline: true);
                writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
                writer.WriteLine($"partial void On{propertyInfo.PropertyName}PropertyChanged(global::{WellKnownTypeNames.DependencyPropertyChangedEventArgs} e);");
            }

            // OnPropertyChanged, for the shared property metadata callback
            writer.WriteLine();
            writer.WriteLine($"""
                /// <summary>Executes the logic for when any dependency property has just changed.</summary>
                /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
                /// <remarks>This method is invoked by the <see cref="global::{WellKnownTypeNames.DependencyProperty}"/> infrastructure, after the value of any dependency property has just changed.</remarks>
                """, isMultiline: true);
            writer.WriteGeneratedAttributes(GeneratorName, includeNonUserCodeAttributes: false);
            writer.WriteLine($"partial void OnPropertyChanged(global::{WellKnownTypeNames.DependencyPropertyChangedEventArgs} e);");
        }

        /// <summary>
        /// Checks whether additional types are required for the input set of properties.
        /// </summary>
        /// <param name="propertyInfos">The input set of declared dependency properties.</param>
        /// <returns>Whether additional types are required.</returns>
        public static bool RequiresAdditionalTypes(EquatableArray<DependencyPropertyInfo> propertyInfos)
        {
            // If the target is not .NET 8, we never need additional types (as '[UnsafeAccessor]' is not available)
            if (!propertyInfos[0].IsNet8OrGreater)
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
            writer.WriteLine("using global::System.Runtime.CompilerServices;");
            writer.WriteLine($"using global::{WellKnownTypeNames.XamlNamespace};");
            writer.WriteLine();
            writer.WriteLine($$"""
                /// <summary>
                /// Contains shared property changed callbacks for <see cref="{{propertyInfos[0].Hierarchy.Hierarchy[0].QualifiedName}}"/>.
                /// </summary>
                """, isMultiline: true);
            writer.WriteGeneratedAttributes(GeneratorName);
            writer.WriteLine("file static class PropertyChangedCallbacks");

            using (writer.WriteBlock())
            {
                string fullyQualifiedTypeName = propertyInfos[0].Hierarchy.GetFullyQualifiedTypeName();

                // Write the public accessors to use in property initializers
                writer.WriteLineSeparatedMembers(propertyInfos.AsSpan(), (propertyInfo, writer) =>
                {
                    writer.WriteLine($$"""
                        /// <summary>
                        /// Gets a <see cref="PropertyChangedCallback"/> value for <see cref="{{fullyQualifiedTypeName}}.{{propertyInfo.PropertyName}}Property"/>.
                        /// </summary>
                        /// <returns>The <see cref="PropertyChangedCallback"/> value with the right callbacks.</returns>
                        public static PropertyChangedCallback {{propertyInfo.PropertyName}}()
                        {
                            static void Invoke(object d, DependencyPropertyChangedEventArgs e)
                            {
                                {{fullyQualifiedTypeName}} __this = ({{fullyQualifiedTypeName}})d;

                        """, isMultiline: true);
                    writer.IncreaseIndent();
                    writer.IncreaseIndent();

                    // Per-property callback, if present
                    if (propertyInfo.IsPropertyChangedCallbackImplemented)
                    {
                        writer.WriteLine($"On{propertyInfo.PropertyName}PropertyChanged(__this, e);");
                    }

                    // Shared callback, if present
                    if (propertyInfo.IsSharedPropertyChangedCallbackImplemented)
                    {
                        writer.WriteLine("OnPropertyChanged(__this, e);");
                    }

                    // Close the method and return the 'Invoke' method as a delegate (just one allocation here)
                    writer.DecreaseIndent();
                    writer.DecreaseIndent();
                    writer.WriteLine("""
                            }

                            return new(Invoke);
                        }
                        """, isMultiline: true);
                });

                // Write the accessors for all WinRT-based callbacks (not the shared one)
                foreach (DependencyPropertyInfo propertyInfo in propertyInfos.Where(static property => property.IsPropertyChangedCallbackImplemented))
                {
                    writer.WriteLine();
                    writer.WriteLine($"""
                        /// <inheritdoc cref="{fullyQualifiedTypeName}.On{propertyInfo.PropertyName}PropertyChanged"/>
                        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "On{propertyInfo.PropertyName}PropertyChanged")]
                        private static extern void On{propertyInfo.PropertyName}PropertyChanged({fullyQualifiedTypeName} _, DependencyPropertyChangedEventArgs e);
                        """, isMultiline: true);
                }

                // Also emit one for the shared callback, if it's ever used
                if (propertyInfos.Any(static property => property.IsSharedPropertyChangedCallbackImplemented))
                {
                    writer.WriteLine();
                    writer.WriteLine($"""
                        /// <inheritdoc cref="{fullyQualifiedTypeName}.OnPropertyChanged"/>
                        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "OnPropertyChanged")]
                        private static extern void OnPropertyChanged({fullyQualifiedTypeName} _, DependencyPropertyChangedEventArgs e);
                        """, isMultiline: true);
                }
            }
        }
    }
}