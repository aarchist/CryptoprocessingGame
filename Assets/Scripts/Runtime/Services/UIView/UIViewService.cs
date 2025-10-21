using System;
using System.Collections.Generic;
using Services.UIView.Core;
using UI.Views.Core;
using UnityEngine;

namespace Services.UIView
{
    public sealed class UIViewService : MonoBehaviour, IUIViewService
    {
        private readonly Dictionary<Type, IUIView> _uiViews = new();
        [SerializeField]
        private List<UIViewBehaviour> _uiViewBehaviours;

        public void Initialize()
        {
            foreach (var uiView in _uiViewBehaviours)
            {
                _uiViews.Add(uiView.GetType(), uiView);
            }

            foreach (var uiView in _uiViewBehaviours)
            {
                uiView.Initialize();
            }
        }

        public TUIView Get<TUIView>() where TUIView : IUIView
        {
            return (TUIView)_uiViews[typeof(TUIView)];
        }

        public void Dispose()
        {
            _uiViews.Clear();
        }
    }
}
