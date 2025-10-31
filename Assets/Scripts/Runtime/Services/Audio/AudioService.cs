using System;
using Services.Audio.Core;
using UnityEngine;
using Object = System.Object;

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
        [SerializeField] [Range(0.0F, 1.0F)]
        private Single _coinAnimationFXAudioClipVolume = 0.5F;
        [SerializeField]
        private AudioClip _coinAnimationFXAudioClip;
        [SerializeField] [Range(0.0F, 1.0F)]
        private Single _showCapsuleAudioClipVolume = 0.5F;
        [SerializeField]
        private AudioClip _showCapsuleAudioClip;
        [SerializeField] [Range(0.0F, 1.0F)]
        private Single _spinAudioClipVolume = 0.5F;
        [SerializeField]
        private AudioClip _spinAudioClip;
        [SerializeField] [Range(0.0F, 1.0F)]
        private Single _loseAudioClipVolume = 0.5F;
        [SerializeField]
        private AudioClip _loseAudioClip;
        [SerializeField] [Range(0.0F, 1.0F)]
        private Single _winAudioClipVolume = 0.5F;
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
            _audioSource.PlayOneShot(Get(audioFX), GetVolume(audioFX));
        }

        public void Dispose()
        {
        }

        private Single GetVolume(AudioFX audioFX)
        {
            return audioFX switch
            {
                AudioFX.ShowCapsuleAnimation => _showCapsuleAudioClipVolume,
                AudioFX.CoinAnimation => _showCapsuleAudioClipVolume,
                AudioFX.Spin => _showCapsuleAudioClipVolume,
                AudioFX.Lose => _showCapsuleAudioClipVolume,
                AudioFX.Win => _showCapsuleAudioClipVolume,
                _ => throw new ArgumentOutOfRangeException(nameof(audioFX), audioFX, null)
            };
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
