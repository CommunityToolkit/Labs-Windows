// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.WinUI.CompositionCollectionView;
using Microsoft.Toolkit.Uwp.UI.Animations.ExpressionsFork;
using System.Xml.Linq;

#if !WINAPPSDK
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#endif

namespace CompositionCollectionView.Sample
{
    [ToolkitSample(id: nameof(BasicSample), "Simple layout", description: "Displaying elements in a simple layout.")]
    public sealed partial class BasicSample : Page
    {
        public BasicSample()
        {
            this.InitializeComponent();

            var elements = new Dictionary<uint, object?>()
            {
                { 0, null },
                { 1, null },
                { 2, null },
                { 3, null },
                { 4, null }
            };

            var layout = new SampleLayout((id) =>
                new Rectangle()
                {
                    Width = 100,
                    Height = 100,
                    Fill = new SolidColorBrush(Windows.UI.Colors.CornflowerBlue)
                }
            , (_) => { });
            compositionCollectionView.SetLayout(layout);
            compositionCollectionView.UpdateSource(elements);

            addButton.Click += (object sender, RoutedEventArgs e) =>
            {
                elements.Add((uint)elements.Count, null);
                compositionCollectionView.UpdateSource(elements);
            };
        }

        public class SampleLayout : Layout<uint, object?>
        {
            public SampleLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            public override Vector3Node GetElementPositionNode(ElementReference<uint, object?> element)
            {
                return ExpressionFunctions.Vector3(element.Id * 120, 0, 0);
            }

            public override ScalarNode GetElementScaleNode(ElementReference<uint, object?> element) => 1;
        }
    }
}
