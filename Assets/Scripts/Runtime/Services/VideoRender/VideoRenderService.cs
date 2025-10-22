using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Data.Video;
using Services.Data.Core;
using Services.VideoRender.Core;
using UnityEngine;
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

            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().VideoRemoved += RemovePlayback;
            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().VideoAdded += AddPlayback;
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

        public async void GetPlayback(VideoData videoData, Action<VideoPlayback> onComplete)
        {
            var playback = _playbacks[videoData];
            await UniTask.WaitUntil(playback, videoPlayback => videoPlayback.IsLoaded);
            onComplete.Invoke(playback);
        }

        private void AddPlayback(VideoData videoData)
        {
            var playback = new VideoPlayback(_gameObject);
            _playbacks.Add(videoData, playback);
            playback.Load(videoData.Path, () =>
            {
                videoData.Resolution = new Vector2(playback.Width, playback.Height);
                videoData.Duration = playback.Duration;
            });
        }

        private void RemovePlayback(VideoData videoData)
        {
            _playbacks.Remove(videoData, out var playback);
            playback.Dispose();
        }

        public void Dispose()
        {
            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().VideoRemoved -= RemovePlayback;
            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().VideoAdded -= RemovePlayback;
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
    }
}
