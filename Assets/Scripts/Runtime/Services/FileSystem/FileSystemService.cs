using System;
using Services.FileSystem.Core;
using SFB;

namespace Services.FileSystem
{
    public sealed class FileSystemService : IFileSystemService
    {
        private static readonly ExtensionFilter[] _videoFilter =
        {
            new("Video Files", "mp4", "avi", "mov", "wmv", "mkv")
        };

        public void ChooseVideoFiles(Action<String[]> onComplete)
        {
            StandaloneFileBrowser.OpenFilePanelAsync("Select Video", "", _videoFilter, true, onComplete);
        }

        public void ChooseVideoFile(Action<String> onComplete)
        {
            StandaloneFileBrowser.OpenFilePanelAsync("Select Video", "", _videoFilter, false, videoPaths =>
            {
                if (videoPaths.Length == 0)
                {
                    return;
                }

                onComplete.Invoke(videoPaths[0]);
            });
        }

        public void Dispose()
        {
        }
    }
}
