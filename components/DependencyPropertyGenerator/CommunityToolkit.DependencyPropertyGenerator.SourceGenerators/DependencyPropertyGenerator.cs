// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using CommunityToolkit.GeneratedDependencyProperty.Helpers;
using CommunityToolkit.GeneratedDependencyProperty.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A source generator creating implementations of dependency properties.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class DependencyPropertyGenerator : IIncrementalGenerator
{
    /// <summary>
    /// The name of generator to include in the generated code.
    /// </summary>
    internal const string GeneratorName = "CommunityToolkit.WinUI.DependencyPropertyGenerator";

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Generate the sources for the 'PrivateAssets="all"' mode
        context.RegisterPostInitializationOutput(Execute.GeneratePostInitializationSources);

        // Get the info on all dependency properties to generate
        IncrementalValuesProvider<DependencyPropertyInfo> propertyInfo =
            context.SyntaxProvider
            .ForAttributeWithMetadataName(
                WellKnownTypeNames.GeneratedDependencyPropertyAttribute,
                Execute.IsCandidateSyntaxValid,
                static (context, token) =>
                {
                    // We need C# 13, double check that it's the case
                    if (!context.SemanticModel.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp13))
                    {
                        return null;
                    }

                    bool isLocalCachingEnabled = Execute.IsLocalCachingEnabled(context.Attributes[0]);

                    // This generator requires C# preview to be used (due to the use of the 'field' keyword).
                    // The 'field' keyword is actually only used when local caching is enabled, so filter to that.
                    if (!isLocalCachingEnabled && !context.SemanticModel.Compilation.IsLanguageVersionPreview())
                    {
                        return null;
                    }

                    token.ThrowIfCancellationRequested();

                    // Ensure we do have a property
                    if (context.TargetSymbol is not IPropertySymbol propertySymbol)
                    {
                        return null;
                    }

                    // Do an initial filtering on the symbol as well
                    if (!Execute.IsCandidateSymbolValid(propertySymbol))
                    {
                        return null;
                    }

                    token.ThrowIfCancellationRequested();

                    // Get the accessibility values, if the property is valid
                    if (!Execute.TryGetAccessibilityModifiers(
                        node: (PropertyDeclarationSyntax)context.TargetNode,
                        propertySymbol: propertySymbol,
                        out Accessibility declaredAccessibility,
                        out Accessibility getterAccessibility,
                        out Accessibility setterAccessibility))
                    {
                        return default;
                    }

                    token.ThrowIfCancellationRequested();

                    string typeName = propertySymbol.Type.GetFullyQualifiedName();
                    string typeNameWithNullabilityAnnotations = propertySymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations();

                    token.ThrowIfCancellationRequested();

                    bool isRequired = Execute.IsRequiredProperty(propertySymbol);
                    bool isPropertyChangedCallbackImplemented = Execute.IsPropertyChangedCallbackImplemented(propertySymbol);
                    bool isSharedPropertyChangedCallbackImplemented = Execute.IsSharedPropertyChangedCallbackImplemented(propertySymbol);
                    bool isNet8OrGreater = !context.SemanticModel.Compilation.IsWindowsRuntimeApplication();

                    token.ThrowIfCancellationRequested();

                    // We're using IsValueType here and not IsReferenceType to also cover unconstrained type parameter cases.
                    // This will cover both reference types as well T when the constraints are not struct or unmanaged.
                    // If this is true, it means the field storage can potentially be in a null state (even if not annotated).
                    bool isReferenceTypeOrUnconstraindTypeParameter = !propertySymbol.Type.IsValueType;

                    // Also get the default value (this might be slightly expensive, so do it towards the end)
                    TypedConstantInfo defaultValue = Execute.GetDefaultValue(
                        context.Attributes[0],
                        propertySymbol,
                        context.SemanticModel,
                        token);

                    // The 'UnsetValue' can only be used when local caching is disabled
                    if (defaultValue is TypedConstantInfo.UnsetValue && isLocalCachingEnabled)
                    {
                        return null;
                    }

                    token.ThrowIfCancellationRequested();

                    // Finally, get the hierarchy too
                    HierarchyInfo hierarchyInfo = HierarchyInfo.From(propertySymbol.ContainingType);

                    token.ThrowIfCancellationRequested();

                    return new DependencyPropertyInfo(
                        Hierarchy: hierarchyInfo,
                        PropertyName: propertySymbol.Name,
                        DeclaredAccessibility: declaredAccessibility,
                        GetterAccessibility: getterAccessibility,
                        SetterAccessibility: setterAccessibility,
                        TypeName: typeName,
                        TypeNameWithNullabilityAnnotations: typeNameWithNullabilityAnnotations,
                        DefaultValue: defaultValue,
                        IsReferenceTypeOrUnconstraindTypeParameter: isReferenceTypeOrUnconstraindTypeParameter,
                        IsRequired: isRequired,
                        IsLocalCachingEnabled: isLocalCachingEnabled,
                        IsPropertyChangedCallbackImplemented: isPropertyChangedCallbackImplemented,
                        IsSharedPropertyChangedCallbackImplemented: isSharedPropertyChangedCallbackImplemented,
                        IsNet8OrGreater: isNet8OrGreater);
                })
            .WithTrackingName(WellKnownTrackingNames.Execute)
            .Where(static item => item is not null)!;

        // Split and group by containing type
        IncrementalValuesProvider<EquatableArray<DependencyPropertyInfo>> groupedPropertyInfo =
            propertyInfo
            .GroupBy(
                keySelector: static item => item.Hierarchy,
                elementSelector: static item => item,
                resultSelector: static item => item.Values)
            .WithTrackingName(WellKnownTrackingNames.Output);

        // Generate the source files, if any
        context.RegisterSourceOutput(groupedPropertyInfo, static (context, item) =>
        {
            using IndentedTextWriter writer = new();

            item[0].Hierarchy.WriteSyntax(
                state: item,
                writer: writer,
                baseTypes: [],
                memberCallbacks: [Execute.WritePropertyDeclarations]);

            if (Execute.RequiresAdditionalTypes(item))
            {
                writer.WriteLine();
                writer.WriteLine($"namespace {GeneratorName}");

                using (writer.WriteBlock())
                {
                    Execute.WriteAdditionalTypes(item, writer);
                }
            }

            context.AddSource($"{item[0].Hierarchy.FullyQualifiedMetadataName}.g.cs", writer.ToString());
        });
    }
}
