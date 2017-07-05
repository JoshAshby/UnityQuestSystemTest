// using UnityEngine;
// using UnityEngine.UI;

// using GrandCentral;

// [RequireComponent(typeof(CanvasGroup))]
// public class DialogueDisplay : MonoBehaviour, IHandle<DialogueEvent>
// {
//     private Text _text = null;
//     private CanvasGroup _canvasGroup = null;

//     [SerializeField]
//     private DialogueChoiceBox ChoiceBoxPrefab = null;

//     private DialogueChoiceBox _choiceBox = null;

//     private void Awake()
//     {
//         _text = GetComponentInChildren<Text>();
//         _canvasGroup = GetComponent<CanvasGroup>();

//         _choiceBox = Instantiate(ChoiceBoxPrefab) as DialogueChoiceBox;
//         _choiceBox.transform.parent = transform;

//         ClearInfo();
//     }

//     private void Start()
//     {
//         EventBus.Subscribe(this);
//     }

//     private void OnDestroy()
//     {
//         EventBus.Unsubscribe(this);
//     }

//     public void ShowInfo(IDialogueEntry entry)
//     {
//         Debug.LogFormat("DialogueDisplay - Got entry {0} - [{1}] with ({2})", entry.Key, entry.DisplayText, entry.Choices.Count);

//         if (entry.Choices.Count != 0)
//         {
//             _choiceBox.gameObject.SetActive(true);
//             _choiceBox.Populate(entry.Choices);
//         }
//         else
//             _choiceBox.gameObject.SetActive(false);

//         _text.text = entry.DisplayText;
//         _canvasGroup.alpha = 1f;
//     }

//     public void ClearInfo()
//     {
//         _canvasGroup.alpha = 0f;
//         _text.text = null;
//     }

//     public void Handle(DialogueEvent msg)
//     {
//         ClearInfo();
//         ShowInfo(msg.DialogueEntry);
//     }
// }