using System;
using Services.Audio.Core;
using UnityEngine;
using Object = System.Object;

namespace Services.Audio
{
    public sealed class AudioService : MonoBehaviour, IAudioService
    {
        [SerializeField]
        private AudioClip _coinAnimationFXAudioClip;
        [SerializeField]
        private AudioSource _coinFXAudioSource;
        [SerializeField]
        private AudioSource _spinFXAudioSource;
        [SerializeField]
        private AudioClip _showCapsuleAudioClip;
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _spinAudioClip;
        [SerializeField]
        private AudioClip _loseAudioClip;
        [SerializeField]
        private AudioClip _winAudioClip;

        public AudioSource SpinFXAudioSource => _spinFXAudioSource;

        public AudioSource CoinFXAudioSource => _coinFXAudioSource;

        public void PlayAudioFX(AudioFX audioFX, Single volume)
        {
            _audioSource.PlayOneShot(Get(audioFX), volume);
        }

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
                AudioFX.ShowCapsuleAnimation => _showCapsuleAudioClip,
                AudioFX.CoinAnimation => _coinAnimationFXAudioClip,
                AudioFX.Spin => _spinAudioClip,
                AudioFX.Lose => _loseAudioClip,
                AudioFX.Win => _winAudioClip,
                _ => throw new ArgumentOutOfRangeException(nameof(audioFX), audioFX, null)
            };
        }
    }
}
