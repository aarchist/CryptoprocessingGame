using Services.Gameplay.StateMachine.States.Core;

namespace Services.Gameplay.StateMachine.Core
{
    public interface IGameStateMachine
    {
        public IGameState ActiveState { get; }

        public void Enter<TState>() where TState : IGameState;
    }
}
