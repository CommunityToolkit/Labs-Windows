// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.WinUI;

namespace TransitionHelperExperiment.Samples;

public sealed class CustomTextScalingCalculator : IScalingCalculator
{
    /// <inheritdoc/>
    public Vector2 GetScaling(UIElement source, UIElement target)
    {
        var sourceTextElement = source?.FindDescendantOrSelf<TextBlock>();
        var targetTextElement = target?.FindDescendantOrSelf<TextBlock>();
        if (sourceTextElement is not null && targetTextElement is not null)
        {
            var scale = targetTextElement.FontSize / sourceTextElement.FontSize;
            return new Vector2((float)scale);
        }

        return new Vector2(1);
    }
}
