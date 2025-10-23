using System;
using System.Collections.Generic;
using Data.Video;
using Services;
using Services.Data.Core;
using Services.UIView.Core;
using UI.Views.Core;
using UI.Views.VideoOutput;
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
        private GameObject _disconnectedDisplayIcon;
        [SerializeField]
        private GameObject _connectedDisplayIcon;
        [SerializeField]
        private ScrollRect _scrollRect;

        private UploadedVideosData _uploadedVideosData;
        private VideoItemUIView _activeVideoItemUIView;

        public Boolean IsVideoActive
        {
            set
            {
                var videoOutput = ServiceLocator.Get<IUIViewService>().Get<VideoOutputUIView>();
                if (value)
                {
                    videoOutput.Show();
                }
                else
                {
                    videoOutput.Hide();
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _uploadedVideosData = ServiceLocator.Get<IDataService>().Get<UploadedVideosData>();
            foreach (var videoData in _uploadedVideosData.UploadedVideos)
            {
                CreateItem(videoData);
            }
        }

        private void OnEnable()
        {
            _uploadedVideosData.VideoRemoved += DestroyItem;
            _uploadedVideosData.VideoAdded += CreateItem;
        }

        private void Update()
        {
            if (_activeVideoItemUIView && (_activeVideoItemUIView.ActiveState is Completed))
            {
                var nextVideoData = _uploadedVideosData.NextFrom(_activeVideoItemUIView.VideoData);
                var uiView = _uploadedVideoItemsUIViews[nextVideoData];
                if (_activeVideoItemUIView == uiView)
                {
                    _activeVideoItemUIView.ActiveState = Played;
                }
                else
                {
                    OnPlayClicked(uiView);
                }
            }
        }

        private void OnDisable()
        {
            _uploadedVideosData.VideoRemoved -= DestroyItem;
            _uploadedVideosData.VideoAdded -= CreateItem;
        }

        private void CreateItem(VideoData videoData)
        {
            var uiView = Instantiate(_videoItemUIViewPrefab, _scrollRect.content);
            uiView.Setup(videoData);
            uiView.PlayClicked += OnPlayClicked;
            _uploadedVideoItemsUIViews.Add(videoData, uiView);
        }

        private void DestroyItem(VideoData videoData)
        {
            _uploadedVideoItemsUIViews.Remove(videoData, out var uiView);
            if (_activeVideoItemUIView == uiView)
            {
                _activeVideoItemUIView = null;
            }

            uiView.PlayClicked -= OnPlayClicked;
            Destroy(uiView.gameObject);
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
