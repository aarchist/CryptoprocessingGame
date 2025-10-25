using System;
using Data.Game;
using Services.Data.Core;
using Services.Gameplay.StateMachine.States.Core;
using Services.UIView.Core;
using UI.Views.Capsule;
using UnityEngine;

namespace Services.Gameplay.StateMachine.States
{
    public sealed class ShowRewardsGameState : GameState
    {
        private readonly GameplayService _gameplayService;
        private readonly GameData _gameData;

        private Single _inactiveDuration;

        public ShowRewardsGameState(GameplayService gameplayService) : base(gameplayService)
        {
            _gameplayService = gameplayService;
            _gameData = ServiceLocator.Get<IDataService>().Get<GameData>();
        }

        public override void Enter()
        {
            base.Enter();
            ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().ShowShowRewards();
            _inactiveDuration = 0.0F;
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

            StateMachine.Enter<SpinCapsuleGameState>();
        }

        public override void Exit()
        {
            base.Exit();
            ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().Clear();
        }
    }
}
