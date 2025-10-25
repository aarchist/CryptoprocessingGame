using System;
using Data.Video;
using Services;
using Services.Data.Core;
using Services.FileSystem.Core;
using Services.UIView.Core;
using Services.VideoRender;
using Services.VideoRender.Core;
using TMPro;
using UI.Views.VideoOutput;
using UnityEngine;
using UnityEngine.UI;
using static UI.Views.Admin.UploadedVideos.VideoItemUIView.State;

namespace UI.Views.Admin.UploadedVideos
{
    public sealed class VideoItemUIView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _resolutionTextMeshProUGUI;
        [SerializeField]
        private TextMeshProUGUI _durationTextMeshProUGUI;
        [SerializeField]
        private TextMeshProUGUI _nameTextMeshProUGUI;
        [SerializeField]
        private Image _progressImage;
        [SerializeField]
        private Button _button;

        private VideoPlayback _playback;
        private State _state;

        public event Action<VideoItemUIView> Clicked;

        public RectTransform RectTransform => (RectTransform)transform;

        public VideoData VideoData { get; private set; }

        public State ActiveState
        {
            get => _state;
            set
            {
                _state = value;
                if (_state is Stopped or Completed)
                {
                    _durationTextMeshProUGUI.text = $"{VideoData.Duration / 60:D2}:{VideoData.Duration % 60:D2}";
                    _resolutionTextMeshProUGUI.text = $"[{VideoData.Resolution.x}x{VideoData.Resolution.y}]";
                    _nameTextMeshProUGUI.text = VideoData.Name;
                    ShowedProgress = 0.0F;
                    _playback.Stop();
                    return;
                }

                if (_state is Played)
                {
                    ServiceLocator.Get<IUIViewService>().Get<VideoOutputUIView>().Setup(_playback.RenderTexture);
                    _playback.Play();
                    return;
                }

                if (_state is Paused)
                {
                    _playback.Pause();
                    return;
                }
            }
        }

        private Single ShowedProgress
        {
            set
            {
                _progressImage.enabled = value > 0.001F;
                _progressImage.fillAmount = value;
            }
        }

        public void Setup(VideoData videoData)
        {
            VideoData = videoData;
            ServiceLocator.Get<IVideoRenderService>().GetPlayback(videoData, playback =>
            {
                _playback = playback;
                UpdateView();
            });
            VideoData.Reloaded += UpdateView;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void Update()
        {
            if ((ActiveState is Played) && (_playback != null))
            {
                ShowPlaybackProgress();
                if (_playback.LoopProgress >= 0.99F)
                {
                    ActiveState = Completed;
                }
            }
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnDestroy()
        {
            VideoData.Reloaded -= UpdateView;
        }

        private void UpdateView()
        {
            ActiveState = Stopped;
        }

        private void ShowPlaybackProgress()
        {
            _durationTextMeshProUGUI.text = $"{_playback.Time / 60:D2}:{_playback.Time % 60:D2} / {VideoData.Duration / 60:D2}:{VideoData.Duration % 60:D2}";
            ShowedProgress = _playback.LoopProgress;
        }

        public void ReplaceFile()
        {
            ServiceLocator.Get<IFileSystemService>().ChooseVideoFile(videoPath =>
            {
                var previousState = ActiveState;
                ActiveState = Stopped;
                ServiceLocator.Get<IVideoRenderService>().ReplaceVideo(videoPath, VideoData, () => ActiveState = previousState);
            });
        }

        public void Delete()
        {
            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().Remove(VideoData);
            _playback = null;
        }

        private void OnClick()
        {
            Clicked?.Invoke(this);
        }

        public enum State
        {
            Played,
            Paused,
            Stopped,
            Completed,
        }
    }
}
