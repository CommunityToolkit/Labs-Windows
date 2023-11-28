// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace DataTableExperiment.Samples;

[ToolkitSample(id: nameof(DataTableDynamicSample), "DataTable Dynamic Columns Example", description: $"A sample for showing how to create and use a {nameof(DataTable)} control with dynamic columns.")]
public sealed partial class DataTableDynamicSample : Page
{
    public ObservableCollection<string> ColumnInfo { get; set; } = new()
    {
        "One",
        "Two",
        "Three",
        "Four",
    };

    public ObservableCollection<List<int>> DataItems { get; set; } = new()
    {
        new()
        {
            1, 2, 3, 4
        },
        new()
        {
            5, 6, 7, 8
        },
        new()
        {
            9, 10, 11, 12
        },
        new()
        {
            13, 14, 15, 16
        },
    };

    public DataTableDynamicSample()
    {
        this.InitializeComponent();
    }
}
