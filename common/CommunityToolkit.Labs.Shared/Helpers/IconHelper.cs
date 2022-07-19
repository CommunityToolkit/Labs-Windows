// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace CommunityToolkit.Labs.Shared.Helpers;

public static class IconHelper
{
    public static IconElement? GetCategoryIcon(ToolkitSampleCategory category)
    {
        IconElement? iconElement = null;
        switch (category)
        {
            case ToolkitSampleCategory.Controls: iconElement = new SymbolIcon() { Symbol = Symbol.Keyboard }; break;
        }
        return iconElement;
    }

    public static string GetSubcategoryIcon(ToolkitSampleSubcategory subcategory)
    {
        string imagePath = string.Empty;
        switch (subcategory)
        {
            case ToolkitSampleSubcategory.None: imagePath = "ms-appx:///Assets/Images/AutoSuggestBox.png"; break;
            case ToolkitSampleSubcategory.Layout: imagePath = "ms-appx:///Assets/Images/AutoSuggestBox.png"; break;
        }
        return imagePath;
    }
}
