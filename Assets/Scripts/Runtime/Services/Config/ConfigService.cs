using System;
using System.Collections.Generic;
using ScriptableObjects;
using Services.Config.Core;
using UnityEngine;

namespace Services.Config
{
    public sealed class ConfigService : MonoBehaviour, IConfigService
    {
        [SerializeField]
        private List<RewardConfig> _rewardConfigs;

        public RewardConfig this[String rewardID] => _rewardConfigs.Find(reward => reward.ID == rewardID);

        public IReadOnlyList<RewardConfig> RewardConfigs => _rewardConfigs;

        public void Dispose()
        {
        }
    }
}
