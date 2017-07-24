using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class aaEventHandler : ScriptableObject
{
    [SerializeField]
    public string EventName = "UntitledEvent";
    [SerializeField]
    public List<aaCriterion> Criteria = new List<aaCriterion>();
    [SerializeField]
    public int Padding = 0;

    [SerializeField]
    public List<aaResponse> Responses = new List<aaResponse>();

    public int Weight
    {
        get { return Criteria.Count + Padding; }
    }

    public void Execute(aaEvent @event)
    {
        if (@event.EventName != EventName)
            return;

        if (!Criteria.TrueForAll(criterion => criterion.Check(@event)))
            return;

        Responses.ForEach(response => response.Execute(@event));
    }

    public override string ToString() =>
        $"aaEventHandler(Score: {Weight}, Criteria: {Criteria.Count}, Padding: {Padding}, Responses: {Responses.Count})";

    public void OnCustomGUI()
    {
#if UNITY_EDITOR
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField($"({Weight}) {EventName}");

        EditorGUILayout.BeginHorizontal();
        EventName = EditorGUILayout.TextField(EventName);
        Padding = EditorGUILayout.IntField(Padding);
        EditorGUILayout.EndHorizontal();

        Criteria.ForEach(criterion => criterion.OnCustomGUI());
        if (GUILayout.Button("Add Criterion"))
        {
            aaCriterion newCriteria = ScriptableObject.CreateInstance<aaCriterion>();
            Criteria.Add(newCriteria);
            AssetDatabase.AddObjectToAsset(newCriteria, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
        }

        Responses.ForEach(response => response.OnCustomGUI());
        if (GUILayout.Button("Add Response"))
        {
            aaResponse newResponse = ScriptableObject.CreateInstance<aaDebugResponse>();
            Responses.Add(newResponse);
            AssetDatabase.AddObjectToAsset(newResponse, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
        }

        EditorGUILayout.EndVertical();
#endif
    }
}