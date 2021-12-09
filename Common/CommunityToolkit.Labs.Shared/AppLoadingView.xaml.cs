using CommunityToolkit.Labs.Core;
using CommunityToolkit.Labs.Core.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
                ScheduleNavigate(samplePages[0].Type);
                return;
            }

            if (samplePages.Length > 1)
            {
                ScheduleNavigate(typeof(MainPage), samplePages);
                return;
            }
        }

        // Needed because Frame.Navigate doesn't work inside of the OnNavigatedTo override.
        private void ScheduleNavigate(Type type, object? param = null)
        {
            _ = Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
#if __WASM__
                Frame.Navigate(type, param);
#else
                Frame.NavigateToType(type, param, new FrameNavigationOptions { IsNavigationStackEnabled = false });
#endif
            });
        }

        private IEnumerable<ToolkitSampleMetadata> FindReferencedSamplePages()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                // Sample projects are templated and must contain the word "sample".
                // Skip iterating non-sample assemblies.
                if (!assembly.FullName.ToLowerInvariant().Contains("sample"))
                    continue;

                foreach (var type in assembly.ExportedTypes)
                {
                    // Sample pages must derive from Page.
                    if (!type.IsSubclassOf(typeof(Page)))
                        continue;

                    var attributes = type.GetCustomAttributes(typeof(ToolkitSampleAttribute), false).Cast<ToolkitSampleAttribute>();

                    foreach (var attribute in attributes)
                        yield return new ToolkitSampleMetadata(attribute.DisplayName, attribute.Description, type);
                }
            }
        }
    }
}
