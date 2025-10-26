using System;
using System.Collections.Generic;
using UI.Views.Core;
using UnityEngine;

namespace UI.Views.Capsule.Subviews
{
    public sealed class ShowRewardsUIView : FadeUIViewBehaviour
    {
        private readonly Dictionary<String, RewardUIView> _rewards = new();

        [SerializeField]
        private RectTransform _contentRectTransform;

        public override void Initialize()
        {
            base.Initialize();

        }
    }
}
