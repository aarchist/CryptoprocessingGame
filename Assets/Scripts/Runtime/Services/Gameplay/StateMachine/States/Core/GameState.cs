using Services.Gameplay.StateMachine.Core;

namespace Services.Gameplay.StateMachine.States.Core
{
    public abstract class GameState : IGameState
    {
        protected IGameStateMachine StateMachine { get; }

        public GameState(IGameStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Exit()
        {
        }
    }
}
