using System;
using Data.Video;
using Services;
using Services.Data.Core;
using Services.FileSystem.Core;
using Services.UIView.Core;
using Services.VideoRender;
using Services.VideoRender.Core;
using TMPro;
using UI.Views.Popups;
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
        private Button _replaceFileButton;
        [SerializeField]
        private Button _removeButton;
        [SerializeField]
        private Image _progressImage;
        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private Image _playIconImage;
        [SerializeField]
        private Image _pauseIconImage;

        private VideoPlayback _playback;
        private State _state;

        public event Action<VideoItemUIView> PlayClicked;

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
                    PlayIconEnabled = true;
                    ShowedProgress = 0.0F;
                    _playback.Stop();
                    return;
                }

                if (_state is Played)
                {
                    ServiceLocator.Get<IUIViewService>().Get<VideoOutputUIView>().Setup(_playback.RenderTexture);
                    PlayIconEnabled = false;
                    _playback.Play();
                    return;
                }

                if (_state is Paused)
                {
                    PlayIconEnabled = true;
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

        private Boolean PlayIconEnabled
        {
            set
            {
                _playIconImage.enabled = value;
                _pauseIconImage.enabled = !value;
            }
        }

        public void Setup(VideoData videoData)
        {
            VideoData = videoData;
            ServiceLocator.Get<IVideoRenderService>().GetPlayback(videoData, playback =>
            {
                _playback = playback;
                ActiveState = Stopped;
            });
        }

        private void OnEnable()
        {
            _replaceFileButton.onClick.AddListener(ReplaceFile);
            _removeButton.onClick.AddListener(ShowConfirmation);
            _playButton.onClick.AddListener(OnPlayClicked);
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
            _replaceFileButton.onClick.RemoveListener(ReplaceFile);
            _removeButton.onClick.RemoveListener(ShowConfirmation);
            _playButton.onClick.RemoveListener(OnPlayClicked);
        }

        private void ShowPlaybackProgress()
        {
            _durationTextMeshProUGUI.text = $"{_playback.Time / 60:D2}:{_playback.Time % 60:D2} / {VideoData.Duration / 60:D2}:{VideoData.Duration % 60:D2}";
            ShowedProgress = _playback.LoopProgress;
        }

        private void ReplaceFile()
        {
            ServiceLocator.Get<IFileSystemService>().ChooseVideoFile(videoPath =>
            {
                var previousState = ActiveState;
                ActiveState = Stopped;
                ServiceLocator.Get<IVideoRenderService>().ReplaceVideo(videoPath, VideoData, () => ActiveState = previousState);
            });
        }

        private void ShowConfirmation()
        {
            ServiceLocator.Get<IUIViewService>().Get<ConfirmationPopupUIView>()
                .Setup("Are you sure?", $"Remove {VideoData.Name} video from uploaded list?", Delete)
                .Show();
        }

        private void Delete()
        {
            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().Remove(VideoData);
            _playback = null;
        }

        private void OnPlayClicked()
        {
            PlayClicked?.Invoke(this);
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
