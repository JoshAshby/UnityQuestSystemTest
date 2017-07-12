using UnityEngine;

/// <summary>
/// Handles keeping the HUD objects update to date with whats going on as
/// part of the CrosshairSystem, as its raycast hits and doesn't hit objects
/// </summmary>
public class HUDManager : Singleton<HUDManager>
{
    [Header("UI Canvas")]
    [SerializeField]
    private Reticle ReticlePrefab = null;

    [SerializeField]
    private ReticleInfo ReticleInfoPrefab = null;

    // [SerializeField]

    private CanvasGroup _canvasGroup = null;
    private CrosshairSystem _crosshair = null;

    private Reticle _reticle = null;
    private ReticleInfo _reticleInfo = null;

    private void Awake()
    {
        _crosshair = GetComponentInParent<CrosshairSystem>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _reticle = Instantiate(ReticlePrefab) as Reticle;
        _reticle.transform.parent = transform;

        _reticleInfo = Instantiate(ReticleInfoPrefab) as ReticleInfo;
        _reticleInfo.transform.parent = transform;

        _crosshair.OnLookChange += OnLookChange;
        _crosshair.OnInteract += OnInteract;
    }

    private void Destroy()
    {
        _crosshair.OnLookChange -= OnLookChange;
        _crosshair.OnInteract -= OnInteract;
    }

    private void Update()
    {
        if (GameManager.Instance.State == GameStates.Menu)
        {
            HideHUD();
            return;
        }

        ShowHUD();
    }

    public void ShowHUD()
    {
        _canvasGroup.alpha = 1f;
    }

    public void HideHUD()
    {
        _canvasGroup.alpha = 0f;
    }

    public void OnLookChange(Transform obj)
    {
        if (obj != null)
            Debug.LogFormat("Looking at tranform group {0}", obj.name);

        _reticle.LookAt(obj);
        _reticleInfo.ShowInfo(obj);
    }

    public void OnInteract(Transform obj)
    {
        if (obj != null)
            Debug.LogFormat("Interacting with tranform group {0}", obj.name);

        _reticle.InteractWith(obj);
    }
}