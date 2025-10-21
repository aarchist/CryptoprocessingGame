using Services;
using Services.Data;
using Services.Data.Core;
using Services.Displays;
using Services.Displays.Service;
using Services.FileSystem;
using Services.FileSystem.Core;
using Services.Loops;
using Services.Loops.Core;
using Services.UIView;
using Services.UIView.Core;
using Services.VideoRender;
using Services.VideoRender.Core;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public sealed class Bootstrapper : MonoBehaviour
{
    [SerializeField]
    private UIViewService _uiViewService;
    [SerializeField]
    private LoopsService _loopsService;

    private void Awake()
    {
        ServiceLocator.Register<IVideoRenderService>(new VideoRenderService());
        ServiceLocator.Register<IFileSystemService>(new FileSystemService());
        ServiceLocator.Register<IDisplaysService>(new DisplaysService());
        ServiceLocator.Register<IDataService>(new DataService());
        ServiceLocator.Register<IUIViewService>(_uiViewService);
        ServiceLocator.Register<ILoopsService>(_loopsService);
        ServiceLocator.Initialize();
    }

    private void OnDestroy()
    {
        ServiceLocator.Dispose();
    }
}
