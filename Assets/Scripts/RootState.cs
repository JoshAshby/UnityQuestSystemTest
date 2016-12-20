using UnityEngine;

public class RootState : MonoBehaviour, IRootState
{
    [SerializeField]
    [InstallPrefab(typeof(IFader), Single=true, NonLazy=true)]
    private Fader fadeSpritePrefab;

    [SerializeField]
    [InstallPrefab(typeof(ILoadingScreenManager), Single=true, NonLazy=true)]
    private LoadingScreenManager loadingScreenManagerPrefab;

    [SerializeField]
    [InstallString("LoadingSceneName")]
    private string LoadingSceneName;
}