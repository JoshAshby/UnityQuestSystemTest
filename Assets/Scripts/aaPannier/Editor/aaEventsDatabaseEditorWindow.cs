using System.Linq;
using UnityEditor;
using UnityEngine;

public class aaEventsDatabaseEditorWindow : EditorWindow
{
    [MenuItem("Databases/Events Database")]
    public static void ShowWindow()
    {
        GetWindow<aaEventsDatabaseEditorWindow>(false, "Events Database", true);
    }

    private int selectedIndex = 0;
    private aaEventsDatabase[] databases;

    private aaEventsDatabase db
    {
        get { return databases.Count() > selectedIndex ? databases[selectedIndex] : null; }
    }

    private void OnGUI()
    {
        databases = Resources.FindObjectsOfTypeAll<aaEventsDatabase>();
        selectedIndex = EditorGUILayout.Popup(selectedIndex, databases.ToList().Select(x => x.Name).ToArray());

        if (db != null)
            DrawDB();
    }

    private void DrawDB()
    {
        EditorGUI.BeginChangeCheck();
        db.Name = EditorGUILayout.TextField(db.Name);

        if (GUILayout.Button("Add Handler"))
            AddHandler();

        ListHandlers();

        if (EditorGUI.EndChangeCheck())
            Dirty();
    }

    private void AddHandler()
    {
        aaEventHandler handler = ScriptableObject.CreateInstance<aaEventHandler>();
        db.Handlers.Add(handler);

        AssetDatabase.AddObjectToAsset(handler, db);
        Dirty();
    }

    private void ListHandlers()
    {
        int? removeHandleIndex = null;

        for (int handleIndex = 0; handleIndex < db.Handlers.Count; handleIndex++)
        {

            var handle = db.Handlers[handleIndex];

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField($"({handle.Weight}) {handle.EventName}");

            EditorGUILayout.BeginHorizontal();
            handle.EventName = EditorGUILayout.TextField(handle.EventName);
            handle.Padding = EditorGUILayout.IntField(handle.Padding);

            removeHandleIndex = RemoveButton(handleIndex);

            EditorGUILayout.EndHorizontal();

            int? removeCriteriaIndex = null;
            for (int criteriaIndex = 0; criteriaIndex < handle.Criteria.Count; criteriaIndex++)
            {
                var criteria = handle.Criteria[criteriaIndex];

                EditorGUILayout.BeginHorizontal();
                criteria.OnCustomGUI();
                removeCriteriaIndex = RemoveButton(criteriaIndex);
                EditorGUILayout.EndHorizontal();
            }

            if (removeCriteriaIndex != null)
            {
                var criteria = handle.Criteria[(int)removeCriteriaIndex];
                DestroyImmediate(criteria, true);
                handle.Criteria.RemoveAt((int)removeCriteriaIndex);
                Dirty();
            }

            if (GUILayout.Button("Add Criterion"))
            {
                aaCriterion newCriteria = ScriptableObject.CreateInstance<aaCriterion>();
                handle.Criteria.Add(newCriteria);
                AssetDatabase.AddObjectToAsset(newCriteria, handle);
                Dirty();
            }

            int? removeResponseIndex = null;
            for (int responseIndex = 0; responseIndex < handle.Responses.Count; responseIndex++)
            {
                var response = handle.Responses[responseIndex];

                EditorGUILayout.BeginHorizontal();
                response.OnCustomGUI();
                removeResponseIndex = RemoveButton(responseIndex);
                EditorGUILayout.EndHorizontal();
            }

            if (removeResponseIndex != null)
            {
                var response = handle.Responses[(int)removeResponseIndex];
                DestroyImmediate(response, true);
                handle.Criteria.RemoveAt((int)removeResponseIndex);
                Dirty();
            }

            if (GUILayout.Button("Add Response"))
            {
                aaResponse newResponse = ScriptableObject.CreateInstance<aaDebugResponse>();
                handle.Responses.Add(newResponse);
                AssetDatabase.AddObjectToAsset(newResponse, handle);
                Dirty();
            }

            EditorGUILayout.EndVertical();
        }

        if (removeHandleIndex != null)
        {
            var handle = db.Handlers[(int)removeHandleIndex];
            DestroyImmediate(handle, true);
            db.Handlers.RemoveAt((int)removeHandleIndex);
            Dirty();
        }
    }

    private void Dirty()
    {
        EditorUtility.SetDirty(db);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(db));
    }

    private void Save()
    {
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
    }

    private int? RemoveButton(int index)
    {
        int? removeIndex = null;
        Color temp = GUI.color;

        GUI.color = new Color(1f, 0f, 0f);
        if (GUILayout.Button("-", GUILayout.Width(20)))
            removeIndex = index;

        GUI.color = temp;

        return removeIndex;
    }
}