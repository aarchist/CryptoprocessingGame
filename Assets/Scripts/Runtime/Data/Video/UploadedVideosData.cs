using System;
using System.Collections.Generic;
using System.Linq;
using Services.Data.Core;
using UnityEngine;

namespace Data.Video
{
    [Serializable]
    public sealed class UploadedVideosData : BaseData
    {
        [SerializeField]
        private List<VideoData> _uploadedVideos = new();
        private Boolean _changed;

        public event Action<VideoData> VideoAdded;

        public event Action<VideoData> VideoRemoved;

        public IReadOnlyList<VideoData> UploadedVideos => _uploadedVideos;

        public override Boolean IsChanged
        {
            get => _changed || _uploadedVideos.Any(data => data.IsChanged);
            set
            {
                _uploadedVideos.ForEach(data => data.IsChanged = value);
                _changed = value;
            }
        }

        protected override void LoadChanges(IData other)
        {
            if (other is not UploadedVideosData uploadedVideosData)
            {
                return;
            }

            var otherUploadedVideos = uploadedVideosData._uploadedVideos;
            var countDifference = _uploadedVideos.Count - otherUploadedVideos.Count;
            if (countDifference > 0)
            {
                while (otherUploadedVideos.Count != _uploadedVideos.Count)
                {
                    Remove(_uploadedVideos[^1]);
                }

                for (var index = 0; index < _uploadedVideos.Count; index++)
                {
                    _uploadedVideos[index].LoadChanges(otherUploadedVideos[index]);
                }
            }
            else if (countDifference < 0)
            {
                var startIndex = otherUploadedVideos.Count + countDifference;
                for (var index = startIndex; index < otherUploadedVideos.Count; index++)
                {
                    Add(otherUploadedVideos[index]);
                }

                for (var index = 0; index < startIndex; index++)
                {
                    _uploadedVideos[index].LoadChanges(otherUploadedVideos[index]);
                }
            }
            else
            {
                for (var index = 0; index < _uploadedVideos.Count; index++)
                {
                    _uploadedVideos[index].LoadChanges(otherUploadedVideos[index]);
                }
            }
        }

        public VideoData NextFrom(VideoData videoData)
        {
            var index = _uploadedVideos.IndexOf(videoData);
            for (var offset = 1; offset <= _uploadedVideos.Count; offset++)
            {
                var data = _uploadedVideos[(index + offset) % _uploadedVideos.Count];
                if (!data.IsInvalid)
                {
                    return data;
                }
            }

            return null;
        }

        public void Add(VideoData videoData)
        {
            _uploadedVideos.Add(videoData);
            _changed = true;
            VideoAdded?.Invoke(videoData);
        }

        public void Remove(VideoData videoData)
        {
            if (!_uploadedVideos.Remove(videoData))
            {
                return;
            }

            _changed = true;
            VideoRemoved?.Invoke(videoData);
        }
    }
}
