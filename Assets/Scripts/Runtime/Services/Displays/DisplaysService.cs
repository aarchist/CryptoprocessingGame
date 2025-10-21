using System;
using Services.Displays.Service;
using Services.Loops.Core;
using UnityEngine;

namespace Services.Displays
{
    public sealed class DisplaysService : IDisplaysService
    {
        private Boolean HasSecondsDisplay => Display.displays.Length < 2;

        private Display SecondsDisplay => Display.displays[1];

        public void Initialize()
        {
            ServiceLocator.Get<ILoopsService>().Updated += CheckSecondDisplay;
        }

        private void CheckSecondDisplay()
        {
            if (HasSecondsDisplay && !SecondsDisplay.active)
            {
                SecondsDisplay.Activate();
            }
        }

        public void Dispose()
        {
            ServiceLocator.Get<ILoopsService>().Updated -= CheckSecondDisplay;
        }
    }
}
