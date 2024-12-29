// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using CommunityToolkit.GeneratedDependencyProperty.Helpers;
using CommunityToolkit.GeneratedDependencyProperty.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A diagnostic analyzer that generates a suggestion whenever <c>[GeneratedDependencytProperty]</c> should be used over a manual property.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseGeneratedDependencyPropertyOnManualPropertyAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The number of pooled flags per stack (ie. how many properties we expect on average per type).
    /// </summary>
    private const int NumberOfPooledFlagsPerStack = 20;

    /// <summary>
    /// Shared pool for <see cref="Dictionary{TKey, TValue}"/> instances for properties.
    /// </summary>
    [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1008", Justification = "This is a pool of (empty) dictionaries, it is not actually storing compilation data.")]
    private static readonly ObjectPool<Dictionary<IPropertySymbol, PropertyFlags>> PropertyMapPool = new(static () => new Dictionary<IPropertySymbol, PropertyFlags>(SymbolEqualityComparer.Default));

    /// <summary>
    /// Shared pool for <see cref="Dictionary{TKey, TValue}"/> instances for fields, for dependency properties.
    /// </summary>
    [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1008", Justification = "This is a pool of (empty) dictionaries, it is not actually storing compilation data.")]
    private static readonly ObjectPool<Dictionary<IFieldSymbol, FieldFlags>> FieldMapPool = new(static () => new Dictionary<IFieldSymbol, FieldFlags>(SymbolEqualityComparer.Default));

    /// <summary>
    /// Shared pool for <see cref="Stack{T}"/>-s of property flags, one per type being processed.
    /// </summary>
    private static readonly ObjectPool<Stack<PropertyFlags>> PropertyFlagsStackPool = new(CreateFlagsStack<PropertyFlags>);

    /// <summary>
    /// Shared pool for <see cref="Stack{T}"/>-s of field flags, one per type being processed.
    /// </summary>
    private static readonly ObjectPool<Stack<FieldFlags>> FieldFlagsStackPool = new(CreateFlagsStack<FieldFlags>);

    /// <summary>
    /// The property name for the serialized property value, if present.
    /// </summary>
    public const string DefaultValuePropertyName = "DefaultValue";

    /// <summary>
    /// The property name for the fully qualified metadata name of the default value, if present.
    /// </summary>
    public const string DefaultValueTypeReferenceIdPropertyName = "DefaultValueTypeReferenceId";

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [UseGeneratedDependencyPropertyForManualProperty];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the XAML mode to use
            bool useWindowsUIXaml = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.GetMSBuildBooleanPropertyValue(WellKnownPropertyNames.DependencyPropertyGeneratorUseWindowsUIXaml);

            // Get the '[GeneratedDependencyProperty]' symbol (there might be multiples, due to embedded mode)
            ImmutableArray<INamedTypeSymbol> generatedDependencyPropertyAttributeSymbols = context.Compilation.GetTypesByMetadataName(WellKnownTypeNames.GeneratedDependencyPropertyAttribute);

            // Get the 'DependencyObject' symbol
            if (context.Compilation.GetTypeByMetadataName(WellKnownTypeNames.DependencyObject(useWindowsUIXaml)) is not { } dependencyObjectSymbol)
            {
                return;
            }

            // Get the symbol for the 'GetValue' method as well
            if (dependencyObjectSymbol.GetMembers("GetValue") is not [IMethodSymbol { Parameters: [_], ReturnType.SpecialType: SpecialType.System_Object } getValueSymbol])
            {
                return;
            }

            // Get the symbol for the 'SetValue' method as well
            if (dependencyObjectSymbol.GetMembers("SetValue") is not [IMethodSymbol { Parameters: [_, _], ReturnsVoid: true } setValueSymbol])
            {
                return;
            }

            // We also need the 'DependencyProperty' and 'PropertyMetadata' symbols
            if (context.Compilation.GetTypeByMetadataName(WellKnownTypeNames.DependencyProperty(useWindowsUIXaml)) is not { } dependencyPropertySymbol ||
                context.Compilation.GetTypeByMetadataName(WellKnownTypeNames.PropertyMetadata(useWindowsUIXaml)) is not { } propertyMetadataSymbol)
            {
                return;
            }

            // Next, we need to get the 'DependencyProperty.Register' symbol as well, to validate initializers.
            // No need to validate this more, as there's only a single overload defined on this type.
            if (dependencyPropertySymbol.GetMembers("Register") is not [IMethodSymbol dependencyPropertyRegisterSymbol])
            {
                return;
            }

            context.RegisterSymbolStartAction(context =>
            {
                // We only care about types that could derive from 'DependencyObject'
                if (context.Symbol is not INamedTypeSymbol { IsStatic: false, IsReferenceType: true, BaseType.SpecialType: not SpecialType.System_Object } typeSymbol)
                {
                    return;
                }

                // If the type does not derive from 'DependencyObject', ignore it
                if (!typeSymbol.InheritsFromType(dependencyObjectSymbol))
                {
                    return;
                }

                Dictionary<IPropertySymbol, PropertyFlags> propertyMap = PropertyMapPool.Allocate();
                Dictionary<IFieldSymbol, FieldFlags> fieldMap = FieldMapPool.Allocate();
                Stack<PropertyFlags> propertyFlagsStack = PropertyFlagsStackPool.Allocate();
                Stack<FieldFlags> fieldFlagsStack = FieldFlagsStackPool.Allocate();

                // Crawl all members to discover properties that might be of interest
                foreach (ISymbol memberSymbol in typeSymbol.GetMembers())
                {
                    // First, look for properties that might be valid candidates for conversion
                    if (memberSymbol is IPropertySymbol
                        {
                            IsStatic: false,
                            IsPartialDefinition: false,
                            PartialDefinitionPart: null,
                            PartialImplementationPart: null,
                            ReturnsByRef: false,
                            ReturnsByRefReadonly: false,
                            Type.IsRefLikeType: false,
                            GetMethod: not null,
                            SetMethod.IsInitOnly: false
                        } propertySymbol)
                    {
                        // We can safely ignore properties that already have '[GeneratedDependencyProperty]'
                        if (propertySymbol.HasAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols))
                        {
                            continue;
                        }

                        // Take a new flags object from the stack or create a new one otherwise
                        PropertyFlags flags = propertyFlagsStack.Count > 0
                            ? propertyFlagsStack.Pop()
                            : new();

                        // Track the property for later
                        propertyMap.Add(propertySymbol, flags);
                    }
                    else if (memberSymbol is IFieldSymbol
                            {
                                DeclaredAccessibility: Accessibility.Public,
                                IsStatic: true,
                                IsReadOnly: true,
                                IsFixedSizeBuffer: false,
                                IsRequired: false,
                                Type.IsReferenceType: true,
                                IsVolatile: false
                            } fieldSymbol)
                    {
                        // We only care about fields that are 'DependencyProperty'
                        if (!SymbolEqualityComparer.Default.Equals(dependencyPropertySymbol, fieldSymbol.Type))
                        {
                            continue;
                        }

                        // Same as above for the field flags
                        fieldMap.Add(
                            key: fieldSymbol,
                            value: fieldFlagsStack.Count > 0 ? fieldFlagsStack.Pop() : new FieldFlags());
                    }
                }

                // We want to process both accessors, where we specifically need both the syntax
                // and their semantic model to verify what they're doing. We can use a code callback.
                context.RegisterOperationBlockAction(context =>
                {
                    // Handle a 'get' accessor (for any property)
                    void HandleGetAccessor(IPropertySymbol propertySymbol, PropertyFlags propertyFlags)
                    {
                        // We expect a top-level block operation, that immediately returns an expression
                        if (context.OperationBlocks is not [IBlockOperation { Operations: [IReturnOperation returnOperation] }])
                        {
                            return;
                        }

                        // Next, check whether we have an invocation operation. This is the case when the getter is just
                        // calling 'GetValue' and returning it directly, which only works when the property type allows
                        // direct conversion. Generally speaking this happens when properties are just of type 'object'.
                        if (returnOperation is not { ReturnedValue: IInvocationOperation invocationOperation })
                        {
                            // Alternatively, we expect a conversion (a cast)
                            if (returnOperation is not { ReturnedValue: IConversionOperation { IsTryCast: false } conversionOperation })
                            {
                                return;
                            }

                            // Check the conversion is actually valid
                            if (!SymbolEqualityComparer.Default.Equals(propertySymbol.Type, conversionOperation.Type))
                            {
                                return;
                            }

                            // Try to extract the invocation from the conversion
                            if (conversionOperation.Operand is not IInvocationOperation operandInvocationOperation)
                            {
                                return;
                            }

                            invocationOperation = operandInvocationOperation;
                        }

                        // Now that we have the invocation, first filter the target method
                        if (invocationOperation.TargetMethod is not { Name: "GetValue", IsGenericMethod: false, IsStatic: false } methodSymbol)
                        {
                            return;
                        }

                        // Next, make sure we're actually calling 'DependencyObject.GetValue'
                        if (!SymbolEqualityComparer.Default.Equals(methodSymbol, getValueSymbol))
                        {
                            return;
                        }

                        // Make sure we have one argument, which will be the dependency property
                        if (invocationOperation.Arguments is not [{ } dependencyPropertyArgument])
                        {
                            return;
                        }

                        // This argument should be a field reference (we'll fully validate it later)
                        if (dependencyPropertyArgument.Value is not IFieldReferenceOperation { Field: { } fieldSymbol })
                        {
                            return;
                        }

                        // The field must follow the expected naming pattern. We can check this just here in the getter.
                        // If this is valid, the whole property will be skipped anyway, so no need to do it twice.
                        if (fieldSymbol.Name != $"{propertySymbol.Name}Property")
                        {
                            return;
                        }

                        // We can in the meantime at least verify that we do have the field in the set
                        if (!fieldMap.ContainsKey(fieldSymbol))
                        {
                            return;
                        }

                        // If the property is also valid, then the accessor is valid
                        propertyFlags.GetValueTargetField = fieldSymbol;
                    }

                    // Handle a 'set' accessor (for any property)
                    void HandleSetAccessor(IPropertySymbol propertySymbol, PropertyFlags propertyFlags)
                    {
                        // We expect a top-level block operation, that immediately performs an invocation
                        if (context.OperationBlocks is not [IBlockOperation { Operations: [IExpressionStatementOperation { Operation: IInvocationOperation invocationOperation }] }])
                        {
                            return;
                        }

                        // Brief filtering of the target method
                        if (invocationOperation.TargetMethod is not { Name: "SetValue", IsGenericMethod: false, IsStatic: false } methodSymbol)
                        {
                            return;
                        }

                        // First, check that we're calling 'DependencyObject.SetValue'
                        if (!SymbolEqualityComparer.Default.Equals(methodSymbol, setValueSymbol))
                        {
                            return;
                        }

                        // We matched the method, now let's validate the arguments
                        if (invocationOperation.Arguments is not [{ } dependencyPropertyArgument, { } valueArgument])
                        {
                            return;
                        }

                        // Like for the getter, the first argument should be a field reference...
                        if (dependencyPropertyArgument.Value is not IFieldReferenceOperation { Field: { } fieldSymbol })
                        {
                            return;
                        }

                        // ...and the field should be in the set (not fully guaranteed to be valid yet, but partially)
                        if (!fieldMap.ContainsKey(fieldSymbol))
                        {
                            return;
                        }

                        // The value is just the 'value' keyword
                        if (valueArgument.Value is not IParameterReferenceOperation { Syntax: IdentifierNameSyntax { Identifier.Text: "value" } })
                        {
                            // If this is not the case, check whether the parameter reference was wrapped in a conversion (boxing)
                            if (valueArgument.Value is not IConversionOperation { IsTryCast: false, Type.SpecialType: SpecialType.System_Object } conversionOperation)
                            {
                                return;
                            }

                            // Check for 'value' again
                            if (conversionOperation.Operand is not IParameterReferenceOperation { Syntax: IdentifierNameSyntax { Identifier.Text: "value" } })
                            {
                                return;
                            }
                        }

                        // The 'set' accessor is valid if the field is valid, like above
                        propertyFlags.SetValueTargetField = fieldSymbol;
                    }

                    // Only look for method symbols, for property accessors
                    if (context.OwningSymbol is not IMethodSymbol { MethodKind: MethodKind.PropertyGet or MethodKind.PropertySet, AssociatedSymbol: IPropertySymbol propertySymbol })
                    {
                        return;
                    }

                    // If so, check that we are actually processing one of the properties we care about
                    if (!propertyMap.TryGetValue(propertySymbol, out PropertyFlags? propertyFlags))
                    {
                        return;
                    }

                    // Handle the 'get' and 'set' logic
                    if (SymbolEqualityComparer.Default.Equals(propertySymbol.GetMethod, context.OwningSymbol))
                    {
                        HandleGetAccessor(propertySymbol, propertyFlags);
                    }
                    else if (SymbolEqualityComparer.Default.Equals(propertySymbol.SetMethod, context.OwningSymbol))
                    {
                        HandleSetAccessor(propertySymbol, propertyFlags);
                    }
                });

                // Same as above, but targeting field initializers (we can't just inspect field symbols)
                context.RegisterOperationAction(context =>
                {
                    // Only look for field symbols, which we should always get here, and an invocation in the initializer block (the 'DependencyProperty.Register' call)
                    if (context.Operation is not IFieldInitializerOperation { InitializedFields: [{ } fieldSymbol], Value: IInvocationOperation invocationOperation })
                    {
                        return;
                    }

                    // Check that the field is one of the ones we expect to encounter
                    if (!fieldMap.TryGetValue(fieldSymbol, out FieldFlags? fieldFlags))
                    {
                        return;
                    }

                    // Validate that we are calling 'DependencyProperty.Register'
                    if (!SymbolEqualityComparer.Default.Equals(invocationOperation.TargetMethod, dependencyPropertyRegisterSymbol))
                    {
                        return;
                    }

                    // Next, make sure we have the arguments we expect
                    if (invocationOperation.Arguments is not [{ } nameArgument, { } propertyTypeArgument, { } ownerTypeArgument, { } propertyMetadataArgument])
                    {
                        return;
                    }

                    // We cannot validate the property name from here yet, but let's check it's a constant, and save it for later
                    if (nameArgument.Value.ConstantValue is not { HasValue: true, Value: string propertyName })
                    {
                        return;
                    }

                    // Extract the property type, we can validate it later
                    if (propertyTypeArgument.Value is not ITypeOfOperation { TypeOperand: { } propertyTypeSymbol })
                    {
                        return;
                    }

                    // Extract the owning type, we can validate it right now
                    if (ownerTypeArgument.Value is not ITypeOfOperation { TypeOperand: { } owningTypeSymbol })
                    {
                        return;
                    }

                    // The owning type always has to be exactly the same as the containing type
                    if (!SymbolEqualityComparer.Default.Equals(owningTypeSymbol, typeSymbol))
                    {
                        return;
                    }

                    // First, check if the metadata is 'null' (simplest case)
                    if (propertyMetadataArgument.Value.ConstantValue is { HasValue: true, Value: null })
                    {
                        // Here we need to special case non nullable value types that are not well known WinRT projected types.
                        // In this case, we cannot rely on XAML calling their default constructor. Rather, we need to preserve
                        // the explicit 'null' value that users had in their code. The analyzer will then warn on these cases
                        if (!propertyTypeSymbol.IsDefaultValueNull() &&
                            !propertyTypeSymbol.IsWellKnownWinRTProjectedValueType(useWindowsUIXaml))
                        {
                            fieldFlags.DefaultValue = TypedConstantInfo.Null.Instance;
                        }
                    }
                    else
                    {
                        // Next, check if the argument is 'new PropertyMetadata(...)' with the default value for the property type
                        if (propertyMetadataArgument.Value is not IObjectCreationOperation { Arguments: [{ } defaultValueArgument, ..] } objectCreationOperation)
                        {
                            return;
                        }

                        // Make sure the object being created is actually 'PropertyMetadata'
                        if (!SymbolEqualityComparer.Default.Equals(objectCreationOperation.Type, propertyMetadataSymbol))
                        {
                            return;
                        }

                        // If we have a second argument, a 'null' literal is the only supported value for it
                        if (objectCreationOperation.Arguments is not ([_] or [_, { Value.ConstantValue: { HasValue: true, Value: null } }]))
                        {
                            return;
                        }

                        // The argument should be a conversion operation (boxing)
                        if (defaultValueArgument.Value is not IConversionOperation { IsTryCast: false, Type.SpecialType: SpecialType.System_Object } conversionOperation)
                        {
                            return;
                        }

                        bool isNullableValueType = propertyTypeSymbol is INamedTypeSymbol { IsValueType: true, IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T };

                        // Check whether the value is a default constant value. If it is, then the property is valid (no explicit value).
                        // We need to special case nullable value types, as the default value for the underlying type is not the actual default.
                        if (!conversionOperation.Operand.IsConstantValueDefault() || isNullableValueType)
                        {
                            // The value is just 'null' with no type, special case this one and skip the other checks below
                            if (conversionOperation.Operand is { Type: null, ConstantValue: { HasValue: true, Value: null } })
                            {
                                // This is only allowed for reference or nullable types. This 'null' is redundant, but support it nonetheless.
                                // It's not that uncommon for especially legacy codebases to have this kind of pattern in dependency properties.
                                if (!propertyTypeSymbol.IsReferenceType && !isNullableValueType)
                                {
                                    return;
                                }
                            }
                            else if (TypedConstantInfo.TryCreate(conversionOperation.Operand, out fieldFlags.DefaultValue))
                            {
                                // We have found a valid constant. If it's an enum type, we have a couple special cases to handle.
                                if (conversionOperation.Operand.Type is { TypeKind: TypeKind.Enum } operandType)
                                {
                                    // As an optimization, we check whether the constant was the value
                                    // of some projected built-in WinRT enum type (ie. not any user-defined enum type). If that is the
                                    // case, the XAML infrastructure can default that values automatically, meaning we can skip the
                                    // overhead of instantiating a 'PropertyMetadata' instance in code, and marshalling default value.
                                    if (operandType.IsContainedInNamespace(WellKnownTypeNames.XamlNamespace(useWindowsUIXaml)))
                                    {
                                        // Before actually enabling the optimization, validate that the default value is actually
                                        // the same as the default value of the enum (ie. the value of its first declared field).
                                        if (operandType.TryGetDefaultValueForEnumType(out object? defaultValue) &&
                                            conversionOperation.Operand.ConstantValue.Value == defaultValue)
                                        {
                                            fieldFlags.DefaultValue = null;
                                        }
                                    }
                                    else if (operandType.ContainingType is not null)
                                    {
                                        // If the enum is nested, we need to also track the type symbol specifically, as the fully qualified
                                        // expression we'd be using otherwise would not be the same as the metadata name, and resolving the
                                        // enum type symbol from that in the code fixer would fail. This is an edge case, but it can happen.
                                        fieldFlags.DefaultValueTypeReferenceId = DocumentationCommentId.CreateReferenceId(operandType);
                                    }
                                }
                            }
                            else if (conversionOperation.Operand is IFieldReferenceOperation { Field: { ContainingType.SpecialType: SpecialType.System_String, Name: "Empty" } })
                            {
                                // Special handling of the 'string.Empty' field. This is not a constant value, but we can still treat it as a constant, by just
                                // pretending this were the empty string literal instead. This way we can still support the property and convert to an attribute.
                                fieldFlags.DefaultValue = TypedConstantInfo.Primitive.String.Empty;
                            }
                            else
                            {
                                // If we don't have a constant, check if it's some constant value we can forward. In this case, we
                                // did not retrieve it. As a last resort, check if this is explicitly a 'default(T)' expression.
                                if (conversionOperation.Operand is not IDefaultValueOperation { Type: { } defaultValueExpressionType })
                                {
                                    return;
                                }

                                // Also make sure the type matches the property type (it's not technically guaranteed).
                                // If this succeeds, we can safely convert the property, the generated code will be fine.
                                if (!SymbolEqualityComparer.Default.Equals(defaultValueExpressionType, propertyTypeSymbol))
                                {
                                    return;
                                }
                            }
                        }
                    }

                    // Find the parent field for the operation (we're guaranteed to only fine one)
                    if (context.Operation.Syntax.FirstAncestor<FieldDeclarationSyntax>() is not { } fieldDeclaration)
                    {
                        return;
                    }

                    // Ensure that the field only has attributes we can forward, or not attributes at all
                    if (fieldDeclaration.AttributeLists.Any(static list => list.Target is { Identifier: not SyntaxToken(SyntaxKind.FieldKeyword) }))
                    {
                        return;
                    }

                    fieldFlags.PropertyName = propertyName;
                    fieldFlags.PropertyType = propertyTypeSymbol;
                    fieldFlags.FieldLocation = fieldDeclaration.GetLocation();
                }, OperationKind.FieldInitializer);

                // Finally, we can consume this information when we finish processing the symbol
                context.RegisterSymbolEndAction(context =>
                {
                    // Emit a diagnostic for each property that was a valid match
                    foreach (KeyValuePair<IPropertySymbol, PropertyFlags> pair in propertyMap)
                    {
                        // Make sure we have target fields for each accessor. This also implies the accessors themselves are valid.
                        if (pair.Value is not { GetValueTargetField: { } getValueFieldSymbol, SetValueTargetField: { } setValueFieldSymbol })
                        {
                            continue;
                        }

                        // The two fields must of course be the same
                        if (!SymbolEqualityComparer.Default.Equals(getValueFieldSymbol, setValueFieldSymbol))
                        {
                            continue;
                        }

                        // Next, check that the field are present in the mapping (otherwise for sure they're not valid)
                        if (!fieldMap.TryGetValue(getValueFieldSymbol, out FieldFlags? fieldFlags))
                        {
                            continue;
                        }

                        // We only support rewriting when the property name matches the field being initialized.
                        // Note that the property name here is the literal being passed for the 'name' parameter.
                        if (fieldFlags.PropertyName != pair.Key.Name)
                        {
                            continue;
                        }

                        // Make sure that the 'propertyType' value matches the actual type of the property.
                        // We are intentionally not handling combinations of nullable value types here.
                        if (!SymbolEqualityComparer.Default.Equals(fieldFlags.PropertyType, pair.Key.Type))
                        {
                            continue;
                        }

                        // Finally, check whether the field was valid (if so, we will have a valid location)
                        if (fieldFlags.FieldLocation is Location fieldLocation)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                UseGeneratedDependencyPropertyForManualProperty,
                                pair.Key.Locations.FirstOrDefault(),
                                [fieldLocation],
                                ImmutableDictionary.Create<string, string?>()
                                    .Add(DefaultValuePropertyName, fieldFlags.DefaultValue?.ToString())
                                    .Add(DefaultValueTypeReferenceIdPropertyName, fieldFlags.DefaultValueTypeReferenceId),
                                pair.Key));
                        }
                    }

                    // Before clearing the dictionary, move back all values to the stack
                    foreach (PropertyFlags propertyFlags in propertyMap.Values)
                    {
                        // Make sure the flags is cleared before returning it
                        propertyFlags.GetValueTargetField = null;
                        propertyFlags.SetValueTargetField = null;

                        propertyFlagsStack.Push(propertyFlags);
                    }

                    // Same for the field flags
                    foreach (FieldFlags fieldFlags in fieldMap.Values)
                    {
                        fieldFlags.PropertyName = null;
                        fieldFlags.PropertyType = null;
                        fieldFlags.DefaultValue = null;
                        fieldFlags.DefaultValueTypeReferenceId = null;
                        fieldFlags.FieldLocation = null;

                        fieldFlagsStack.Push(fieldFlags);
                    }

                    // We are now done processing the symbol, we can return the dictionary.
                    // Note that we must clear it before doing so to avoid leaks and issues.
                    propertyMap.Clear();

                    PropertyMapPool.Free(propertyMap);

                    // Also do the same for the stacks, except we don't need to clean them (since it roots no compilation objects)
                    PropertyFlagsStackPool.Free(propertyFlagsStack);
                    FieldFlagsStackPool.Free(fieldFlagsStack);
                });
            }, SymbolKind.NamedType);
        });
    }

    /// <summary>
    /// Produces a new <see cref="Stack{T}"/> instance to pool.
    /// </summary>
    /// <typeparam name="T">The type of flags objects to create.</typeparam>
    /// <returns>The resulting <see cref="Stack{T}"/> instance to use.</returns>
    private static Stack<T> CreateFlagsStack<T>()
        where T : class, new()
    {
        static IEnumerable<T> EnumerateFlags()
        {
            for (int i = 0; i < NumberOfPooledFlagsPerStack; i++)
            {
                yield return new();
            }
        }

        return new(EnumerateFlags());
    }

    /// <summary>
    /// Flags to track properties to warn on.
    /// </summary>
    private sealed class PropertyFlags
    {
        /// <summary>
        /// The target field for the <c>GetValue</c> method.
        /// </summary>
        public IFieldSymbol? GetValueTargetField;

        /// <summary>
        /// The target field for the <c>SetValue</c> method.
        /// </summary>
        public IFieldSymbol? SetValueTargetField;
    }

    /// <summary>
    /// Flags to track fields.
    /// </summary>
    private sealed class FieldFlags
    {
        /// <summary>
        /// The name of the property.
        /// </summary>
        public string? PropertyName;

        /// <summary>
        /// The type of the property (as in, of values that can be assigned to it).
        /// </summary>
        public ITypeSymbol? PropertyType;

        /// <summary>
        /// The default value to use (not present if it does not need to be set explicitly).
        /// </summary>
        public TypedConstantInfo? DefaultValue;

        /// <summary>
        /// The documentation comment reference id for type of the default value, if needed.
        /// </summary>
        public string? DefaultValueTypeReferenceId;

        /// <summary>
        /// The location of the target field being initialized.
        /// </summary>
        public Location? FieldLocation;
    }
}
