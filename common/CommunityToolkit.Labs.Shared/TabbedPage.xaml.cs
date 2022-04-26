using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

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

namespace CommunityToolkit.Labs.Shared
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabbedPage : Page
    {
        public TabbedPage()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<object> Items { get; } = new();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var info = e.Parameter as (IEnumerable<ToolkitSampleMetadata> Samples, IEnumerable<ToolkitFrontMatter> Docs, bool AreDocsFirst)?;

            if (info is not null)
            {
                if (info.Value.AreDocsFirst)
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
            }

            base.OnNavigatedTo(e);
        }
    }

    public class DocOrSampleTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? Document { get; set; }
        public DataTemplate? Sample { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ToolkitFrontMatter)
            {
                return Document!;
            }
            else
            {
                return Sample!;
            }
        }
    }
}
