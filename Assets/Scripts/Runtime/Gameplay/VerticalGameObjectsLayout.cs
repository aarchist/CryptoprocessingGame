using System;
using UnityEngine;

namespace Gameplay
{
    [ExecuteAlways]
    public sealed class VerticalGameObjectsLayout : MonoBehaviour
    {
        [SerializeField]
        private Single _spacing;
        [SerializeField]
        private Single _scale;

        private void Update()
        {
            var spacing = 0.0F;
            for (var index = 0; index < transform.childCount; index++)
            {
                var child = transform.GetChild(index);
                var position = child.localPosition;
                position.x = 0.0F;
                position.y = spacing;
                spacing += _spacing;
                child.localPosition = position;
            }
        }
    }
}
