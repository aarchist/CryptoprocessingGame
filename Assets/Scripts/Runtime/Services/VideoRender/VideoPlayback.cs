using System;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace Services.VideoRender
{
    public sealed class VideoPlayback
    {
        private readonly RenderTexture _renderTexture = new(1920, 1080, 24);
        private readonly VideoPlayer _videoPlayer;
        private IRenderOutput _output;

        public VideoPlayback(GameObject gameObject)
        {
            _videoPlayer = CreateVideoPlayer(gameObject);
            _videoPlayer.targetTexture = _renderTexture;
        }

        public Single LoopProgress => (Single)(_videoPlayer.time / _videoPlayer.length);

        public RenderTexture RenderTexture => _renderTexture;

        public Boolean IsLoaded => _videoPlayer.isPrepared;

        public Int32 Duration => (Int32)_videoPlayer.length;

        public Int32 Width => (Int32)_videoPlayer.width;

        public Int32 Height => (Int32)_videoPlayer.height;

        public Int32 Time => (Int32)_videoPlayer.time;

        public void Play()
        {
            _renderTexture.Create();
            _videoPlayer.Play();
        }

        public void Pause()
        {
            _videoPlayer.Pause();
        }

        public void Stop()
        {
            _videoPlayer.Stop();
            _renderTexture.Release();
        }

        public void Load(String videoPath, Action onComplete = null)
        {
            Stop();
            _videoPlayer.url = "file://" + videoPath;
            _videoPlayer.prepareCompleted += OnPrepareCompleted;
            _videoPlayer.Prepare();

            void OnPrepareCompleted(VideoPlayer videoPlayer)
            {
                _videoPlayer.prepareCompleted -= OnPrepareCompleted;
                onComplete?.Invoke();
            }
        }

        public void Dispose()
        {
            if (_videoPlayer)
            {
                _videoPlayer.Stop();
                Object.Destroy(_videoPlayer);
            }

            _renderTexture.Release();
            Object.Destroy(_renderTexture);
        }

        private static VideoPlayer CreateVideoPlayer(GameObject gameObject)
        {
            var videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
            videoPlayer.source = VideoSource.Url;
            videoPlayer.playOnAwake = false;
            return videoPlayer;
        }
    }
}
