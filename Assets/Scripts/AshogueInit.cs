using UnityEngine;
using Ashogue;

public class AshogueInit : MonoBehaviour
{
    private bool _visible = true;

    [SerializeField]
    private string QuestID = "test";

    private void Awake()
    {
        DialogueController.Initialize();
        DialogueController.Events.Ended += AshogueInit_Ended;
        Debug.Assert(DialogueController.dialogues.Dialogues.ContainsKey(QuestID), "No good :/");
    }

    private void Destroy()
    {
        DialogueController.Events.Ended -= AshogueInit_Ended;
    }

    private void OnGUI()
    {
        if (!_visible)
            return;

        if (GUI.Button(new Rect(10, 10, 500, 80), "Start"))
        {
            _visible = false;
            DialogueController.StartDialogue(QuestID, delegate () { _visible = true; });
        }
    }

    private void AshogueInit_Ended(object sender, EndedEventArgs e)
    {
        _visible = true;
    }
}