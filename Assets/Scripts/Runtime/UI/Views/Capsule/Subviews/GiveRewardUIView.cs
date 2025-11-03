using System;
using Data.Game;
using Services;
using Services.Config.Core;
using Services.Data.Core;
using Services.Gameplay.Core;
using TMPro;
using UI.Views.Core;
using UnityEngine;

namespace UI.Views.Capsule.Subviews
{
    public sealed class GiveRewardUIView : FadeUIViewBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _rewardNameTextMeshProUGUI;
        [SerializeField]
        private TextMeshProUGUI _headerTextMeshProUGUI;

        public GiveRewardUIView Setup(String rewardID)
        {
            if (rewardID == null)
            {
                _headerTextMeshProUGUI.text = ServiceLocator.Get<IGameplayService>().HasAttempts ? "Try again!" : "Try you luck next time!";
                _rewardNameTextMeshProUGUI.text = "";
                return this;
            }

            var rewardConfig = ServiceLocator.Get<IConfigService>()[rewardID];
            _headerTextMeshProUGUI.text = "Your reward:";
            _rewardNameTextMeshProUGUI.text = rewardConfig.Name + '!';
            return this;
        }
    }
}
