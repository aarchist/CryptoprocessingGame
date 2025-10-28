using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

        public async Task Spin()
        {
            if (!_spinIsPrepared)
            {
                _spinIsPrepared = true;
                await UniTask.WaitForSeconds(0.967F);
            }

            var spinStarted = false;
            _motionHandle = LMotion.Create(0.0F, 1.0F, 0.5F).WithEase(Ease.OutSine).Bind(async speed =>
            {
                if (!spinStarted)
                {
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
