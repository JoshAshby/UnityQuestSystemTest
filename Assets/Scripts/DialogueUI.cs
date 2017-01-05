using System;
using UnityEngine;
using Ashogue;

public class DialogueUI : MonoBehaviour
{
    private bool _visible = false;
    private string _displayText = "";
    private string[] _displayBranches;

    private void Awake()
    {
        DialogueController.Events.Started += DialogueUI_Started;
        DialogueController.Events.Node += DialogueUI_Node;
        DialogueController.Events.Ended += DialogueUI_Ended;
        DialogueController.Events.Message += DialogueUI_Message;
    }

    private void Destroy()
    {
        DialogueController.Events.Started -= DialogueUI_Started;
        DialogueController.Events.Node -= DialogueUI_Node;
        DialogueController.Events.Ended -= DialogueUI_Ended;
        DialogueController.Events.Message -= DialogueUI_Message;
    }

    private void OnGUI()
    {
        if (!_visible)
            return;

        GUI.Box(new Rect(10, 10, 200, 150), _displayText);

        for (int i = 0; i < _displayBranches.Length; i++)
        {
            string text = _displayBranches[i];
            if (GUI.Button(new Rect(10, 220 + (40 * i), 200, 30), text))
                DialogueController.ContinueDialogue(text);
        }
    }

    private void DialogueUI_Started(object sender, EventArgs e)
    {
        _visible = true;
    }

    private void DialogueUI_Node(object sender, NodeEventArgs e)
    {
        _displayText = e.text.Text;
        _displayBranches = e.text.Branches;
    }

    private void DialogueUI_Ended(object sender, EndedEventArgs e)
    {
        _visible = false;
    }

    private void DialogueUI_Message(object sender, MessageEventArgs e)
    {
        Debug.Log(e.message);
    }
}