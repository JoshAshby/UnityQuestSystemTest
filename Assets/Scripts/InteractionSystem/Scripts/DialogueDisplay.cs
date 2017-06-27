using UnityEngine;
using UnityEngine.UI;
using GrandCentral;
using GrandCentral.Events;
using GrandCentral.Switchboard;
using GrandCentral.Facts;

[RequireComponent(typeof(CanvasGroup))]
public class DialogueDisplay : MonoBehaviour, IHandle<DialogueRequest>
{
    public static void RequestLine(string character, string line, FactDictionary context)
    {
        Debug.LogFormat("DialogueController - Got request for {1} from {0}", character, line);
        IEntry entry = SwitchboardController.QueryFor(character, line, context);

        if (entry != null)
            EventsController.Publish<DialogueRequest>(new DialogueRequest { Entry = entry });
    }

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
        EventsController.Subscribe(this);
    }

    private void OnDestroy()
    {
        EventsController.Unsubscribe(this);
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