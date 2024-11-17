// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI;

namespace BehaviorsExperiment.Samples;

[ToolkitSampleNumericOption("LoadingOffset", 100d, 50d, 500d, 50d, Title = "LoadingOffset")]

[ToolkitSample(id: nameof(NeedMoreItemTriggerBehaviorSample), "NeedMoreItemTriggerBehavior", description: $"A sample for showing how to create and use a NeedMoreItemTriggerBehavior.")]
public sealed partial class NeedMoreItemTriggerBehaviorSample : Page
{
    private static readonly Random _random = new();

    public NeedMoreItemTriggerBehaviorSample()
    {
        this.InitializeComponent();
    }

    public static SolidColorBrush GetColor()
    {
        var rand = _random.Next(4);
        var color = rand switch
        {
            0 => Colors.Red,
            1 => Colors.Blue,
            2 => Colors.Green,
            3 => Colors.Yellow,
            _ => Colors.Black
        };
        return new(color);
    }

    public IEnumerable<int> ViewModels { get; } = Enumerable.Repeat(0, 50).Select(_ => _random.Next(5, 10) * 500);

    private int _num;

    public void IncrementCount()
    {
        _num++;
        MyRun.Text = _num.ToString();
    }
}

#endif
