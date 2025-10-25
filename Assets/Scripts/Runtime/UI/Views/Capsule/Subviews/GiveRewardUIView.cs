using System;
using Services;
using Services.Config.Core;
using TMPro;
using UI.Views.Core;
using UnityEngine;

namespace UI.Views.Capsule
{
    public sealed class GiveRewardUIView : UIViewBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _rewardNameTextMeshProUGUI;

        public GiveRewardUIView Setup(String rewardID)
        {
            var rewardConfig = ServiceLocator.Get<IConfigService>()[rewardID];
            _rewardNameTextMeshProUGUI.text = rewardConfig.Name;
            return this;
        }
    }
}
