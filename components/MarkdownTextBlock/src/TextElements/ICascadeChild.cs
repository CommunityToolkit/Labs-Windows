// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock.TextElements;

/// <summary>
/// Interface for elements that inherit properties from their parent.
/// </summary>
public interface ICascadeChild
{
    void InheritProperties(IAddChild parent);
}
