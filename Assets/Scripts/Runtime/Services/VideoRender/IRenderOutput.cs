using UnityEngine;

namespace Services.VideoRender
{
    public interface IRenderOutput
    {
        public void Setup(RenderTexture renderTexture);
    }
}
