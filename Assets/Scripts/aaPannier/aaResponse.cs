using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class aaResponse : ScriptableObject
{
    public abstract void Execute(aaEvent @event);
    public abstract void OnCustomGUI();
    public abstract string GetDebugText();
}

[Serializable]
public class aaDebugResponse : aaResponse
{
    public override void Execute(aaEvent @event) =>
        Debug.Log($"EVENT LOG: {@event.EventName}");

    public override void OnCustomGUI()
    {
    #if UNITY_EDITOR
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Debuging Response");
        EditorGUILayout.EndVertical();
    #endif
    }

    public override string GetDebugText() =>
        $"";
}