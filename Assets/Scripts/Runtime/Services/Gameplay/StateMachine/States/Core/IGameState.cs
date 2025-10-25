namespace Services.Gameplay.StateMachine.States.Core
{
    public interface IGameState
    {
        public static readonly IGameState Dummy = new DummyState();

        public void Enter();

        public void Update();

        public void Exit();

        private class DummyState : IGameState
        {
            void IGameState.Enter()
            {
            }

            void IGameState.Update()
            {
            }

            void IGameState.Exit()
            {
            }
        }
    }
}
