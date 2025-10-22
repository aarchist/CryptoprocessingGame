using System.Collections.Generic;
using ScriptableObjects;
using Services.Core;

namespace Services.Config.Core
{
    public interface IConfigService : IService
    {
        public IReadOnlyList<RewardConfig> RewardConfigs { get; }
    }
}
