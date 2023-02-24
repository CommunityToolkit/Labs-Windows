// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Metadata;
using CommunityToolkit.App.Shared.Helpers;

#if !WINAPPSDK
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#endif

namespace CommunityToolkit.App.Shared.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TabbedPage : Page
{
    public TabbedPage()
    {
        this.InitializeComponent();

        BackgroundHelper.SetBackground(this);
    }


    public ObservableCollection<object> Items { get; } = new();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        // Note: Need to use as for tuple, think this is the tracking issue here: https://github.com/dotnet/csharplang/issues/3197
        var info = e.Parameter as (IEnumerable<ToolkitSampleMetadata> Samples, IEnumerable<ToolkitFrontMatter> Docs, bool AreDocsFirst)?;

        if (info is null)
        {
            return;
        }
        else if (info.Value.AreDocsFirst)
        {
            foreach (var item in info.Value.Docs)
            {
                Items.Add(item);
            }
        }

        foreach (var item in info.Value.Samples)
        {
            Items.Add(item);
        }

        if (!info.Value.AreDocsFirst)
        {
            foreach (var item in info.Value.Docs)
            {
                Items.Add(item);
            }
        }

        base.OnNavigatedTo(e);
    }
}
