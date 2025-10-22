using System;
using Data.Reward;
using TMPro;
using UI.Views.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Admin.Options.Rewards
{
    public sealed class RewardItemUIView : UIViewBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _nameTextMeshProUGUI;
        [SerializeField]
        private TMP_InputField _weightInputField;
        [SerializeField]
        private Toggle _toggle;

        private RewardData _rewardData;

        public void Setup(RewardData rewardData)
        {
            _rewardData = rewardData;
            _weightInputField.SetTextWithoutNotify(rewardData.Weight.ToString());
            _toggle.SetIsOnWithoutNotify(rewardData.IsActive);
            _nameTextMeshProUGUI.text = rewardData.ID;
        }

        private void OnEnable()
        {
            _weightInputField.onSubmit.AddListener(UpdateWeight);
            _toggle.onValueChanged.AddListener(UpdateData);
        }

        private void OnDisable()
        {
            _weightInputField.onSubmit.RemoveListener(UpdateWeight);
            _toggle.onValueChanged.RemoveListener(UpdateData);
        }

        private void UpdateWeight(String text)
        {
            if (Int32.TryParse(text, out var weight))
            {
                _rewardData.Weight = weight;
                return;
            }

            _weightInputField.SetTextWithoutNotify(text);
        }

        private void UpdateData(Boolean active)
        {
            _rewardData.IsActive = active;
        }
    }
}
