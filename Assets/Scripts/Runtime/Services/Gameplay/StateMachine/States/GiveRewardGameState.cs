using System;
using Data.Game;
using Services.Data.Core;
using Services.Gameplay.StateMachine.States.Core;
using Services.Rewards.Core;
using Services.UIView.Core;
using UI.Views.Capsule;
using UnityEngine;

namespace Services.Gameplay.StateMachine.States
{
    public sealed class GiveRewardGameState : GameState
    {
        private readonly GameplayService _gameplayService;
        private readonly GameData _gameData;

        private Single _inactiveDuration;

        public GiveRewardGameState(GameplayService gameplayService) : base(gameplayService)
        {
            _gameplayService = gameplayService;
            _gameData = ServiceLocator.Get<IDataService>().Get<GameData>();
        }

        public override void Enter()
        {
            base.Enter();

            _inactiveDuration = 0.0F;
            var randomID = ServiceLocator.Get<IRewardsService>().RandomRewardID();
            ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().ShowGiveReward(randomID);
            _gameplayService.Capsule.GiveRewards();
        }

        public override void Update()
        {
            base.Update();

            _inactiveDuration += Time.deltaTime;
            if (_inactiveDuration >= _gameData.InactiveSeconds)
            {
                StateMachine.Enter<ShowVideoGameState>();
                return;
            }

            if (!Input.GetKeyDown(_gameData.GameplayButtonKey))
            {
                return;
            }

            if (_gameplayService.AttemptsCount > 0)
            {
                StateMachine.Enter<SpinCapsuleGameState>();
                return;
            }

            StateMachine.Enter<ShowVideoGameState>();
        }

        public override void Exit()
        {
            base.Exit();

            ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().Clear();
            _gameplayService.Capsule.ClearReward();
        }
    }
}
