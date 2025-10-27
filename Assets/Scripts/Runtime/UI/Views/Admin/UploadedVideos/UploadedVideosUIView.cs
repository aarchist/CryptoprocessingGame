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
        private VideoItemControlsUIView _videoItemControlsUIView;
        [SerializeField]
        private ChangedDataTextUIView _changedDataTextUIView;
        [SerializeField]
        private VideoItemUIView _videoItemUIViewPrefab;
        [SerializeField]
        private ScrollRect _scrollRect;

        private UploadedVideosData _uploadedVideosData;

        public Boolean IsVideoActive
        {
            set
            {
                var videoOutput = ServiceLocator.Get<IUIViewService>().Get<VideoOutputUIView>();
                if (value)
                {
                    _videoItemControlsUIView.Play();
                    videoOutput.Show();
                }
                else
                {
                    _videoItemControlsUIView.Pause();
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

            _changedDataTextUIView.Setup(_uploadedVideosData);
            _videoItemControlsUIView.Setup(current =>
            {
                var nextVideoData = _uploadedVideosData.NextFrom(current.VideoData);
                return _uploadedVideoItemsUIViews[nextVideoData];
            });
        }

        private void OnEnable()
        {
            _uploadedVideosData.VideoRemoved += DestroyItem;
            _uploadedVideosData.VideoAdded += CreateItem;
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
            uiView.Clicked += OnClick;
            _uploadedVideoItemsUIViews.Add(videoData, uiView);
        }

        private void DestroyItem(VideoData videoData)
        {
            _uploadedVideoItemsUIViews.Remove(videoData, out var uiView);
            uiView.Clicked -= OnClick;
            if (_videoItemControlsUIView.Selected == uiView)
            {
                _videoItemControlsUIView.Selected = null;
            }

            Destroy(uiView.gameObject);
        }

        private void OnClick(VideoItemUIView uiView)
        {
            _videoItemControlsUIView.Selected = uiView;
        }
    }
}
