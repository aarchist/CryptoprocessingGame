using Data.Reward;
using Services;
using Services.Data.Core;
using UI.Views.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Admin.Rewards
{
    public sealed class RewardsUIView : UIViewBehaviour
    {
        [SerializeField]
        private ChangedDataTextUIView _changedDataTextUIView;
        [SerializeField]
        private RewardItemUIView _rewardItemUIViewPrefab;
        [SerializeField]
        private ScrollRect _scrollRect;

        public override void Initialize()
        {
            base.Initialize();

            var rewardsData = ServiceLocator.Get<IDataService>().Get<RewardsData>();
            _changedDataTextUIView.Setup(rewardsData);
            foreach (var rewardData in rewardsData.Rewards)
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
