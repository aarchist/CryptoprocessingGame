using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LitMotion;
using ScriptableObjects;
using Services;
using Services.Rewards.Core;
using Services.UIView.Core;
using UI.Views.Capsule;
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
        [SerializeField]
        private AnimationClip _burstAnimationClip;

        private GameObject _createdReward;
        private DepthOfField _depthOfField;
        private Vignette _vignette;
        private GameObject _reward;
        private String _rewardTargetID;

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
            _rewardTargetID = null;
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

        private Single _rotatedAngle;

        public void Spin()
        {
            ClearReward();

            _animator.Play("BurstState");
            _rotatedAngle = 0.0F;
            _motionHandle = LMotion.Create(0.0F, 1.0F, 1.0F)
                .WithLoops(-1, LoopType.Incremental)
                .WithEase(Ease.OutSine)
                .Bind(progress =>
                {
                    progress = Mathf.Min(progress, 1.0F);
                    _animator.SetFloat("BurstSpeed", progress);
                    var angle = _capsuleSpeed * progress * Time.deltaTime;
                    _rotatedAngle += angle;
                    _capsuleRotationCenter.RotateAround(_capsuleRotationCenter.position, _capsuleRotationCenter.up, angle);
                });
        }

        public async void Stop(Single minDuration, Action onComplete)
        {
            if (_motionHandle.IsActive())
            {
                _motionHandle.Cancel();
            }

            _rewardTargetID = ServiceLocator.Get<IRewardsService>().RandomRewardID();
            var config = GetConfigWithID(_rewardTargetID);
            var remainingSeconds = (minDuration + GetRemainingSeconds(config, minDuration)) * 2.0F;
            _reward = GetReward(config);
            LMotion.Create(1.0F, 0.0F, remainingSeconds)
                .Bind(progress => _animator.SetFloat("BurstSpeed", progress));

            var speed = _capsuleSpeed / 2;
            var startRotation = _capsuleRotationCenter.rotation;
            var extraRotation = Mathf.Floor((remainingSeconds * speed) / 360.0F) * 360.0F;
            var remainingAngle = 360.0F - (_rotatedAngle % 360.0F);
            _motionHandle = LMotion.Create(0.0F, extraRotation + remainingAngle, (remainingSeconds + (remainingAngle / speed)))
                .WithEase(Ease.OutCubic)
                .Bind(angle =>
                {
                    _capsuleRotationCenter.rotation = startRotation;
                    _capsuleRotationCenter.transform.RotateAround(_capsuleRotationCenter.transform.position, _capsuleRotationCenter.transform.up, angle);
                });

            await _motionHandle;
            onComplete?.Invoke();
        }

        private RewardConfig GetConfigWithID(String id)
        {
            return _ids.Find(config => config.ID == id);
        }

        private Single GetRemainingSeconds(RewardConfig rewardConfig, Single timeOffset)
        {
            var currentTime = timeOffset + (_burstAnimationClip.length * (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0F));
            var targetTime = 0.0F;

            foreach (var animationEvent in _burstAnimationClip.events)
            {
                if (animationEvent.objectReferenceParameter == rewardConfig)
                {
                    targetTime = animationEvent.time;
                    break;
                }
            }

            if (targetTime < currentTime)
            {
                return targetTime + (_burstAnimationClip.length - currentTime);
            }

            return targetTime - currentTime;
        }

        private GameObject GetReward(RewardConfig rewardConfig)
        {
            return _rewards[_ids.IndexOf(rewardConfig)];
        }

        public void ShowRewards()
        {
            _animator.Play("EmptyState");
        }

        public async void GiveRewards(Action onComplete)
        {
            ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().ShowGiveReward(_rewardTargetID);
            await LMotion.Create(0.0F, 1.0F, 0.5F).Bind(progress => _animator.SetFloat(_openProgressParameter, progress));

            _createdReward = Instantiate(_reward);
            var createdReward = _createdReward;
            _createdReward.transform.localScale = Vector3.one * 5.0F;
            var startRotation = createdReward.transform.rotation;
            _depthOfField.active = true;
            _vignette.active = true;
            var seconds = 0.5F;
            LMotion.Create(0.0F, 1.0F, seconds).WithEase(Ease.OutSine).WithOnComplete(() =>
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

            LMotion.Create(0.0F, 1.0F, seconds).Bind(progress =>
            {
                _depthOfField.focusDistance.value = Mathf.Lerp(10.0F, 0.5F, progress);
                _vignette.intensity.value = progress;
            });

            await UniTask.WaitForSeconds(seconds);
            onComplete?.Invoke();
        }
    }
}
