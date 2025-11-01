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

        private Vector3 _worldPosition;

        public void ReturnPosition()
        {
            _nameTextComponent.transform.rotation = Quaternion.identity;
            _nameTextComponent.transform.position = _worldPosition;
        }

        private void Awake()
        {
            _nameTextComponent = GetComponentInChildren<TMP_Text>();
            _worldPosition = _nameTextComponent.transform.position;
        }

        private void OnEnable()
        {
            _nameTextComponent.text = _rewardConfig.Name;
        }
    }
}
