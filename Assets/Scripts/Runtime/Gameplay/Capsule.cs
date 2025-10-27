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
        private GameObject _view;
        [SerializeField]
        private Single _speed = 2.0F;

        public void Spin()
        {
            _animator.Play("SpinState");
            _motionHandle = LMotion.Create(0.0F, 1.0F, 1.0F)
                .WithEase(Ease.InSine)
                .Bind(speed => _animator.SetFloat(_spinSpeed, speed));
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
        }

        public void GiveRewards()
        {
            _animator.Play("GiveRewardState");
        }
    }
}
