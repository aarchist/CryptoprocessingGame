using System;
using System.Collections.Generic;
using System.Linq;
using Data.Game;
using Data.Reward;
using Data.Video;
using Services;
using Services.Data.Core;
using UI.Views.Core;
using UI.Views.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Admin.Options
{
    public sealed class OptionsUIView : UIViewBehaviour
    {
        [SerializeField]
        private Int32PropertyUIView _capsuleStopDurationProperty;
        [SerializeField]
        private Int32PropertyUIView _inactiveDurationProperty;
        [SerializeField]
        private Int32PropertyUIView _attemptsCountProperty;
        [SerializeField]
        private KeyCodePropertyUIView _keyCodePropertyUIView;
        [SerializeField]
        private ChangedDataTextUIView _changedDataTextUIView;
        [SerializeField]
        private Int32PropertyUIView _lossWeightProperty;
        [SerializeField]
        private Button _resetButton;
        [SerializeField]
        private Button _saveButton;

        private List<IData> _trackedData;
        private GameData _gameData;

        public override void Initialize()
        {
            base.Initialize();
            var dataService = ServiceLocator.Get<IDataService>();
            _gameData = dataService.Get<GameData>();
            _capsuleStopDurationProperty.Setup(() => _gameData.CapsuleStopDuration, duration => _gameData.CapsuleStopDuration = duration);
            _inactiveDurationProperty.Setup(() => _gameData.InactiveSeconds, duration => _gameData.InactiveSeconds = duration);
            _keyCodePropertyUIView.Setup(() => _gameData.GameplayButtonKey, keyCode => _gameData.GameplayButtonKey = keyCode);
            _attemptsCountProperty.Setup(() => _gameData.AttemptsCount, count => _gameData.AttemptsCount = count);
            _lossWeightProperty.Setup(() => _gameData.LossWeight, weight => _gameData.LossWeight = weight);
            _changedDataTextUIView.Setup(_gameData);
            _trackedData = new List<IData> { _gameData, dataService.Get<RewardsData>(), dataService.Get<UploadedVideosData>() };
            UpdateView();
            _gameData.Reloaded += UpdateView;
        }

        private void OnEnable()
        {
            _resetButton.onClick.AddListener(OnResetButtonClick);
            _saveButton.onClick.AddListener(OnSaveButtonClick);
        }

        private void Update()
        {
            var hasChanges = _trackedData.Any(data => data.IsChanged);
            _resetButton.interactable = hasChanges;
            _saveButton.interactable = hasChanges;
        }

        private void OnDisable()
        {
            _resetButton.onClick.RemoveListener(OnResetButtonClick);
            _saveButton.onClick.RemoveListener(OnSaveButtonClick);
        }

        private void OnDestroy()
        {
            _gameData.Reloaded -= UpdateView;
        }

        private void OnResetButtonClick()
        {
            foreach (var data in _trackedData)
            {
                data.Reset();
            }
        }

        private void OnSaveButtonClick()
        {
            foreach (var data in _trackedData)
            {
                data.Save();
            }
        }

        private void UpdateView()
        {
            _capsuleStopDurationProperty.Actualize();
            _inactiveDurationProperty.Actualize();
            _attemptsCountProperty.Actualize();
            _keyCodePropertyUIView.Actualize();
            _lossWeightProperty.Actualize();
        }
    }
}
