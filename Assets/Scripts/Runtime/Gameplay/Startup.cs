using System;
using System.Collections.Generic;
using LitMotion;
using Services;
using Services.Audio;
using Services.Audio.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Gameplay
{
    public sealed class Startup : MonoBehaviour
    {
        private static readonly Int32 _hideProgress = Animator.StringToHash("HideProgress");

        [SerializeField]
        private List<RotateCoinBehaviour> _coinBehaviours;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Volume _volume;
        [SerializeField]
        private Single _coinsRotationSpeed = 100.0F;
        [SerializeField]
        private Single _circleCoinScale = 2.8F;
        [SerializeField]
        private Single _rotateSpeed = 50.0F;
        [SerializeField]
        private Single _focusDistance = 0.2F;
        [SerializeField]
        private Single _radius = 0.225F;

        [SerializeField]
        private Single _capsuleShowDuration = 0.5F;
        [SerializeField]
        private Single _coinsShowDuration = 0.3F;

        private DepthOfField _depthOfField;
        private Vignette _vignette;

        private MotionHandle _motionHandle;
        private MotionHandle _capsuleMotionHandle;
        private Single _rotation;

        private void Awake()
        {
            _volume.profile.TryGet(out _depthOfField);
            _volume.profile.TryGet(out _vignette);
        }

        public void ShowRewards()
        {
            _capsuleMotionHandle.TryCancel();
            _depthOfField.active = true;
            _vignette.active = true;
            _animator.Play("HideState");
            _animator.SetFloat(_hideProgress, 1.0F);

            _motionHandle.TryCancel();
            _motionHandle = LMotion.Create(0.0F, 1.0F, _coinsShowDuration)
                .WithLoops(-1, LoopType.Incremental)
                .Bind(progress =>
                {
                    progress = Mathf.Min(1.0F, progress);
                    _depthOfField.focusDistance.value = Mathf.Lerp(3.0F, _focusDistance, progress);
                    _vignette.intensity.value = progress;

                    foreach (var rotateCoinBehaviour in _coinBehaviours)
                    {
                        var coin = rotateCoinBehaviour.transform;
                        coin.localScale = new Vector3(progress, progress, progress);
                        coin.transform.RotateAround(coin.transform.position, coin.transform.up, _coinsRotationSpeed * Time.deltaTime);
                    }
                });
        }

        public void HideRewards()
        {
            ServiceLocator.Get<IAudioService>().PlayAudioFX(AudioFX.ShowCapsuleAnimation);
            _capsuleMotionHandle.TryCancel();
            _capsuleMotionHandle = LMotion.Create(1.0F, 0.0F, _capsuleShowDuration)
                .WithOnComplete(() =>
                {
                    _depthOfField.active = false;
                    _vignette.active = false;
                    _animator.Play("OpenCapsuleState");
                })
                .Bind(progress =>
                {
                    _depthOfField.focusDistance.value = Mathf.Lerp(3.0F, _focusDistance, progress);
                    _vignette.intensity.value = progress;
                    _animator.SetFloat(_hideProgress, progress);
                });

            _motionHandle.TryCancel();
            _motionHandle = LMotion.Create(1.0F, 0.0F, _coinsShowDuration)
                .WithEase(Ease.OutSine)
                .Bind(progress =>
                {
                    for (var index = 0; index < transform.childCount; index++)
                    {
                        transform.GetChild(index).localScale = new Vector3(progress, progress, progress);
                    }
                });
        }
    }
}
