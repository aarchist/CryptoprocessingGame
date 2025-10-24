using System;

namespace Data.Reward
{
    [Serializable]
    public class RewardData
    {
        public Boolean IsActive = true;
        public Int32 Weight = 1;
        public String ID = "reward_id";

        public RewardData Clone()
        {
            return (RewardData)MemberwiseClone();
        }
    }
}
