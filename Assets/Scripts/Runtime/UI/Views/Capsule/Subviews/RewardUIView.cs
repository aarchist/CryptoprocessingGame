using ScriptableObjects;
using TMPro;
using UI.Views.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Capsule.Subviews
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
