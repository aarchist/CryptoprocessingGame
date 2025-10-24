using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Properties.Core
{
    public abstract class PropertyUIView<TValue> : MonoBehaviour
    {
        private static readonly Color InvalidValueBackgroundColor = new Color32(0x3F, 0x2A, 0x2A, 0xFF);
        private static readonly Color InvalidValueTextColor = new Color32(0xF4, 0x47, 0x47, 0xFF);

        [SerializeField]
        private TMP_InputField _inputField;
        [SerializeField]
        private Image _backgroundImage;

        private Color _defaultPlaceholderColor;
        private Color _defaultBackgroundColor;
        private Color _defaultTextColor;

        private Getter _onGetValue;
        private Setter _onSetValue;
        private Boolean _isValid;

        protected TValue Value;

        private TValue ActualValue
        {
            get => _onGetValue.Invoke();
            set => _onSetValue.Invoke(value);
        }

        protected Boolean IsValid
        {
            set
            {
                _isValid = value;
                if (_isValid)
                {
                    _inputField.placeholder.color = _defaultPlaceholderColor;
                    _inputField.textComponent.color = _defaultTextColor;
                    _backgroundImage.color = _defaultBackgroundColor;
                }
                else
                {
                    _inputField.placeholder.color = InvalidValueTextColor;
                    _inputField.textComponent.color = InvalidValueTextColor;
                    _backgroundImage.color = InvalidValueBackgroundColor;
                }
            }
        }

        public void Setup(Getter onGetValue, Setter onSetValue)
        {
            _onGetValue = onGetValue;
            _onSetValue = onSetValue;
            _inputField.SetTextWithoutNotify(ActualValue.ToString());
        }

        protected virtual void Awake()
        {
            _defaultPlaceholderColor = _inputField.placeholder.color;
            _defaultTextColor = _inputField.textComponent.color;
            _defaultBackgroundColor = _backgroundImage.color;
        }

        protected virtual void OnEnable()
        {
            _inputField.onValueChanged.AddListener(ValidateInput);
            _inputField.onEndEdit.AddListener(Commit);
        }

        protected virtual void OnDisable()
        {
            _inputField.onValueChanged.RemoveListener(ValidateInput);
            _inputField.onEndEdit.RemoveListener(Commit);
        }

        protected abstract void ValidateInput(String text);

        protected void Commit(Boolean applyValue)
        {
            if (applyValue)
            {
                ActualValue = Value;
            }

            _inputField.SetTextWithoutNotify(ActualValue.ToString());
            IsValid = true;
        }

        private void Commit(String text)
        {
            Commit(_isValid);
        }

        public delegate TValue Getter();

        public delegate void Setter(TValue value);
    }
}
