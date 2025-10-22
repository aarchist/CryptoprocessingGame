using System;
using Services.Data.Core;
using UnityEngine;

namespace Data.Game
{
    public sealed class GameData : IData
    {
        public KeyCode StartGameKey = KeyCode.Return;
        public Int32 RewardDuration;
        public Int32 SpinDuration;
    }
}
