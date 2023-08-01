// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace DataTableExperiment.Samples;

[ToolkitSample(id: nameof(DataTableBlankHeaderSample), "DataTable Blank Header Example", description: $"A sample for showing how to create and use a {nameof(DataRow)} control without a header.")]
public sealed partial class DataTableBlankHeaderSample : Page
{
    public ObservableCollection<InventoryItem> InventoryItems { get; set; } = new()
    {
        new()
        {
            Id = 1002,
            Name = "Hydra",
            Description = "Multiple Launch Rocket System-2 Hydra",
            Quantity = 1,
        },
        new()
        {
            Id = 3456,
            Name = "MA40 AR",
            Description = "Regular assault rifle - updated version of MA5B or MA37 AR",
            Quantity = 4,
        },
        new()
        {
            Id = 5698,
            Name = "Needler",
            Description = "Alien weapon well-known for its iconic design with pink crystals",
            Quantity = 2,
        },
        new()
        {
            Id = 7043,
            Name = "Ravager",
            Description = "An incendiary plasma launcher",
            Quantity = 1,
        },
        // TODO: Add more items, maybe abstract these to a helper for other samples?
    };

    public DataTableBlankHeaderSample()
    {
        this.InitializeComponent();
    }
}
