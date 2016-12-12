using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HUDManager : MonoBehaviour
{
    [Header("UI Canvas")]
    [SerializeField]
    public CanvasGroup Reticle = null;

    [SerializeField]
    public ReticleInfo ReticleInfo = null;

    private CanvasGroup _canvasGroup = null;
    private CrosshairSystem _crosshair = null;
    private Animator _reticleAnimator = null;

    [Inject]
    private IGameManager _gameManager;

    private void Awake()
    {
        _crosshair = GetComponentInParent<CrosshairSystem>();
        _reticleAnimator = Reticle.GetComponent<Animator>();
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
        if(_gameManager.fsm.CurrentState == GameStates.Menu) {
            _canvasGroup.alpha = 0f;
            return;
        }

        _canvasGroup.alpha = 1f;
    }

    public void OnLookChange(Transform obj)
    {
        Debug.Log("Encountered object");
        if (obj != null)
        {
            _reticleAnimator.SetBool("Looking", true);
            ReticleInfo.ShowInfo(obj.GetComponent<InteractiveSettings>().Name);
        }
        else
        {
            _reticleAnimator.SetBool("Looking", false);
            ReticleInfo.ClearInfo();
        }
    }

    public void OnInteract(Transform obj) {
        _reticleAnimator.SetTrigger("Interact");
    }
}