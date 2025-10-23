using System;
using UnityEngine;

namespace Data.Reward
{
    [Serializable]
    public class RewardData
    {
        [SerializeField]
        private Int32 _weight = 1;

        public Boolean IsActive = true;
        public String ID;

        public Int32 Weight
        {
            get => _weight;
            set => _weight = Math.Min(value, 1);
        }
    }
}
