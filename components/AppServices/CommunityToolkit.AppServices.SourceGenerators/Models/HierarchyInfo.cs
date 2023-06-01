// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file is ported and adapted from ComputeSharp (Sergio0694/ComputeSharp),
// more info in ThirdPartyNotices.txt in the root of the project.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CommunityToolkit.AppServices.SourceGenerators.Extensions;
using CommunityToolkit.AppServices.SourceGenerators.Helpers;
using static Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;

namespace CommunityToolkit.AppServices.SourceGenerators.Models;

/// <summary>
/// A model describing the hierarchy info for a specific type.
/// </summary>
/// <param name="FilenameHint">The filename hint for the current type.</param>
/// <param name="MetadataName">The metadata name for the current type.</param>
/// <param name="Modifiers">The modifiers for the type declaration.</param>
/// <param name="Namespace">Gets the namespace for the current type.</param>
/// <param name="Hierarchy">Gets the sequence of type definitions containing the current type.</param>
internal sealed partial record HierarchyInfo(
    string FilenameHint,
    string MetadataName,
    EquatableArray<ushort> Modifiers,
    string Namespace,
    EquatableArray<TypeInfo> Hierarchy)
{
    /// <summary>
    /// Creates a new <see cref="HierarchyInfo"/> instance from a given <see cref="INamedTypeSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance to gather info for.</param>
    /// <returns>A <see cref="HierarchyInfo"/> instance describing <paramref name="typeSymbol"/>.</returns>
    public static HierarchyInfo From(INamedTypeSymbol typeSymbol)
    {
        using ImmutableArrayBuilder<TypeInfo> hierarchy = ImmutableArrayBuilder<TypeInfo>.Rent();

        for (INamedTypeSymbol? parent = typeSymbol;
             parent is not null;
             parent = parent.ContainingType)
        {
            hierarchy.Add(new TypeInfo(
                parent.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                parent.TypeKind,
                parent.IsRecord));
        }

        return new(
            typeSymbol.GetFullMetadataNameForFileName(),
            typeSymbol.MetadataName,
            ImmutableArray.Create((ushort)SyntaxKind.PartialKeyword),
            typeSymbol.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: NameAndContainingTypesAndNamespaces)),
            hierarchy.ToImmutable());
    }

    /// <summary>
    /// Creates a new <see cref="HierarchyInfo"/> instance from a given <see cref="INamedTypeSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance to gather info for.</param>
    /// <param name="metadataName">The name of the new type to describe.</param>
    /// <returns>A <see cref="HierarchyInfo"/> instance describing <paramref name="typeSymbol"/>.</returns>
    public static HierarchyInfo From(INamedTypeSymbol typeSymbol, string metadataName)
    {
        using ImmutableArrayBuilder<TypeInfo> hierarchy = ImmutableArrayBuilder<TypeInfo>.Rent();

        hierarchy.Add(new TypeInfo(metadataName, TypeKind.Class, IsRecord: false));

        for (INamedTypeSymbol? parent = typeSymbol.ContainingType;
             parent is not null;
             parent = parent.ContainingType)
        {
            hierarchy.Add(new TypeInfo(
                parent.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                parent.TypeKind,
                parent.IsRecord));
        }

        return new(
            typeSymbol.GetFullMetadataNameForFileName(),
            metadataName,
            ImmutableArray.Create((ushort)SyntaxKind.PublicKeyword, (ushort)SyntaxKind.SealedKeyword),
            typeSymbol.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: NameAndContainingTypesAndNamespaces)),
            hierarchy.ToImmutable());
    }
}
