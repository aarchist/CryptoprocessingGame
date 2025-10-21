using Services.Core;

namespace Services.Data.Core
{
    public interface IDataService : IService
    {
        public TData Get<TData>() where TData : IData, new();
    }
}
