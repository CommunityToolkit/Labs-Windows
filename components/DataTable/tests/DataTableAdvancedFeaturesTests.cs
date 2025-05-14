// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET5_0_WINDOWS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.WinUI.Controls;
using System.Collections.Generic;
using System.Linq;

namespace DataTableTests
{
    [TestClass]
    public class DataTableAdvancedFeaturesTests
    {
        [TestMethod]
        public void MultiColumnFiltering_WorksCorrectly()
        {
            var dataTable = new DataTable();

            var column1 = new DataColumn();
            var column2 = new DataColumn();
            dataTable.Columns.Add(column1);
            dataTable.Columns.Add(column2);

            var row1 = new DataRow();
            var row2 = new DataRow();
            var row3 = new DataRow();

            row1.SetValue(column1, 5);
            row1.SetValue(column2, "A");

            row2.SetValue(column1, 10);
            row2.SetValue(column2, "B");

            row3.SetValue(column1, 15);
            row3.SetValue(column2, "A");

            dataTable.Rows.Add(row1);
            dataTable.Rows.Add(row2);
            dataTable.Rows.Add(row3);

            // Filter column1 for values greater than 7
            column1.FilterPredicate = value => (int)value! > 7;
            // Filter column2 for values equal to "A"
            column2.FilterPredicate = value => (string)value! == "A";

            dataTable.ApplyFilters();

            var filteredRows = dataTable.Rows.Where(r =>
                column1.FilterPredicate!(r.GetValue(column1)) &&
                column2.FilterPredicate!(r.GetValue(column2))).ToList();

            Assert.AreEqual(1, filteredRows.Count);
            Assert.AreEqual(row3, filteredRows[0]);
        }

        [TestMethod]
        public void ColumnReordering_UpdatesColumnsCollection()
        {
            var dataTable = new DataTable();

            var column1 = new DataColumn();
            var column2 = new DataColumn();
            var column3 = new DataColumn();

            dataTable.Columns.Add(column1);
            dataTable.Columns.Add(column2);
            dataTable.Columns.Add(column3);

            dataTable.ReorderColumns(0, 2);

            Assert.AreEqual(column2, dataTable.Columns[0]);
            Assert.AreEqual(column3, dataTable.Columns[1]);
            Assert.AreEqual(column1, dataTable.Columns[2]);
        }
    }
}
#endif
