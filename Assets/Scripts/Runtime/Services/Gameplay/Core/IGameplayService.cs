using System;
using Services.Core;
using Services.Gameplay.StateMachine.Core;

namespace Services.Gameplay.Core
{
    public interface IGameplayService : IService, IGameStateMachine
    {
        public Boolean HasAttempts { get; }
    }
}
