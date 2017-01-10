using UnityEditor;
using UnityEngine;
using System;
using Ashode;

public class SimpleNode : Node
{
    public override void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Button("Info", EditorStyles.toolbarButton);
        GUILayout.Button("Metadata", EditorStyles.toolbarButton);
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        GUILayout.Label("Simple Node!");
        Title = GUILayout.TextField(Title);

        GUILayout.EndVertical();
    }
}