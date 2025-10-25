using Data.Game;
using Services.Data.Core;
using Services.Displays.Service;
using Services.Gameplay.StateMachine.States.Core;
using Services.UIView.Core;
using UI.Views.Admin.UploadedVideos;
using UnityEngine;

namespace Services.Gameplay.StateMachine.States
{
    public sealed class ShowVideoGameState : GameState
    {
        private readonly GameplayService _gameplayService;
        private readonly GameData _gameData;

        public ShowVideoGameState(GameplayService gameplayService) : base(gameplayService)
        {
            _gameplayService = gameplayService;
            _gameData = ServiceLocator.Get<IDataService>().Get<GameData>();
        }

        public override void Enter()
        {
            base.Enter();

            ServiceLocator.Get<IDisplaysService>().ActivateSecondDisplayWhenConnected();
            ServiceLocator.Get<IUIViewService>().Get<UploadedVideosUIView>().IsVideoActive = true;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(_gameData.GameplayButtonKey))
            {
                _gameplayService.AttemptsCount = _gameData.AttemptsCount;
                StateMachine.Enter<ShowRewardsGameState>();
            }
        }

        public override void Exit()
        {
            base.Exit();

            ServiceLocator.Get<IUIViewService>().Get<UploadedVideosUIView>().IsVideoActive = false;
        }
    }
}
