using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("Loading Visuals")]
    public Image fadeOverlay = null;

    [Header("Timing Settings")]
    public float waitOnLoadEnd = 0.25f;
    public float fadeDuration = 0.25f;

    [Header("Loading Settings")]
    public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
    public ThreadPriority loadThreadPriority = ThreadPriority.High;

    [Header("Other")]
    // If loading additive, link to the cameras audio listener, to avoid multiple active audio listeners
    public AudioListener audioListener = null;

    private AsyncOperation operation = null;
    private Scene currentScene;

    private static string sceneToLoad = "";
    private static string loadingSceneName = "LoadingScene";

    public static void LoadScene(string levelName)
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        sceneToLoad = levelName;
        SceneManager.LoadScene(loadingSceneName);
    }

    void Start()
    {
        if (sceneToLoad == "")
            return;

        fadeOverlay.gameObject.SetActive(true); // Making sure it's on so that we can crossfade Alpha
        currentScene = SceneManager.GetActiveScene();
        StartCoroutine(LoadAsync(sceneToLoad));
    }

    private IEnumerator LoadAsync(string levelName)
    {
        yield return null;

        FadeIn();
        StartLoading(levelName);

        float lastProgress = 0f;

        while (DoneLoading() == false)
        {
            yield return null;

            if (Mathf.Approximately(operation.progress, lastProgress) == false)
                lastProgress = operation.progress;
        }

        if (loadSceneMode == LoadSceneMode.Additive)
            audioListener.enabled = false;

        yield return new WaitForSeconds(waitOnLoadEnd);

        FadeOut();
        yield return new WaitForSeconds(fadeDuration);

        if (loadSceneMode == LoadSceneMode.Additive)
            SceneManager.UnloadSceneAsync(currentScene.name);
        else
            operation.allowSceneActivation = true;
    }

    private void StartLoading(string levelName)
    {
        Application.backgroundLoadingPriority = loadThreadPriority;
        operation = SceneManager.LoadSceneAsync(levelName, loadSceneMode);

        if (loadSceneMode == LoadSceneMode.Single)
            operation.allowSceneActivation = false;
    }

    private bool DoneLoading()
    {
        bool check = (loadSceneMode == LoadSceneMode.Additive && operation.isDone);
        check  = check || (loadSceneMode == LoadSceneMode.Single && operation.progress >= 0.9f);

        return check;
    }

    void FadeIn()
    {
        fadeOverlay.CrossFadeAlpha(0, fadeDuration, true);
    }

    void FadeOut()
    {
        fadeOverlay.CrossFadeAlpha(1, fadeDuration, true);
    }
}