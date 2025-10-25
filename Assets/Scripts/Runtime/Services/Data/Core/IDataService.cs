using System;
using Services.Core;

namespace Services.Data.Core
{
    public interface IDataService : IService
    {
        public TData Get<TData>() where TData : IData, new();

        public IData LoadData(Type type);

        public void Save(IData data);
    }
}
