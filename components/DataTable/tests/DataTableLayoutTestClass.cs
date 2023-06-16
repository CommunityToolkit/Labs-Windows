// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.TestGen;
using CommunityToolkit.Tests;
using CommunityToolkit.WinUI.Controls;

namespace DataTableExperiment.Tests;

/// <summary>
/// Class for testing various aspects of layout in relation to DataTable components.
/// </summary>
[TestClass]
public partial class DataTableLayoutTestClass : VisualUITestBase
{
    // The UIThreadTestMethod automatically dispatches to the UI for us to work with UI objects.
    [UIThreadTestMethod]
    public void SimpleUIAttributeExampleTest()
    {
        var component = new DataRow();
        Assert.IsNotNull(component);
    }

    // The UIThreadTestMethod can also easily grab a XAML Page for us by passing its type as a parameter.
    // This lets us actually test a control as it would behave within an actual application.
    // The page will already be loaded by the time your test is called.
    [UIThreadTestMethod]
    public void DataColumnDataRowAutoSizeTest(DataTableColumnAutoSizeTestPage page)
    {
        // Setup: Check the core item we want to adapt to
        var row = page.FindDescendant<DataRow>();

        Assert.IsNotNull(row);

        var itemBorder = row.FindDescendant<Border>();

        Assert.IsNotNull(itemBorder);
        Assert.AreEqual("BorderItem", itemBorder.Tag, "Couldn't find proper border item");

        Assert.IsTrue(itemBorder.ActualWidth > 50, "Border should be larger than MinWidth");

        var longItemTextBlock = itemBorder.FindDescendant<TextBlock>();

        Assert.IsNotNull(longItemTextBlock, "Couldn't find long text textblock");
        Assert.AreEqual("SuperLongString", longItemTextBlock.Text, "Didn't find right textblock");

        var targetWidth = longItemTextBlock.ActualWidth; // or desired size?

        // Test: Check the column is the appropriate size
        var column = page.FindDescendant<DataColumn>();

        Assert.IsNotNull(column);
        Assert.AreEqual(targetWidth, column.ActualWidth, 1.0, "Column didn't autosize");

        // Test2: Check the other row is the same size too.

        var row2 = page.FindDescendants().Where(element => element is DataRow).Skip(1).FirstOrDefault() as DataRow;

        Assert.IsNotNull(row2, "Couldn't find short row");
        var shortItemTextBlock = row2.FindDescendant<TextBlock>();

        Assert.IsNotNull(shortItemTextBlock);
        Assert.AreEqual("Short", shortItemTextBlock.Text, "Couldn't find short text");

        var borderShort = row2.FindDescendant<Border>();

        Assert.IsNotNull(borderShort);
        Assert.AreEqual(targetWidth, borderShort.ActualWidth, 1.0, "Other row didn't match size of larger row");
    }
}
