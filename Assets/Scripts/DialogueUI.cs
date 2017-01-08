using System;
using UnityEngine;
using Ashogue;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    private bool _visible = false;
    private string _displayText = "";
    private List<string> _displayBranches;

    private void Awake()
    {
        DialogueController.Events.Started += DialogueUI_Started;
        DialogueController.Events.Node += DialogueUI_Node;
        DialogueController.Events.Dialogue += DialogueUI_Dialogue;
        DialogueController.Events.Message += DialogueUI_Message;
        DialogueController.Events.Ended += DialogueUI_Ended;
    }

    private void Destroy()
    {
        DialogueController.Events.Started -= DialogueUI_Started;
        DialogueController.Events.Node -= DialogueUI_Node;
        DialogueController.Events.Dialogue -= DialogueUI_Dialogue;
        DialogueController.Events.Message -= DialogueUI_Message;
        DialogueController.Events.Ended -= DialogueUI_Ended;
    }

    private void OnGUI()
    {
        if (!_visible)
            return;

        GUI.Box(new Rect(10, 10, 200, 150), _displayText);

        for (int i = 0; i < _displayBranches.Count; i++)
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

    private void DialogueUI_Node(object sender, NodeEventArgs e) { }

    private void DialogueUI_Dialogue(object sender, DialogueEventArgs e)
    {
        _displayText = e.Text;
        _displayBranches = e.Branches;
    }

    private void DialogueUI_Message(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Message);
    }

    private void DialogueUI_Ended(object sender, EventArgs e)
    {
        _visible = false;
    }
}