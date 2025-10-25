using System;
using UnityEngine;

namespace Data.Video
{
    [Serializable]
    public sealed class VideoData
    {
        [SerializeField]
        private Vector2 _resolution;
        [SerializeField]
        private Int32 _duration;
        [SerializeField]
        private Boolean _isValid;
        [SerializeField]
        private String _name;
        [SerializeField]
        private String _path;

        public event Action Reloaded;

        public Boolean IsChanged { get; internal set; }

        public Vector2 Resolution
        {
            get => _resolution;
            set
            {
                _resolution = value;
                IsChanged = true;
            }
        }

        public Int32 Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                IsChanged = true;
            }
        }

        public Boolean IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                IsChanged = true;
            }
        }

        public String Name
        {
            get => _name;
            set
            {
                _name = value;
                IsChanged = true;
            }
        }

        public String Path
        {
            get => _path;
            set
            {
                _path = value;
                IsChanged = true;
            }
        }

        public void LoadChanges(VideoData other)
        {
            _resolution = other._resolution;
            _duration = other._duration;
            _name = other._name;
            _path = other._path;
            IsChanged = false;
            Reloaded?.Invoke();
        }
    }
}
