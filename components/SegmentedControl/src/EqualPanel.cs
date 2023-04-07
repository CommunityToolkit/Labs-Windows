// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public partial class EqualPanel : Panel
{
    private double maxItemWidth = 0;
    private double maxItemHeight = 0;

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }

    /// <summary>
    /// Identifies the Spacing dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="Spacing"/> dependency property.</returns>
    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
        nameof(Spacing),
        typeof(double),
        typeof(EqualPanel),
        new PropertyMetadata(default(double), OnSpacingChanged));

    public EqualPanel()
    {
        RegisterPropertyChangedCallback(HorizontalAlignmentProperty, OnHorizontalAlignmentChanged);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        maxItemWidth = 0;
        maxItemHeight = 0;
        int UncollapsedChildrenCount = Children.Where(x => x.Visibility != Visibility.Collapsed).Count();

        if (UncollapsedChildrenCount > 0)
        {
            foreach (var child in Children)
            {
                if (child.Visibility != Visibility.Collapsed)
                {
                    child.Measure(availableSize);
                    maxItemWidth = Math.Max(maxItemWidth, child.DesiredSize.Width);
                    maxItemHeight = Math.Max(maxItemHeight, child.DesiredSize.Height);
                }
            }

            // Return equal widths based on the widest item
            // In very specific edge cases the AvailableWidth might be infinite resulting in a crash.
            if (HorizontalAlignment != HorizontalAlignment.Stretch || double.IsInfinity(availableSize.Width))
            {
                return new Size((maxItemWidth * UncollapsedChildrenCount) + (Spacing * (UncollapsedChildrenCount - 1)), maxItemHeight);
            }
            else
            {
                // Equal columns based on the available width, adjust for spacing
                double totalWidth = availableSize.Width - (Spacing * (UncollapsedChildrenCount - 1));
                maxItemWidth = totalWidth / UncollapsedChildrenCount;
                return new Size(availableSize.Width, maxItemHeight);
            }
        }
        else
        {
            return new Size(0, 0);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double newWidth = 0.0;

        foreach (var child in Children)
        {
            if (child.Visibility != Visibility.Collapsed)
            {
                child.Arrange(new Rect(newWidth, 0, maxItemWidth, maxItemHeight));
                newWidth += maxItemWidth + Spacing;
            }
        }

        // Check if there's more width available - if so, recalculate (e.g. whenever Grid.Column is set to Auto)
        if (finalSize.Width > newWidth)
        {
            MeasureOverride(finalSize);
        }
      
        return finalSize;
    }
    private void OnHorizontalAlignmentChanged(DependencyObject sender, DependencyProperty dp)
    {
        InvalidateMeasure();
    }

    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (EqualPanel)d;
        panel.InvalidateMeasure();
    }
}
