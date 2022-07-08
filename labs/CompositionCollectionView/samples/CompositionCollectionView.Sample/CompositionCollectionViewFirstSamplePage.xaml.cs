// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.WinUI.CompositionCollectionView;
using Microsoft.Toolkit.Uwp.UI.Animations.ExpressionsFork;

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
    [ToolkitSample(id: nameof(CompositionCollectionViewFirstSamplePage), "Simple layout", description: "Displaying elements in a simple layout.")]
    public sealed partial class CompositionCollectionViewFirstSamplePage : Page
    {
        private List<(uint, Action<CompositionPropertySet, Dictionary<string, object>>)> elements { get; init; } = new()
            {
                (0, (_, _)=>{ }),
                (1, (_, _)=>{ }),
                (2, (_, _)=>{ }),
                (3, (_, _)=>{ })
            };

        public CompositionCollectionViewFirstSamplePage()
        {
            this.InitializeComponent();

            var layout = new SampleLayout((id) =>
                new Rectangle()
                {
                    Width = 100,
                    Height = 100,
                    Fill = new SolidColorBrush(Windows.UI.Colors.BlueViolet)
                }
            , (_) => { });
            compositionCollectionView.SetLayout(layout);
            compositionCollectionView.UpdateSource(elements);
        }

        public class SampleLayout : Layout<uint>
        {
            public SampleLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            public override Vector3Node GetElementPositionNode(ElementReference<uint> element)
            {
                return ExpressionFunctions.Vector3(element.Id * 120, 0, 0);
            }

            public override ScalarNode GetElementScaleNode(ElementReference<uint> element) => 1;

            protected override void ConfigureElement(ElementReference<uint> element)
            {
            }

            public override void UpdateElement(ElementReference<uint> element)
            {
            }
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            elements.Add(((uint)elements.Count, (_, _) => { }));
            compositionCollectionView.UpdateSource(elements);
        }
    }
}
