using System;
using System.Collections.Generic;
using System.Linq;
using Services.Data.Core;
using UnityEngine;

namespace Data.Reward
{
    [Serializable]
    public class RewardsData : IData
    {
        [SerializeField]
        private List<RewardData> _rewards = new();

        public IReadOnlyList<RewardData> Rewards => _rewards;

        public Boolean ContainsID(String dataID)
        {
            return _rewards.Any(reward => reward.ID == dataID);
        }

        public void Add(RewardData rewardData)
        {
            _rewards.Add(rewardData);
        }

        public void RemoveAllExcept(List<String> ids)
        {
            _rewards.RemoveAll(reward => !ids.Contains(reward.ID));
        }
    }
}
