using Data.Game;
using Services;
using Services.Data.Core;
using UI.Views.Core;
using UI.Views.Properties;
using UnityEngine;  

namespace UI.Views.Admin.Options
{
    public sealed class OptionsUIView : UIViewBehaviour
    {
        [SerializeField]
        private Int32PropertyUIView _capsuleStopDurationProperty;
        [SerializeField]
        private Int32PropertyUIView _inactiveDurationProperty;
        [SerializeField]
        private KeyCodePropertyUIView _keyCodePropertyUIView;

        private GameData _gameData;

        public override void Initialize()
        {
            base.Initialize();
            _gameData = ServiceLocator.Get<IDataService>().Get<GameData>();
            _capsuleStopDurationProperty.Setup(() => _gameData.SpinDuration, duration => _gameData.SpinDuration = duration);
            _inactiveDurationProperty.Setup(() => _gameData.InactiveSeconds, duration => _gameData.InactiveSeconds = duration);
            _keyCodePropertyUIView.Setup(() => _gameData.GameplayButtonKey, keyCode => _gameData.GameplayButtonKey = keyCode);
        }
    }
}
