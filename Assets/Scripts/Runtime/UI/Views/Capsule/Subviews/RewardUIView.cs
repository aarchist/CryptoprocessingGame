using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Rewards
{
    public sealed class RewardUIView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _nameTextMeshProUGUI;
        [SerializeField]
        private Image _iconImage;

        public void Setup(RewardConfig rewardConfig)
        {
            transform.localPosition = rewardConfig.Position;
            _nameTextMeshProUGUI.text = rewardConfig.Name;
            _iconImage.sprite = rewardConfig.Icon;
        }
    }
}
