using UnityEngine;
using UnityEngine.UI;
using GrandCentral;

public class DialogueEvent : IEvent {
    public IEntry Entry { get; set; }
}

[RequireComponent(typeof(CanvasGroup))]
public class DialogueDisplay : MonoBehaviour, IHandle<DialogueEvent>
{
    private Text _text = null;
    private CanvasGroup _canvasGroup = null;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _text.text = "";
        _canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        EventBus.Subscribe(this);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(this);
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

    public void Handle(DialogueEvent msg)
    {
        Debug.LogFormat("DialogueDisplay - Got entry {0} - [{1}]", msg.Entry.Name, msg.Entry.Payload);
        ShowInfo(msg.Entry.Payload);
    }
}