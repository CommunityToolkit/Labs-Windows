// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace DataTableExperiment.Samples;

[ToolkitSample(id: nameof(DataTableVirtualizationSample), "DataTable Virtualization Example", description: $"A sample for showing how to create and use a {nameof(DataTable)} control with many rows.")]
public sealed partial class DataTableVirtualizationSample : Page
{
    public const int NumberOfRows = 10000;

    public ObservableCollection<InventoryItem> InventoryItems { get; set; }

    public DataTableVirtualizationSample()
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

        items[6].Name = "Hello, testing!";

        items[1500].Description = "This is a very long description that should have been out of view at the start...";

        InventoryItems = new(items);

        this.InitializeComponent();
    }
}
