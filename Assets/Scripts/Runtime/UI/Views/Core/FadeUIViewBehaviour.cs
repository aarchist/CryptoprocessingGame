using System;
using LitMotion;
using UnityEngine;

namespace UI.Views.Core
{
    public abstract class FadeUIViewBehaviour : UIViewBehaviour
    {
        private MotionHandle _motionHandle;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        public override void Show()
        {
            Fade(1.0F);
        }

        public override void Hide()
        {
            Fade(0.0F);
        }

        private void Fade(Single alpha)
        {
            if (_motionHandle.IsActive())
            {
                _motionHandle.Cancel();
            }

            _motionHandle = LMotion.Create(_canvasGroup.alpha, alpha, 0.25F).Bind(_canvasGroup, (value, group) => group.alpha = value);
        }
    }
}
