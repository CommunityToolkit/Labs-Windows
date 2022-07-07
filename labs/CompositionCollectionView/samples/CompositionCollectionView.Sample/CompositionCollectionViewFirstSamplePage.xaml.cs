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
    [ToolkitSampleBoolOption("IsTextVisible", "IsVisible", true)]
    // Single values without a colon are used for both label and value.
    // To provide a different label for the value, separate with a colon surrounded by a single space on both sides ("label : value").
    [ToolkitSampleMultiChoiceOption("TextSize", title: "Text size", "Small : 12", "Normal : 16", "Big : 32")]
    [ToolkitSampleMultiChoiceOption("TextFontFamily", title: "Font family", "Segoe UI", "Arial", "Consolas")]
    [ToolkitSampleMultiChoiceOption("TextForeground", title: "Text foreground",
        "Teal       : #0ddc8c",
        "Sand       : #e7a676",
        "Dull green : #5d7577")]

    [ToolkitSample(id: nameof(CompositionCollectionViewFirstSamplePage), "Simple Options", description: "A sample page for showing how to do simple options.")]
    public sealed partial class CompositionCollectionViewFirstSamplePage : Page
    {
        public CompositionCollectionViewFirstSamplePage()
        {
            this.InitializeComponent();

            var layout = new SampleLayout((id) =>
                new Rectangle() {
                    Width = 100,
                    Height = 100,
                    Fill = new SolidColorBrush(Windows.UI.Colors.BlueViolet)
                }
            , (_) => { });
            compositionCollectionView.SetLayout(layout);
            compositionCollectionView.UpdateSource(new List<(uint, Action<CompositionPropertySet, Dictionary<string, object?>>)>()
            {
                (0, (_,_)=>{ }),
                (1, (_,_)=>{ }),
                (2, (_,_)=>{ }),
                (3, (_,_)=>{ })
            });
        }

        public class SampleLayout : Layout<uint>
        {
            public SampleLayout(Func<uint, FrameworkElement> elementFactory, Action<string>? log) : base(elementFactory, log)
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
    }
}
