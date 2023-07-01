// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace DataTableExperiment.Samples;

[ToolkitSample(id: nameof(TreeTableSample), "DataTable as Tree Example", description: $"A sample for showing how to create and use a {nameof(DataTable)} control with a TreeView.")]
public sealed partial class TreeTableSample : Page
{
    public ObservableCollection<InventoryItem> InventoryItems { get; set; } = new()
    {
        new()
        {
            Id = 1,
            Name = "Laptop",
            Description = "A portable computer",
            Quantity = 10,
            SubItems = new List<InventoryItem>
            {
                new()
                {
                    Id = 11,
                    Name = "Battery",
                    Description = "A rechargeable power source",
                    Quantity = 20,
                    SubItems = new List<InventoryItem>
                    {
                        new()
                        {
                            Id = 111,
                            Name = "Anode",
                            Description = "Oxidized electrodes made of powered zinc.",
                            Quantity = 10
                        },
                        new()
                        {
                            Id = 112,
                            Name = "Cathode",
                            Description = "Electrode reduced by chemical reaction made of carbon.",
                            Quantity = 10
                        }
                    }
                },
                new()
                {
                    Id = 12,
                    Name = "Charger",
                    Description = "A device that plugs into an outlet and charges the battery",
                    Quantity = 15
                }
            }
        },
        new()
        {
            Id = 2,
            Name = "Printer",
            Description = "A device that prints documents",
            Quantity = 5,
            SubItems = new List<InventoryItem>
            {
                new()
                {
                    Id = 21,
                    Name = "Ink cartridge",
                    Description = "A container that holds ink for printing",
                    Quantity = 30
                },
                new()
                {
                    Id = 22,
                    Name = "Paper",
                    Description = "A thin material that can be printed on",
                    Quantity = 100
                }
            }
        },
        // Added two more items
        new()
        {
            Id = 3,
            Name = "Mouse",
            Description = "A device that controls the cursor on the screen",
            Quantity = 8,
            SubItems = new List<InventoryItem>
            {
                new()
                {
                    Id = 31,
                    Name = "USB cable",
                    Description = "A cable that connects the mouse to the computer",
                    Quantity = 10
                },
                new()
                {
                    Id = 32,
                    Name = "Battery",
                    Description = "A power source for wireless mice",
                    Quantity = 12,
                    SubItems = new List<InventoryItem>
                    {
                        new()
                        {
                            Id = 321,
                            Name = "Anode",
                            Description = "Oxidized electrodes made of powered zinc.",
                            Quantity = 10
                        },
                        new()
                        {
                            Id = 322,
                            Name = "Cathode",
                            Description = "Electrode reduced by chemical reaction made of carbon.",
                            Quantity = 10
                        }
                    }
                }
            }
        },
        new()
        {
            Id = 4,
            Name = "Keyboard",
            Description = "A device that allows typing text and commands",
            Quantity = 6,
            SubItems = new List<InventoryItem>
            {
                new()
                {
                    Id = 41,
                    Name = "USB cable",
                    Description = "A cable that connects the keyboard to the computer",
                    Quantity = 8
                },
                new()
                {
                    Id = 42,
                    Name = "Battery",
                    Description = "A power source for wireless keyboards",
                    Quantity = 10,
                    SubItems = new List<InventoryItem>
                    {
                        new()
                        {
                            Id = 421,
                            Name = "Anode",
                            Description = "Oxidized electrodes made of powered zinc.",
                            Quantity = 10
                        },
                        new()
                        {
                            Id = 422,
                            Name = "Cathode",
                            Description = "Electrode reduced by chemical reaction made of carbon.",
                            Quantity = 10
                        }
                    }
                }
            }
        }
    };

    public TreeTableSample()
    {
        this.InitializeComponent();
    }
}
