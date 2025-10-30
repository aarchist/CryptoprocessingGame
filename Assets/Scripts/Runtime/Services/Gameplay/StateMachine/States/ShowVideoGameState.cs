using Data.Game;
using LitMotion;
using Services.Audio.Core;
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

        private MotionHandle _motionHandle;

        public override void Enter()
        {
            base.Enter();

            ServiceLocator.Get<IDisplaysService>().ActivateSecondDisplayWhenConnected();
            ServiceLocator.Get<IUIViewService>().Get<UploadedVideosUIView>().IsVideoActive = true;
            var source = ServiceLocator.Get<IAudioService>().SpinFXAudioSource;

            _motionHandle.TryCancel();
            _motionHandle = LMotion.Create(1.0F, 0.0F, 0.6F).WithOnComplete(source.Stop).Bind(volume => source.volume = volume);
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
            _motionHandle.TryCancel();
            ServiceLocator.Get<IUIViewService>().Get<UploadedVideosUIView>().IsVideoActive = false;
        }
    }
}
