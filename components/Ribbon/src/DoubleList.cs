// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Foundation.Metadata;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A list of <see cref="double"/> values.
/// </summary>
[CreateFromString(MethodName = "CommunityToolkit.WinUI.Controls.DoubleList.CreateFromString")]
public class DoubleList : List<double>
{
    /// <summary>
    /// Initializes a new instance of <see cref="DoubleList"/> that is empty and has the default
    /// initial capacity.
    /// </summary>
    public DoubleList()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DoubleList"/> that contains elements copied from
    /// the specified collection and has sufficient capacity to accommodate the number of elements
    /// copied.
    /// </summary>
    /// <param name="values">The collection whose elements are copied to the new list.</param>
    public DoubleList(IEnumerable<double> values)
        : base(values)
    {
    }

    /// <summary>
    /// Create a <see cref="DoubleList"/> from the <paramref name="value"/> string.
    /// </summary>
    /// <param name="value">The list of doubles separated by ','.</param>
    /// <returns>A <see cref="DoubleList"/> instance with the content of <paramref name="value"/>.</returns>
    public static DoubleList CreateFromString(string value)
    {
        var list = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(double.Parse);
        return new DoubleList(list);
    }
}
