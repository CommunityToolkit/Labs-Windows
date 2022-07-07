#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235
namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;


    public sealed class CompositionCollectionView : Control
    {
        private Canvas? _contentPanel;
        private ILayout? _layout;
        private Action? _pendingSourceUpdate;
        public Layout<TId>? Layout<TId>() => _layout as Layout<TId>;

        public delegate void LayoutChangedHandler(CompositionCollectionView sender, ILayout newLayout);
        public event LayoutChangedHandler? LayoutChanged;

        public CompositionCollectionView()
        {
            this.DefaultStyleKey = typeof(CompositionCollectionView);
        }

        public void SetLayout(ILayout layout)
        {
            _layout = layout;
            _layout.LayoutReplaced += OnLayoutReplaced;

            if (_contentPanel is not null)
            {
                _layout.Activate(_contentPanel);
            }
        }

        public void UpdateSource<TId>(IEnumerable<(TId id, Action<CompositionPropertySet, Dictionary<string, object?>> populateProperties)> source, Action? updateCallback = null)
        {
            .if (_contentPanel is not null)
            {
                (_layout as Layout<TId>)?.UpdateSource(source, updateCallback);
            }
            else
            {
                _pendingSourceUpdate = () => (_layout as Layout<TId>)?.UpdateSource(source);
                updateCallback?.Invoke();
            }
        }

        private void OnLayoutReplaced(ILayout sender, ILayout newLayout)
        {
            if (_layout is not null)
            {
                _layout.LayoutReplaced -= OnLayoutReplaced;
            }

            _layout = newLayout;
            _layout.LayoutReplaced += OnLayoutReplaced;
            LayoutChanged?.Invoke(this, _layout);
        }

        protected override void OnApplyTemplate()
        {
            if (GetTemplateChild("contentPanel") is Canvas contentPanel)
            {
                _contentPanel = contentPanel;
                if (_layout is not null)
                {
                    _layout.Activate(_contentPanel);
                }
                if(_pendingSourceUpdate is not null)
                {
                    _pendingSourceUpdate?.Invoke();
                    _pendingSourceUpdate = null;
                }
            }
        }
    }
