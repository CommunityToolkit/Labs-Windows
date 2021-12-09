// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace CommunityToolkit.Labs.Uwp
{
    public class CanvasLayout : VirtualizingLayout
    {
        #region Setup / teardown
        protected override void InitializeForContextCore(VirtualizingLayoutContext context)
        {
            base.InitializeForContextCore(context);

            if (!(context.LayoutState is CanvasLayoutState state))
            {
                // Store any state we might need since (in theory) the layout could be in use by multiple 
                // elements simultaneously
                context.LayoutState = new CanvasLayoutState();
            }
        }

        protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
        {
            base.UninitializeForContextCore(context);

            // clear any state
            context.LayoutState = null;
        }

        #endregion

        #region Layout

        protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
        {
            int maxWidth = 0;
            int maxHeight = 0;

            // Get underlying data about positioning of items and determine if in viewport.
            for (int i = 0; i < context.ItemCount; i++)
            {
                if (context.GetItemAt(i) is CanvasLayoutItem item)
                {
                    // See if this item pushes our maximum boundary
                    maxWidth = Math.Max(item.Left + item.Width, maxWidth);
                    maxHeight = Math.Max(item.Top + item.Height, maxHeight);

                    // Calculate if this item is in our current viewport
                    Rect rect = new(item.Left, item.Top, item.Width, item.Height);
                    rect.Intersect(context.RealizationRect);

                    // TODO: If the item is currently in view and will be in view, we should recycle the element's container
                    item.IsInView = rect.Width > 0 || rect.Height > 0;

                    // TODO: If item is in view, we should call GetElementOrCreateAt to realize container here. (And call it's measure method.)
                }
            }

            return new Size(maxWidth, maxHeight);
        }

        protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
        {
            for (int i = 0; i < context.ItemCount; i++)
            {
                if (context.GetItemAt(i) is CanvasLayoutItem item && item.IsInView)
                {
                    var container = context.GetOrCreateElementAt(i);
                    // Is it better to have cached this from above?
                    container.Arrange(new Rect(item.Left, item.Top, item.Width, item.Height));
                }
            }

            return finalSize;
        }

        #endregion
    }

    internal class CanvasLayoutState
    {
        public int FirstRealizedIndex { get; set; }

        /// <summary>
        /// List of layout bounds for items starting with the
        /// FirstRealizedIndex.
        /// </summary>
        public List<Rect> LayoutRects
        {
            get
            {
                if (_layoutRects == null)
                {
                    _layoutRects = new List<Rect>();
                }

                return _layoutRects;
            }
        }

        private List<Rect> _layoutRects;
    }

    // TODO: Make DP? Can we do this with property mapping instead?
    public class CanvasLayoutItem
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsInView { get; internal set; }
    }
}
