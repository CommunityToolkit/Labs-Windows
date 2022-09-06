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
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
    [ToolkitSample(id: nameof(MazeSample), "Maze layout", description: "Layout driven by an interaction tracker.")]
    public sealed partial class MazeSample : Page
    {
        enum TileType { Floor, Ceiling, HorizontalWall, VerticalWall }

        const int TileWidth = 100;
        const int MazeSize = 10;

        static readonly Vector3 GoalPosition = new Vector3(900, -900, 0);

        private uint nextEntityId;

        private List<(uint, Action<CompositionPropertySet, Dictionary<string, object>>)> elements { get; init; }

        public MazeSample()
        {
            this.InitializeComponent();


            List<Tile> tiles = new();

            for (int i = 0; i < MazeSize; i++)
            {
                for (int j = 0; j < MazeSize; j++)
                {
                    tiles.Add(new(TileType.Floor, i, j));
                    tiles.Add(new(TileType.Ceiling, i, j));

                    if (j == 0)
                    {
                        tiles.Add(new(TileType.HorizontalWall, i, j));
                    }
                    else if (j == MazeSize - 1)
                    {
                        tiles.Add(new(TileType.HorizontalWall, i, j + 1));
                    }

                    if (i == 0)
                    {
                        tiles.Add(new(TileType.VerticalWall, i, j));
                    }
                    else if (i == MazeSize - 1)
                    {
                        tiles.Add(new(TileType.VerticalWall, i + 1, j));
                    }
                }
            }

            elements = tiles.OrderBy(x => x.Y).Select(x => CreateElement(x)).ToList();

            var layout = new SpinningMazeLayout((id) => TileControl.Create(), (_) => { });
            compositionCollectionView.SetLayout(layout);
            compositionCollectionView.UpdateSource(elements);

            Visual visual = ElementCompositionPreview.GetElementVisual(compositionCollectionView);
            var viewSize = visual.GetReference().Size;

            visual.StartAnimation(AnimationConstants.TransformMatrix,
                ExpressionFunctions.CreateTranslation(ExpressionFunctions.Vector3(-viewSize.X / 2, -viewSize.Y / 2, 0))
                * ExpressionFunctions.Matrix4x4(
                        1.0f, 0.0f, 0.0f, 0.0f,
                        0.0f, 1.0f, 0.0f, 0.0f,
                        0.0f, 0.0f, 1.0f, -1.0f / viewSize.X,
                        0.0f, 0.0f, 0.0f, 1.0f) *
                        ExpressionFunctions.CreateTranslation(ExpressionFunctions.Vector3(viewSize.X / 2, viewSize.Y / 2, 0)));
        }

        private record Tile(TileType Type, int X, int Y);

        private (uint, Action<CompositionPropertySet, Dictionary<string, object>>) CreateElement(Tile tile)
        {
            return (nextEntityId++, (_, dict) =>
            {
                dict[nameof(Tile.Type)] = tile.Type;
                dict[nameof(Tile.X)] = tile.X;
                dict[nameof(Tile.Y)] = tile.Y;
            }
            );
        }

        public abstract class MazeLayout : Layout<uint>
        {
            protected const string PositionNode = nameof(PositionNode);
            protected const string ScaleNode = nameof(ScaleNode);
            protected const string RotationNode = nameof(RotationNode);
            const string CameraTransformNode = nameof(CameraTransformNode);

            public MazeLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            public MazeLayout(Layout<uint> sourceLayout) : base(sourceLayout)
            {
            }

            protected override void OnActivated()
            {
                var translation = ExpressionFunctions.CreateTranslation(AnimatableNodes.GetOrCreateVector3Node(PositionNode, Vector3.Zero).Reference);

                var scaleNode = AnimatableNodes.GetOrCreateScalarNode(ScaleNode, 1).Reference;
                var scale = ExpressionFunctions.CreateScale(ExpressionFunctions.Vector3(scaleNode, scaleNode, scaleNode));

                var rotationNode = AnimatableNodes.GetOrCreateVector3Node(RotationNode, Vector3.Zero).Reference;
                var rotation = ExpressionFunctions.CreateMatrix4x4FromAxisAngle(Vector3.UnitX, rotationNode.X) *
                    ExpressionFunctions.CreateMatrix4x4FromAxisAngle(Vector3.UnitY, rotationNode.Y) *
                    ExpressionFunctions.CreateMatrix4x4FromAxisAngle(Vector3.UnitZ, rotationNode.Z);

                AnimatableNodes.GetOrCreateMatrix4x4Node(CameraTransformNode, Matrix4x4.Identity).Animate(translation * rotation * scale);

                TileControl.LoadBrushes();
            }


            public override Vector3Node GetElementPositionNode(ElementReference<uint> element)
            {
                element.Properties.TryGetValue(nameof(Tile.X), out var x);
                element.Properties.TryGetValue(nameof(Tile.Y), out var y);
                element.Properties.TryGetValue(nameof(Tile.Type), out var tileType);

                var xPosition = tileType switch
                {
                    _ => (int)x! * TileWidth
                };

                var yPosition = tileType switch
                {
                    _ => (int)y! * TileWidth
                };

                var height = tileType switch
                {
                    TileType.Floor => TileWidth,
                    TileType.Ceiling => 0,
                    _ => 0
                };

                var camera = AnimatableNodes.GetOrCreateMatrix4x4Node(CameraTransformNode, Matrix4x4.Identity).Reference;

                return ExpressionFunctions.Transform(ExpressionFunctions.Vector4(xPosition, height, yPosition, 1), camera).XYZ;
            }

            public override ScalarNode GetElementScaleNode(ElementReference<uint> element)
            {
                var camera = AnimatableNodes.GetOrCreateMatrix4x4Node(CameraTransformNode, Matrix4x4.Identity).Reference;
                //return camera.Channel11;

                //return RootPanelVisual.GetReference().Size.Y / TileWidth;

                //return ExpressionFunctions.Transform(ExpressionFunctions.Vector4(1, 0, 0, 0), camera).X -
                //    ExpressionFunctions.Transform(ExpressionFunctions.Vector4(0, 0, 0, 0), camera).X;

                return AnimatableNodes.GetOrCreateScalarNode(ScaleNode, 1).Reference;
            }

            public override QuaternionNode GetElementOrientationNode(ElementReference<uint> element)
            {
                var localOrientation = element.Properties[nameof(Tile.Type)] switch
                {
                    TileType.Floor => Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 2, 0),
                    TileType.Ceiling => Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 2, 0),
                    TileType.VerticalWall => Quaternion.CreateFromYawPitchRoll(-MathF.PI / 2, 0, 0),
                    TileType.HorizontalWall => Quaternion.Identity,
                    _ => Quaternion.Identity
                };

                var rotationNode = AnimatableNodes.GetOrCreateVector3Node(RotationNode, Vector3.Zero).Reference;
                var cameraOrientation = ExpressionFunctions.CreateQuaternionFromAxisAngle(Vector3.UnitX, rotationNode.X) *
                    ExpressionFunctions.CreateQuaternionFromAxisAngle(Vector3.UnitY, rotationNode.Y) *
                    ExpressionFunctions.CreateQuaternionFromAxisAngle(Vector3.UnitZ, rotationNode.Z);

                return cameraOrientation * localOrientation;
            }

            protected override void ConfigureElement(ElementReference<uint> element)
            {
                if (element.Container is Rectangle rect)
                {
                    rect.Fill = TileControl.BrushFor((TileType)element.Properties[nameof(Tile.Type)]!);
                }
            }

            public override void UpdateElement(ElementReference<uint> element)
            {
            }

            protected override Transition GetElementTransitionEasingFunction(ElementReference<uint> element) =>
                   new(600,
                       Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f)));
        }

        public class SpinningMazeLayout : MazeLayout
        {
            public SpinningMazeLayout(Layout<uint> sourceLayout) : base(sourceLayout)
            {
            }

            public SpinningMazeLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            protected override void OnActivated()
            {
                base.OnActivated();

                var animation = Compositor.CreateVector3KeyFrameAnimation();
                animation.Duration = TimeSpan.FromSeconds(5);
                animation.InsertKeyFrame(0, Vector3.Zero);
                animation.InsertKeyFrame(1, new Vector3(0, MathF.PI * 2, 0));
                animation.IterationBehavior = AnimationIterationBehavior.Forever;

                var rotationNode = AnimatableNodes.GetOrCreateVector3Node(RotationNode, Vector3.Zero);
                rotationNode.Animate(animation);

                float mazeSide = TileWidth * MazeSize;
                var mazeCenter = new Vector3(-mazeSide / 2, 0, -mazeSide / 2);

                AnimatableNodes.GetOrCreateVector3Node(PositionNode, Vector3.Zero).Animate(ExpressionFunctions.Vector3(
                    ExpressionFunctions.Sin(rotationNode.Reference.Y) * mazeSide * 1.5f,
                    mazeSide / 5,
                    -ExpressionFunctions.Cos(rotationNode.Reference.Y) * mazeSide * 1.5f
                ) + (Vector3Node)mazeCenter);

                AnimatableNodes.GetOrCreateScalarNode(ScaleNode, 1).Animate(RootPanelVisual.GetReference().Size.Y / TileWidth);
                RootPanel.Tapped += this.OnTapped;
            }

            protected override void OnDeactivated()
            {
                RootPanel.Tapped -= OnTapped;
            }

            private void OnTapped(object sender, TappedRoutedEventArgs e)
            {
                TransitionTo(x => new TraversableMazeLayout(this));
            }
        }


        public class TraversableMazeLayout : MazeLayout
        {
            public TraversableMazeLayout(Func<uint, FrameworkElement> elementFactory, Action<string> log) : base(elementFactory, log)
            {
            }

            public TraversableMazeLayout(Layout<uint> sourceLayout) : base(sourceLayout)
            {
            }

            protected override void OnActivated()
            {
                base.OnActivated();

                var trackerBehavior = TryGetBehavior<InteractionTrackerBehavior<uint>>();

                if (trackerBehavior is null)
                {
                    // Tracker can't be created until activation, we don't have access to the root panel until then
                    trackerBehavior = new InteractionTrackerBehavior<uint>(RootPanel);
                    AddBehavior(trackerBehavior);
                }

                var tracker = trackerBehavior.Tracker;
                var interactionSource = trackerBehavior.InteractionSource;

                UpdateTrackerLimits();

                interactionSource.ScaleSourceMode = InteractionSourceMode.Disabled;
                interactionSource.PositionXSourceMode = InteractionSourceMode.EnabledWithInertia;
                interactionSource.PositionYSourceMode = InteractionSourceMode.EnabledWithInertia;

                AnimatableNodes.GetOrCreateVector3Node(PositionNode, Vector3.Zero).Animate(
                    ExpressionFunctions.Vector3(
                        -tracker.GetReference().Position.X,
                        0,
                        tracker.GetReference().Position.Y));

                trackerBehavior.TrackerOwner.OnIdleStateEntered += this.OnTrackerIdleStateEntered;

                AnimatableNodes.GetOrCreateScalarNode(ScaleNode, 1).Animate(RootPanelVisual.GetReference().Size.Y / TileWidth);
                AnimatableNodes.GetOrCreateVector3Node(RotationNode, Vector3.Zero).Value = new Vector3(0, 0, 0);

                RootPanel.Background = new SolidColorBrush(Colors.Black);
                RootPanel.PointerPressed += RootPointerPressed;
            }

            protected override void OnDeactivated()
            {
                RootPanel.PointerPressed -= RootPointerPressed;
                GetBehavior<InteractionTrackerBehavior<uint>>().TrackerOwner.OnIdleStateEntered -= this.OnTrackerIdleStateEntered;
            }

            private void OnTrackerIdleStateEntered(InteractionTracker sender, InteractionTrackerIdleStateEnteredArgs args)
            {
                if (Vector3.Distance(sender.Position, GoalPosition) < TileWidth)
                {
                    TransitionTo(x => new SpinningMazeLayout(this));
                }
            }

            void RootPointerPressed(object sender, PointerRoutedEventArgs e)
            {
                if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    var position = e.GetCurrentPoint(RootPanel);
                    GetBehavior<InteractionTrackerBehavior<uint>>().InteractionSource.TryRedirectForManipulation(position);
                }
            }

            protected override void OnElementsUpdated()
            {
                UpdateTrackerLimits();
            }

            private void UpdateTrackerLimits()
            {
                var trackerBehavior = GetBehavior<InteractionTrackerBehavior<uint>>();

                trackerBehavior.Tracker.MaxPosition = new Vector3(TileWidth * (MazeSize - 1), 0, 0);
                trackerBehavior.Tracker.MinPosition = new Vector3(0, -TileWidth * (MazeSize - 1), 0);
            }
        }

        private class TileControl
        {
            private static ImageBrush WallBrush, CeilingBrush, FloorBrush;

            public static Rectangle Create() => new Rectangle()
            {
                Width = TileWidth,
                Height = TileWidth
            };

            public static void LoadBrushes()
            {
                WallBrush = WallBrush ?? new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri("ms-appx:///CompositionCollectionViewExperiment.Samples/Assets/wall.bmp"))
                };

                CeilingBrush = CeilingBrush ?? new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri("ms-appx:///CompositionCollectionViewExperiment.Samples/Assets/ceiling.bmp"))
                };

                FloorBrush = FloorBrush ?? new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri("ms-appx:///CompositionCollectionViewExperiment.Samples/Assets/floor.bmp"))
                };
            }

            public static ImageBrush BrushFor(TileType type) => type switch
            {
                TileType.Ceiling => CeilingBrush,
                TileType.Floor => FloorBrush,
                _ => WallBrush
            };
        }
    }
}
