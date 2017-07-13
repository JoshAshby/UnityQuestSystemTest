using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class aaEventsDatabase : ScriptableObject
{
    public List<aaEventHandler> Handlers = new List<aaEventHandler>();

    public void Handle(aaEvent @event) =>
        Handlers.ForEach(handler => handler.Execute(@event));
}

[CustomEditor(typeof(aaEventsDatabase))]
public class aaEventsDatabaseEditor : Editor
{
     aaEventsDatabase db;

    private void OnEnable() =>
        db = (aaEventsDatabase)target;

    public override void OnInspectorGUI()
    {
        db.Handlers.ForEach(handler => handler.OnCustomGUI());
        EditorUtility.SetDirty(target);
    }
}