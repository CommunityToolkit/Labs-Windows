// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

    /// <summary>
    /// Provides the base implementation for all controls that contain single content and have a header.
    /// </summary>
    public partial class HeaderedContentControl : ContentControl
    {
        private const string PartHeaderPresenter = "HeaderPresenter";
        private const string PartIconPresenter = "IconPresenter";

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedContentControl"/> class.
        /// </summary>
        public HeaderedContentControl()
        {
            DefaultStyleKey = typeof(HeaderedContentControl);
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(HeaderedContentControl),
            new PropertyMetadata(null, OnHeaderChanged));

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(object),
            typeof(HeaderedContentControl),
            new PropertyMetadata(null, OnIconChanged));

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(HeaderedContentControl),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(HeaderedContentControl),
            new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        /// <summary>
        /// Identifies the <see cref="IconPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconPositionProperty = DependencyProperty.Register(
            nameof(IconPosition),
            typeof(IconPosition),
            typeof(HeaderedContentControl),
            new PropertyMetadata(IconPosition.LeftCenter, OnIconPositionChanged));

        /// <summary>
        /// Gets or sets the <see cref="Orientation"/> used for the header.
        /// </summary>
        /// <remarks>
        /// If set to <see cref="Orientation.Vertical"/> the header will be above the content.
        /// If set to <see cref="Orientation.Horizontal"/> the header will be to the left of the content.
        /// </remarks>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="IconPosition"/> used for the header.
        /// </summary>
        /// <remarks>
        /// If set to <see cref="IconPosition.Left"/> the icon will be left of the content.
        /// If set to <see cref="IconPosition.Up"/> the icon will be above the content.
        /// If set to <see cref="IconPosition.Right"/> the icon will be right of the content.
        /// If set to <see cref="IconPosition.Down"/> the icon will be under the content.
        /// </remarks>
        public IconPosition IconPosition
        {
            get { return (IconPosition)GetValue(IconPositionProperty); }
            set { SetValue(IconPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data used for the header of each control.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data used for the icon of each control.
        /// </summary>
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template used to display the content of the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SetHeaderVisibility();
            SetIconVisibility();
            SetOrientation();
            SetIconPosition();
        }

        /// <summary>
        /// Called when the <see cref="Header"/> property changes.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="Header"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="Header"/> property.</param>
        protected virtual void OnHeaderChanged(object oldValue, object newValue)
        {
        }

        /// <summary>
        /// Called when the <see cref="Icon"/> property changes.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="Icon"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="Icon"/> property.</param>
        protected virtual void OnIconChanged(object oldValue, object newValue)
        {
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (HeaderedContentControl)d;
            control.SetOrientation();
        }
        private static void OnIconPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (HeaderedContentControl)d;
            control.SetIconPosition();
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (HeaderedContentControl)d;
            control.SetHeaderVisibility();
            control.OnHeaderChanged(e.OldValue, e.NewValue);
        }
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (HeaderedContentControl)d;
            control.SetIconVisibility();
            control.OnIconChanged(e.OldValue, e.NewValue);
        }

        private void SetHeaderVisibility()
        {
            if (GetTemplateChild(PartHeaderPresenter) is FrameworkElement headerPresenter)
            {
                if (Header is string headerText)
                {
                    headerPresenter.Visibility = string.IsNullOrEmpty(headerText)
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                }
                else
                {
                    headerPresenter.Visibility = Header != null
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }

        private void SetIconVisibility()
        {
            if (GetTemplateChild(PartIconPresenter) is FrameworkElement iconPresenter)
            {
                iconPresenter.Visibility = Icon != null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void SetOrientation()
        {
            var orientation = this.Orientation == Orientation.Vertical
                ? nameof(Orientation.Vertical)
                : nameof(Orientation.Horizontal);

            VisualStateManager.GoToState(this, orientation, true);
        }

        private void SetIconPosition()
        {
            var position = "";

            switch (this.IconPosition)
            {
                case IconPosition.LeftCenter:
                    position = nameof(IconPosition.LeftCenter); break;
                case IconPosition.LeftTop:
                    position = nameof(IconPosition.LeftTop); break;
                case IconPosition.LeftBottom:
                    position = nameof(IconPosition.LeftBottom); break;
                case IconPosition.TopCenter:
                    position = nameof(IconPosition.TopCenter); break;
                case IconPosition.TopLeft:
                    position = nameof(IconPosition.TopLeft); break;
                case IconPosition.TopRight:
                    position = nameof(IconPosition.TopRight); break;
                case IconPosition.RightCenter:
                    position = nameof(IconPosition.RightCenter); break;
                case IconPosition.RightTop:
                    position = nameof(IconPosition.RightTop); break;
                case IconPosition.RightBottom:
                    position = nameof(IconPosition.RightBottom); break;
                case IconPosition.BottomCenter:
                    position = nameof(IconPosition.BottomCenter); break;
                case IconPosition.BottomLeft:
                    position = nameof(IconPosition.BottomLeft); break;
                case IconPosition.BottomRight:
                    position = nameof(IconPosition.BottomRight); break;
            }
            VisualStateManager.GoToState(this, position, true);
        }
    }


public enum IconPosition
{
    LeftCenter,
    LeftTop,
    LeftBottom,
    TopCenter,
    TopLeft,
    TopRight,
    RightCenter,
    RightTop,
    RightBottom,
    BottomCenter,
    BottomLeft,
    BottomRight
}
