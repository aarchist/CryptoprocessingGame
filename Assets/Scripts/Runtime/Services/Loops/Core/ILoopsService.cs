using System;
using Services.Core;

namespace Services.Loops.Core
{
    public interface ILoopsService : IService
    {
        public event Action Updated;
    }
}
