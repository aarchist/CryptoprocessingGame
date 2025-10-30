using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Game;
using LitMotion;
using ScriptableObjects;
using Services;
using Services.Audio;
using Services.Audio.Core;
using Services.Data.Core;
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
        private const Single OpenCapsuleSeconds = 0.75F;
        private const Single RotationFXVolume = 1.0F;
        private static readonly Int32 _openProgressParameter = Animator.StringToHash("OpenProgress");
        private static readonly Int32 _burstSpeed = Animator.StringToHash("BurstSpeed");
        private MotionHandle _openCapsuleMotionHandle;
        private MotionHandle _motionHandle;

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Transform _capsuleRotationCenter;
        [SerializeField]
        private Transform _targetTransform;
        [SerializeField]
        private SplineContainer _splineContainer;
        [SerializeField]
        private List<GameObject> _rewards;
        [SerializeField]
        private GameObject _lossGameObject;
        [SerializeField]
        private RewardConfig _lossRewardConfig;
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
        [SerializeField]
        private Single _anglePerAudioFXLoop = 360.0F;
        [SerializeField] [Range(0.0F, 360.0F)]
        private Single _anglePerAudioFXLoopOffset = 45.0F;

        private GameObject _createdReward;
        private DepthOfField _depthOfField;
        private Vignette _vignette;
        private GameObject _reward;
        private String _rewardTargetID;
        private GameData _gameData;

        private void Awake()
        {
            _gameData = ServiceLocator.Get<IDataService>().Get<GameData>();
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
                _createdReward = null;
                LMotion.Create(reward.transform.localScale, Vector3.zero, 0.25F)
                    .WithEase(Ease.InOutSine)
                    .WithOnComplete(() => Destroy(reward))
                    .Bind(scale => reward.transform.localScale = scale);

                LMotion.Create(0.0F, 1.0F, 1.0F)
                    .WithOnComplete(() =>
                    {
                        _depthOfField.active = false;
                        _vignette.active = false;
                    })
                    .Bind(progress =>
                    {
                        _depthOfField.focusDistance.value = Mathf.Lerp(0.4F, 10.0F, progress);
                        _vignette.intensity.value = 1.0F - progress;
                    });
            }
        }

        private Single _rotatedAngle;

        public async void Spin(Action onSpinStarted)
        {
            ServiceLocator.Get<IAudioService>().PlayAudioFX(AudioFX.Spin);
            ClearReward();

            await _openCapsuleMotionHandle;

            _animator.Play("BurstState", 0);
            _rotatedAngle = 0.0F;
            var loopsCount = 0;
         
            _motionHandle.TryCancel();
            _motionHandle = LMotion.Create(0.0F, 1.0F, 1.0F)
                .WithLoops(-1, LoopType.Incremental)
                .WithEase(Ease.OutSine)
                .Bind(progress =>
                {
                    progress = Mathf.Min(progress, 1.0F);
                    _animator.SetFloat(_burstSpeed, progress);
                    var angle = _gameData.SpinSpeed * progress * Time.deltaTime;
                    _rotatedAngle += angle;
                    if (Mathf.Floor((_anglePerAudioFXLoopOffset + _rotatedAngle) / _anglePerAudioFXLoop) > loopsCount)
                    {
                        loopsCount++;
                        ServiceLocator.Get<IAudioService>().PlayAudioFX(AudioFX.Spin);
                    }

                    _capsuleRotationCenter.RotateAround(_capsuleRotationCenter.position, _capsuleRotationCenter.up, angle);
                });

            onSpinStarted?.Invoke();
        }

        public async void Stop(Single minDuration, Action onComplete)
        {
            _motionHandle.TryCancel();
            _rewardTargetID = ServiceLocator.Get<IRewardsService>().RandomRewardID();
            var config = GetConfigWithID(_rewardTargetID);
            minDuration *= 0.5F;
            var remainingSeconds = (minDuration + GetRemainingSeconds(config, minDuration)) * 2.0F;
            _reward = GetReward(config);
            LMotion.Create(1.0F, 0.0F, remainingSeconds)
                .Bind(progress => _animator.SetFloat(_burstSpeed, progress));

            var speed = _gameData.SpinSpeed / 2;
            var startRotation = _capsuleRotationCenter.rotation;
            var extraRotation = Mathf.Floor((remainingSeconds * speed) / 360.0F) * 360.0F;
            var remainingAngle = 360.0F - (_rotatedAngle % 360.0F);
            var loopsCount = Mathf.Floor((_anglePerAudioFXLoopOffset + _rotatedAngle) / _anglePerAudioFXLoop);
            var seconds = (remainingSeconds + (remainingAngle / speed));
            _motionHandle = LMotion.Create(0.0F, extraRotation + remainingAngle, seconds)
                .WithEase(Ease.OutCubic)
                .Bind(angle =>
                {
                    if (Mathf.Floor((angle + _rotatedAngle + _anglePerAudioFXLoopOffset) / _anglePerAudioFXLoop) > loopsCount)
                    {
                        loopsCount++;
                        ServiceLocator.Get<IAudioService>().PlayAudioFX(AudioFX.Spin);
                    }

                    _capsuleRotationCenter.rotation = startRotation;
                    _capsuleRotationCenter.transform.RotateAround(_capsuleRotationCenter.transform.position, _capsuleRotationCenter.transform.up, angle);
                });
            await _motionHandle;
            onComplete?.Invoke();
        }

        private RewardConfig GetConfigWithID(String id)
        {
            if (id == null)
            {
                return _lossRewardConfig;
            }

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
                return targetTime + (_burstAnimationClip.length - (currentTime % _burstAnimationClip.length));
            }

            return targetTime - currentTime;
        }

        private GameObject GetReward(RewardConfig rewardConfig)
        {
            if (rewardConfig == _lossRewardConfig)
            {
                return _lossGameObject;
            }

            return _rewards[_ids.IndexOf(rewardConfig)];
        }

        public void ShowRewards()
        {
            _animator.Play("EmptyState", 0);
        }

        public async void GiveRewards(Action onComplete)
        {
            ServiceLocator.Get<IAudioService>().PlayAudioFX(_rewardTargetID == null ? AudioFX.Lose : AudioFX.Win);
            ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().ShowGiveReward(_rewardTargetID);

            _createdReward = Instantiate(_reward, _reward.transform.position, _reward.transform.rotation);
            _animator.Play("EmptyState", 0);
            _openCapsuleMotionHandle = LMotion.Create(0.0F, 1.0F, OpenCapsuleSeconds)
                .WithEase(Ease.InOutCubic)
                .WithLoops(2, LoopType.Flip)
                .Bind(progress => _animator.SetFloat(_openProgressParameter, progress));
            await UniTask.WaitForSeconds(OpenCapsuleSeconds * 0.25F);

            _depthOfField.active = true;
            _vignette.active = true;
            if (_reward == _lossGameObject)
            {
                foreach (var child in _createdReward.GetComponentsInChildren<Renderer>())
                {
                    child.gameObject.layer = 3;
                }
            }

            var seconds = 1.0F;
            var reward = _createdReward;
            var startScale = _createdReward.transform.localScale;
            var startRotation = _createdReward.transform.rotation;
            var coinAudioSource = ServiceLocator.Get<IAudioService>().CoinFXAudioSource;
            LMotion.Create(0.0F, 1.0F, seconds)
                .WithEase(Ease.InOutSine)
                .WithOnComplete(() =>
                {
                    LMotion.Create(0.0F, 1.0F, 0.5F)
                        .WithLoops(-1, LoopType.Incremental)
                        .Bind(reward, (progress, createdReward) =>
                        {
                            if (!createdReward)
                            {
                                return;
                            }

                            progress = Math.Min(progress, 1.0F);
                            createdReward.transform.RotateAround(createdReward.transform.position, Vector3.up, _rewardRotationSpeed * progress * Time.deltaTime);
                        });
                })
                .Bind(_createdReward, (progress, createdReward) =>
                {
                    _depthOfField.focusDistance.value = Mathf.Lerp(10.0F, 0.4F, progress);
                    _vignette.intensity.value = progress;

                    createdReward.transform.position = _splineContainer.EvaluatePosition(progress);
                    createdReward.transform.rotation = Quaternion.Lerp(startRotation, _targetTransform.rotation, progress);
                    createdReward.transform.localScale = Vector3.Lerp(startScale, _targetTransform.localScale, progress);
                });

            await UniTask.WaitForSeconds(seconds);
            onComplete?.Invoke();
        }
    }
}
