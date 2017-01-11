using UnityEditor;
using UnityEngine;
using System;
using Ashode;
using System.Collections.Generic;
using System.Linq;

public class SimpleNode : Node
{
    public override void OnGUI()
    {
        if(!Knobs.Values.Any()) {
            Knobs = new Dictionary<string, Knob> {
                { "o", new Knob() }
            };
        }

        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Button("Info", EditorStyles.toolbarButton);
        GUILayout.Button("Metadata", EditorStyles.toolbarButton);
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        GUILayout.Label("Simple Node!");
        Title = GUILayout.TextField(Title);

        GUILayout.BeginHorizontal();
        GUILayout.Label("output");
        DrawKnob("o");
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}