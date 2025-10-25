using System;
using Data.Game;
using Gameplay;
using Services.Data.Core;
using Services.Gameplay.StateMachine.States.Core;
using Services.UIView.Core;
using UI.Views.Capsule;
using UnityEngine;

namespace Services.Gameplay.StateMachine.States
{
    public class SpinCapsuleGameState : GameState
    {
        private readonly GameplayService _gameplayService;
        private readonly GameData _gameData;
        private readonly Capsule _capsule;
        private Boolean _stopping;

        public SpinCapsuleGameState(GameplayService gameplayService) : base(gameplayService)
        {
            _gameplayService = gameplayService;
            _gameData = ServiceLocator.Get<IDataService>().Get<GameData>();
            _capsule = gameplayService.Capsule;
        }

        public override void Enter()
        {
            base.Enter();
            _stopping = false;
            _gameplayService.AttemptsCount--;
            ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().ShowTapButton();
            _capsule.Spin();
        }

        public override void Update()
        {
            base.Update();

            if (_stopping || !Input.GetKeyDown(_gameData.GameplayButtonKey))
            {
                return;
            }

            ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().Clear();
            _capsule.Stop(_gameData.CapsuleStopDuration, StateMachine.Enter<GiveRewardGameState>);
            _stopping = true;
        }
    }
}
