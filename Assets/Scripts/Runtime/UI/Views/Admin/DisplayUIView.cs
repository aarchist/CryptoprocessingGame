using System;
using Services;
using Services.Displays.Service;
using UI.Views.Core;
using UnityEngine;

namespace UI.Views.Admin
{
    public sealed class DisplayUIView : UIViewBehaviour
    {
        private IDisplaysService _displaysService;

        [SerializeField]
        private CanvasGroup _disconnectedDisplay;
        [SerializeField]
        private CanvasGroup _connectedDisplay;

        private Boolean ShowConnectedDisplay
        {
            set
            {
                if (_connectedDisplay.gameObject.activeSelf == value)
                {
                    return;
                }

                _connectedDisplay.gameObject.SetActive(value);
                _disconnectedDisplay.gameObject.SetActive(!value);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _displaysService = ServiceLocator.Get<IDisplaysService>();
        }

        private void Update()
        {
            ShowConnectedDisplay = _displaysService.IsSecondDisplayActive;
        }
    }
}
