// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.CanvasViewInternal;

public static partial class FrameworkElementExtensions
{
    /// <summary>
    /// Normally when trying to set a value of a <see cref="DependencyProperty"/> this will update the raw value of the property
    /// and break any <see cref="Binding"/> associated with that property. This method instead retrieves the underlying
    /// <see cref="BindingExpression"/> of the <see cref="DependencyProperty"/>, if one exists, and instead updates
    /// the underlying bound property value directly (using reflection). This is an advanced technique and has not been
    /// widely tested, use with caution.
    /// </summary>
    /// <param name="fe">The <see cref="FrameworkElement"/> with the property to update.</param>
    /// <param name="property">The <see cref="DependencyProperty"/> to update the underlying bound value of.</param>
    /// <param name="value">The new value to update the bound property to.</param>
    public static void SetBindingExpressionValue(this FrameworkElement fe, DependencyProperty property, object value)
    {
        var subBinding = fe.GetBindingExpression(property);
        if (subBinding == null)
        {
            fe.SetValue(property, value);
        }
        else if (subBinding.DataItem is FrameworkElement subfe)
        {
            SetBindingExpressionValue(subfe, property, value);
        }
        else if (subBinding.DataItem != null && subBinding.ParentBinding.Path != null)
        {
            var prop = subBinding.DataItem.GetType().GetProperty(subBinding.ParentBinding.Path.Path);

            prop?.SetValue(subBinding.DataItem, Convert.ChangeType(value, prop.PropertyType));
        }
    }
}
