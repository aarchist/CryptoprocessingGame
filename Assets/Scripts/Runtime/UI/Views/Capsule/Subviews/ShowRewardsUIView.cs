using System;
using System.Collections.Generic;
using UI.Views.Core;
using UI.Views.Rewards;
using UnityEngine;

namespace UI.Views.Capsule
{
    public sealed class ShowRewardsUIView : UIViewBehaviour
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
