using System;
using LitMotion;
using Services.VideoRender;
using UI.Views.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.VideoOutput
{
    public sealed class VideoOutputUIView : UIViewBehaviour, IRenderOutput
    {
        private const Single FadeDuration = 0.2F;

        private MotionHandle _motionHandle;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private RawImage _videoDisplay;

        public void Setup(RenderTexture renderTexture)
        {
            _videoDisplay.texture = renderTexture;
        }

        public override void Show()
        {
            Fade(_canvasGroup.alpha, 1.0F);
        }

        public override void Hide()
        {
            Fade(_canvasGroup.alpha, 0.0F);
        }

        private void Fade(Single from, Single to)
        {
            if (_motionHandle.IsActive())
            {
                _motionHandle.Cancel();
            }

            _motionHandle = LMotion.Create(from, to, FadeDuration).Bind(_canvasGroup, static (alpha, group) => group.alpha = alpha);
        }
    }
}
