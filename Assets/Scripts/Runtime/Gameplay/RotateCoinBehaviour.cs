using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public sealed class RotateCoinBehaviour : MonoBehaviour
    {
        [SerializeField]
        private RewardConfig _rewardConfig;
        [SerializeField]
        private TMP_Text _nameTextComponent;

        private void OnEnable()
        {
            _nameTextComponent = GetComponentInChildren<TMP_Text>();
            _nameTextComponent.text = _rewardConfig.Name;
        }

        private void Update()
        {
            _nameTextComponent.transform.forward = transform.parent.forward;
        }
    }
}
