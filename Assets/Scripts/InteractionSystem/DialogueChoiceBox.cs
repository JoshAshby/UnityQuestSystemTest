// using System.Collections.Generic;
// using System.Linq;

// using UnityEngine;
// using UnityEngine.UI;

// using GrandCentral;

// public class DialogueChoiceBox : MonoBehaviour
// {
//     [SerializeField]
//     private DialogueChoiceButton DialogueChoiceButtonPrefab = null;

//     private ScrollRect _scrollView = null;

//     private void Awake()
//     {
//         _scrollView = GetComponentInChildren<ScrollRect>();
//         Clear();
//     }

//     public void Populate(List<IDialogueChoice> choices)
//     {
//         Clear();

//         choices.ForEach(choice => {
//             RectTransform content = _scrollView.content;
//             DialogueChoiceButton button = Instantiate(DialogueChoiceButtonPrefab) as DialogueChoiceButton;

//             button.Setup(choice);
//             button.transform.parent = content.transform;
//         });
//     }

//     public void Clear()
//     {
//         Buttons().ForEach(obj => Destroy(obj));
//     }

//     private List<DialogueChoiceButton> Buttons()
//     {
//         return _scrollView.content.GetComponentsInChildren<DialogueChoiceButton>().ToList();
//     }
// }