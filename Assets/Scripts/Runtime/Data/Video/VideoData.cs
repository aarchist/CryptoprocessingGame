using System;
using UnityEngine;

namespace Data.Video
{
    [Serializable]
    public sealed class VideoData
    {
        public Vector2 Resolution;
        public Int32 Duration;
        public String Name;
        public String Path;
    }
}
