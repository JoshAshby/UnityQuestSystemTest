using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HUDManager : MonoBehaviour
{
    public CanvasGroup Reticle = null;
    public CanvasGroup ReticleInfo = null;

    private CrosshairSystem _crosshair = null;
    private string _info = "";

    private void Awake()
    {
        _crosshair = GetComponentInParent<CrosshairSystem>();

        ReticleInfo.alpha = 0f;

        _crosshair.OnLookChange += OnLookChange;
    }

    private void Destroy() {
        _crosshair.OnLookChange -= OnLookChange;
    }

    private void Update()
    {
        ReticleInfo.GetComponentInChildren<Text>().text = _info;

        if (_info != "")
            ReticleInfo.alpha = 1f;
        else
            ReticleInfo.alpha = 0f;
    }

    public void OnLookChange(Transform obj)
    {
        Debug.Log("Encountered object");
        if (obj != null)
            ShowInfo(obj.GetComponentInParent<InteractiveSettings>().Name);
        else
            ClearInfo();
    }

    public void ClearInfo()
    {
        _info = "";
    }

    public void ShowInfo(string info)
    {
        _info = info;
    }
}