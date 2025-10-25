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
        private String _name;
        [SerializeField]
        private String _path;

        private Boolean _isInvalid;

        public event Action ValidationUpdated;

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

        public Boolean IsInvalid
        {
            get => _isInvalid;
            set
            {
                _isInvalid = value;
                IsChanged = true;
                ValidationUpdated?.Invoke();
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
