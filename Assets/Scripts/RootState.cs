using UnityEngine;

public class RootState : MonoBehaviour, IRootState
{
    [InstallPrefab(typeof(FadeSprite), Single=true, NonLazy=true)]
    public FadeSprite fadeSpritePrefab;

    [InstallPrefab(typeof(LoadingScreenManager), Single=true, NonLazy=true)]
    public LoadingScreenManager loadingScreenManagerPrefab;

    [InstallString("LoadingSceneName")]
    public string LoadingSceneName;
}