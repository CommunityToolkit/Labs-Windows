// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.Controls;

namespace BehaviorsExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSampleNumericOption("LoadingOffset", 100d, 500d, 50d, Title = "LoadingOffset")]

[ToolkitSample(id: nameof(NeedMoreItemTriggerBehaviorSample), "Custom control", description: $"A sample for showing how to create and use a {nameof(Behaviors)} custom control.")]
public sealed partial class NeedMoreItemTriggerBehaviorSample : Page
{
    private static readonly Random _random = new Random();

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

    public MyCollection ViewModels { get; } = [.. Enumerable.Repeat(0, 50).Select(_ => _random.Next(5, 10) * 500)];
}

public partial class MyCollection : ObservableCollection<int>, ISupportIncrementalLoading
{
    private static readonly Random _random = new Random();

    public int Num
    {
        get => _num;
        set
        {
            if (_num == value)
            {
                return;
            }

            _num = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Num)));
        }
    }

    private int _num;

    public bool HasMoreItems => true;

    public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
    {
        return LoadMoreItemsTaskAsync(count).AsAsyncOperation();
    }

    public async Task<LoadMoreItemsResult> LoadMoreItemsTaskAsync(uint count)
    {
        await Task.Delay(1000);
        foreach (var i in Enumerable.Repeat(0, (int)count).Select(_ => _random.Next(5, 10) * 500))
        {
            Add(i);
        }

        Num++;
        return new LoadMoreItemsResult { Count = count };
    }
}
