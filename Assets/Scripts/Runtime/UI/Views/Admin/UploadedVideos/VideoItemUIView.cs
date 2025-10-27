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
        private Button _removeButton;
        [SerializeField]
        private Image _progressImage;
        [SerializeField]
        private Button _button;

        private VideoPlayback _playback;
        private State _state;

        public event Action<VideoItemUIView> Clicked;

        public RectTransform RectTransform => (RectTransform)transform;

        public VideoData VideoData { get; private set; }

        public Boolean IsPlaying => _playback.IsPlaying;

        public State ActiveState
        {
            get => _state;
            set
            {
                if (VideoData.IsInvalid)
                {
                    return;
                }

                if (value is not Paused)
                {
                    _durationTextMeshProUGUI.text = $"{VideoData.Duration / 60:D2}:{VideoData.Duration % 60:D2}";
                    _resolutionTextMeshProUGUI.text = $"[{VideoData.Resolution.x}x{VideoData.Resolution.y}]";
                    _nameTextMeshProUGUI.text = VideoData.Name;
                    ShowedProgress = 0.0F;
                }

                _state = value;
                if (_state is Stopped or Completed)
                {
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
            VideoData.ValidationUpdated += OnUpdateValidation;
            VideoData.Reloaded += UpdateView;
        }

        private void OnEnable()
        {
            _removeButton.onClick.AddListener(Delete);
            _button.onClick.AddListener(OnClick);
        }

        private void Update()
        {
            if ((ActiveState is not Played) || (_playback == null))
            {
                return;
            }

            ShowPlaybackProgress();
            if (_playback.LoopProgress >= 0.98F)
            {
                ActiveState = Completed;
            }
        }

        private void OnDisable()
        {
            _removeButton.onClick.RemoveListener(Delete);
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnDestroy()
        {
            VideoData.ValidationUpdated -= OnUpdateValidation;
            VideoData.Reloaded -= UpdateView;
        }

        private void UpdateView()
        {
            ActiveState = Stopped;
        }

        private void OnUpdateValidation()
        {
            if (VideoData.IsInvalid)
            {
                _removeButton.gameObject.SetActive(true);
                _resolutionTextMeshProUGUI.text = "[????:????]";
                _durationTextMeshProUGUI.text = "??:??/??:??";
                _nameTextMeshProUGUI.text = "<color=#F44747FF>Invalid (Probably video file is deleted or damaged)</color>";
                _button.enabled = false;
            }
            else
            {
                _removeButton.gameObject.SetActive(false);
                _button.enabled = true;
                ActiveState = Stopped;
            }
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
