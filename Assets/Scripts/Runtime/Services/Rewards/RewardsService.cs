using System;
using System.Collections.Generic;
using System.Linq;
using Data.Reward;
using Services.Config.Core;
using Services.Data.Core;
using Services.Rewards.Core;
using Random = UnityEngine.Random;

namespace Services.Rewards
{
    public sealed class RewardsService : IRewardsService
    {
        private RewardsData _rewardsData;

        private IEnumerable<RewardData> ActiveRewards => _rewardsData.Rewards.Where(reward => reward.IsActive);

        private Single TotalWeight => ActiveRewards.Sum(reward => reward.Weight);

        public void Initialize()
        {
            _rewardsData = ServiceLocator.Get<IDataService>().Get<RewardsData>();
            var rewardConfigs = ServiceLocator.Get<IConfigService>().RewardConfigs;
            _rewardsData.RemoveAllExcept(rewardConfigs.Select(rewardConfig => rewardConfig.ID).ToList());
            foreach (var rewardConfig in rewardConfigs)
            {
                if (_rewardsData.ContainsID(rewardConfig.ID))
                {
                    continue;
                }

                _rewardsData.Add(rewardConfig.CreateData());
            }
            _rewardsData.Save();

            ServiceLocator.Get<IDataService>().Get<RewardsData>();
        }

        public String RandomRewardID()
        {
            var randomWeight = Random.Range(0.0f, TotalWeight);
            var currentWeight = 0.0f;

            foreach (var reward in ActiveRewards)
            {
                currentWeight += reward.Weight;
                if (randomWeight < currentWeight)
                {
                    return reward.ID;
                }
            }

            return _rewardsData.Rewards[^1].ID;
        }

        public void Dispose()
        {
        }
    }
}
