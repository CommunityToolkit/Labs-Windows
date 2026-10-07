// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace DataTableExperiment.Samples;

[ToolkitSample(id: nameof(DataRowWithoutDataTableSample), "DataRows without DataTable Example", description: $"A sample for showing the default layout of {nameof(DataRow)} withou {nameof(DataTable)} control.")]
public sealed partial class DataRowWithoutDataTableSample : Page
{
    public const int NumberOfRows = 6;

    public ObservableCollection<InventoryItem> InventoryItems { get; set; }

    public DataRowWithoutDataTableSample()
    {
        InventoryItem[] items = new InventoryItem[NumberOfRows];

        for (int i = 0; i < NumberOfRows; i++)
        {
            items[i] = new()
            {
                Id = i,
                Name = i.ToString(),
                Description = i.ToString(),
                Quantity = i,
            };
        }

        items[3].Name = "Hello, testing!";

        items[5].Description = "This is a very long description that should have been out of view at the start...";

        InventoryItems = new(items);

        this.InitializeComponent();
    }
}
