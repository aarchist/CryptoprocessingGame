using System;
using LitMotion;
using UnityEngine;

namespace Gameplay
{
    public sealed class Capsule : MonoBehaviour
    {
        private const Single LoopSeconds = 0.5F;

        private MotionHandle _motionHandle;

        public void Spin()
        {
            _motionHandle = LMotion.Create(0.0F, 180.0F, LoopSeconds)
                .WithEase(Ease.OutQuad)
                .WithLoops(-1, LoopType.Incremental)
                .Bind(transform, (eulerAngleY, currentTransform) =>
                {
                    var eulerAngles = currentTransform.eulerAngles;
                    eulerAngles.y = eulerAngleY;
                    currentTransform.eulerAngles = eulerAngles;
                });
        }

        public void Stop(Single duration, Action onComplete)
        {
            if (_motionHandle.IsActive())
            {
                _motionHandle.Cancel();
            }

            _motionHandle = LMotion.Create(transform.eulerAngles.y, transform.eulerAngles.y + ((duration / LoopSeconds) * 180.0F), duration)
                .WithOnComplete(onComplete)
                .Bind(transform, static (eulerAngleY, currentTransform) =>
                {
                    var localEulerAngles = currentTransform.localEulerAngles;
                    localEulerAngles.y = eulerAngleY;
                    currentTransform.localEulerAngles = localEulerAngles;
                });
        }
    }
}
