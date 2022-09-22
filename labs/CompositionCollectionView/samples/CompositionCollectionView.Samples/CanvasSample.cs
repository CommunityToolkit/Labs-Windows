// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.WinUI;
using Microsoft.Toolkit.Uwp.UI.Animations.ExpressionsFork;
using System.Numerics;
using System.Xml.Linq;

#if !WINAPPSDK
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    [ToolkitSample(id: nameof(CanvasSample), "Canvas layout", description: "DisplayS elements in a 2d canvas, animating between updates.")]
    public sealed partial class CanvasSample : Page
    {
        public CanvasSample()
        {
            this.InitializeComponent();

#if !WINAPPSDK
            var elements = new Dictionary<uint, Vector2>()
            {
                { 0, Vector2.Zero },
                { 1, Vector2.Zero },
                { 2, Vector2.Zero },
                { 3, Vector2.Zero },
                { 4, Vector2.Zero }
            };

            var layout = new CanvasLayout((id) =>
                new Rectangle()
                {
                    Width = 100,
                    Height = 100,
                    Fill = new SolidColorBrush(Windows.UI.Colors.CornflowerBlue),
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = 1
                }
            , (_) => { });

            compositionCollectionView.SetLayout(layout);
            compositionCollectionView.UpdateSource(elements);

            rearrangeButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var rnd = new Random();

                for(uint i =0; i< elements.Count; i++)
                {
                    elements[i] = new Vector2(
                        (float)(rnd.NextDouble() * compositionCollectionView.ActualWidth),
                        (float)(rnd.NextDouble() * compositionCollectionView.ActualHeight));
                }
                compositionCollectionView.UpdateSource(elements);
            };
#endif
        }

#if !WINAPPSDK
        public class CanvasLayout : CompositionCollectionLayout<uint, Vector2>
        {
            public CanvasLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            public override Vector3Node GetElementPositionNode(ElementReference<uint, Vector2> element)
            {
                return new Vector3(element.Model.X, element.Model.Y, 0);
            }

            protected override Transition GetElementTransitionEasingFunction(ElementReference<uint, Vector2> element) =>
               new(200,
                   Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f)));
        }
#endif
    }
}
