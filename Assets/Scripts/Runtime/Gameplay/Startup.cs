using System;
using LitMotion;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Gameplay
{
    public sealed class Startup : MonoBehaviour
    {
        private const Single Duration = 0.3F;
        private static readonly Int32 _hideProgress = Animator.StringToHash("HideProgress");

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

        private DepthOfField _depthOfField;
        private Vignette _vignette;

        private MotionHandle _motionHandle;
        private Single _rotation;

        private void Awake()
        {
            _volume.profile.TryGet(out _depthOfField);
            _volume.profile.TryGet(out _vignette);
        }

        public void ShowRewards()
        {
            _depthOfField.active = true;
            _vignette.active = true;
            _animator.Play("HideState");
            _animator.SetFloat(_hideProgress, 1.0F);
            _motionHandle.TryCancel();
            _motionHandle = LMotion.Create(0.0F, 1.0F, Duration)
                .WithLoops(-1, LoopType.Incremental)
                .Bind(progress =>
                {
                    progress = Mathf.Min(1.0F, progress);
                    _depthOfField.focusDistance.value = Mathf.Lerp(3.0F, _focusDistance, progress);
                    _vignette.intensity.value = progress;

                    _rotation += Time.deltaTime * _rotateSpeed;

                    var angle = 360.0F / (transform.childCount - 1);
                    for (var index = 0; index < transform.childCount; index++)
                    {
                        var coin = transform.GetChild(index);
                        coin.localScale = new Vector3(progress, progress, progress);
                        if (index == 0)
                        {
                            coin.localPosition = Vector3.zero;
                            coin.transform.RotateAround(coin.transform.position, coin.transform.up, _coinsRotationSpeed * Time.deltaTime);
                        }
                        else
                        {
                            var radians = _rotation + (angle * index * Mathf.Deg2Rad);
                            var normalizedPosition = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0.0F);
                            coin.transform.localPosition = normalizedPosition * _radius;
                        }
                    }
                });
        }

        public void HideRewards()
        {
            _motionHandle.TryCancel();
            _motionHandle = LMotion.Create(1.0F, 0.0F, Duration)
                .WithEase(Ease.OutSine)
                .WithOnComplete(() =>
                {
                    _depthOfField.active = false;
                    _vignette.active = false;
                    _animator.Play("OpenCapsuleState");
                })
                .Bind(progress =>
                {
                    for (var index = 0; index < transform.childCount; index++)
                    {
                        transform.GetChild(index).localScale = new Vector3(progress, progress, progress);
                    }

                    _animator.SetFloat(_hideProgress, progress);
                    _depthOfField.focusDistance.value = Mathf.Lerp(3.0F, _focusDistance, progress);
                    _vignette.intensity.value = progress;
                });
        }
    }
}
