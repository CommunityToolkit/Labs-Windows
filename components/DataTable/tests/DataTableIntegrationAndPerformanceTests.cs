using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Controls.DataTable;

namespace DataTable.Tests
{
    [TestClass]
    public class DataTableIntegrationAndPerformanceTests
    {
        [TestMethod]
        public void TestComplexUserInteractionSequence()
        {
            // Simulate complex user interactions such as filtering, sorting, and reordering columns
            var dataTable = new DataTable();
            dataTable.AddColumn("Name");
            dataTable.AddColumn("Age");
            dataTable.AddRow(new object[] { "Alice", 30 });
            dataTable.AddRow(new object[] { "Bob", 25 });
            dataTable.AddRow(new object[] { "Charlie", 35 });

            // Apply multi-column filter
            dataTable.SetFilter("Name", "A");
            dataTable.SetFilter("Age", "3");

            // Reorder columns
            dataTable.ReorderColumn("Age", 0);

            // Assert filtered and reordered results
            var filteredRows = dataTable.GetFilteredRows();
            Assert.AreEqual(1, filteredRows.Count);
            Assert.AreEqual("Alice", filteredRows[0]["Name"]);
            Assert.AreEqual(30, filteredRows[0]["Age"]);
        }

        [TestMethod]
        public void TestPerformanceWithLargeDataSet()
        {
            var dataTable = new DataTable();
            dataTable.AddColumn("ID");
            dataTable.AddColumn("Value");

            int rowCount = 100000;
            for (int i = 0; i < rowCount; i++)
            {
                dataTable.AddRow(new object[] { i, $"Value {i}" });
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Apply a filter that matches a subset
            dataTable.SetFilter("Value", "Value 9999");

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds<edit_file>
<path>components/DataTable/tests/DataTableIntegrationAndPerformanceTests.cs</path>
<content>
<<<<<<< SEARCH
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Controls.DataTable;

namespace DataTable.Tests
{
    [TestClass]
    public class DataTableIntegrationAndPerformanceTests
    {
        [TestMethod]
        public void TestComplexUserInteractionSequence()
        {
            // Simulate complex user interactions such as filtering, sorting, and reordering columns
            var dataTable = new DataTable();
            dataTable.AddColumn("Name");
            dataTable.AddColumn("Age");
            dataTable.AddRow(new object[] { "Alice", 30 });
            dataTable.AddRow(new object[] { "Bob", 25 });
            dataTable.AddRow(new object[] { "Charlie", 35 });

            // Apply multi-column filter
            dataTable.SetFilter("Name", "A");
            dataTable.SetFilter("Age", "3");

            // Reorder columns
            dataTable.ReorderColumn("Age", 0);

            // Assert filtered and reordered results
            var filteredRows = dataTable.GetFilteredRows();
            Assert.AreEqual(1, filteredRows.Count);
            Assert.AreEqual("Alice", filteredRows[0]["Name"]);
            Assert.AreEqual(30, filteredRows[0]["Age"]);
        }

        [TestMethod]
        public void TestPerformanceWithLargeDataSet()
        {
            var dataTable = new DataTable();
            dataTable.AddColumn("ID");
            dataTable.AddColumn("Value");

            int rowCount = 100000;
            for (int i = 0; i < rowCount; i++)
            {
                dataTable.AddRow(new object[] { i, $"Value {i}" });
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Apply a filter that matches a subset
            dataTable.SetFilter("Value", "Value 9999");

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            // Assert performance threshold (e.g., filtering should complete within 500ms)
            Assert.IsTrue(elapsedMs < 500, $"Filtering took too long: {elapsedMs}ms");
        }

    }
}
=======
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Controls.DataTable;

namespace DataTable.Tests
{
    [TestClass]
    public class DataTableIntegrationAndPerformanceTests
    {
        [TestMethod]
        public void TestComplexUserInteractionSequence()
        {
            // Simulate complex user interactions such as filtering, sorting, and reordering columns
            var dataTable = new CommunityToolkit.WinUI.Controls.DataTable.DataTable();
            dataTable.AddColumn("Name");
            dataTable.AddColumn("Age");
            dataTable.AddRow(new object[] { "Alice", 30 });
            dataTable.AddRow(new object[] { "Bob", 25 });
            dataTable.AddRow(new object[] { "Charlie", 35 });

            // Apply multi-column filter
            dataTable.SetFilter("Name", "A");
            dataTable.SetFilter("Age", "3");

            // Reorder columns
            dataTable.ReorderColumn("Age", 0);

            // Assert filtered and reordered results
            var filteredRows = dataTable.GetFilteredRows();
            Assert.AreEqual(1, filteredRows.Count);
            Assert.AreEqual("Alice", filteredRows[0]["Name"]);
            Assert.AreEqual(30, filteredRows[0]["Age"]);
        }

        [TestMethod]
        public void TestPerformanceWithLargeDataSet()
        {
            var dataTable = new CommunityToolkit.WinUI.Controls.DataTable.DataTable();
            dataTable.AddColumn("ID");
            dataTable.AddColumn("Value");

            int rowCount = 100000;
            for (int i = 0; i < rowCount; i++)
            {
                dataTable.AddRow(new object[] { i, $"Value {i}" });
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Apply a filter that matches a subset
            dataTable.SetFilter("Value", "Value 9999");

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            // Assert performance threshold (e.g., filtering should complete within 500ms)
            Assert.IsTrue(elapsedMs < 500, $"Filtering took too long: {elapsedMs}ms");
        }

        // Placeholder tests for export, grouping, aggregation, keyboard navigation
        // These should be implemented as the features are developed

        [TestMethod]
        public void TestExportFunctionality_Placeholder()
        {
            Assert.Inconclusive("Export functionality tests are not implemented yet.");
        }

        [TestMethod]
        public void TestGroupingFunctionality_Placeholder()
        {
            Assert.Inconclusive("Grouping functionality tests are not implemented yet.");
        }

        [TestMethod]
        public void TestAggregationFunctionality_Placeholder()
        {
            Assert.Inconclusive("Aggregation functionality tests are not implemented yet.");
        }

        [TestMethod]
        public void TestKeyboardNavigation_Placeholder()
        {
            Assert.Inconclusive("Keyboard navigation tests are not implemented yet.");
        }
    }
}
