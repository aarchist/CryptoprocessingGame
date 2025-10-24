using System;
using Data.Reward;
using TMPro;
using UI.Views.Core;
using UI.Views.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Admin.Options.Rewards
{
    public sealed class RewardItemUIView : UIViewBehaviour
    {
        [SerializeField]
        private Int32PropertyUIView _weightPropertyUIView;
        [SerializeField]
        private TextMeshProUGUI _nameTextMeshProUGUI;
        [SerializeField]
        private Toggle _toggle;

        private RewardData _rewardData;

        public void Setup(RewardData rewardData)
        {
            _rewardData = rewardData;
            _weightPropertyUIView.Setup(() => _rewardData.Weight, weight => _rewardData.Weight = weight);
            _toggle.SetIsOnWithoutNotify(rewardData.IsActive);
            _nameTextMeshProUGUI.text = rewardData.ID;
        }

        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(UpdateData);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(UpdateData);
        }

        private void UpdateData(Boolean active)
        {
            _rewardData.IsActive = active;
        }
    }
}
