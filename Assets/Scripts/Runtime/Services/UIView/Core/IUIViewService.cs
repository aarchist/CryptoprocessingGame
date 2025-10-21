using Services.Core;
using UI.Views.Core;

namespace Services.UIView.Core
{
    public interface IUIViewService : IService
    {
        public TUIView Get<TUIView>() where TUIView : IUIView;
    }
}
