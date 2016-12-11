using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HUDManager : MonoBehaviour
{
    public CanvasGroup Reticle = null;
    public CanvasGroup ReticleInfo = null;

    private CanvasGroup _canvasGroup = null;

    private CrosshairSystem _crosshair = null;

    private Animator _reticleAnimator = null;
    private Text _infoText = null;

    [Inject]
    private IGameManager _gameManager;

    private void Awake()
    {
        _crosshair = GetComponentInParent<CrosshairSystem>();
        _infoText = ReticleInfo.GetComponentInChildren<Text>();
        _reticleAnimator = Reticle.GetComponent<Animator>();
        _canvasGroup = GetComponent<CanvasGroup>();

        ReticleInfo.alpha = 0f;

        _crosshair.OnLookChange += OnLookChange;
        _crosshair.OnInteract += OnInteract;
    }

    private void Destroy() {
        _crosshair.OnLookChange -= OnLookChange;
        _crosshair.OnInteract -= OnInteract;
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
            _reticleAnimator.SetTrigger("Look");
            ShowInfo(obj.GetComponent<InteractiveSettings>().Name);
        }
        else
        {
            ClearInfo();
        }
    }

    public void OnInteract(Transform obj) {
        _reticleAnimator.SetTrigger("Interact");
    }

    public void ClearInfo()
    {
        ReticleInfo.alpha = 0f;
        _infoText.text = "";
    }

    public void ShowInfo(string info)
    {
        _infoText.text = info;
        ReticleInfo.alpha = 1f;
    }
}