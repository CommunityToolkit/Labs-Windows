// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;

namespace SettingsControlsExperiment.Samples;

[ToolkitSample(id: nameof(SettingsExpanderItemsSourceSample), "SettingsExpanderItemsSource", description: "The SettingsExpander can also be filled with items based on a collection.")]
public sealed partial class SettingsExpanderItemsSourceSample : Page
{

    public ObservableCollection<MyDataModel> MyDataSet = new () {
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

    public SettingsExpanderItemsSourceSample()
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
