using System.Linq;
using Data.Reward;
using Services;
using Services.Config.Core;
using Services.Data.Core;
using UI.Views.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Admin.Options.Rewards
{
    public sealed class RewardsUIView : UIViewBehaviour
    {
        [SerializeField]
        private RewardItemUIView _rewardItemUIViewPrefab;
        [SerializeField]
        private ScrollRect _scrollRect;

        public override void Initialize()
        {
            base.Initialize();

            var rewardsData = ServiceLocator.Get<IDataService>().Get<RewardsData>();
            var rewardConfigs = ServiceLocator.Get<IConfigService>().RewardConfigs;
            rewardsData.RemoveAllExcept(rewardConfigs.Select(rewardConfig => rewardConfig.ID).ToList());
            foreach (var rewardConfig in rewardConfigs)
            {
                if (rewardsData.ContainsID(rewardConfig.ID))
                {
                    continue;
                }

                rewardsData.Add(rewardConfig.CreateData());
            }

            foreach (var rewardData in rewardsData.Rewards)
            {
                CreateData(rewardData);
            }
        }

        private void CreateData(RewardData rewardData)
        {
            Instantiate(_rewardItemUIViewPrefab, _scrollRect.content).Setup(rewardData);
        }
    }
}
