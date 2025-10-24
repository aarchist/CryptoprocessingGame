using System;
using UI.Views.Properties.Core;
using UnityEngine;

namespace UI.Views.Properties
{
    public sealed class Int32PropertyUIView : PropertyUIView<Int32>
    {
        [SerializeField]
        private Int32 _minValue;

        protected override void ValidateInput(String text)
        {
            if (Int32.TryParse(text, out var value) && (value >= _minValue))
            {
                Value = value;
                IsValid = true;
                return;
            }

            IsValid = false;
        }
    }
}
