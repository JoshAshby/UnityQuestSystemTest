using System;
using UnityEditor;
using UnityEngine;

[Serializable]
[aaResponse(DisplayName = "Fact Set")]
public class aaFactSetResponse : aaResponse
{
    public string Hint = "global";
    public aaFact Fact = new aaFact();
    public aaFact.PossibleTypes ValueType;

    public override void Execute(aaEvent @event) =>
        Debug.Log($"EVENT LOG: {@event.EventName} - {Hint}.{Fact.Key} set to {Fact.Value}");

    public override void OnCustomGUI()
    {
#if UNITY_EDITOR
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        Hint = EditorGUILayout.TextField("Blackboard Hint", Hint);
        Fact.Key = EditorGUILayout.TextField("Fact Key", Fact.Key);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        ValueType = (aaFact.PossibleTypes)EditorGUILayout.EnumPopup(GUIContent.none, ValueType);

        if (ValueType == aaFact.PossibleTypes.Bool)
            Fact.Value = GUILayout.Toggle((bool)Fact.Value, "");
        if (ValueType == aaFact.PossibleTypes.Int)
            Fact.Value = EditorGUILayout.IntField(GUIContent.none, (int)Fact.Value);
        if (ValueType == aaFact.PossibleTypes.String)
            Fact.Value = EditorGUILayout.TextField(GUIContent.none, (string)Fact.Value);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
#endif
    }

    public override string GetDebugText() =>
        $"";
}