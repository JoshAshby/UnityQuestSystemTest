using UnityEngine;
using UnityEngine.UI;
using GrandCentral;
using GrandCentral.Telegraph;

[RequireComponent(typeof(CanvasGroup))]
public class DialogueDisplay : MonoBehaviour, IHandle<DialogueRequest>
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

    private void Start()
    {
        TelegraphController.Subscribe(this);
    }

    private void OnDestroy()
    {
        TelegraphController.Unsubscribe(this);
    }

    public void ShowInfo(string info)
    {
        _text.text = info;
        _canvasGroup.alpha = 1f;
    }

    public void ClearInfo()
    {
        _canvasGroup.alpha = 0f;
        _text.text = "";
    }

    public void Handle(DialogueRequest msg)
    {
        Debug.LogFormat("DialogueDisplay - Got entry {0} - [{1}] -- next --> {2}", msg.Entry.Name, msg.Entry.Payload, msg.Entry.NextEntry);
        ShowInfo(msg.Entry.Payload);
    }
}