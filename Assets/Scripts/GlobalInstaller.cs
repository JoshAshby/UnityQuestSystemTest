using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller<GlobalInstaller>
{
    [SerializeField]
    private GameObject fadePrefab;

    [SerializeField]
    private GameObject loadingScreenManagerPrefab;

    [SerializeField]
    private string LoadingSceneName;

    public override void InstallBindings()
    {
        Container.Bind<string>().WithId("LoadingSceneName").FromInstance(LoadingSceneName);
        Container.Bind<FadeSprite>().FromPrefab(fadePrefab).AsSingle().NonLazy();
        Container.Bind<LoadingScreenManager>().FromPrefab(loadingScreenManagerPrefab).AsSingle().NonLazy();

        Container.BindAllInterfaces<GameManager>().To<GameManager>().AsSingle();
    }
}