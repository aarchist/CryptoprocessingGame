using System;
using Data.Reward;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(RewardConfig), menuName = "Data/Reward")]
    public sealed class RewardConfig : ScriptableObject
    {
        [SerializeField]
        private Boolean _active;
        [SerializeField]
        private Int32 _weight;
        [SerializeField]
        private String _id;
        [SerializeField]
        private String _name;

        public String Name => _name;

        public String ID => _id;

        public RewardData CreateData()
        {
            return new RewardData()
            {
                IsActive = _active,
                Weight = _weight,
                ID = _id
            };
        }
    }
}
