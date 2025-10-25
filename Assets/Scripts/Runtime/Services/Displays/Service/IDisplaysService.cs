using Services.Core;

namespace Services.Displays.Service
{
    public interface IDisplaysService : IService
    {
        public void ActivateSecondDisplayWhenConnected();
    }
}
