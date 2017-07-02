using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using GrandCentral;

[RequireComponent(typeof(CanvasGroup))]
public class DialogueDisplay : MonoBehaviour, IHandle<DialogueEvent>
{
    private Text _text = null;
    private CanvasGroup _canvasGroup = null;
    private ScrollRect _scrollView = null;

    [SerializeField]
    private DialogueChoiceButton buttonPrefab = null;

    [SerializeField]
    private EventSystem EventSystem = null;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _scrollView = GetComponentInChildren<ScrollRect>();

        _canvasGroup.alpha = 0f;

        _text.text = "";
        _scrollView.content.GetComponentsInChildren<DialogueChoiceButton>().ToList().ForEach(obj => {
            Destroy(obj);
        });
    }

    private void Start()
    {
        EventBus.Subscribe(this);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(this);
    }

    public void ShowInfo(IDialogueEntry entry)
    {
        _text.text = entry.DisplayText;

        for (int i = 0; i < entry.Choices.Count; i++)
        {
            IDialogueChoice choice = entry.Choices[ i ];

            RectTransform content = _scrollView.content;
            DialogueChoiceButton button = Instantiate(buttonPrefab) as DialogueChoiceButton;

            button.Setup(choice);
            button.transform.parent = content.transform;

            if(i == 0)
                EventSystem.SetSelectedGameObject(button.gameObject);
        }

        _canvasGroup.alpha = 1f;
    }

    public void ClearInfo()
    {
        _canvasGroup.alpha = 0f;
        EventSystem.SetSelectedGameObject(null);

        _text.text = null;
        _scrollView.content.GetComponentsInChildren<DialogueChoiceButton>().ToList().ForEach(obj => {
            Destroy(obj.gameObject);
        });
    }

    public void Handle(DialogueEvent msg)
    {
        ClearInfo();
        Debug.LogFormat("DialogueDisplay - Got entry {0} - [{1}]", msg.DialogueEntry.Key, msg.DialogueEntry.DisplayText);
        ShowInfo(msg.DialogueEntry);
    }
}