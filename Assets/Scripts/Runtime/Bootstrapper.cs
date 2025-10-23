using Services;
using Services.Config;
using Services.Config.Core;
using Services.Data;
using Services.Data.Core;
using Services.Displays;
using Services.Displays.Service;
using Services.FileSystem;
using Services.FileSystem.Core;
using Services.Gameplay;
using Services.Gameplay.Core;
using Services.Loops;
using Services.Loops.Core;
using Services.Rewards;
using Services.Rewards.Core;
using Services.UIView;
using Services.UIView.Core;
using Services.VideoRender;
using Services.VideoRender.Core;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public sealed class Bootstrapper : MonoBehaviour
{
    [SerializeField]
    private GameplayService _gameplayService;
    [SerializeField]
    private ConfigService _configService;
    [SerializeField]
    private UIViewService _uiViewService;
    [SerializeField]
    private LoopsService _loopsService;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ServiceLocator.Register<IVideoRenderService>(new VideoRenderService());
        ServiceLocator.Register<IFileSystemService>(new FileSystemService());
        ServiceLocator.Register<IDisplaysService>(new DisplaysService());
        ServiceLocator.Register<IRewardsService>(new RewardsService());
        ServiceLocator.Register<IDataService>(new DataService());
        ServiceLocator.Register<IGameplayService>(_gameplayService);
        ServiceLocator.Register<IConfigService>(_configService);
        ServiceLocator.Register<IUIViewService>(_uiViewService);
        ServiceLocator.Register<ILoopsService>(_loopsService);
        ServiceLocator.Initialize();
    }

    private void OnDestroy()
    {
        ServiceLocator.Dispose();
    }
}
