using ScriptableObjects;
using TMPro;
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
            _nameTextMeshProUGUI.text = rewardConfig.Name;
        }
    }
}
