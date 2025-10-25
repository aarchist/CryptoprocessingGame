using System;
using UI.Views.Capsule.Subviews;
using UI.Views.Core;
using UnityEngine;

namespace UI.Views.Capsule
{
    public sealed class CapsuleUIView : UIViewBehaviour
    {
        [SerializeField]
        private ShowRewardsUIView _showRewardsUIView;
        [SerializeField]
        private GiveRewardUIView _giveRewardUIView;
        [SerializeField]
        private TapButtonUIView _tapButtonUIView;

        private UIViewBehaviour _showedUIView;

        public override void Initialize()
        {
            base.Initialize();
            _showRewardsUIView.Initialize();
            _giveRewardUIView.Initialize();
        }

        public void ShowGiveReward(String rewardID)
        {
            Clear();
            _giveRewardUIView.Setup(rewardID);
            _giveRewardUIView.Show();
            _showedUIView = _giveRewardUIView;
        }

        public void ShowShowRewards()
        {
            Clear();
            _showRewardsUIView.Show();
            _showedUIView = _showRewardsUIView;
        }

        public void ShowTapButton()
        {
            Clear();
            _tapButtonUIView.Show();
            _showedUIView = _tapButtonUIView;
        }

        public void Clear()
        {
            if (_showedUIView)
            {
                _showedUIView.Hide();
                _showedUIView = null;
            }
        }
    }
}
