using System;
using Services.Data.Core;
using TMPro;
using UnityEngine;

namespace UI.Views.Admin
{
    public sealed class ChangedDataTextUIView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textMeshProUGUI;
        private String _defaultText;
        private IData _data;

        public void Setup(IData data)
        {
            _data = data;
            _defaultText = _textMeshProUGUI.text;
        }

        private void Update()
        {
            var dataIsChanged = _data.IsChanged;
            if ((_textMeshProUGUI.text[^1] == '*'))
            {
                if (!dataIsChanged)
                {
                    _textMeshProUGUI.text = _defaultText;
                }
            }
            else
            {
                if (dataIsChanged)
                {
                    _textMeshProUGUI.text = _defaultText + '*';
                }
            }
        }
    }
}
