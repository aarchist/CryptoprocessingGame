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
            var type = typeof(TData);
            if (_loadedData.TryGetValue(type, out var value))
            {
                return (TData)value;
            }

            var data = Load<TData>();
            _loadedData.Add(type, data);
            return data;
        }

        public void Save(IData data)
        {
            PlayerPrefs.SetString(_dataPaths[data.GetType()], JsonUtility.ToJson(data));
        }

        public IData LoadData(Type type)
        {
            var data = (IData)(JsonUtility.FromJson(PlayerPrefs.GetString(_dataPaths[type]), type));
            if (data == null)
            {
                data = (IData)Activator.CreateInstance(type);
                Save(data);
            }

            return data;
        }

        public void Dispose()
        {
            _loadedData.Clear();
        }

        private TData Load<TData>() where TData : IData, new()
        {
            var type = typeof(TData);
            var data = (TData)JsonUtility.FromJson(PlayerPrefs.GetString(_dataPaths[type]), type);
            if (data == null)
            {
                data = new TData();
                Save(data);
            }

            data.IsChanged = false;
            return data;
        }
    }
}
