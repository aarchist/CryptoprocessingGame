using System;
using LitMotion;
using UnityEngine;

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
        private Boolean _spinIsPrepared;

        public void Spin()
        {
            var motionBuilder = LMotion.Create(0.0F, 1.0F, 0.5F).WithEase(Ease.OutSine);

            if (!_spinIsPrepared)
            {
                _spinIsPrepared = true;
                _animator.Play("PrepareSpin");
                Debug.Log("prepare");
                motionBuilder.WithDelay(0.967F, DelayType.FirstLoop, false);
            }

            var spinStarted = false;
            _motionHandle = motionBuilder.Bind(speed =>
            {
             
                if (!spinStarted)
                {
                    Debug.Log("star");
                    spinStarted = true;
                    _animator.Play("SpinState");
                }

                _animator.SetFloat(_spinSpeed, speed);
            });
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
                .Bind(speed => _animator.SetFloat(_spinSpeed, speed));
        }

        public void ShowRewards()
        {
            _animator.Play("ShowRewardsState");
            _spinIsPrepared = false;
        }

        public void GiveRewards()
        {
            _animator.Play("GiveRewardState");
        }
    }
}
