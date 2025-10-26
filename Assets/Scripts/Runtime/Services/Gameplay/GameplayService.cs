using System;
using System.Collections.Generic;
using Gameplay;
using Services.Gameplay.Core;
using Services.Gameplay.StateMachine.States;
using Services.Gameplay.StateMachine.States.Core;
using Services.UIView.Core;
using UI.Views.Capsule;
using UnityEngine;

namespace Services.Gameplay
{
    public sealed class GameplayService : MonoBehaviour, IGameplayService
    {
        [SerializeField]
        private Capsule _capsule;

        private Dictionary<Type, GameState> _availableStates;
        private IGameState _activeState = IGameState.Dummy;
        private Int32 _attemptsCount;

        public Capsule Capsule => _capsule;

        public Int32 AttemptsCount
        {
            get => _attemptsCount;
            set
            {
                _attemptsCount = value;
                ServiceLocator.Get<IUIViewService>().Get<CapsuleUIView>().ShowedAttempts = _attemptsCount;
            }
        }

        public void Initialize()
        {
            _availableStates = new Dictionary<Type, GameState>
            {
                [typeof(ShowVideoGameState)] = new ShowVideoGameState(this),
                [typeof(ShowRewardsGameState)] = new ShowRewardsGameState(this),
                [typeof(SpinCapsuleGameState)] = new SpinCapsuleGameState(this),
                [typeof(GiveRewardGameState)] = new GiveRewardGameState(this),
            };
        }

        public void Enter<TState>() where TState : IGameState
        {
            var targetState = _availableStates[typeof(TState)];
            if (_activeState == targetState)
            {
                return;
            }

            var previousState = _activeState;
            _activeState.Exit();
            _activeState = targetState;
            Debug.Log($"[{nameof(GameplayService)}] {previousState.GetType().Name}->{_activeState.GetType().Name}");
            _activeState.Enter();
        }

        public void Update()
        {
            _activeState.Update();
        }

        public void Dispose()
        {
        }
    }
}
