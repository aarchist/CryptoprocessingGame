using System;
using Cysharp.Threading.Tasks;
using Data.Game;
using Services;
using Services.Data.Core;
using TMPro;
using UI.Views.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Admin.Options
{
    public sealed class OptionsUIView : UIViewBehaviour
    {
        [SerializeField]
        private TMP_InputField _capsuleSpinDurationInputField;
        [SerializeField]
        private TMP_InputField _showRewardDurationInputField;
        [SerializeField]
        private TMP_InputField _startGameKeyInputField;
        [SerializeField]
        private Button _listenButton;

        private GameData _gameData;

        public override void Initialize()
        {
            base.Initialize();
            _gameData = ServiceLocator.Get<IDataService>().Get<GameData>();
            _showRewardDurationInputField.SetTextWithoutNotify(_gameData.RewardDuration.ToString());
            _capsuleSpinDurationInputField.SetTextWithoutNotify(_gameData.SpinDuration.ToString());
            _startGameKeyInputField.SetTextWithoutNotify(_gameData.StartGameKey.ToString());
          
        }

        private void OnEnable()
        {
            _showRewardDurationInputField.onSubmit.AddListener(ChangeRewardDuration);
            _capsuleSpinDurationInputField.onSubmit.AddListener(ChangeSpinDuration);
            _startGameKeyInputField.onSubmit.AddListener(ChangeStartGameKey);
            _listenButton.onClick.AddListener(ListenButton);
        }

        private void OnDisable()
        {
            _showRewardDurationInputField.onSubmit.RemoveListener(ChangeRewardDuration);
            _capsuleSpinDurationInputField.onSubmit.RemoveListener(ChangeSpinDuration);
            _startGameKeyInputField.onSubmit.RemoveListener(ChangeStartGameKey);
            _listenButton.onClick.RemoveListener(ListenButton);
        }

        private void ChangeRewardDuration(String text)
        {
            if (Int32.TryParse(text, out var duration))
            {
                _gameData.RewardDuration = duration;
                return;
            }

            _showRewardDurationInputField.SetTextWithoutNotify(_gameData.RewardDuration.ToString());
        }

        private void ChangeSpinDuration(String text)
        {
            if (Int32.TryParse(text, out var duration))
            {
                _gameData.SpinDuration = duration;
                return;
            }

            _capsuleSpinDurationInputField.SetTextWithoutNotify(_gameData.SpinDuration.ToString());
        }

        private void ChangeStartGameKey(String text)
        {
            if (TryParse(text, out var keyCode))
            {
                _gameData.StartGameKey = keyCode;
                return;
            }

            _startGameKeyInputField.SetTextWithoutNotify(_gameData.StartGameKey.ToString());
        }

        private async void ListenButton()
        {
            _listenButton.interactable = false;
            await UniTask.WaitUntil(() => Input.anyKeyDown);
            _listenButton.interactable = true;
            if (TryReadKeyDown(out var keyCode))
            {
                _gameData.StartGameKey = keyCode;
                _startGameKeyInputField.SetTextWithoutNotify(_gameData.StartGameKey.ToString());
                Input.ResetInputAxes();
            }
        }

        private static Boolean TryReadKeyDown(out KeyCode keyCode)
        {
            keyCode = 0;
            foreach (var value in Enum.GetValues(typeof(KeyCode)))
            {
                keyCode = (KeyCode)value;
                if (Input.GetKeyDown(keyCode))
                {
                    return true;
                }
            }

            return false;
        }

        private static Boolean TryParse(String text, out KeyCode keyCode)
        {
            keyCode = KeyCode.Return;
            foreach (var value in Enum.GetValues(typeof(KeyCode)))
            {
                if (text != value.ToString())
                {
                    continue;
                }

                keyCode = (KeyCode)value;
                return true;
            }

            return false;
        }
    }
}
