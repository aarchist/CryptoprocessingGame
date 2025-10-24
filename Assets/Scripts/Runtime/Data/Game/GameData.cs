using System;
using Services.Data.Core;
using UnityEngine;

namespace Data.Game
{
    public sealed class GameData : IData
    {
        public KeyCode GameplayButtonKey = KeyCode.Return;
        public Int32 InactiveSeconds = 60;
        public Int32 SpinDuration = 4;
    }
}
