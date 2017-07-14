using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(aaEventsDatabase))]
public class aaEventsDatabaseEditor : Editor
{
     aaEventsDatabase db;

    private void OnEnable() =>
        db = (aaEventsDatabase)target;

    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Add Handler"))
            db.Handlers.Add(ScriptableObject.CreateInstance<aaEventHandler>());

        db.Handlers.ForEach(handler => handler.OnCustomGUI());

        EditorUtility.SetDirty(target);
    }
}