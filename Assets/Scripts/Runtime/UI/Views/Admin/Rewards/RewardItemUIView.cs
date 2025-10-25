using System;
using Data.Reward;
using TMPro;
using UI.Views.Core;
using UI.Views.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Admin.Rewards
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
            UpdateView();
            _rewardData.Reloaded += UpdateView;
        }

        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnToggleClicked);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleClicked);
        }

        private void OnDestroy()
        {
            _rewardData.Reloaded -= UpdateView;
        }

        private void UpdateView()
        {
            _toggle.SetIsOnWithoutNotify(_rewardData.IsActive);
            _nameTextMeshProUGUI.text = _rewardData.ID;
            _weightPropertyUIView.Actualize();
        }

        private void OnToggleClicked(Boolean active)
        {
            _rewardData.IsActive = active;
        }
    }
}
