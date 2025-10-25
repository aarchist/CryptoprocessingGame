using System;
using Services.Displays.Service;
using Services.Loops.Core;
using UnityEngine;

namespace Services.Displays
{
    public sealed class DisplaysService : IDisplaysService
    {
        private Boolean HasSecondDisplay => Display.displays.Length >= 2;

        private Display SecondsDisplay => Display.displays[1];

        private Boolean _secondDisplayShouldBeActive;

        public void Initialize()
        {
            if (HasSecondDisplay)
            {
                SecondsDisplay.Activate();
            }
        }

        public void ActivateSecondDisplayWhenConnected()
        {
            if (_secondDisplayShouldBeActive)
            {
                return;
            }

            _secondDisplayShouldBeActive = true;
            ServiceLocator.Get<ILoopsService>().Updated += CheckSecondDisplay;
        }

        private void CheckSecondDisplay()
        {
            if (HasSecondDisplay && (!SecondsDisplay.active))
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
