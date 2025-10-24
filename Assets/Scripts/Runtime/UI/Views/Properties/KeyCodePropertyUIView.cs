using System;
using Cysharp.Threading.Tasks;
using UI.Views.Properties.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Properties
{
    public sealed class KeyCodePropertyUIView : PropertyUIView<KeyCode>
    {
        [SerializeField]
        private Button _listenButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            _listenButton.onClick.AddListener(ListenButton);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _listenButton.onClick.RemoveListener(ListenButton);
        }

        protected override void ValidateInput(String text)
        {
            if (TryParse(text, out var keyCode))
            {
                Value = keyCode;
                IsValid = true;
                return;
            }

            IsValid = false;
        }

        private async void ListenButton()
        {
            _listenButton.interactable = false;
            await UniTask.WaitUntil(() => Input.anyKeyDown);
            _listenButton.interactable = true;
            if (!TryReadKeyDown(out var keyCode))
            {
                return;
            }

            Value = keyCode;
            Commit(true);
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
            if (text.Length == 0)
            {
                return false;
            }

            foreach (var value in Enum.GetValues(typeof(KeyCode)))
            {
                var keyCodeString = value.ToString();
                if (text != keyCodeString)
                {
                    text = Char.ToUpper(text[0]) + text[1..];
                    if (text != keyCodeString)
                    {
                        continue;
                    }
                }

                keyCode = (KeyCode)value;
                return true;
            }

            return false;
        }
    }
}
