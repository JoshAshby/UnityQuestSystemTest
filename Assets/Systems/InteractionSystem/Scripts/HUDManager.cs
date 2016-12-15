using UnityEngine;
using Zenject;

/// <summary>
/// Handles keeping the HUD objects update to date with whats going on as
/// part of the CrosshairSystem, as its raycast hits and doesn't hit objects
/// </summmary>
public class HUDManager : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField]
    public Reticle ReticleObj         = null;

    [SerializeField]
    public ReticleInfo ReticleInfoObj = null;

    private CanvasGroup _canvasGroup   = null;
    private CrosshairSystem _crosshair = null;

    [Inject]
    private IGameManager _gameManager;

    private void Awake()
    {
        _crosshair   = GetComponentInParent<CrosshairSystem>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _crosshair.OnLookChange += OnLookChange;
        _crosshair.OnInteract   += OnInteract;
    }

    private void Destroy() {
        _crosshair.OnLookChange -= OnLookChange;
        _crosshair.OnInteract   -= OnInteract;
    }

    private void Update()
    {
        if(_gameManager.State == GameStates.Menu) {
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

        ReticleObj.LookAt(obj);
        ReticleInfoObj.ShowInfo(obj);
    }

    public void OnInteract(Transform obj) {
        if (obj != null)
            Debug.LogFormat("Interacting with tranform group {0}", obj.name);

        ReticleObj.InteractWith(obj);
    }
}