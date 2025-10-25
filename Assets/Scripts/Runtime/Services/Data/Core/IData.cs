using System;

namespace Services.Data.Core
{
    public interface IData
    {
        public Boolean IsChanged { get; set; }

        public void Save();

        public void Reset();
    }
}
