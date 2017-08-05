using System;
using UnityEditor;
using UnityEngine;

[Serializable]
[aaResponse(DisplayName = "Debug")]
public class aaDebugResponse : aaResponse
{
    public string CustomMessage = "";

    public override void Execute(aaEvent @event) =>
        Debug.Log($"EVENT LOG: {@event.EventName} - {CustomMessage}");

    public override void OnCustomGUI()
    {
#if UNITY_EDITOR
        EditorGUILayout.BeginHorizontal();
        CustomMessage = EditorGUILayout.TextField("Debug Response", CustomMessage);
        EditorGUILayout.EndHorizontal();
#endif
    }

    public override string GetDebugText() =>
        $"";
}