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
            Name = "First Item",
            Info = "More about first item.",
            LinkDescription = "Click here for more on first item.",
            Url = "https://microsoft.com/",
        },
        new()
        {
            Name = "Second Item",
            Info = "More about second item.",
            LinkDescription = "Click here for more on second item.",
            Url = "https://xbox.com/",
        },
        new()
        {
            Name = "Third Item",
            Info = "More about third item.",
            LinkDescription = "Click here for more on third item.",
            Url = "https://toolkitlabs.dev/",
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

    public string? Info { get; set; }

    public string? LinkDescription { get; set; }

    public string? Url { get; set; }
}
