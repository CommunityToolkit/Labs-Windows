// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.Rive;

// Manages a collection of StateMachineInput objects for RivePlayer. The [ContentProperty] tag
// on RivePlayer instructs the XAML engine automatically route nested inputs through this
// collection:
//
//   <rive:RivePlayer Source="...">
//       <rive:BoolInput Target=... />
//   </rive:RivePlayer>
//
public class StateMachineInputCollection : DependencyObjectCollection
{
    public StateMachineInputCollection()
    {
        VectorChanged += InputsVectorChanged;
    }

    private WeakReference<RivePlayer> _rivePlayer = new WeakReference<RivePlayer>(null!);

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
