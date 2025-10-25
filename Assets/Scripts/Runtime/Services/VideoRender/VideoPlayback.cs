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
        private Boolean _hasError;

        public VideoPlayback(GameObject gameObject)
        {
            _videoPlayer = CreateVideoPlayer(gameObject);
            _videoPlayer.targetTexture = _renderTexture;
        }

        public Single LoopProgress => (Single)(_videoPlayer.time / _videoPlayer.length);

        public RenderTexture RenderTexture => _renderTexture;

        public Boolean IsLoaded => _videoPlayer.isPrepared || _hasError;

        public Int32 Duration => (Int32)_videoPlayer.length;

        public Int32 Width => (Int32)_videoPlayer.width;

        public Int32 Height => (Int32)_videoPlayer.height;

        public Boolean IsPlaying => _videoPlayer.isPlaying;

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

        public void Load(String videoPath, Action onComplete = null, Action onError = null)
        {
            Stop();
            _hasError = false;
            _videoPlayer.url = "file://" + videoPath;
            _videoPlayer.prepareCompleted += OnPrepareCompleted;
            _videoPlayer.errorReceived += OnErrorReceived;
            try
            {
                _videoPlayer.Prepare();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[{nameof(VideoPlayback)}] Failed to load video at path: {videoPath}. Cause: {exception}");
            }

            void OnPrepareCompleted(VideoPlayer videoPlayer)
            {
                _videoPlayer.prepareCompleted -= OnPrepareCompleted;
                onComplete?.Invoke();
            }

            void OnErrorReceived(VideoPlayer source, String message)
            {
                _hasError = true;
                _videoPlayer.errorReceived -= OnErrorReceived;
                onError?.Invoke();
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
