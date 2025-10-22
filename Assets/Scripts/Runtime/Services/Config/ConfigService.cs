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

        public IReadOnlyList<RewardConfig> RewardConfigs => _rewardConfigs;

        public void Dispose()
        {
        }
    }
}
