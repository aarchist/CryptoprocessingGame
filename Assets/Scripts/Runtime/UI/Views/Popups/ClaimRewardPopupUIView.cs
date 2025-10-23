using System;
using Services;
using Services.Config.Core;
using TMPro;
using UI.Views.Popups.Core;
using UnityEngine;

namespace UI.Views.Popups
{
    public sealed class ClaimRewardPopupUIView : PopupUIView
    {
        [SerializeField]
        private TextMeshProUGUI _rewardNameTextMeshProUGUI;

        public ClaimRewardPopupUIView Setup(String rewardID)
        {
            var rewardConfig = ServiceLocator.Get<IConfigService>()[rewardID];
            _rewardNameTextMeshProUGUI.text = rewardConfig.Name;
            return this;
        }
    }
}
