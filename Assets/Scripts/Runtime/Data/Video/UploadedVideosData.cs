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
            }
            else if (countDifference < 0)
            {
                for (var index = (otherUploadedVideos.Count + countDifference); index < otherUploadedVideos.Count; index++)
                {
                    Add(otherUploadedVideos[index]);
                }
            }

            for (var index = 0; index < _uploadedVideos.Count; index++)
            {
                _uploadedVideos[index].LoadChanges(otherUploadedVideos[index]);
            }

            _changed = false;
        }

        public VideoData NextFrom(VideoData videoData)
        {
            return _uploadedVideos[(_uploadedVideos.IndexOf(videoData) + 1) % _uploadedVideos.Count];
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
