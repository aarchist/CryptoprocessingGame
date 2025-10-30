using System;
using Services.Audio.Core;
using UnityEngine;

namespace Services.Audio
{
    public sealed class AudioService : MonoBehaviour, IAudioService
    {
        [SerializeField]
        private AudioSource _coinFXAudioSource;
        [SerializeField]
        private AudioSource _spinFXAudioSource;
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _spinAudioClip;
        [SerializeField]
        private AudioClip _loseAudioClip;
        [SerializeField]
        private AudioClip _winAudioClip;
        [SerializeField]
        private AudioClip _coinAnimationFXAudioClip;

        public AudioSource SpinFXAudioSource => _spinFXAudioSource;

        public AudioSource CoinFXAudioSource => _coinFXAudioSource;

        public void PlayAudioFX(AudioFX audioFX)
        {
            _audioSource.PlayOneShot(Get(audioFX));
        }

        public void Dispose()
        {
        }

        private AudioClip Get(AudioFX audioFX)
        {
            return audioFX switch
            {
                AudioFX.CoinAnimation => _coinAnimationFXAudioClip,
                AudioFX.Spin => _spinAudioClip,
                AudioFX.Lose => _loseAudioClip,
                AudioFX.Win => _winAudioClip,
                _ => throw new ArgumentOutOfRangeException(nameof(audioFX), audioFX, null)
            };
        }
    }
}
