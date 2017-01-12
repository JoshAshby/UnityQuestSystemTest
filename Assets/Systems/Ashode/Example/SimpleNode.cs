using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Ashode;

public class SimpleNode : Node
{
    private string id = "";
    public override void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if(GUILayout.Button("Left", EditorStyles.toolbarButton))
                AddKnob(id, NodeSide.Left);
            if(GUILayout.Button("Right", EditorStyles.toolbarButton))
                AddKnob(id, NodeSide.Right);
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
            GUILayout.Label("Simple Node!");
            id = GUILayout.TextField(id);

            string removeKnob = null;
            foreach(var knob in Knobs)
            {
                GUILayout.BeginHorizontal();
                    GUILayout.Label(knob.Key);
                    DrawKnob(knob.Key);
                    if(GUILayout.Button("X"))
                        removeKnob = knob.Key;
                GUILayout.EndHorizontal();
            }

            if(!string.IsNullOrEmpty(removeKnob))
                RemoveKnob(removeKnob);
        GUILayout.EndVertical();
    }
}