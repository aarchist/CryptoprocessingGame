using System;
using JetBrains.Annotations;
using ScriptableObjects;
using Services;
using Services.Audio;
using Services.Audio.Core;
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
            ServiceLocator.Get<IAudioService>().PlayAudioFX(AudioFX.CoinAnimation, _volume);
        }
    }
}
