using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using ScriptableObjects;
using Services;
using Services.Rewards.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;

namespace Gameplay
{
    public sealed class Capsule : MonoBehaviour
    {
        private static readonly Int32 _openProgressParameter = Animator.StringToHash("OpenProgress");
        private MotionHandle _motionHandle;

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Single _speed = 2.0F;
        [SerializeField]
        private Single _capsuleSpeed = 450.0F;
        [SerializeField]
        private Transform _capsuleRotationCenter;
        [SerializeField]
        private Transform _targetTransform;
        [SerializeField]
        private SplineContainer _splineContainer;
        [SerializeField]
        private List<GameObject> _rewards;
        [SerializeField]
        private List<RewardConfig> _ids;
        [SerializeField]
        private Single _rewardRotationSpeed = 2.0F;
        [SerializeField]
        private Volume _volume;
        [SerializeField]
        private Coin _coin;

        private GameObject _createdReward;
        private DepthOfField _depthOfField;
        private Vignette _vignette;
        private GameObject _reward;

        private void Awake()
        {
            if (_volume.profile.TryGet<DepthOfField>(out var depthOfField))
            {
                _depthOfField = depthOfField;
            }

            if (_volume.profile.TryGet<Vignette>(out var vignette))
            {
                _vignette = vignette;
            }
        }

        public void ClearReward()
        {
            var reward = _createdReward;
            if (reward)
            {
                LMotion.Create(reward.transform.localScale, Vector3.zero, 0.25F)
                    .WithEase(Ease.InOutSine)
                    .WithOnComplete(() => Destroy(reward))
                    .Bind(scale => reward.transform.localScale = scale);
                _createdReward = null;

                LMotion.Create(0.0F, 1.0F, 1.0F).WithOnComplete(() =>
                {
                    _depthOfField.active = false;
                    _vignette.active = false;
                }).Bind(progress =>
                {
                    _depthOfField.focusDistance.value = Mathf.Lerp(0.5F, 10.0F, progress);
                    _vignette.intensity.value = 1.0F - progress;
                });
            }
        }

        public void Spin()
        {
            ClearReward();

            _animator.Play("BurstState");
            _motionHandle = LMotion.Create(0.0F, 1.0F, 1.0F)
                .WithLoops(-1, LoopType.Incremental)
                .WithEase(Ease.OutSine)
                .Bind(progress =>
                {
                    progress = Mathf.Min(progress, 1.0F);
                    _animator.SetFloat("BurstSpeed", progress);
                    _capsuleRotationCenter.RotateAround(_capsuleRotationCenter.position, _capsuleRotationCenter.up, _capsuleSpeed * progress * Time.deltaTime);
                });
        }

        public async void Stop(Single duration, Action onComplete)
        {
            if (_motionHandle.IsActive())
            {
                _motionHandle.Cancel();
            }

            _motionHandle = LMotion.Create(1.0F, 0.0F, duration)
                .WithEase(Ease.OutSine)
                .Bind(progress => { _capsuleRotationCenter.transform.RotateAround(_capsuleRotationCenter.transform.position, _capsuleRotationCenter.transform.up, progress * Time.deltaTime * _capsuleSpeed); });

            await LMotion.Create(1.0F, 0.1F, duration).Bind(progress => _animator.SetFloat("BurstSpeed", progress));

            _coin.LastReward = null;
            await UniTask.WaitUntil(_coin, coin => coin.LastReward);
            _animator.SetFloat("BurstSpeed", 0.0F);
            _reward = GetReward(_coin.LastReward);
            ServiceLocator.Get<IRewardsService>().Set(_coin.LastReward.ID);
            onComplete?.Invoke();
        }

        private GameObject GetReward(RewardConfig rewardConfig)
        {
            return _rewards[_ids.IndexOf(rewardConfig)];
        }

        public void ShowRewards()
        {
        }

        public async void GiveRewards()
        {
            await LMotion.Create(0.0F, 1.0F, 0.5F).Bind(progress => _animator.SetFloat(_openProgressParameter, progress));

            _createdReward = Instantiate(_reward);
            var createdReward = _createdReward;
            _createdReward.transform.localScale = Vector3.one * 5.0F;
            var startRotation = createdReward.transform.rotation;
            _depthOfField.active = true;
            _vignette.active = true;
            LMotion.Create(0.0F, 1.0F, 0.5F).WithEase(Ease.OutSine).WithOnComplete(() =>
            {
                LMotion.Create(0.0F, 1.0F, 0.5F)
                    .WithLoops(-1, LoopType.Incremental)
                    .Bind(progress =>
                    {
                        if (!createdReward)
                        {
                            return;
                        }

                        progress = Math.Min(progress, 1.0F);
                        createdReward.transform.RotateAround(createdReward.transform.position, Vector3.up, _rewardRotationSpeed * progress * Time.deltaTime);
                    });
            }).Bind(progress =>
            {
                createdReward.transform.rotation = Quaternion.Lerp(startRotation, _targetTransform.rotation, progress);
                createdReward.transform.position = _splineContainer.EvaluatePosition(progress);
            });

            LMotion.Create(0.0F, 1.0F, 0.5F).Bind(progress =>
            {
                _depthOfField.focusDistance.value = Mathf.Lerp(10.0F, 0.5F, progress);
                _vignette.intensity.value = progress;
            });
        }
    }
}
