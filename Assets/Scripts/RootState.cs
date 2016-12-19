using UnityEngine;

public class RootState : MonoBehaviour, IRootState
{
    [InstallPrefab(typeof(FadeSprite))]
    public FadeSprite fadeSpritePrefab;

    [InstallPrefab(typeof(LoadingScreenManager))]
    public LoadingScreenManager loadingScreenManagerPrefab;
}