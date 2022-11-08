// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.Rive;

/// <summary>
/// This is the base class for objects that manage a custom, named state machine input value on a
/// Rive state machine. An application can modify inputs on a state machine by setting the `Value`
/// property of a `StateMachineInput`.
/// </summary>
public abstract partial class StateMachineInput : DependencyObject
{
    private string? _target;

    private WeakReference<RivePlayer> _rivePlayer = new WeakReference<RivePlayer>(null!);

    /// <summary>
    /// Name of the input on the Rive state machine that this class will manage.
    /// </summary>
    public string? Target
    {
        get => _target;
        set
        {
            _target = value;
            Apply();
        }
    }

    protected WeakReference<RivePlayer> RivePlayer => _rivePlayer;

    /// <summary>
    /// Establishes the <see cref="RivePlayer"/> whose state machine inputs this class will manage.
    /// If <see cref="_rivePlayer"/> was not already equal to `rivePlayer`, the subclass also
    /// applies the current `Value` on the state machine.
    /// </summary>
    internal void SetRivePlayer(WeakReference<RivePlayer> rivePlayer)
    {
        _rivePlayer = rivePlayer;
        Apply();
    }

    /// <summary>
    /// Sets the subclass's current `Value` on the Rive state machine.
    /// </summary>
    protected void Apply()
    {
        if (!String.IsNullOrEmpty(_target) && _rivePlayer.TryGetTarget(out var rivePlayer))
        {
            Apply(rivePlayer, _target!);
        }
    }

    /// <summary>
    /// Sets the subclass's current `Value` on the Rive state machine.
    /// </summary>
    /// <param name="rivePlayer"><see cref="RivePlayer"/> whose state machine to modify.</param>
    /// <param name="inputName">Name of the state machine input to modify.</param>
    protected abstract void Apply(RivePlayer rivePlayer, string inputName);
}

/// <summary>
/// Manages a boolean input on a Rive state machine.
/// </summary>
[ContentProperty(Name = nameof(Value))]
public class BoolInput : StateMachineInput
{
    /// <summary>
    /// Identifies the <see cref="Value"/> property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(bool),
        typeof(BoolInput),
        new PropertyMetadata(false, OnValueChanged)
    );

    /// <summary>
    /// Mirrors a boolean value on a Rive state machine. When this property is set, this class also
    /// updates the corresponding value on the Rive state machine.
    /// </summary>
    public bool? Value
    {
        get => (bool?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((BoolInput)d).Apply();
    }

    /// <inheritdoc/>
    protected override void Apply(RivePlayer rivePlayer, string inputName)
    {
        bool? boolean = this.Value;
        if (boolean.HasValue)
        {
            rivePlayer.SetBool(inputName, boolean.Value);
        }
    }
}

/// <summary>
/// Manages a number (double) input on a Rive state machine.
/// </summary>
[ContentProperty(Name = nameof(Value))]
public class NumberInput : StateMachineInput
{
    /// <summary>
    /// Identifies the <see cref="Value"/> property.
    /// </summary>
    private static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double),
        typeof(NumberInput),
        new PropertyMetadata(null, OnValueChanged)
    );

    /// <summary>
    /// Mirrors a number (double) value on a Rive state machine. When this property is set, this
    /// class also updates the corresponding value on the Rive state machine.
    /// </summary>
    public double? Value
    {
        get => (double?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((NumberInput)d).Apply();
    }

    /// <inheritdoc/>
    protected override void Apply(RivePlayer rivePlayer, string inputName)
    {
        double? number = this.Value;
        if (number.HasValue)
        {
            rivePlayer.SetNumber(inputName, (float)number.Value);
        }
    }
}

/// <summary>
/// Manages a trigger input on a Rive state machine. Unlike booleans and numbers, triggers do not
/// have an underlying value. They can instead be thought of as "function calls" on a state machine.
/// </summary>
public class TriggerInput : StateMachineInput
{
    /// <summary>
    /// Calls the trigger input on our underlying state machine.
    /// </summary>
    public void Fire()
    {
        string? target = this.Target;
        if (!String.IsNullOrEmpty(target) && this.RivePlayer.TryGetTarget(out var rivePlayer))
        {
            rivePlayer.FireTrigger(target!);
        }
    }

    /// <summary>
    /// Does nothing -- triggers don't have an underlying value to apply.
    /// </summary>
    protected override void Apply(RivePlayer rivePlayer, string inputName) { }
}
