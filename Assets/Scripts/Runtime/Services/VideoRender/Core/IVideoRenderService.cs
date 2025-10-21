using System;
using Data.Video;
using Services.Core;

namespace Services.VideoRender.Core
{
    public interface IVideoRenderService : IService
    {
        public void GetPlaybackOrCreateNew(VideoData videoData, Action<VideoPlayback> onComplete);

        public void ReplaceVideo(String videoPath, VideoData videoData, Action onComplete);

        public void LoadVideo(String videoPath, Action<VideoData> onComplete);
    }
}
