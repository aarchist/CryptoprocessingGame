using LitMotion;
using TMPro;
using UI.Views.Core;
using UnityEngine;

namespace UI.Views.Capsule.Subviews
{
    public sealed class TapButtonUIView : UIViewBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textMeshProUGUI;
        private MotionHandle _motionHandle;

        public override void Show()
        {
            base.Show();
            _motionHandle = BlinkText();
        }

        public override void Hide()
        {
            base.Hide();
            _motionHandle.Cancel();
        }

        private MotionHandle BlinkText()
        {
            return LMotion.Create(_textMeshProUGUI.color.a, 0.5F, 1.0F).WithLoops(-1, LoopType.Yoyo)
                .WithEase(Ease.InSine)
                .Bind(_textMeshProUGUI, (alpha, text) => text.alpha = alpha);
        }
    }
}
