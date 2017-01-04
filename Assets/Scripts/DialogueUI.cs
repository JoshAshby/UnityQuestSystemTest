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
        DialogueController.Events.Ended += DialogueUI_Ended
    }

    private void DialogueUI_Started(EventArgs e)
    {
        _visible = true;
    }

    private void DialogueUI_Node(NodeEventArgs e)
    {
        _displayText = e.text.Text;
        _displayBranches = e.text.Branches;
    }

    private void DialogueUI_Ended(EventArgs e)
    {
        _visible = false;
    }
}