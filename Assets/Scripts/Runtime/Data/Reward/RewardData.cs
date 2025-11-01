using System;
using UnityEngine;

namespace Data.Reward
{
    [Serializable]
    public class RewardData
    {
        [SerializeField]
        private Boolean _isActive = true;
        [SerializeField]
        internal Int32 _weight = 1;
        [SerializeField]
        private String _id = "reward_id";

        public Action Reloaded;

        public Boolean IsChanged { get; internal set; }

        public Boolean IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                IsChanged = true;
            }
        }
        public Int32 Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                IsChanged = true;
            }
        }
        public String ID
        {
            get => _id;
            set
            {
                _id = value;
                IsChanged = true;
            }
        }

        public RewardData LoadChanges(RewardData other)
        {
            _isActive = other._isActive;
            _weight = other._weight;
            _id = other._id;
            IsChanged = false;
            Reloaded?.Invoke();
            return this;
        }

        public RewardData Clone()
        {
            var data = (RewardData)MemberwiseClone();
            data.IsChanged = true;
            return data;
        }
    }
}
