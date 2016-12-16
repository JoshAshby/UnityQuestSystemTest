using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ReticleInfo : MonoBehaviour
{
    private Text _text = null;
    private CanvasGroup _canvasGroup = null;

    private void Awake()
    {
        _text        = GetComponentInChildren<Text>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _text.text = "";
        _canvasGroup.alpha = 0f;
    }

    public void ShowInfo(string info)
    {
        _text.text = info;
        _canvasGroup.alpha = 1f;
    }

    public void ShowInfo(Transform obj)
    {
        if (obj == null) {
            ClearInfo();
            return;
        }

        InteractiveSettings settings = obj.GetComponent<InteractiveSettings>();

        if (settings == null) {
            ClearInfo();
            return;
        }

        if (!settings.DisplayInHud) {
            ClearInfo();
            return;
        }

        ShowInfo(settings.HUDName);
    }

    public void ClearInfo()
    {
        _canvasGroup.alpha = 0f;
        _text.text = "";
    }
}