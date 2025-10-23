using Data.Reward;
using Services;
using Services.Data.Core;
using UI.Views.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Admin.Options.Rewards
{
    public sealed class RewardsUIView : UIViewBehaviour
    {
        [SerializeField]
        private RewardItemUIView _rewardItemUIViewPrefab;
        [SerializeField]
        private ScrollRect _scrollRect;

        public override void Initialize()
        {
            base.Initialize();

            foreach (var rewardData in ServiceLocator.Get<IDataService>().Get<RewardsData>().Rewards)
            {
                CreateData(rewardData);
            }
        }

        private void CreateData(RewardData rewardData)
        {
            Instantiate(_rewardItemUIViewPrefab, _scrollRect.content).Setup(rewardData);
        }
    }
}
