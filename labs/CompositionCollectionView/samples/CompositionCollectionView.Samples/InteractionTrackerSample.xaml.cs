// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using CommunityToolkit.Labs.Core.SourceGenerators;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.WinUI;
using Microsoft.Toolkit.Uwp.UI.Animations.ExpressionsFork;
using System.Numerics;

#if !WINAPPSDK
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
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
    [ToolkitSample(id: nameof(InteractionTrackerSample), "Interaction tracker layout", description: "Layout driven by an interaction tracker.")]
    public sealed partial class InteractionTrackerSample : Page
    {
        const int ElementWidth = 100;

        public InteractionTrackerSample()
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
                        Width = ElementWidth,
                        Height = ElementWidth,
                        Fill = new SolidColorBrush(Colors.CornflowerBlue),
                        Stroke = new SolidColorBrush(Colors.Gray),
                        StrokeThickness = 1
                    }
                , (_) => { });

            compositionCollectionView.SetLayout(layout);
            compositionCollectionView.UpdateSource(elements);
#endif
        }
#if !WINAPPSDK
        public class LinearLayout : CompositionCollectionLayout<uint, object?>
        {
            public LinearLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            protected override void OnActivated()
            {
                if (TryGetBehavior<InteractionTrackerBehavior<uint, object?>>() is null)
                {
                    // Tracker can't be created until activation, we don't have access to the root panel until then
                    var trackerBehavior = new InteractionTrackerBehavior<uint, object?>(RootPanel);
                    AddBehavior(trackerBehavior);

                    var tracker = trackerBehavior.Tracker;
                    var interactionSource = trackerBehavior.InteractionSource;

                    UpdateTrackerLimits();

                    interactionSource.ScaleSourceMode = InteractionSourceMode.Disabled;
                    interactionSource.PositionXSourceMode = InteractionSourceMode.EnabledWithInertia;
                    interactionSource.PositionYSourceMode = InteractionSourceMode.Disabled;
                }

                RootPanel.Background = new SolidColorBrush(Colors.Transparent);
                RootPanel.PointerPressed += RootPointerPressed;
            }

            protected override void OnDeactivated()
            {
                RootPanel.PointerPressed -= RootPointerPressed;
            }

            void RootPointerPressed(object sender, PointerRoutedEventArgs e)
            {
                if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    var position = e.GetCurrentPoint(RootPanel);
                    GetBehavior<InteractionTrackerBehavior<uint, object?>>().InteractionSource.TryRedirectForManipulation(position);
                }
            }

            protected override void OnElementsUpdated()
            {
                UpdateTrackerLimits();
            }

            private void UpdateTrackerLimits()
            {
                var trackerBehavior = GetBehavior<InteractionTrackerBehavior<uint, object?>>();

                var availableWidth = (float)RootPanel.ActualWidth - ElementWidth;
                var elementsWidth = Elements.Count * ElementWidth * 1.2f;

                trackerBehavior.Tracker.MaxPosition = new Vector3(elementsWidth - ((float)RootPanel.ActualWidth + ElementWidth) / 2, 0, 0);
                trackerBehavior.Tracker.MinPosition = new Vector3(-((float)RootPanel.ActualWidth - ElementWidth) / 2, 0, 0);
            }

            private ScalarNode ScrollProgress(ElementReference<uint, object?> element)
            {
                var availableWidth = RootPanelVisual.GetReference().Size.X - ElementWidth;

                return ExpressionFunctions.Clamp(
                            (element.Id * ElementWidth * 1.2f
                             - GetBehavior<InteractionTrackerBehavior<uint, object?>>().Tracker.GetReference().Position.X) / availableWidth,
                            0,
                            1);
            }

            public override Vector3Node GetElementPositionNode(ElementReference<uint, object?> element)
            {
                var availableWidth = RootPanelVisual.GetReference().Size.X - ElementWidth;

                var xPosition = availableWidth * ScrollProgress(element);

                var yPosition = 50 * ExpressionFunctions.Sin(ScrollProgress(element) * (float)Math.PI);

                return ExpressionFunctions.Vector3(xPosition, yPosition, 0);
            }

            public override ScalarNode GetElementScaleNode(ElementReference<uint, object?> element) => 1.5f - ExpressionFunctions.Abs(0.5f - ScrollProgress(element));
        }
#endif
    }
}
