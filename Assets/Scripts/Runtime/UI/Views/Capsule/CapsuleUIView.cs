using System;
using System.Collections.Generic;
using UI.Views.Capsule.Subviews;
using UI.Views.Core;
using UnityEngine;

namespace UI.Views.Capsule
{
    public sealed class CapsuleUIView : UIViewBehaviour
    {
        private readonly List<GameObject> _attemptsImages = new();

        [SerializeField]
        private ShowRewardsUIView _showRewardsUIView;
        [SerializeField]
        private GiveRewardUIView _giveRewardUIView;
        [SerializeField]
        private TapButtonUIView _tapButtonUIView;
        [SerializeField]
        private RectTransform _attemptsContent;
        [SerializeField]
        private GameObject _attemptPrefab;

        private UIViewBehaviour _showedUIView;

        public Int32 ShowedAttempts
        {
            set
            {
                while (_attemptsImages.Count < value)
                {
                    _attemptsImages.Add(Instantiate(_attemptPrefab, _attemptsContent));
                }

                for (var index = 0; index < _attemptsImages.Count; index++)
                {
                    _attemptsImages[index].gameObject.SetActive(index < value);
                }
            }
        }

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
