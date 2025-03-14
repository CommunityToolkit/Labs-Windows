// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Diagnostics;

/// <summary>
/// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
/// </summary>
internal static class DiagnosticDescriptors
{
    /// <summary>
    /// The category to use for all diagnostics produced by analyzers in this project.
    /// </summary>
    private const string DiagnosticCategory = "DependencyPropertyGenerator";

    /// <summary>
    /// The diagnostic id for <see cref="UseGeneratedDependencyPropertyForManualProperty"/>.
    /// </summary>
    public const string UseGeneratedDependencyPropertyForManualPropertyId = "WCTDPG0017";

    /// <summary>
    /// The diagnostic id for <see cref="IncorrectDependencyPropertyFieldDeclaration"/>.
    /// </summary>
    public const string IncorrectDependencyPropertyFieldDeclarationId = "WCTDPG0020";

    /// <summary>
    /// The diagnostic id for <see cref="DependencyPropertyFieldDeclaration"/>.
    /// </summary>
    public const string DependencyPropertyFieldDeclarationId = "WCTDPG0021";

    /// <summary>
    /// <c>"The property '{0}' cannot be used to generate a dependency property, as its declaration is not valid (it must be an instance (non static) partial property, with a getter and a setter that is not init-only)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclaration = new(
        id: "WCTDPG0001",
        title: "Invalid property declaration for [GeneratedDependencyProperty]",
        messageFormat: "The property '{0}' cannot be used to generate a dependency property, as its declaration is not valid (it must be an instance (non static) partial property, with a getter and a setter that is not init-only)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must be instance (non static) partial properties, with a getter and a setter that is not init-only.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' is not an incomplete partial definition ([ObservableProperty] must be used on a partial property definition with no implementation part)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationIsNotIncompletePartialDefinition = new(
        id: "WCTDPG0002",
        title: "Using [GeneratedDependencyProperty] on an invalid partial property (not incomplete partial definition)",
        messageFormat: """The property '{0}' is not an incomplete partial definition ([ObservableProperty] must be used on a partial property definition with no implementation part)""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A property using [GeneratedDependencyProperty] is not a partial implementation part ([GeneratedDependencyProperty] must be used on partial property definitions with no implementation part).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' cannot be used to generate a dependency property, as it returns a ref value ([GeneratedDependencyProperty] must be used on properties returning a non byref-like type by value)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationReturnsByRef = new(
        id: "WCTDPG0003",
        title: "Using [GeneratedDependencyProperty] on a property that returns byref",
        messageFormat: """The property '{0}' cannot be used to generate a dependency property, as it returns a ref value ([GeneratedDependencyProperty] must be used on properties returning a non byref-like type by value)""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must not return a ref value (only reference types and non byref-like types are supported).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' cannot be used to generate a dependency property, as it returns a byref-like value ([GeneratedDependencyProperty] must be used on properties returning a non byref-like type by value)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationReturnsRefLikeType = new(
        id: "WCTDPG0004",
        title: "Using [GeneratedDependencyProperty] on a property that returns byref-like",
        messageFormat: """The property '{0}' cannot be used to generate a dependency property, as it returns a byref-like value ([GeneratedDependencyProperty] must be used on properties returning a non byref-like type by value)""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must not return a byref-like value (only reference types and non byref-like types are supported).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' cannot be used to generate a dependency property, as its containing type doesn't inherit from DependencyObject"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationContainingTypeIsNotDependencyObject = new(
        id: "WCTDPG0005",
        title: "Using [GeneratedDependencyProperty] on a property with invalid containing type",
        messageFormat: "The property '{0}' cannot be used to generate a dependency property, as its containing type doesn't inherit from DependencyObject",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must be contained in a type that inherits from DependencyObject.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' cannot be used to generate a dependency property, as the project is not using C# 13 or greater (add <LangVersion>13.0</LangVersion> to your .csproj/.props file)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor PropertyDeclarationRequiresCSharp13 = new(
        id: "WCTDPG0006",
        title: "Using [GeneratedDependencyProperty] requires C# 13",
        messageFormat: "The property '{0}' cannot be used to generate a dependency property, as the project is not using C# 13 or greater (add <LangVersion>13.0</LangVersion> to your .csproj/.props file)",
        category: typeof(UnsupportedCSharpLanguageVersionAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must be contained in a project using C# 13 or greater. Make sure to add <LangVersion>13.0</LangVersion> to your .csproj/.props file.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' cannot be used to generate a dependency property, as the project is not using C# 'preview', which is required when using the 'IsLocalCachingEnabled' option (add <LangVersion>preview</LangVersion> to your .csproj/.props file)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor LocalCachingRequiresCSharpPreview = new(
        id: "WCTDPG0007",
        title: "Using [GeneratedDependencyProperty] with 'IsLocalCachingEnabled' requires C# 'preview'",
        messageFormat: """The property '{0}' cannot be used to generate a dependency property, as the project is not using C# 'preview', which is required when using the 'IsLocalCachingEnabled' option (add <LangVersion>preview</LangVersion> to your .csproj/.props file)""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and using the 'IsLocalCachingEnabled' option must be contained in a project using C# 'preview'. Make sure to add <LangVersion>preview</LangVersion> to your .csproj/.props file.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' cannot be used to generate an dependency property, as its name or type would cause conflicts with other generated members ([GeneratedDependencyProperty] must not be used on properties named 'Property' of type either 'object' or 'DependencyPropertyChangedEventArgs')</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationWouldCauseConflicts = new(
        id: "WCTDPG0008",
        title: "Conflicting property declaration for [GeneratedDependencyProperty]",
        messageFormat: "The property '{0}' cannot be used to generate an dependency property, as its name or type would cause conflicts with other generated members ([GeneratedDependencyProperty] must not be used on properties named 'Property' of type either 'object' or 'DependencyPropertyChangedEventArgs')",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must not be declared in such a way that would cause generate members to cause conflicts. In particular, they cannot be named 'Property' and be of type either 'object' or 'DependencyPropertyChangedEventArgs'.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' is not annotated as nullable, but it might contain a null value upon exiting the constructor (consider adding the 'required' modifier, setting a non-null default value if possible, or declaring the property as nullable)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor NonNullablePropertyDeclarationIsNotEnforced = new(
        id: "WCTDPG0009",
        title: "Non-nullable dependency property is not guaranteed to not be null",
        messageFormat: "The property '{0}' is not annotated as nullable, but it might contain a null value upon exiting the constructor (consider adding the 'required' modifier, setting a non-null default value if possible, or declaring the property as nullable)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Non-nullable properties annotated with [GeneratedDependencyProperty] should guarantee that their values will not be null upon exiting the constructor. This can be enforced by adding the 'required' modifier, setting a non-null default value if possible, or declaring the property as nullable.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' is declared with type '{1}', but 'DefaultValue' is set to 'null', which is not compatible (consider changing the default value, implementing the 'On{2}Get(ref object)' partial method to handle the type mismatch, or suppressing the diagnostic if this is the intended behavior)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDefaultValueNull = new(
        id: "WCTDPG0010",
        title: "Invalid 'null' default value for [GeneratedDependencyProperty] use",
        messageFormat: "The property '{0}' is declared with type '{1}', but 'DefaultValue' is set to 'null', which is not compatible (consider changing the default value, implementing the 'On{2}Get(ref object)' partial method to handle the type mismatch, or suppressing the diagnostic if this is the intended behavior)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'DefaultValue' should do so with an expression of a type comparible with the property type. Alternatively, the generated getter method (eg. 'OnNameGet', if the property is called 'Name') should be implemented to handle the type mismatch.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' is declared with type '{1}', but 'DefaultValue' is set to value '{2}' (type '{3}'), which is not compatible (consider fixing the default value, or implementing the 'On{4}Get(ref object)' partial method to handle the type mismatch)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDefaultValueType = new(
        id: "WCTDPG0011",
        title: "Invalid default value type for [GeneratedDependencyProperty] use",
        messageFormat: "The property '{0}' is declared with type '{1}', but 'DefaultValue' is set to value '{2}' (type '{3}'), which is not compatible (consider fixing the default value, or implementing the 'On{4}Get(ref object)' partial method to handle the type mismatch)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'DefaultValue' should do so with an expression of a type comparible with the property type. Alternatively, the generated getter method (eg. 'OnNameGet', if the property is called 'Name') should be implemented to handle the type mismatch.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' returns a pointer or function pointer value ([ObservableProperty] must be used on properties of a non pointer-like type)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationReturnsPointerType = new(
        id: "WCTDPG0012",
        title: "Using [GeneratedDependencyProperty] on a property that returns pointer type",
        messageFormat: """The property '{0}' cannot be used to generate a dependency property, as it returns a pointer value ([GeneratedDependencyProperty] must be used on properties returning a non pointer value)""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must not return a pointer value (only reference types and non byref-like types are supported).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' is using [GeneratedDependencyProperty] with both 'DefaultValue' and 'DefaultValueCallback' and being set, which is not supported (only one of these properties can be set at a time)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationDefaultValueCallbackMixed = new(
        id: "WCTDPG0013",
        title: "Using [GeneratedDependencyProperty] with both 'DefaultValue' and 'DefaultValueCallback'",
        messageFormat: """The property '{0}' is using [GeneratedDependencyProperty] with both 'DefaultValue' and 'DefaultValueCallback' and being set, which is not supported (only one of these properties can be set at a time)""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] cannot use both 'DefaultValue' and 'DefaultValueCallback' at the same time.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' is using [GeneratedDependencyProperty] with 'DefaultValueCallback' set to '{1}', but no accessible method with that name was found (make sure the target method is in the same containing type)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationDefaultValueCallbackNoMethodFound = new(
        id: "WCTDPG0014",
        title: "Using [GeneratedDependencyProperty] with missing 'DefaultValueCallback' method",
        messageFormat: """The property '{0}' is using [GeneratedDependencyProperty] with 'DefaultValueCallback' set to '{1}', but no accessible method with that name was found (make sure the target method is in the same containing type)""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'DefaultValueCallback' must use the name of an accessible method in their same containing type.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' is using [GeneratedDependencyProperty] with 'DefaultValueCallback' set to '{1}', but the method has an invalid signature (it must be a static method with no parameters, returning a value compatible with the property type: either the same type, or 'object')"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationDefaultValueCallbackInvalidMethod = new(
        id: "WCTDPG0015",
        title: "Using [GeneratedDependencyProperty] with invalid 'DefaultValueCallback' method",
        messageFormat: """The property '{0}' is using [GeneratedDependencyProperty] with 'DefaultValueCallback' set to '{1}', but the method has an invalid signature (it must be a static method with no parameters, returning a value compatible with the property type: either the same type, or 'object')""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'DefaultValueCallback' must use the name of a method with a valid signature (it must be a static method with no parameters, returning a value compatible with the property type: either the same type, or 'object').",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' is using [GeneratedDependencyProperty] and has a name ending with the 'Property' suffix, which is redundant (the generated dependency property will always add the 'Property' suffix to the name of its associated property)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor PropertyDeclarationWithPropertySuffix = new(
        id: "WCTDPG0016",
        title: "Using [GeneratedDependencyProperty] on a property with the 'Property' suffix",
        messageFormat: """The property '{0}' is using [GeneratedDependencyProperty] and has a name ending with the 'Property' suffix, which is redundant (the generated dependency property will always add the 'Property' suffix to the name of its associated property)""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] should not have the 'Property' suffix in their name, as it is redundant (the generated dependency properties will always add the 'Property' suffix to the name of their associated properties).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The manual property '{0}' can be converted to a partial property using [GeneratedDependencyProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor UseGeneratedDependencyPropertyForManualProperty = new(
        id: UseGeneratedDependencyPropertyForManualPropertyId,
        title: "Prefer using [GeneratedDependencyProperty] over manual properties",
        messageFormat: """The manual property '{0}' can be converted to a partial property using [GeneratedDependencyProperty], which is recommended (doing so makes the code less verbose and results in more optimized code)""",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Manual properties should be converted to partial properties using [GeneratedDependencyProperty] when possible, which is recommended (doing so makes the code less verbose and results in more optimized code).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' annotated with [GeneratedDependencyProperty] is using attribute '{1}' which was not recognized as a valid type (are you missing a using directive?)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidDependencyPropertyTargetedAttributeType = new(
        id: "WCTDPG0018",
        title: "Invalid dependency property targeted attribute type",
        messageFormat: "The property '{0}' annotated with [GeneratedDependencyProperty] is using attribute '{1}' which was not recognized as a valid type (are you missing a using directive?)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All attributes targeting the generated dependency property for a property annotated with [GeneratedDependencyProperty] must correctly be resolved to valid types.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' annotated with [GeneratedDependencyProperty] is using attribute '{1}' with an invalid expression (are you passing any incorrect parameters to the attribute constructor?)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidDependencyPropertyTargetedAttributeTypeArgumentExpression = new(
        id: "WCTDPG0019",
        title: "Invalid dependency property targeted attribute expression",
        messageFormat: "The property '{0}' annotated with [GeneratedDependencyProperty] is using attribute '{1}' with an invalid expression (are you passing any incorrect parameters to the attribute constructor?)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All attributes targeting the generated dependency property for a property annotated with [GeneratedDependencyProperty] must have arguments using supported expressions.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The field '{0}' is a dependency property, but it is not declared correctly (all dependency property fields should be declared as 'public static readonly', and not be nullable)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor IncorrectDependencyPropertyFieldDeclaration = new(
        id: IncorrectDependencyPropertyFieldDeclarationId,
        title: "Incorrect dependency property field declaration",
        messageFormat: "The field '{0}' is a dependency property, but it is not declared correctly (all dependency property fields should be declared as 'public static readonly', and not be nullable)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All dependency property fields should be declared as 'public static readonly', and not be nullable.",
        helpLinkUri: "https://learn.microsoft.com/windows/uwp/xaml-platform/custom-dependency-properties#checklist-for-defining-a-dependency-property");

    /// <summary>
    /// <c>"The property '{0}' is a dependency property, which is not the correct declaration type (all dependency properties should be declared as fields, unless implementing interface members or in authored WinRT component types)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor DependencyPropertyFieldDeclaration = new(
        id: DependencyPropertyFieldDeclarationId,
        title: "Dependency property declared as a property",
        messageFormat: "The property '{0}' is a dependency property, which is not the correct declaration type (all dependency properties should be declared as fields, unless implementing interface members or in authored WinRT component types)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All dependency properties should be declared as fields, unless implementing interface members or in authored WinRT component types.",
        helpLinkUri: "https://learn.microsoft.com/windows/uwp/xaml-platform/custom-dependency-properties#checklist-for-defining-a-dependency-property");

    /// <summary>
    /// <c>"The property '{0}' annotated with [GeneratedDependencyProperty] is specifying '{1}' as its property type in metadata, which is unnecessary (the type is the same as the declared property type)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor UnnecessaryDependencyPropertyExplicitMetadataType = new(
        id: "WCTDPG0022",
        title: "Unnecessary dependency property explicit metadata type",
        messageFormat: "The property '{0}' annotated with [GeneratedDependencyProperty] is specifying '{1}' as its property type in metadata, which is unnecessary (the type is the same as the declared property type)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'PropertyType' should only do so when the explicit type would not match the declared property type.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' annotated with [GeneratedDependencyProperty] is specifying '{1}' as its property type in metadata, which is not compatible with its declared type '{2}' (the 'PropertyType' option should be used with a compatible type)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor IncompatibleDependencyPropertyExplicitMetadataType = new(
        id: "WCTDPG0023",
        title: "Incompatible dependency property explicit metadata type",
        messageFormat: "The property '{0}' annotated with [GeneratedDependencyProperty] is specifying '{1}' as its property type in metadata, which is not compatible with its declared type '{2}' (the 'PropertyType' option should be used with a compatible type)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'PropertyType' must do so with a type that is compatible with the declared property type.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' is not annotated as nullable and is using [AllowNull], but neither of its accessors are null-resilient (at least one generated 'On{1}Get' or 'On{1}Set' method must be implemented with [NotNull] on the 'propertyValue' parameter, to ensure assigning null values does not break the nullable annotations on the property)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor NotNullResilientAccessorsForNotNullablePropertyDeclaration = new(
        id: "WCTDPG0024",
        title: "Non-nullable dependency property using [AllowNull] incorrectly",
        messageFormat: "The property '{0}' is not annotated as nullable and is using [AllowNull], but neither of its accessors are null-resilient (at least one generated 'On{1}Get' or 'On{1}Set' method must be implemented with [NotNull] on the 'propertyValue' parameter, to ensure assigning null values does not break the nullable annotations on the property)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Non-nullable properties annotated with [GeneratedDependencyProperty] using [AllowNull] should have at least one generated getter or setter method (eg. 'OnNameGet', if the property is called 'Name') implemented as null-resilient (by adding [NotNull] on the 'propertyValue' parameter) to ensure assigning null values does not break the nullable annotations on the property.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' is annotated as nullable and is using [NotNull], but it's not guaranteeing that returned values will not be null (it must either make its 'get' accessor null-resilient, by implementing at least one generated 'On{1}Get' method with [NotNull] on the 'propertyValue' parameter, or it must either add [DisallowNull] or implement at least one generated 'On{1}Set' method with [NotNull], and also either mark the property as required, or ensure that its default value is not null)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor NotNullResilientAccessorsForNullablePropertyDeclaration = new(
        id: "WCTDPG0025",
        title: "Nullable dependency property using [NotNull] incorrectly",
        messageFormat: "The property '{0}' is annotated as nullable and is using [NotNull], but it's not guaranteeing that returned values will not be null (it must either make its 'get' accessor null-resilient, by implementing at least one generated 'On{1}Get' method with [NotNull] on the 'propertyValue' parameter, or it must either add [DisallowNull] or implement at least one generated 'On{1}Set' method with [NotNull], and also either mark the property as required, or ensure that its default value is not null)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Nullable properties annotated with [GeneratedDependencyProperty] using [NotNull] should make their 'get' accessors null-resilient, by implementing at least one generated getter method (eg. 'OnNameGet', if the property is called 'Name') with [NotNull] on the 'propertyValue' parameter, or they must either add [DisallowNull] or implement at least one generated setter method (eg. 'OnNameSet', if the property is called 'Name') with [NotNull], and also either be marked as required properties, or ensure that the default value is not null.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The field '{0}' is registering a dependency property, but its name is not following the recommended naming convention (all dependency property fields should use the 'Property' suffix in their names)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor NoPropertySuffixOnDependencyPropertyField = new(
        id: "WCTDPG0026",
        title: "No 'Property' suffix on dependency property field name",
        messageFormat: "The field '{0}' is registering a dependency property, but its name is not following the recommended naming convention (all dependency property fields should use the 'Property' suffix in their names)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All dependency property fields should be named following the recommended naming convention (they should use the 'Property' suffix in their names).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The field '{0}' is registering a dependency property, but the property name '{1}' declared in metadata is not following the recommended naming convention (all property names should match the name of their declared fields, minus the 'Property' suffix)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyNameOnDependencyPropertyField = new(
        id: "WCTDPG0027",
        title: "Invalid property name in dependency property field metadata",
        messageFormat: "The field '{0}' is registering a dependency property, but the property name '{1}' declared in metadata is not following the recommended naming convention (all property names should match the name of their declared fields, minus the 'Property' suffix)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All dependency property fields should declare property names in metadata following the recommended naming convention (they should match the name of the declared fields, minus the 'Property' suffix).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The field '{0}' is registering a dependency property, but the property name '{1}' declared in metadata does not match the property name '{2}' wrapping the dependency property (all property names should match the name of the wrapping properties of each dependency property field)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor MismatchedPropertyNameOnDependencyPropertyField = new(
        id: "WCTDPG0028",
        title: "Mismatched property name in dependency property field metadata",
        messageFormat: "The field '{0}' is registering a dependency property, but the property name '{1}' declared in metadata does not match the property name '{2}' wrapping the dependency property (all property names should match the name of the wrapping properties of each dependency property field)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All dependency property fields should declare property names in metadata following the recommended naming convention (they should exactly match the name of the wrapping properties).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The field '{0}' is registering a dependency property, but its owning type '{1}' declared in metadata does not match the containing type '{2}' for the field (the owning type of a dependency property should always match the containing type of the field declaration)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidOwningTypeOnDependencyPropertyField = new(
        id: "WCTDPG0029",
        title: "Invalid owning type in dependency property field metadata",
        messageFormat: "The field '{0}' is registering a dependency property, but its owning type '{1}' declared in metadata does not match the containing type '{2}' for the field (the owning type of a dependency property should always match the containing type of the field declaration)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All dependency property fields should declare owning types in metadata matching their containing types.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The field '{0}' is registering a dependency property, but its property type '{1}' does not match the type '{2}' of the wrapping property '{3}', and there is no valid type conversion between the two (all property types should either match the type of the wrapping properties for each dependency property field, or be of a valid assignable type from the type of each wrapping property)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyTypeOnDependencyPropertyField = new(
        id: "WCTDPG0030",
        title: "Invalid property type in dependency property field metadata",
        messageFormat: "The field '{0}' is registering a dependency property, but its property type '{1}' does not match the type '{2}' of the wrapping property '{3}', and there is no valid type conversion between the two (all property types should either match the type of the wrapping properties for each dependency property field, or be of a valid assignable type from the type of each wrapping property)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All dependency property fields should declare property types in metadata matching the type of their wrapping properties, or with a valid type conversion between the two.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The field '{0}' is registering a dependency property, but its default value is set to 'null', which is not compatible with the property type '{1}' declared in metadata (consider changing the default value, setting the property value to a non 'null' value upon object construction, and/or suppressing the diagnostic if this is the intended behavior)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidDefaultValueNullOnDependencyPropertyField = new(
        id: "WCTDPG0031",
        title: "Invalid 'null' default value in dependency property field metadata",
        messageFormat: "The field '{0}' is registering a dependency property, but its default value is set to 'null', which is not compatible with the property type '{1}' declared in metadata (consider changing the default value, setting the property value to a non 'null' value upon object construction, and/or suppressing the diagnostic if this is the intended behavior)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All dependency property fields setting an explicit default value in metadata should do so with an expression of a type comparible with the property type. Alternatively, the property value should be set to a non 'null' value upon object construction.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The field '{0}' is registering a dependency property, but its default value has type '{1}', which is not compatible with the property type '{2}' declared in metadata (consider fixing the default value, or suppressing the diagnostic if this is the intended behavior)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidDefaultValueTypeOnDependencyPropertyField = new(
        id: "WCTDPG0032",
        title: "Invalid default value type in dependency property field metadata",
        messageFormat: "The field '{0}' is registering a dependency property, but its default value has type '{1}', which is not compatible with the property type '{2}' declared in metadata (consider fixing the default value, or suppressing the diagnostic if this is the intended behavior)",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All dependency property fields setting an explicit default value in metadata should do so with an expression of a type comparible with the property type.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");
}
