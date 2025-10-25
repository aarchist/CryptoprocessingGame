using System;
using System.IO;
using Data.Video;
using Services;
using Services.Data.Core;
using Services.FileSystem.Core;
using Services.Gameplay.Core;
using Services.Gameplay.StateMachine.States;
using Services.UIView.Core;
using UI.Views.Popups;
using UnityEngine;
using UnityEngine.UI;
using static UI.Views.Admin.UploadedVideos.VideoItemUIView.State;

namespace UI.Views.Admin.UploadedVideos
{
    public sealed class VideoItemControlsUIView : MonoBehaviour
    {
        [SerializeField]
        private Button _replaceFileButton;
        [SerializeField]
        private Button _removeButton;
        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private Image _playIconImage;
        [SerializeField]
        private Sprite _playSprite;
        [SerializeField]
        private Sprite _pauseSprite;
        [SerializeField]
        private Button _addButton;
        [SerializeField]
        private Image _selectionImage;

        private VideoItemUIView _selectedVideoItemUIView;
        private Next _next;

        public VideoItemUIView Selected
        {
            get => _selectedVideoItemUIView;
            set
            {
                if (_selectedVideoItemUIView == value)
                {
                    return;
                }

                var continuePlaying = _selectedVideoItemUIView && (_selectedVideoItemUIView.ActiveState is Played or Completed);
                if (_selectedVideoItemUIView && _selectedVideoItemUIView.ActiveState is not Stopped)
                {
                    _selectedVideoItemUIView.ActiveState = Stopped;
                }

                _selectedVideoItemUIView = value;
                if (_selectedVideoItemUIView && continuePlaying)
                {
                    _selectedVideoItemUIView.ActiveState = Played;
                }

                _selectionImage.enabled = value;

                if (value)
                {
                    var targetRectTransform = _selectedVideoItemUIView.RectTransform;
                    _selectionImage.rectTransform.sizeDelta = targetRectTransform.sizeDelta;
                    _selectionImage.rectTransform.position = targetRectTransform.position;
                    _selectionImage.rectTransform.pivot = targetRectTransform.pivot;
                }
            }
        }

        private Boolean IsInteractable
        {
            set
            {
                _replaceFileButton.interactable = value;
                _removeButton.interactable = value;
                _playButton.interactable = value;
            }
        }

        private Boolean PlayIsOn
        {
            set => _playIconImage.sprite = value ? _pauseSprite : _playSprite;
        }

        private void OnAddButtonClicked()
        {
            ServiceLocator.Get<IFileSystemService>().ChooseVideoFiles(filePaths =>
            {
                var data = ServiceLocator.Get<IDataService>().Get<UploadedVideosData>();
                foreach (var filePath in filePaths)
                {
                    data.Add(new VideoData()
                    {
                        Path = filePath,
                        Name = Path.GetFileName(filePath)
                    });
                }
            });
        }

        public void Setup(Next next)
        {
            _next = next;
        }

        private void OnEnable()
        {
            _replaceFileButton.onClick.AddListener(OnReplaceButtonClicked);
            _removeButton.onClick.AddListener(OnRemoveButtonClicked);
            _addButton.onClick.AddListener(OnAddButtonClicked);
            _playButton.onClick.AddListener(OnPlayClicked);
        }

        private void Update()
        {
            var isValid = (_selectedVideoItemUIView) && (!_selectedVideoItemUIView.VideoData.IsInvalid);
            IsInteractable = isValid;
            if (!isValid)
            {
                return;
            }

            PlayIsOn = _selectedVideoItemUIView.IsPlaying;
            if (_selectedVideoItemUIView.ActiveState is not Completed)
            {
                return;
            }

            var next = _next.Invoke(_selectedVideoItemUIView);
            if (next == _selectedVideoItemUIView)
            {
                _selectedVideoItemUIView.ActiveState = Played;
                return;
            }

            Selected = next;
        }

        private void OnDisable()
        {
            _replaceFileButton.onClick.RemoveListener(OnReplaceButtonClicked);
            _removeButton.onClick.RemoveListener(OnRemoveButtonClicked);
            _addButton.onClick.RemoveListener(OnAddButtonClicked);
            _playButton.onClick.RemoveListener(OnPlayClicked);
        }

        private void OnReplaceButtonClicked()
        {
            _selectedVideoItemUIView.ReplaceFile();
        }

        private void OnRemoveButtonClicked()
        {
            ServiceLocator.Get<IUIViewService>().Get<ConfirmationPopupUIView>()
                .Setup(
                    "Are you sure?", $"Remove {_selectedVideoItemUIView.VideoData.Name} video from uploaded list?",
                    _selectedVideoItemUIView.Delete)
                .Show();
        }

        private void OnPlayClicked()
        {
            if (_selectedVideoItemUIView.ActiveState is Played)
            {
                _selectedVideoItemUIView.ActiveState = Paused;
            }
            else
            {
                ServiceLocator.Get<IGameplayService>().Enter<ShowVideoGameState>();
                _selectedVideoItemUIView.ActiveState = Played;
            }
        }

        public delegate VideoItemUIView Next(VideoItemUIView from);
    }
}
