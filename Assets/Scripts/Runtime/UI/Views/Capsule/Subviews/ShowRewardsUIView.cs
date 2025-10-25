using System;
using System.Collections.Generic;
using Services;
using Services.Config.Core;
using UI.Views.Core;
using UI.Views.Rewards;
using UnityEngine;

namespace UI.Views.Capsule
{
    public sealed class ShowRewardsUIView : UIViewBehaviour
    {
        private readonly Dictionary<String, RewardUIView> _rewards = new();

        [SerializeField]
        private RectTransform _contentRectTransform;
        [SerializeField]
        private RewardUIView _rewardUIViewPrefab;

        public override void Initialize()
        {
            base.Initialize();

            foreach (var rewardConfig in ServiceLocator.Get<IConfigService>().RewardConfigs)
            {
                var uiView = Instantiate(_rewardUIViewPrefab, _contentRectTransform);
                uiView.Setup(rewardConfig);
                _rewards.Add(rewardConfig.ID, uiView);
            }
        }
    }
}
