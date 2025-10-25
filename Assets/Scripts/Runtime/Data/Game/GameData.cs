using System;
using Services.Data.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Game
{
    [Serializable]
    public sealed class GameData : BaseData
    {
        [SerializeField]
        private KeyCode _gameplayButtonKey = KeyCode.Return;
        [SerializeField]
        private Int32 _inactiveSeconds = 60;
        [SerializeField]
        private Int32 _capsuleStopDuration = 4;
        [SerializeField]
        private Int32 _attemptsCount = 3;

        private Boolean _changed;

        public override Boolean IsChanged
        {
            get => _changed;
            set => _changed = value;
        }

        public KeyCode GameplayButtonKey
        {
            get => _gameplayButtonKey;
            set
            {
                _gameplayButtonKey = value;
                _changed = true;
            }
        }

        public Int32 InactiveSeconds
        {
            get => _inactiveSeconds;
            set
            {
                _inactiveSeconds = value;
                _changed = true;
            }
        }

        public Int32 CapsuleStopDuration
        {
            get => _capsuleStopDuration;
            set
            {
                _capsuleStopDuration = value;
                _changed = true;
            }
        }

        public Int32 AttemptsCount
        {
            get => _attemptsCount;
            set
            {
                _attemptsCount = value;
                _changed = true;
            }
        }

        protected override void LoadChanges(IData other)
        {
            if (other is not GameData gameData)
            {
                return;
            }

            _gameplayButtonKey = gameData._gameplayButtonKey;
            _inactiveSeconds = gameData._inactiveSeconds;
            _attemptsCount = gameData._attemptsCount;
            _capsuleStopDuration = gameData._capsuleStopDuration;
            _changed = false;
        }
    }
}
