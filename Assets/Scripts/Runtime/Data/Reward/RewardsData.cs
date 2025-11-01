using System;
using System.Collections.Generic;
using System.Linq;
using Services.Data.Core;
using UnityEngine;

namespace Data.Reward
{
    [Serializable]
    public sealed class RewardsData : BaseData
    {
        [SerializeField]
        private List<RewardData> _rewards = new();

        public IReadOnlyList<RewardData> Rewards => _rewards;

        public override Boolean IsChanged
        {
            get => _rewards.Any(reward => reward.IsChanged);
            set => _rewards.ForEach(data => data.IsChanged = value);
        }

        protected override void LoadChanges(IData other)
        {
            if (other is not RewardsData rewardData)
            {
                return;
            }

            var otherRewards = rewardData._rewards;
            for (var index = 0; index < _rewards.Count; index++)
            {
                _rewards[index].LoadChanges(otherRewards[index]);
            }
        }

        public Boolean ContainsID(String dataID)
        {
            return _rewards.Any(reward => reward.ID == dataID);
        }

        public RewardData Get(String rewardID)
        {
            foreach (var rewardData in _rewards)
            {
                if (rewardData.ID == rewardID)
                {
                    return rewardData;
                }
            }

            return null;
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
