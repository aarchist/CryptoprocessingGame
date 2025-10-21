using System.Collections.Generic;
using Data.Video;
using Services;
using Services.Data.Core;
using UI.Views.Core;
using UnityEngine;
using UnityEngine.UI;
using static UI.Views.Admin.UploadedVideos.VideoItemUIView.State;

namespace UI.Views.Admin.UploadedVideos
{
    public sealed class UploadedVideosUIView : UIViewBehaviour
    {
        private readonly Dictionary<VideoData, VideoItemUIView> _uploadedVideoItemsUIViews = new();

        [SerializeField]
        private VideoItemUIView _videoItemUIViewPrefab;
        [SerializeField]
        private ScrollRect _scrollRect;

        private VideoItemUIView _activeVideoItemUIView;
        private UploadedVideosData _uploadedVideosData;

        public override void Initialize()
        {
            foreach (var videoData in ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().UploadedVideos)
            {
                CreateItem(videoData);
            }
        }

        private void OnEnable()
        {
            _uploadedVideosData = ServiceLocator.Get<IDataService>().Get<UploadedVideosData>();
            _uploadedVideosData.VideoRemoved += DestroyItem;
            _uploadedVideosData.VideoAdded += CreateItem;
        }

        private void OnDisable()
        {
            _uploadedVideosData.VideoRemoved -= DestroyItem;
            _uploadedVideosData.VideoAdded -= CreateItem;
        }

        private void DestroyItem(VideoData videoData)
        {
            _uploadedVideoItemsUIViews.Remove(videoData, out var uiView);
            uiView.PlayClicked -= OnPlayClicked;
            Destroy(uiView.gameObject);
        }

        private void CreateItem(VideoData videoData)
        {
            var uiView = Instantiate(_videoItemUIViewPrefab, _scrollRect.content);
            uiView.Setup(videoData);
            uiView.PlayClicked += OnPlayClicked;
            _uploadedVideoItemsUIViews.Add(videoData, uiView);
        }

        private void OnPlayClicked(VideoItemUIView videoItemUIView)
        {
            if (_activeVideoItemUIView == videoItemUIView)
            {
                videoItemUIView.ActiveState = (videoItemUIView.ActiveState is Played) ? Paused : Played;
                return;
            }

            if (_activeVideoItemUIView)
            {
                _activeVideoItemUIView.ActiveState = Stopped;
            }

            _activeVideoItemUIView = videoItemUIView;
            _activeVideoItemUIView.ActiveState = Played;
        }
    }
}
