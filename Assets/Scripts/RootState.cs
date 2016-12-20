using UnityEngine;

public class RootState : MonoBehaviour, IRootState
{
    [SerializeField]
    [InstallPrefab(typeof(FadeSprite), Single=true, NonLazy=true)]
    private FadeSprite fadeSpritePrefab;

    [SerializeField]
    [InstallPrefab(typeof(LoadingScreenManager), Single=true, NonLazy=true)]
    private LoadingScreenManager loadingScreenManagerPrefab;

    [SerializeField]
    [InstallString("LoadingSceneName")]
    private string LoadingSceneName;
}