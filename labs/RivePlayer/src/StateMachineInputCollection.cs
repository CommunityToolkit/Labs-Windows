// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.Rive;

/// <summary>
/// Manages a collection of StateMachineInput objects for <see cref="RivePlayer"/>. The
/// [ContentProperty] tag on RivePlayer instructs the XAML engine to automatically populate this
/// collection with nested inputs:
///
///   <rive:RivePlayer Source="...">
///       <!--  Adds this input to the StateMachineInputCollection.  -->
///       <rive:BoolInput Target=... />
///   </rive:RivePlayer>
///
/// </summary>
public class StateMachineInputCollection : DependencyObjectCollection
{
    private WeakReference<RivePlayer> _rivePlayer = new WeakReference<RivePlayer>(null!);

    public StateMachineInputCollection()
    {
        VectorChanged += InputsVectorChanged;
    }

    /// <summary>
    /// Establishes the <see cref="RivePlayer"/> whose state machine inputs this class will manage.
    /// If any given <see cref="StateMachineInput"/> was not already bound to this rive player, it also
    /// applies its current `Value` to the state machine.
    /// </summary>
    public void SetRivePlayer(RivePlayer? rivePlayer)
    {
        _rivePlayer = new WeakReference<RivePlayer>(rivePlayer!);
        foreach (StateMachineInput input in this)
        {
            input.SetRivePlayer(_rivePlayer);
        }
    }

    private void InputsVectorChanged(IObservableVector<DependencyObject> sender,
                                     IVectorChangedEventArgs @event)
    {
        switch (@event.CollectionChange)
        {
            case CollectionChange.ItemInserted:
            case CollectionChange.ItemChanged:
            {
                var input = (StateMachineInput)sender[(int)@event.Index];
                input.SetRivePlayer(_rivePlayer);
            }
            break;
            case CollectionChange.ItemRemoved:
            {
                var input = (StateMachineInput)sender[(int)@event.Index];
                input.SetRivePlayer(new WeakReference<RivePlayer>(null!));
                break;
            }
            case CollectionChange.Reset:
                foreach (StateMachineInput input in sender)
                {
                    input.SetRivePlayer(new WeakReference<RivePlayer>(null!));
                }
                break;
        }
    }
}
