// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityToolkit.Labs.Shared.Converters;

public sealed class SubcategoryToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        ToolkitSampleSubcategory subcategory = (ToolkitSampleSubcategory)value;
        return new Uri(IconHelper.GetSubcategoryIcon(subcategory));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}
