using System;
using Services.Loops.Core;
using UnityEngine;

namespace Services.Loops
{
    public sealed class LoopsService : MonoBehaviour, ILoopsService
    {
        public event Action Updated;

        private void Update()
        {
            Updated?.Invoke();
        }

        public void Dispose()
        {
        }
    }
}
