using System;
using Services.Core;

namespace Services.FileSystem.Core
{
    public interface IFileSystemService : IService
    {
        public void ChooseVideoFiles(Action<String[]> onComplete);

        public void ChooseVideoFile(Action<String> onComplete);
    }
}
