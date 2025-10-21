using System;

namespace UI.Views.Core
{
    public interface IUIView
    {
        public Boolean IsShowed { get; }

        public void Initialize();

        public void Show();

        public void Hide();
    }
}
