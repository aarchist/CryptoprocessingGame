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

        private MotionHandle _displayMotionHandle;
        private MotionHandle _motionHandle;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private RawImage _videoDisplay;
        [SerializeField]
        private RawImage _otherVideoDisplay;

        private RawImage _foregroundDisplay;

        private void Awake()
        {
            _foregroundDisplay = _otherVideoDisplay;
        }

        public void Setup(RenderTexture renderTexture)
        {
            _foregroundDisplay = (_foregroundDisplay == _otherVideoDisplay) ? _videoDisplay : _otherVideoDisplay;

            _foregroundDisplay.transform.SetSiblingIndex(1);
            _foregroundDisplay.texture = renderTexture;
            _displayMotionHandle.TryCancel();
            _displayMotionHandle = LMotion.Create(0.0F, 1.0F, 0.25F)
                .Bind(alpha =>
                {
                    var color = _foregroundDisplay.color;
                    color.a = alpha;
                    _foregroundDisplay.color = color;
                    var other = (_foregroundDisplay == _otherVideoDisplay) ? _videoDisplay : _otherVideoDisplay;
                    color.a = 1.0F - alpha;
                    other.color = color;
                });
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
