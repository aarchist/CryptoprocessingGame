using System;
using Data.Reward;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(RewardConfig), menuName = "Data/Reward")]
    public sealed class RewardConfig : ScriptableObject
    {
        [SerializeField]
        private RewardData _rewardData;
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private Sprite _icon;
        [SerializeField]
        private String _name;

        public Vector2 Position => _position;

        public Sprite Icon => _icon;

        public String Name => _name;

        public String ID => _rewardData.ID;

        public RewardData CreateData()
        {
            return _rewardData.Clone();
        }
    }
}
