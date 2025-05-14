// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET5_0_WINDOWS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.WinUI.Controls;

namespace DataTableTests;

[TestClass]
public class DataTableIsAnyColumnAutoCacheTests
{
    [TestMethod]
    public void IsAnyColumnAuto_CachesValueCorrectly()
    {
        var dataTable = new DataTable();

        var column1 = new DataColumn();
        column1.CurrentWidth = new GridLength(100, GridUnitType.Pixel);

        var column2 = new DataColumn();
        column2.CurrentWidth = new GridLength(1, GridUnitType.Auto);

        dataTable.Children.Add(column1);
        dataTable.Children.Add(column2);

        // Initially, IsAnyColumnAuto should be true because column2 is Auto
        Assert.IsTrue(dataTable.IsAnyColumnAuto);

        // Change column2 to Pixel, cache should update and IsAnyColumnAuto should be false
        column2.CurrentWidth = new GridLength(50, GridUnitType.Pixel);
        Assert.IsFalse(dataTable.IsAnyColumnAuto);

        // Change column1 to Auto, cache should update and IsAnyColumnAuto should be true
        column1.CurrentWidth = new GridLength(1, GridUnitType.Auto);
        Assert.IsTrue(dataTable.IsAnyColumnAuto);
    }

    [TestMethod]
    public void IsAnyColumnAuto_UpdatesCacheWhenChildrenChange()
    {
        var dataTable = new DataTable();

        var column1 = new DataColumn();
        column1.CurrentWidth = new GridLength(100, GridUnitType.Pixel);

        dataTable.Children.Add(column1);

        // Initially, no Auto columns
        Assert.IsFalse(dataTable.IsAnyColumnAuto);

        var column2 = new DataColumn();
        column2.CurrentWidth = new GridLength(1, GridUnitType.Auto);

        dataTable.Children.Add(column2);

        // After adding Auto column, cache should update
        Assert.IsTrue(dataTable.IsAnyColumnAuto);

        dataTable.Children.Remove(column2);

        // After removing Auto column, cache should update
        Assert.IsFalse(dataTable.IsAnyColumnAuto);
    }
}
#endif
