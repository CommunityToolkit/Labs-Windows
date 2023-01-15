// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace TokenViewExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSampleBoolOption("CanRemoveTokens", "Can tokens be removed", false)]

[ToolkitSample(id: nameof(TokenViewRemoveSample), "Remove sample", description: $"A sample for showing how to create and use a {nameof(TokenView)} control.")]
public sealed partial class TokenViewRemoveSample : Page
{
    public ObservableCollection<MyDataModel> MyDataSet = new() {
        new()
        {
            Name = "Item 1",
        },
        new()
        {
            Name = "Item 2",
        },
        new()
        {
            Name = "Item 3",
        },
        new()
        {
            Name = "Item 4",
        },
        new()
        {
            Name = "Item 5",
        },
        new()
        {
            Name = "Item 6",
        },
        new()
        {
            Name = "Item 7",
        },
        new()
        {
            Name = "Item 8",
        },
        new()
        {
            Name = "Item 9",
        },
        new()
        {
            Name = "Item 10",
        },
        new()
        {
            Name = "Item 11",
        },
        new()
        {
            Name = "Item 12",
        },
    };
    public TokenViewRemoveSample()
    {
        this.InitializeComponent();
    }
}

public class MyDataModel
{
    public string? Name { get; set; }
}
