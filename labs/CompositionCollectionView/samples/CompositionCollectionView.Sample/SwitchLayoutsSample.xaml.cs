// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.WinUI.CompositionCollectionView;
using Microsoft.Toolkit.Uwp.UI.Animations.ExpressionsFork;
using System.Numerics;

#if !WINAPPSDK
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
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
    [ToolkitSample(id: nameof(SwitchLayoutsSample), "Layout transition", description: "Transition between different layouts.")]
    public sealed partial class SwitchLayoutsSample : Page
    {
        private List<(uint, Action<CompositionPropertySet, Dictionary<string, object>>)> elements { get; init; } = new()
            {
                (0, (_, _)=>{ }),
                (1, (_, _)=>{ }),
                (2, (_, _)=>{ }),
                (3, (_, _)=>{ }),
                (4, (_, _)=>{ }),
                (5, (_, _)=>{ })
            };

        public SwitchLayoutsSample()
        {
            this.InitializeComponent();

            var layout = new LinearLayout((id) =>
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

        public class LinearLayout : Layout<uint>
        {
            public LinearLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            public LinearLayout(Layout<uint> sourceLayout) : base(sourceLayout)
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

            protected override Transition GetElementTransitionEasingFunction(ElementReference<uint> element) =>
               new(100,
                   Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f)));
        }

        public class StackLayout : Layout<uint>
        {
            public StackLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            public StackLayout(Layout<uint> sourceLayout) : base(sourceLayout)
            {
            }

            public override Vector3Node GetElementPositionNode(ElementReference<uint> element)
            {
                return ExpressionFunctions.Vector3(element.Id * 10, element.Id * 10, 0);
            }

            public override ScalarNode GetElementScaleNode(ElementReference<uint> element) => (float)Math.Pow(0.95f, element.Id);

            protected override void ConfigureElement(ElementReference<uint> element)
            {
            }

            public override void UpdateElement(ElementReference<uint> element)
            {
            }

            protected override Transition GetElementTransitionEasingFunction(ElementReference<uint> element) =>
                new(100,
                    Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f)));
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggle)
            {
                if (toggle.IsOn && compositionCollectionView.Layout<uint>() is LinearLayout currentLinearLayout)
                {
                    currentLinearLayout.TransitionTo(_ => new StackLayout(currentLinearLayout));
                }
                else if (!toggle.IsOn && compositionCollectionView.Layout<uint>() is StackLayout currentStackLayout)
                {
                    currentStackLayout.TransitionTo(_ => new LinearLayout(currentStackLayout));
                }
            }
        }
    }
}
