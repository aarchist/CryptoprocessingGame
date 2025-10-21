using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Data.Video;
using Services.Data.Core;
using Services.VideoRender.Core;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace Services.VideoRender
{
    public sealed class VideoRenderService : IVideoRenderService
    {
        private readonly Dictionary<VideoData, VideoPlayback> _playbacks = new();
        private readonly RenderTexture _renderTexture;
        private readonly GameObject _gameObject;

        public VideoRenderService()
        {
            _gameObject = new GameObject(nameof(VideoRenderService));
            Object.DontDestroyOnLoad(_gameObject);
        }

        public void Initialize()
        {
            foreach (var uploadedVideo in ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().UploadedVideos)
            {
                AddPlayback(uploadedVideo);
            }

            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().VideoRemoved -= RemovePlayback;
            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().VideoAdded -= AddPlayback;
        }

        public async void ReplaceVideo(String videoPath, VideoData videoData, Action onComplete)
        {
            videoData.Path = videoPath;
            videoData.Name = Path.GetFileName(videoPath);
            if (_playbacks.TryGetValue(videoData, out var playback))
            {
                playback.Load(videoPath, () =>
                {
                    videoData.Resolution = new Vector2(playback.Width, playback.Height);
                    videoData.Duration = playback.Duration;
                    onComplete?.Invoke();
                });
                return;
            }

            AddPlayback(videoData);
            playback = _playbacks[videoData];
            await UniTask.WaitUntil(playback, videoPlayback => videoPlayback.IsLoaded);
            onComplete?.Invoke();
        }

        public async void GetPlaybackOrCreateNew(VideoData videoData, Action<VideoPlayback> onComplete)
        {
            if (_playbacks.TryGetValue(videoData, out var playback))
            {
                onComplete.Invoke(playback);
                return;
            }

            AddPlayback(videoData);
            playback = _playbacks[videoData];
            await UniTask.WaitUntil(playback, videoPlayback => videoPlayback.IsLoaded);
            onComplete.Invoke(playback);
        }

        public void LoadVideo(String videoPath, Action<VideoData> onComplete)
        {
            var playback = new VideoPlayback(CreateVideoPlayer(_gameObject));
            var videoData = new VideoData
            {
                Path = videoPath,
                Name = Path.GetFileName(videoPath)
            };
            _playbacks.Add(videoData, playback);
            playback.Load(videoPath, () =>
            {
                videoData.Resolution = new Vector2(playback.Width, playback.Height);
                videoData.Duration = playback.Duration;
                onComplete?.Invoke(videoData);
            });
        }

        private void AddPlayback(VideoData videoData)
        {
            var playback = new VideoPlayback(CreateVideoPlayer(_gameObject));
            _playbacks.Add(videoData, playback);
            playback.Load(videoData.Path);
        }

        private void RemovePlayback(VideoData videoData)
        {
            _playbacks.Remove(videoData, out var playback);
            playback.Dispose();
        }

        public void Dispose()
        {
            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().VideoRemoved -= RemovePlayback;
            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().VideoAdded -= AddPlayback;
            foreach (var playback in _playbacks.Values)
            {
                playback.Dispose();
            }

            _playbacks.Clear();
            if (_gameObject)
            {
                Object.Destroy(_gameObject);
            }
        }

        private static VideoPlayer CreateVideoPlayer(GameObject gameObject)
        {
            var videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
            videoPlayer.source = VideoSource.Url;
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = true;
            return videoPlayer;
        }
    }
}
