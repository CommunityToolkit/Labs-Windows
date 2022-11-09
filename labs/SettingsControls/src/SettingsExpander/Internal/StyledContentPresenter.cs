// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.Internal;

/// <summary>
/// A <see cref="ContentPresenter"/> which also can restyle it's realized <see cref="ContentPresenter.Content"/> with a <see cref="StyleSelector"/> using the <see cref="ContentStyleSelector"/> property. Currently, only checked once on loading.
/// </summary>
public sealed partial class StyledContentPresenter : ContentPresenter
{
    public StyleSelector ContentStyleSelector
    {
        get { return (StyleSelector)GetValue(ContentStyleSelectorProperty); }
        set { SetValue(ContentStyleSelectorProperty, value); }
    }

    public static readonly DependencyProperty ContentStyleSelectorProperty =
        DependencyProperty.Register(nameof(ContentStyleSelector), typeof(StyleSelector), typeof(StyledContentPresenter), new PropertyMetadata(null));

    public StyledContentPresenter()
    {
        // TODO: Not sure if we need to worry about content/template changing and restyling in response to that
        // Not sure if we can detect that and hook in at the right spot regardless...
        Loaded += this.StyledContentPresenter_Loaded;
    }

    private void StyledContentPresenter_Loaded(object sender, RoutedEventArgs e)
    {
        // We need to wait and get the child element when the presenter is loaded, as Content is generally the data item itself.
        var child = VisualTreeHelper.GetChild(this, 0);
    
        if (child is FrameworkElement element)
        {
            var style = ContentStyleSelector.SelectStyle(Content, element);

            // We don't want to blank out the style if we don't have a new one to provide.
            if (style != null)
            {
                element.Style = style;
            }
        }
    }
}
