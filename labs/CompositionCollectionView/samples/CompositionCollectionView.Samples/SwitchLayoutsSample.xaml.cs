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
        public SwitchLayoutsSample()
        {
            this.InitializeComponent();

#if !WINAPPSDK
            Dictionary<uint, object?> elements = new()
            {
                { 0, null },
                { 1, null },
                { 2, null },
                { 3, null },
                { 4, null }
            };

            var layout = new LinearLayout((id) =>
                new Rectangle()
                {
                    Width = 100,
                    Height = 100,
                    Fill = new SolidColorBrush(Windows.UI.Colors.CornflowerBlue)
                }
            , (_) => { });
            compositionCollectionView.SetLayout(layout);
            compositionCollectionView.UpdateSource(elements);
#endif
        }

#if !WINAPPSDK
        public class LinearLayout : Layout<uint, object?>
        {
            public LinearLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            public LinearLayout(Layout<uint, object?> sourceLayout) : base(sourceLayout)
            {
            }

            public override Vector3Node GetElementPositionNode(ElementReference<uint, object?> element)
            {
                return ExpressionFunctions.Vector3(element.Id * 120, 0, 0);
            }

            public override ScalarNode GetElementScaleNode(ElementReference<uint, object?> element) => 1;

            protected override Transition GetElementTransitionEasingFunction(ElementReference<uint, object?> element) =>
               new(100,
                   Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f)));
        }

        public class StackLayout : Layout<uint, object?>
        {
            public StackLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            public StackLayout(Layout<uint, object?> sourceLayout) : base(sourceLayout)
            {
            }

            public override Vector3Node GetElementPositionNode(ElementReference<uint, object?> element)
            {
                return ExpressionFunctions.Vector3(element.Id * 10, element.Id * 10, 0);
            }

            public override ScalarNode GetElementScaleNode(ElementReference<uint, object?> element) => (float)Math.Pow(0.95f, element.Id);

            protected override Transition GetElementTransitionEasingFunction(ElementReference<uint, object?> element) =>
                new(100,
                    Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f)));
        }
#endif

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
#if !WINAPPSDK
            if (sender is ToggleSwitch toggle)
            {
                if (toggle.IsOn && compositionCollectionView.Layout<uint, object?>() is LinearLayout currentLinearLayout)
                {
                    currentLinearLayout.TransitionTo(_ => new StackLayout(currentLinearLayout));
                }
                else if (!toggle.IsOn && compositionCollectionView.Layout<uint, object?>() is StackLayout currentStackLayout)
                {
                    currentStackLayout.TransitionTo(_ => new LinearLayout(currentStackLayout));
                }
            }
#endif

        }
    }
}
