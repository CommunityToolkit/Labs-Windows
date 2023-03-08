// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Tooling.TestGen.Extensions;

/// <summary>
/// Extension methods for the <see cref="ITypeSymbol"/> type.
/// </summary>
/// Borrowed from <see href="https://github.com/CommunityToolkit/dotnet/blob/d4a8971ff6f4ffc188bbcf33d2c031c08ca584eb/CommunityToolkit.Mvvm.SourceGenerators/Extensions/ITypeSymbolExtensions.cs">ITypeSymbolExtensions</see> in the dotnet Toolkit's CommunityToolkit.Mvvm.SourceGenerators project.
/// </remarks>
internal static class ITypeSymbolExtensions
{
    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for inheritance.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> is or inherits from <paramref name="name"/>.</returns>
    public static bool HasOrInheritsFromFullyQualifiedName(this ITypeSymbol typeSymbol, string name)
    {
        for (var currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.HasFullyQualifiedName(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for inheritance.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> inherits from <paramref name="name"/>.</returns>
    public static bool InheritsFromFullyQualifiedName(this ITypeSymbol typeSymbol, string name)
    {
        var baseType = typeSymbol.BaseType;

        while (baseType != null)
        {
            if (baseType.HasFullyQualifiedName(name))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> implements an interface with a specified name.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for interface implementation.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an interface with the specified name.</returns>
    public static bool HasInterfaceWithFullyQualifiedName(this ITypeSymbol typeSymbol, string name)
    {
        foreach (var interfaceType in typeSymbol.AllInterfaces)
        {
            if (interfaceType.HasFullyQualifiedName(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits a specified attribute.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="predicate">The predicate used to match available attributes.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute matching <paramref name="predicate"/>.</returns>
    public static bool HasOrInheritsAttribute(this ITypeSymbol typeSymbol, Func<AttributeData, bool> predicate)
    {
        for (var currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.GetAttributes().Any(predicate))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits a specified attribute.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The name of the attribute to look for.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute with the specified type name.</returns>
    public static bool HasOrInheritsAttributeWithFullyQualifiedName(this ITypeSymbol typeSymbol, string name)
    {
        for (var currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.HasAttributeWithFullyQualifiedName(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> inherits a specified attribute.
    /// If the type has no base type, this method will automatically handle that and return <see langword="false"/>.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The name of the attribute to look for.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute with the specified type name.</returns>
    public static bool InheritsAttributeWithFullyQualifiedName(this ITypeSymbol typeSymbol, string name)
    {
        if (typeSymbol.BaseType is INamedTypeSymbol baseTypeSymbol)
        {
            return baseTypeSymbol.HasOrInheritsAttributeWithFullyQualifiedName(name);
        }

        return false;
    }
}
