// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET5_0_WINDOWS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UI.Xaml;
using CommunityToolkit.WinUI.Controls;
using System.Linq;

namespace DataTableTests;

[TestClass]
public class DataTableExtendedTests
{
    [TestMethod]
    public void SortByColumn_SortsRowsAscending()
    {
        var dataTable = new DataTable();

        var column = new DataColumn();
        dataTable.Children.Add(column);

        var row1 = new DataRow();
        var row2 = new DataRow();
        var row3 = new DataRow();

        row1.SetValue(column, 3);
        row2.SetValue(column, 1);
        row3.SetValue(column, 2);

        dataTable.Rows.Add(row1);
        dataTable.Rows.Add(row2);
        dataTable.Rows.Add(row3);

        dataTable.SortByColumn(column, true);

        var sortedValues = dataTable.Rows.Select(r => r.GetValue(column)).ToList();
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, sortedValues);
    }

    [TestMethod]
    public void SortByColumn_SortsRowsDescending()
    {
        var dataTable = new DataTable();

        var column = new DataColumn();
        dataTable.Children.Add(column);

        var row1 = new DataRow();
        var row2 = new DataRow();
        var row3 = new DataRow();

        row1.SetValue(column, 3);
        row2.SetValue(column, 1);
        row3.SetValue(column, 2);

        dataTable.Rows.Add(row1);
        dataTable.Rows.Add(row2);
        dataTable.Rows.Add(row3);

        dataTable.SortByColumn(column, false);

        var sortedValues = dataTable.Rows.Select(r => r.GetValue(column)).ToList();
        CollectionAssert.AreEqual(new[] { 3, 2, 1 }, sortedValues);
    }

    [TestMethod]
    public void SetColumnVisibility_HidesAndShowsColumn()
    {
        var dataTable = new DataTable();

        var column = new DataColumn();
        dataTable.Children.Add(column);

        dataTable.SetColumnVisibility(column, false);
        Assert.AreEqual(Visibility.Collapsed, column.Visibility);

        dataTable.SetColumnVisibility(column, true);
        Assert.AreEqual(Visibility.Visible, column.Visibility);
    }

    [TestMethod]
    public void GetColumnVisibility_ReturnsCorrectVisibility()
    {
        var dataTable = new DataTable();

        var column = new DataColumn();
        dataTable.Children.Add(column);

        column.Visibility = Visibility.Visible;
        Assert.IsTrue(dataTable.GetColumnVisibility(column));

        column.Visibility = Visibility.Collapsed;
        Assert.IsFalse(dataTable.GetColumnVisibility(column));
    }
}
#endif
