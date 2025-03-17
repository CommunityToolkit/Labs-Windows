// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.TestGen;
using CommunityToolkit.Tests;
using CommunityToolkit.WinUI.Controls;

namespace DataTableTests;

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
        // Setup: Find all rows in our table
        var rows = page.FindDescendants().OfType<DataRow>();

        double? targetWidth = null;
        int index = 0;
        bool found = false;

        foreach (DataRow row in rows)
        {
            var itemBorder = row.FindDescendant<Border>();

            Assert.IsNotNull(itemBorder);
            Assert.AreEqual("BorderItem", itemBorder.Tag, "Couldn't find proper border item");
            Assert.IsTrue(itemBorder.ActualWidth >= 50, "Border should be equal to or larger than MinWidth");

            targetWidth ??= itemBorder.ActualWidth;

            // All columns should have the same width
            Assert.AreEqual(targetWidth.Value, itemBorder.ActualWidth, 1.0, $"Column for row at index {index}, is wrong size");

            // Check if this is our 'longest' textbox
            var longItemTextBlock = row.FindDescendant<TextBlock>();
            if (longItemTextBlock?.Text == "SuperLongString")
            {
                Assert.AreEqual(targetWidth.Value, longItemTextBlock.ActualWidth, 1.0, "Column not same size as longest textblock");
                found = true;
            }
        }

        Assert.IsTrue(found, "Didn't find longest textblock to check measurement.");

        // Test: Check the column is the appropriate size
        var column = page.FindDescendant<DataColumn>();

        Assert.IsNotNull(column);
        Assert.AreEqual(targetWidth!.Value, column.ActualWidth, 1.0, "Column didn't autosize to match");
    }
}
