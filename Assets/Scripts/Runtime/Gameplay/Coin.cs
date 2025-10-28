using JetBrains.Annotations;
using ScriptableObjects;
using UnityEngine;

namespace Gameplay
{
    public sealed class Coin : MonoBehaviour
    {
        public RewardConfig LastReward;

        [UsedImplicitly]
        public void Claim(RewardConfig rewardConfig)
        {
            LastReward = rewardConfig;
        }
    }
}
