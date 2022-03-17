using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using CommunityToolkit.Labs.Core.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

#if !WINAPPSDK
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#else
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;
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
    /// Kicks off the loading process and determines whether to display a single-sample or multi-sample view.
    /// </summary>
    public sealed partial class AppLoadingView : Page
    {
        /// <summary>
        /// Creates a new instance of <see cref="AppLoadingView"/>.
        /// </summary>
        public AppLoadingView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Backing dependency property for <see cref="IsLoading"/>.
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(AppLoadingView), new PropertyMetadata(false));

        /// <summary>
        /// Backing dependency property for <see cref="LoadingMessage"/>.
        /// </summary>
        public static readonly DependencyProperty LoadingMessageProperty =
            DependencyProperty.Register(nameof(LoadingMessage), typeof(string), typeof(AppLoadingView), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets a value indicating whether loading operations are being performed.
        /// </summary>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the displayed loading message.
        /// </summary>
        public string LoadingMessage
        {
            get { return (string)GetValue(LoadingMessageProperty); }
            set { SetValue(LoadingMessageProperty, value); }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            IsLoading = true;
            LoadingMessage = "Loading...";

            var samplePages = FindReferencedSamplePages().ToArray();

            if (samplePages.Length == 0)
            {
                IsLoading = false;
                LoadingMessage = "No sample pages were found :(";
                return;
            }

            if (samplePages.Length == 1)
            {
                // Individual samples are UserControls,
                // but multi-sample view and grouped sample views should be a Page.
                // TODO: Remove after creating grouped-sample view.
                if (!samplePages[0].SampleControlType.IsSubclassOf(typeof(Page)))
                {
                    // MacOS and iOS don't know the correct type without a full namespace declaration, confusing it with NSWindow.
                    // Using static will not work.
#if WINAPPSDK
                    var currentWindow = Microsoft.UI.Xaml.Window.Current;
#else
                    var currentWindow = Windows.UI.Xaml.Window.Current;
#endif
                    currentWindow.Content = (UIElement)samplePages[0].SampleControlFactory();
                    return;
                }

                ScheduleNavigate(samplePages[0].SampleControlType);
                return;
            }

            if (samplePages.Length > 1)
            {
                ScheduleNavigate(typeof(MainPage), samplePages);
                return;
            }
        }

        // Needed because Frame.Navigate doesn't work inside of the OnNavigatedTo override.
        private void ScheduleNavigate(Type type, object param = null)
        {
            DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
            {
#if !NETFX_CORE
                Frame.Navigate(type, param);
#else
                Frame.NavigateToType(type, param, new FrameNavigationOptions { IsNavigationStackEnabled = false });
#endif
            });
        }

        private IEnumerable<ToolkitSampleMetadata> FindReferencedSamplePages()
        {
            return ToolkitSampleRegistry.Execute();
        }
    }
}
