using System;
using JetBrains.Annotations;
using ScriptableObjects;
using UnityEngine;

namespace Gameplay
{
    public sealed class Coin : MonoBehaviour
    {
        [SerializeField]
        private Single _volume = 0.1F;

        [NonSerialized]
        public RewardConfig LastReward;

        [UsedImplicitly]
        public void Claim(RewardConfig rewardConfig)
        {
            LastReward = rewardConfig;
        }

        [UsedImplicitly]
        public void Hit()
        {
        }
    }
}
