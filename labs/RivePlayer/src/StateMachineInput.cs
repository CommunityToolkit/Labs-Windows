// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.Rive;

// This base class wraps a custom, named state machine input value.
public abstract partial class StateMachineInput : DependencyObject
{
    // The Target is what the input is named in the Rive state machine.
    private string? _target;
    public string? Target
    {
        get => _target;
        set
        {
            _target = value;
            Apply();
        }
    }

    private WeakReference<RivePlayer> _rivePlayer = new WeakReference<RivePlayer>(null!);
    protected WeakReference<RivePlayer> RivePlayer => _rivePlayer;

    // Sets _rivePlayer to the given rivePlayer object and applies our input value to the state
    // machine. Does nothing if _rivePlayer was already equal to rivePlayer.
    internal void SetRivePlayer(WeakReference<RivePlayer> rivePlayer)
    {
        _rivePlayer = rivePlayer;
        Apply();
    }

    protected void Apply()
    {
        if (!String.IsNullOrEmpty(_target) && _rivePlayer.TryGetTarget(out var rivePlayer))
        {
            Apply(rivePlayer, _target!);
        }
    }

    // Applies our input value to the rivePlayer's state machine.
    protected abstract void Apply(RivePlayer rivePlayer, string inputName);
}

[ContentProperty(Name = nameof(Value))]
public class BoolInput : StateMachineInput
{
    // Define "Value" as a DependencyProperty so it can be data-bound.
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(bool),
        typeof(BoolInput),
        new PropertyMetadata(false, OnValueChanged)
    );

    public bool? Value
    {
        get => (bool?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((BoolInput)d).Apply();
    }

    protected override void Apply(RivePlayer rivePlayer, string inputName)
    {
        bool? boolean = this.Value;
        if (boolean.HasValue)
        {
            rivePlayer.SetBool(inputName, boolean.Value);
        }
    }
}

[ContentProperty(Name = nameof(Value))]
public class NumberInput : StateMachineInput
{
    // Define "Value" as a DependencyProperty so it can be data-bound.
    private static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double),
        typeof(NumberInput),
        new PropertyMetadata(null, OnValueChanged)
    );

    public double? Value
    {
        get => (double?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((NumberInput)d).Apply();
    }

    protected override void Apply(RivePlayer rivePlayer, string inputName)
    {
        double? number = this.Value;
        if (number.HasValue)
        {
            rivePlayer.SetNumber(inputName, (float)number.Value);
        }
    }
}

public class TriggerInput : StateMachineInput
{
    public void Fire()
    {
        string? target = this.Target;
        if (!String.IsNullOrEmpty(target) && this.RivePlayer.TryGetTarget(out var rivePlayer))
        {
            rivePlayer.FireTrigger(target);
        }
    }

    // Triggers don't have any persistent data to apply.
    protected override void Apply(RivePlayer rivePlayer, string inputName) { }
}
