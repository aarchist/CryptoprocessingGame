using System;
using LitMotion;
using UnityEngine;

namespace Gameplay
{
    public sealed class Capsule : MonoBehaviour
    {
        private MotionHandle _motionHandle;

        [SerializeField]
        private GameObject _view;
        [SerializeField]
        private Single _speed = 2.0F;

        public void Spin()
        {
            _motionHandle = LMotion.Create(0.0F, 1.0F, 1.0F)
                .WithLoops(-1, LoopType.Incremental)
                .WithEase(Ease.InSine)
                .Bind(progress => _view.transform.RotateAround(_view.transform.position, _view.transform.up, Mathf.Min(1.0F, progress) * Time.deltaTime * _speed));
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
                .Bind(progress => _view.transform.RotateAround(_view.transform.position, _view.transform.up, progress * Time.deltaTime * _speed));
        }
    }
}
