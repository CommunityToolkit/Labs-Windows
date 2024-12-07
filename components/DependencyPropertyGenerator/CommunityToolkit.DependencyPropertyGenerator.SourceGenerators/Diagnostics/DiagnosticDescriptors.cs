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
    /// <c>"The property '{0}' cannot be used to generate a dependency property, as its declaration is not valid (it must be an instance (non static) partial property, with a getter and a setter that is not init-only)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclaration = new(
        id: "WCTDP0001",
        title: "Invalid property declaration for [GeneratedDependencyProperty]",
        messageFormat: "The property '{0}' cannot be used to generate a dependency property, as its declaration is not valid (it must be an instance (non static) partial property, with a getter and a setter that is not init-only)",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must be instance (non static) partial properties, with a getter and a setter that is not init-only.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' is not an incomplete partial definition ([ObservableProperty] must be used on a partial property definition with no implementation part)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationIsNotIncompletePartialDefinition = new(
        id: "WCTDP0002",
        title: "Using [GeneratedDependencyProperty] on an invalid partial property (not incomplete partial definition)",
        messageFormat: """The property '{0}' is not an incomplete partial definition ([ObservableProperty] must be used on a partial property definition with no implementation part)""",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A property using [GeneratedDependencyProperty] is not a partial implementation part ([GeneratedDependencyProperty] must be used on partial property definitions with no implementation part).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' cannot be used to generate a dependency property, as it returns a ref value ([GeneratedDependencyProperty] must be used on properties returning a non byref-like type by value)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationReturnsByRef = new(
        id: "WCTDP0003",
        title: "Using [GeneratedDependencyProperty] on a property that returns byref",
        messageFormat: """The property '{0}' cannot be used to generate a dependency property, as it returns a ref value ([GeneratedDependencyProperty] must be used on properties returning a non byref-like type by value)""",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must not return a ref value (only reference types and non byref-like types are supported).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' cannot be used to generate a dependency property, as it returns a byref-like value ([GeneratedDependencyProperty] must be used on properties returning a non byref-like type by value)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationReturnsRefLikeType = new(
        id: "WCTDP0004",
        title: "Using [GeneratedDependencyProperty] on a property that returns byref-like",
        messageFormat: """The property '{0}' cannot be used to generate a dependency property, as it returns a byref-like value ([GeneratedDependencyProperty] must be used on properties returning a non byref-like type by value)""",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must not return a byref-like value (only reference types and non byref-like types are supported).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' cannot be used to generate a dependency property, as its containing type doesn't inherit from DependencyObject"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationContainingTypeIsNotDependencyObject = new(
        id: "WCTDP0005",
        title: "Using [GeneratedDependencyProperty] on a property with invalid containing type",
        messageFormat: "The property '{0}' cannot be used to generate a dependency property, as its containing type doesn't inherit from DependencyObject",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must be contained in a type that inherits from DependencyObject.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' cannot be used to generate a dependency property, as the project is not using C# 13 or greater (add <LangVersion>13.0</LangVersion> to your .csproj/.props file)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor PropertyDeclarationRequiresCSharp13 = new(
        id: "WCTDP0006",
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
        id: "WCTDP0007",
        title: "Using [GeneratedDependencyProperty] with 'IsLocalCachingEnabled' requires C# 'preview'",
        messageFormat: """The property '{0}' cannot be used to generate a dependency property, as the project is not using C# 'preview', which is required when using the 'IsLocalCachingEnabled' option (add <LangVersion>preview</LangVersion> to your .csproj/.props file)""",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and using the 'IsLocalCachingEnabled' option must be contained in a project using C# 'preview'. Make sure to add <LangVersion>preview</LangVersion> to your .csproj/.props file.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' cannot be used to generate an dependency property, as its name or type would cause conflicts with other generated members ([GeneratedDependencyProperty] must not be used on properties named 'Property' of type either 'object' or 'DependencyPropertyChangedEventArgs')</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationWouldCauseConflicts = new(
        id: "WCTDP0008",
        title: "Conflicting property declaration for [GeneratedDependencyProperty]",
        messageFormat: "The property '{0}' cannot be used to generate an dependency property, as its name or type would cause conflicts with other generated members ([GeneratedDependencyProperty] must not be used on properties named 'Property' of type either 'object' or 'DependencyPropertyChangedEventArgs')",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must not be declared in such a way that would cause generate members to cause conflicts. In particular, they cannot be named 'Property' and be of type either 'object' or 'DependencyPropertyChangedEventArgs'.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' is not annotated as nullable, but it might contain a null value upon exiting the constructor (consider adding the 'required' modifier, setting a non-null default value if possible, or declaring the property as nullable)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor NonNullablePropertyDeclarationIsNotEnforced = new(
        id: "WCTDP0009",
        title: "Non-nullable dependency property is not guaranteed to not be null",
        messageFormat: "The property '{0}' is not annotated as nullable, but it might contain a null value upon exiting the constructor (consider adding the 'required' modifier, setting a non-null default value if possible, or declaring the property as nullable)",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Non-nullable properties annotated with [GeneratedDependencyProperty] should guarantee that their values will not be null upon exiting the constructor. This can be enforced by adding the 'required' modifier, setting a non-null default value if possible, or declaring the property as nullable.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' is declared with type '{1}', but 'DefaultValue' is set to 'null', which is not compatible (consider fixing the default value, or implementing the 'Get(ref object)' partial method to handle the type mismatch)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDefaultValueNull = new(
        id: "WCTDP0010",
        title: "Invalid 'null' default value for [GeneratedDependencyProperty] use",
        messageFormat: "The property '{0}' is declared with type '{1}', but 'DefaultValue' is set to 'null', which is not compatible (consider fixing the default value, or implementing the 'Get(ref object)' partial method to handle the type mismatch)",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'DefaultValue' should do so with an expression of a type comparible with the property type. Alternatively, the 'Get(ref object)' method should be implemented to handle the type mismatch.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>The property '{0}' is declared with type '{1}', but 'DefaultValue' is set to value '{2}' (type '{3}'), which is not compatible (consider fixing the default value, or implementing the 'Get(ref object)' partial method to handle the type mismatch)</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDefaultValueType = new(
        id: "WCTDP0011",
        title: "Invalid default value type for [GeneratedDependencyProperty] use",
        messageFormat: "The property '{0}' is declared with type '{1}', but 'DefaultValue' is set to value '{2}' (type '{3}'), which is not compatible (consider fixing the default value, or implementing the 'Get(ref object)' partial method to handle the type mismatch)",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'DefaultValue' should do so with an expression of a type comparible with the property type. Alternatively, the 'Get(ref object)' method should be implemented to handle the type mismatch.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' returns a pointer or function pointer value ([ObservableProperty] must be used on properties of a non pointer-like type)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationReturnsPointerType = new(
        id: "WCTDP0012",
        title: "Using [GeneratedDependencyProperty] on a property that returns pointer type",
        messageFormat: """The property '{0}' cannot be used to generate a dependency property, as it returns a pointer value ([GeneratedDependencyProperty] must be used on properties returning a non pointer value)""",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] must not return a pointer value (only reference types and non byref-like types are supported).",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' is using [GeneratedDependencyProperty] with both 'DefaultValue' and 'DefaultValueCallback' and being set, which is not supported (only one of these properties can be set at a time)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationDefaultValueCallbackMixed = new(
        id: "WCTDP0013",
        title: "Using [GeneratedDependencyProperty] with both 'DefaultValue' and 'DefaultValueCallback'",
        messageFormat: """The property '{0}' is using [GeneratedDependencyProperty] with both 'DefaultValue' and 'DefaultValueCallback' and being set, which is not supported (only one of these properties can be set at a time)""",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] cannot use both 'DefaultValue' and 'DefaultValueCallback' at the same time.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' is using [GeneratedDependencyProperty] with 'DefaultValueCallback' set to '{1}', but no accessible method with that name was found (make sure the target method is in the same containing type)"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationDefaultValueCallbackNoMethodFound = new(
        id: "WCTDP0014",
        title: "Using [GeneratedDependencyProperty] with missing 'DefaultValueCallback' method",
        messageFormat: """The property '{0}' is using [GeneratedDependencyProperty] with 'DefaultValueCallback' set to '{1}', but no accessible method with that name was found (make sure the target method is in the same containing type)""",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'DefaultValueCallback' must use the name of an accessible method in their same containing type.",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");

    /// <summary>
    /// <c>"The property '{0}' is using [GeneratedDependencyProperty] with 'DefaultValueCallback' set to '{1}', but the method has an invalid signature (it must be a static method with no parameters, returning a value compatible with the property type: either the same type, or 'object')"</c>.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidPropertyDeclarationDefaultValueCallbackInvalidMethod = new(
        id: "WCTDP0015",
        title: "Using [GeneratedDependencyProperty] with invalid 'DefaultValueCallback' method",
        messageFormat: """The property '{0}' is using [GeneratedDependencyProperty] with 'DefaultValueCallback' set to '{1}', but the method has an invalid signature (it must be a static method with no parameters, returning a value compatible with the property type: either the same type, or 'object')""",
        category: typeof(DependencyPropertyGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Properties annotated with [GeneratedDependencyProperty] and setting 'DefaultValueCallback' must use the name of a method with a valid signature (it must be a static method with no parameters, returning a value compatible with the property type: either the same type, or 'object').",
        helpLinkUri: "https://aka.ms/toolkit/labs/windows");
}
