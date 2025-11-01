using System;
using System.Collections.Generic;
using System.Linq;
using Data.Game;
using Data.Reward;
using Services.Config.Core;
using Services.Data.Core;
using Services.Rewards.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Services.Rewards
{
    public sealed class RewardsService : IRewardsService
    {
        private IDataService _dataService;
        private RewardsData _rewardsData;
        private GameData _gameData;

        private IEnumerable<RewardData> ActiveRewards => _rewardsData.Rewards.Where(reward => reward.IsActive && reward.Weight > 0);

        private Single TotalWeight => ActiveRewards.Sum(reward => reward.Weight) + _gameData.LossWeight;

        public void Initialize()
        {
            _dataService = ServiceLocator.Get<IDataService>();
            _rewardsData = _dataService.Get<RewardsData>();
            _gameData = _dataService.Get<GameData>();
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
        }

        public String RandomRewardID()
        {
            var randomWeight = Random.Range(0.0f, TotalWeight);
            var currentWeight = 0.0f;

            var rewardID = (String)null;
            foreach (var reward in ActiveRewards)
            {
                currentWeight += reward.Weight;
                if (randomWeight < currentWeight)
                {
                    rewardID = reward.ID;
                    break;
                }
            }

            if (rewardID == null)
            {
                _gameData._loseWeight--;
                _dataService.ChangeSaved<GameData>(gameData => gameData._loseWeight = _gameData._loseWeight);
                _gameData.Reloaded?.Invoke();
            }
            else
            {
                var data = _rewardsData.Get(rewardID);
                data._weight--;
                _dataService.ChangeSaved<RewardsData>(rewardsData => rewardsData.Get(rewardID)._weight = data._weight);
                data.Reloaded?.Invoke();
            }

            return rewardID;
        }

        public void Dispose()
        {
        }
    }
}
