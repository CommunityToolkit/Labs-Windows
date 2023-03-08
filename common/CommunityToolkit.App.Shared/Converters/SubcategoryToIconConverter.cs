// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen;
using CommunityToolkit.App.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityToolkit.App.Shared.Converters;

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
