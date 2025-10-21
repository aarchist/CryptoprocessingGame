using System;
using TMPro;
using UI.Views.Popups.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Popups
{
    public sealed class ConfirmationPopupUIView : PopupUIView
    {
        [SerializeField]
        private TextMeshProUGUI _messageTextMeshProUGUI;
        [SerializeField]
        private TextMeshProUGUI _headerTextMeshProUGUI;
        [SerializeField]
        private Button _confirmButton;
        [SerializeField]
        private Button _cancelButton;

        private Action _onConfirm;
        private Action _onCancel;

        public ConfirmationPopupUIView Setup(String header, String message, Action onConfirm = null, Action onCancel = null)
        {
            _messageTextMeshProUGUI.text = message;
            _headerTextMeshProUGUI.text = header;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
            return this;
        }

        private void OnEnable()
        {
            _confirmButton.onClick.AddListener(Confirm);
            _cancelButton.onClick.AddListener(Cancel);
        }

        private void OnDisable()
        {
            _confirmButton.onClick.RemoveListener(Confirm);
            _cancelButton.onClick.RemoveListener(Cancel);
        }

        private void Confirm()
        {
            _onConfirm?.Invoke();
            Hide();
        }

        private void Cancel()
        {
            _onCancel?.Invoke();
            Hide();
        }
    }
}
