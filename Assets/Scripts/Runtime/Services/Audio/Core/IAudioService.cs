using Services.Core;
using UnityEngine;

namespace Services.Audio.Core
{
    public interface IAudioService : IService
    {
        public AudioSource SpinFXAudioSource { get; }

        public AudioSource CoinFXAudioSource { get; }

        public void PlayAudioFX(AudioFX audioFX);
    }
}
