using System.IO;
using Data.Video;
using Services;
using Services.Data.Core;
using Services.FileSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Admin.UploadedVideos
{
    public sealed class AddVideoItemUIView : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        private void OnEnable()
        {
            _button.onClick.AddListener(ChooseFiles);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(ChooseFiles);
        }

        private void ChooseFiles()
        {
            ServiceLocator.Get<IFileSystemService>().ChooseVideoFiles(filePaths =>
            {
                var data = ServiceLocator.Get<IDataService>().Get<UploadedVideosData>();
                foreach (var filePath in filePaths)
                {
                    data.Add(new VideoData()
                    {
                        Path = filePath,
                        Name = Path.GetFileName(filePath)
                    });
                }
            });
        }
    }
}
