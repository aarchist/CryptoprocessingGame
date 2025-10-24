using System;
using Data.Game;
using Gameplay;
using Services.Data.Core;
using Services.Gameplay.Core;
using Services.Rewards.Core;
using Services.UIView.Core;
using UI.Views.Admin.UploadedVideos;
using UI.Views.Popups;
using UnityEngine;
using static Services.Gameplay.GameplayService.State;

namespace Services.Gameplay
{
    public sealed class GameplayService : MonoBehaviour, IGameplayService
    {
        [SerializeField]
        private Capsule _capsule;

        private Single _inactiveSeconds;
        private GameData _gameData;

        private State _activeState = ShowVideo;

        private State ActiveState
        {
            get => _activeState;
            set
            {
                Debug.Log($"{_activeState}->{value}");
                _activeState = value;
                if (_activeState is ShowVideo)
                {
                    ServiceLocator.Get<IUIViewService>().Get<UploadedVideosUIView>().IsVideoActive = true;
                }
                else if (_activeState is ShowRewards)
                {
                    ServiceLocator.Get<IUIViewService>().Get<UploadedVideosUIView>().IsVideoActive = false;
                }
                else if (_activeState is ShakeCapsule)
                {
                    _capsule.Spin();
                }
                else if (_activeState is GiveReward)
                {
                    _capsule.Stop(_gameData.SpinDuration, () =>
                    {
                        var randomID = ServiceLocator.Get<IRewardsService>().RandomRewardID();
                        ServiceLocator.Get<IUIViewService>().Get<ClaimRewardPopupUIView>().Setup(randomID).Show();
                    });
                }
            }
        }

        public void Initialize()
        {
            _gameData = ServiceLocator.Get<IDataService>().Get<GameData>();
        }

        public void Update()
        {
            if (ActiveState is GiveReward)
            {
                _inactiveSeconds += Time.deltaTime;
                if (_inactiveSeconds > _gameData.InactiveSeconds)
                {
                    ActiveState = ShowVideo;
                    _inactiveSeconds = 0.0F;
                    return;
                }
            }

            if (!Input.GetKeyDown(_gameData.GameplayButtonKey))
            {
                return;
            }

            if (ActiveState is GiveReward)
            {
                ServiceLocator.Get<IUIViewService>().Get<ClaimRewardPopupUIView>().Hide();
            }

            ActiveState = ActiveState switch
            {
                ShowVideo => ShowRewards,
                ShowRewards => ShakeCapsule,
                ShakeCapsule => GiveReward,
                GiveReward => ShakeCapsule
            };
        }

        public void Dispose()
        {
        }

        public enum State
        {
            ShowRewards,
            ShakeCapsule,
            GiveReward,
            ShowVideo,
        }
    }
}
