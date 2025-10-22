using System;
using System.Collections.Generic;
using Data.Game;
using Data.Reward;
using Data.Video;
using Services.Data.Core;
using UnityEngine;

namespace Services.Data
{
    public sealed class DataService : IDataService
    {
        private readonly Dictionary<Type, IData> _loadedData = new();
        private readonly Dictionary<Type, String> _dataPaths = new()
        {
            [typeof(UploadedVideosData)] = "uploaded_videos",
            [typeof(RewardsData)] = "rewards",
            [typeof(GameData)] = "game_data",
        };

        public TData Get<TData>() where TData : IData, new()
        {
            if (_loadedData.TryGetValue(typeof(TData), out var value))
            {
                return (TData)value;
            }

            return Load<TData>();
        }

        public void Dispose()
        {
            foreach (var data in _loadedData.Values)
            {
                Save(data);
            }

            _loadedData.Clear();
        }

        private TData Load<TData>() where TData : IData, new()
        {
            var type = typeof(TData);
            var data = (TData)JsonUtility.FromJson(PlayerPrefs.GetString(_dataPaths[type]), type) ?? new TData();
            _loadedData.Add(type, data);
            return data;
        }

        private void Save(IData data)
        {
            PlayerPrefs.SetString(_dataPaths[data.GetType()], JsonUtility.ToJson(data));
        }
    }
}
