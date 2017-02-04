using System;
using UnityEngine;
using Ashogue;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    private bool _visible = false;
    private string _displayText = "";
    private Dictionary<string, string> _displayBranches;

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

        Rect displayRect = new Rect(10, 10, 800, 600);

        GUI.Box(displayRect, _displayText);

        if(_displayBranches == null)
            return;

        int count = 0;
        foreach(var branch in _displayBranches)
        {
            if (GUI.Button(new Rect(10, displayRect.yMax + (40 * count), displayRect.width, 30), branch.Value))
                DialogueController.ContinueDialogue(branch.Key);

            count++;
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