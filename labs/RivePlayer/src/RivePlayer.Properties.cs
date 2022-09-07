// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using RiveSharp;

namespace CommunityToolkit.Labs.WinUI.Rive;

// XAML properies for RivePlayer.
[ContentProperty(Name = nameof(StateMachineInputCollection))]
public partial class RivePlayer
{
    // Filename of the .riv file to open. Can be a file path or a URL.
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(string),
        typeof(RivePlayer),
        new PropertyMetadata(null, OnSourceNameChanged)
    );

    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    // Name of the artbord to load from the .riv file. If null or empty, the default artboard
    // will be loaded.
    public static readonly DependencyProperty ArtboardProperty = DependencyProperty.Register(
        nameof(Artboard),
        typeof(string),
        typeof(RivePlayer),
        new PropertyMetadata(null, OnArtboardNameChanged)
    );

    public string Artboard
    {
        get => (string)GetValue(ArtboardProperty);
        set => SetValue(ArtboardProperty, value);
    }

    // Name of the state machine to load from the .riv file.
    public static readonly DependencyProperty StateMachineProperty = DependencyProperty.Register(
        nameof(StateMachine),
        typeof(string),
        typeof(RivePlayer),
        new PropertyMetadata(null, OnStateMachineNameChanged)
    );

    public string StateMachine
    {
        get => (string)GetValue(StateMachineProperty);
        set => SetValue(StateMachineProperty, value);
    }

    // Name of the fallback animation to load from the .riv if StateMachine is null or empty.
    public static readonly DependencyProperty AnimationProperty = DependencyProperty.Register(
        nameof(Animation),
        typeof(string),
        typeof(RivePlayer),
        new PropertyMetadata(null, OnAnimationNameChanged)
    );

    public string Animation
    {
        get => (string)GetValue(AnimationProperty);
        set => SetValue(AnimationProperty, value);
    }

    public static readonly DependencyProperty StateMachineInputCollectionProperty = DependencyProperty.Register(
        nameof(StateMachineInputCollection),
        typeof(StateMachineInputCollection),
        typeof(RivePlayer),
        new PropertyMetadata(new StateMachineInputCollection(), OnStateMachineInputCollectionChanged)
    );

    public StateMachineInputCollection StateMachineInputCollection
    {
        get => (StateMachineInputCollection)GetValue(StateMachineInputCollectionProperty);
        set => SetValue(StateMachineInputCollectionProperty, value);
    }

    private static void OnSourceNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var player = (RivePlayer)d;
        var newSourceName = (string)e.NewValue;
        // Clear the current Scene while we wait for the new one to load.
        player.sceneActionsQueue.Enqueue(() => player._scene = new Scene());
        if (player._activeSourceFileLoader != null)
        {
            player._activeSourceFileLoader.Cancel();
        }

        player._activeSourceFileLoader = new CancellationTokenSource();
        // Defer state machine inputs here until the new file is loaded.
        player._deferredSMInputsDuringFileLoad = new List<Action>();
        player.LoadSourceFileDataAsync(newSourceName, player._activeSourceFileLoader.Token);
    }

    private static void OnArtboardNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var player = (RivePlayer)d;
        var newArtboardName = (string)e.NewValue;
        player.sceneActionsQueue.Enqueue(() => player._artboardName = newArtboardName);
        if (player._activeSourceFileLoader != null)
        {
            // If a file is currently loading async, it will apply the new artboard once
            // it completes. Loading a new artboard also invalidates any state machine
            // inputs that were waiting for the file load.
            player._deferredSMInputsDuringFileLoad!.Clear();
        }
        else
        {
            player.sceneActionsQueue.Enqueue(() => player.UpdateScene(SceneUpdates.Artboard));
        }
    }

    private static void OnStateMachineNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var player = (RivePlayer)d;
        var newStateMachineName = (string)e.NewValue;
        player.sceneActionsQueue.Enqueue(() => player._stateMachineName = newStateMachineName);
        if (player._activeSourceFileLoader != null)
        {
            // If a file is currently loading async, it will apply the new state machine
            // once it completes. Loading a new state machine also invalidates any state
            // machine inputs that were waiting for the file load.
            player._deferredSMInputsDuringFileLoad!.Clear();
        }
        else
        {
            player.sceneActionsQueue.Enqueue(() => player.UpdateScene(SceneUpdates.AnimationOrStateMachine));
        }
    }

    private static void OnAnimationNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var player = (RivePlayer)d;
        var newAnimationName = (string)e.NewValue;
        player.sceneActionsQueue.Enqueue(() => player._animationName = newAnimationName);
        // If a file is currently loading async, it will apply the new animation once it completes.
        if (player._activeSourceFileLoader == null)
        {
            player.sceneActionsQueue.Enqueue(() => player.UpdateScene(SceneUpdates.AnimationOrStateMachine));
        }
    }

    private static void OnStateMachineInputCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Clear the RivePlayer on the old reference so it quits updating us.
        var oldCollection = (StateMachineInputCollection)e.OldValue;
        oldCollection.SetRivePlayer(null);

        var newCollection = (StateMachineInputCollection)e.NewValue;
        newCollection.SetRivePlayer((RivePlayer)d);
    }
}
