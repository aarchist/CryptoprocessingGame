using System;
using Data.Game;
using Gameplay;
using LitMotion;
using Services.Audio.Core;
using Services.Data.Core;
using Services.Gameplay.StateMachine.States.Core;
using Services.UIView.Core;
using UI.Views.Capsule;
using UnityEngine;

namespace Services.Gameplay.StateMachine.States
{
    public sealed class ShowRewardsGameState : GameState
    {
        private readonly GameData _gameData;
        private readonly Capsule _capsule;

        private MotionHandle _motionHandle;
        private Single _inactiveDuration;

        public ShowRewardsGameState(GameplayService gameplayService) : base(gameplayService)
        {
            _gameData = ServiceLocator.Get<IDataService>().Get<GameData>();
            _capsule = gameplayService.Capsule;
        }

        public override void Enter()
        {
            base.Enter();
            ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().ShowShowRewards();
            _inactiveDuration = 0.0F;
            _capsule.ShowRewards();
            var source = ServiceLocator.Get<IAudioService>().SpinFXAudioSource;
            source.Play();
            _motionHandle.TryCancel();
            _motionHandle = LMotion.Create(0.0F, 0.6F, 1.0F).Bind(volume => source.volume = volume);
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
