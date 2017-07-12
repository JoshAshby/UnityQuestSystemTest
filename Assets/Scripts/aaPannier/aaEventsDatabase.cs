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
}

// [CustomEditor(typeof(aaEventsDatabase))]
// public class aaEventsDatabaseEditor : Editor
// {
//     SerializedProperty Handlers;

//     private void OnEnable()
//     {
//         Handlers = serializedObject.FindProperty("Handlers");
//     }

//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();

//         EditorGUILayout.PropertyField(Handlers);

//         serializedObject.ApplyModifiedProperties();
//     }
// }