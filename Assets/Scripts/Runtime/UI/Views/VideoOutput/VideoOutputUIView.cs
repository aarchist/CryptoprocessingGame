using Services.VideoRender;
using UI.Views.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.VideoOutput
{
    public sealed class VideoOutputUIView : UIViewBehaviour, IRenderOutput
    {
        [SerializeField]
        private RawImage _videoDisplay;
        [SerializeField]
        private Image _backgroundImage;

        public void Setup(RenderTexture renderTexture)
        {
            _videoDisplay.texture = renderTexture;
        }
    }
}
