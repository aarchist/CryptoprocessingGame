using System;

namespace Services.Data.Core
{
    public abstract class BaseData : IData
    {
        public Action Reloaded;

        public abstract Boolean IsChanged { get; set; }

        protected abstract void LoadChanges(IData other);

        public void Save()
        {
            if (!IsChanged)
            {
                return;
            }

            ServiceLocator.Get<IDataService>().Save(this);
            IsChanged = false;
            Reloaded?.Invoke();
        }

        public void Reset()
        {
            if (!IsChanged)
            {
                return;
            }

            LoadChanges(ServiceLocator.Get<IDataService>().LoadData(GetType()));
            IsChanged = false;
            Reloaded?.Invoke();
        }
    }
}
