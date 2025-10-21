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

        private VideoData _videoData;
        private Action _onUpdate;
        private State _state;

        public event Action<VideoItemUIView> PlayClicked;

        public VideoPlayback Playback { get; private set; }

        public State ActiveState
        {
            get => _state;
            set
            {
                _state = value;
                if (value == State.Stopped)
                {
                    _durationTextMeshProUGUI.text = $"{_videoData.Duration / 60:D2}:{_videoData.Duration % 60:D2}";
                    _resolutionTextMeshProUGUI.text = $"[{_videoData.Resolution.x}x{_videoData.Resolution.y}]";
                    _nameTextMeshProUGUI.text = _videoData.Name;
                    PlayIconEnabled = true;
                    Progress = 0.0f;
                    Playback.Stop();
                    return;
                }

                if (value == State.Played)
                {
                    ServiceLocator.Get<IUIViewService>().Get<VideoOutputUIView>().Setup(Playback.RenderTexture);
                    PlayIconEnabled = false;
                    Playback.Play();
                    return;
                }

                if (value == State.Paused)
                {
                    PlayIconEnabled = true;
                    Playback.Pause();
                    return;
                }
            }
        }

        private Single Progress
        {
            set
            {
                _progressImage.enabled = value > 0.001f;
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
            _videoData = videoData;
            ServiceLocator.Get<IVideoRenderService>().GetPlaybackOrCreateNew(videoData, playback =>
            {
                Playback = playback;
                ActiveState = State.Stopped;
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
            if (_state is State.Played)
            {
                ShowPlaybackProgress();
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
            _durationTextMeshProUGUI.text = $"{Playback.Time / 60:D2}:{Playback.Time % 60:D2} / {_videoData.Duration / 60:D2}:{_videoData.Duration % 60:D2}";
            Progress = Playback.LoopProgress;
        }

        private void ReplaceFile()
        {
            ServiceLocator.Get<IFileSystemService>().ChooseVideoFile(videoPath => { ServiceLocator.Get<IVideoRenderService>().ReplaceVideo(videoPath, _videoData, () => ActiveState = State.Stopped); });
        }

        private void ShowConfirmation()
        {
            ServiceLocator.Get<IUIViewService>().Get<ConfirmationPopupUIView>()
                .Setup("Are you sure?", $"Remove {_videoData.Name} video from uploaded list?", Delete)
                .Show();
        }

        private void Delete()
        {
            ServiceLocator.Get<IDataService>().Get<UploadedVideosData>().Remove(_videoData);
            Playback = null;
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
        }
    }
}
