using System;
using System.Collections.Generic;
using Services.Data.Core;
using UnityEngine;

namespace Data.Video
{
    [Serializable]
    public sealed class UploadedVideosData : IData
    {
        [SerializeField]
        private List<VideoData> _uploadedVideos = new();

        public event Action<VideoData> VideoAdded;

        public event Action<VideoData> VideoRemoved;

        public IReadOnlyList<VideoData> UploadedVideos => _uploadedVideos;

        public VideoData NextFrom(VideoData videoData)
        {
            return _uploadedVideos[(_uploadedVideos.IndexOf(videoData) + 1) % _uploadedVideos.Count];
        }

        public void Add(VideoData videoData)
        {
            _uploadedVideos.Add(videoData);
            VideoAdded?.Invoke(videoData);
        }

        public void Remove(VideoData videoData)
        {
            if (_uploadedVideos.Remove(videoData))
            {
                VideoRemoved?.Invoke(videoData);
            }
        }
    }
}
