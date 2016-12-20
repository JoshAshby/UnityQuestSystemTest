using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Zenject;

public class LoadingScreenManager : MonoBehaviour, ILoadingScreenManager
{
    [Header("Timing Settings")]
    [SerializeField]
    private float waitOnLoadEnd = 0.25f;

    [Header("Loading Settings")]
    [SerializeField]
    private LoadSceneMode loadSceneMode = LoadSceneMode.Single;

    [Inject]
    private IFader fadeOverlay;

    [Inject(Id="LoadingSceneName")]
    private string loadingSceneName;

    private IEnumerator operation = null;

    public void LoadScene(string levelName)
    {
        if (levelName == "")
            return;

        operation = LoadAsync(levelName);
        StartCoroutine(operation);
    }

    private IEnumerator LoadAsync(string levelName)
    {
        yield return fadeOverlay.FadeIn();

		yield return SceneManager.LoadSceneAsync(loadingSceneName);

        yield return fadeOverlay.FadeOut();

        if(waitOnLoadEnd != 0)
            yield return new WaitForSeconds(waitOnLoadEnd);

        yield return fadeOverlay.FadeIn();

        yield return SceneManager.LoadSceneAsync(levelName, loadSceneMode);

        yield return SceneManager.UnloadSceneAsync(loadingSceneName);

        yield return fadeOverlay.FadeOut();
    }
}