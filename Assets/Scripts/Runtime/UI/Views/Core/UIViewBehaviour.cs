using System;
using UnityEngine;

namespace UI.Views.Core
{
    public abstract class UIViewBehaviour : MonoBehaviour, IUIView
    {
        public Boolean IsShowed => gameObject.activeSelf;

        public virtual void Initialize()
        {
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
