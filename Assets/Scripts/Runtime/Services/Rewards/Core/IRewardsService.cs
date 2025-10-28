using System;
using Services.Core;

namespace Services.Rewards.Core
{
    public interface IRewardsService : IService
    {
        public String RandomRewardID();
    }
}
