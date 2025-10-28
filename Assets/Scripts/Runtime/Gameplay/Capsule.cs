using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using UnityEngine.Splines;

namespace Gameplay
{
    public sealed class Capsule : MonoBehaviour
    {
        private static readonly Int32 _spinSpeed = Animator.StringToHash("SpinSpeed");
        private MotionHandle _motionHandle;

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Single _speed = 2.0F;
        [SerializeField]
        private Transform _rotationCenter;
        [SerializeField]
        private GameObject _reward;
        [SerializeField]
        private Transform _targetTransform;
        [SerializeField]
        private SplineContainer _splineContainer;

        private GameObject _createdReward;
        private Boolean _spinIsPrepared;
        private Boolean _rewardGiven;

        public async void Spin()
        {
            if (_createdReward)
            {
                Destroy(_createdReward);
                _createdReward = null;
            }

            if (!_spinIsPrepared)
            {
                _spinIsPrepared = true;
                _animator.Play("PrepareSpin");
                await UniTask.WaitForSeconds(0.967F);
            }
            else
            {
                _animator.Play("CloseState");
                await UniTask.WaitForSeconds(0.583F);
            }

            _motionHandle = LMotion.Create(0.0F, 1.0F, 1.0F)
                .WithLoops(-1, LoopType.Incremental)
                .WithEase(Ease.OutSine)
                .Bind(progress => _rotationCenter.RotateAround(_rotationCenter.position, _rotationCenter.up, Mathf.Min(1.0F, progress) * Time.deltaTime * _speed));
        }

        public void Stop(Single duration, Action onComplete)
        {
            if (_motionHandle.IsActive())
            {
                _motionHandle.Cancel();
            }

            _motionHandle = LMotion.Create(1.0F, 0.0F, duration)
                .WithOnComplete(onComplete)
                .WithEase(Ease.OutSine)
                .Bind(progress => _rotationCenter.transform.RotateAround(_rotationCenter.transform.position, _rotationCenter.transform.up, progress * Time.deltaTime * _speed));
        }

        public void ShowRewards()
        {
            _animator.Play("ShowRewardsState");
            _spinIsPrepared = false;
        }

        public async void GiveRewards()
        {
            _animator.Play("GiveRewardState");
            _rewardGiven = true;
            await UniTask.WaitForSeconds(0.583F);
            _createdReward = Instantiate(_reward);
            var startRotation = _createdReward.transform.rotation;
            LMotion.Create(0.0F, 1.0F, 0.5F).Bind(progress =>
            {
                _createdReward.transform.rotation = Quaternion.Lerp(startRotation, _targetTransform.rotation, progress);
                _createdReward.transform.position = _splineContainer.EvaluatePosition(progress);
            });
            _rewardGiven = true;
        }
    }
}
