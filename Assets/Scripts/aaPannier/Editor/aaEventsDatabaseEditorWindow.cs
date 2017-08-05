using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

static class aaEditorUtil
{
    static public bool ColoredButton(string text, Color color, params GUILayoutOption[] options)
    {
        Color temp = GUI.color;
        GUI.color = color;

        bool shouldRemove = GUILayout.Button(text, options);

        GUI.color = temp;

        return shouldRemove;
    }

    static public bool ColoredButton(string text, Color color, GUIStyle style, params GUILayoutOption[] options)
    {
        Color temp = GUI.color;
        GUI.color = color;

        bool shouldRemove = GUILayout.Button(text, style, options);

        GUI.color = temp;

        return shouldRemove;
    }

    static public bool ColoredButton(string text, Color color)
    {
        Color temp = GUI.color;
        GUI.color = color;

        bool shouldRemove = GUILayout.Button(text);

        GUI.color = temp;

        return shouldRemove;
    }
}

public class aaEventsDatabaseEditorWindow : EditorWindow
{
    [MenuItem("Databases/Events Database")]
    public static void ShowWindow()
    {
        GetWindow<aaEventsDatabaseEditorWindow>(false, "Events Database", true);
    }

    private bool initd = false;

    private int selectedDatabaseIndex = 0;
    private int selectedHandleIndex = 0;
    private aaEventsDatabase[] databases;

    private aaEventsDatabase selectedDatabase
    {
        get { return databases.Count() > selectedDatabaseIndex ? databases[selectedDatabaseIndex] : null; }
    }

    private aaEventHandler selectedHandler
    {
        get { return selectedDatabase.Handlers.Count() > selectedHandleIndex ? selectedDatabase.Handlers[selectedHandleIndex] : null; }
    }

    private static KeyValuePair<string, Type>[] ResponseTypesAndNames()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(aaResponse)))
            .SelectMany(x =>
            {
                return x.GetCustomAttributes(typeof(aaResponseAttribute), false)
                    .Select(y => (aaResponseAttribute)y)
                    .Select(y => new KeyValuePair<string, Type>(y.DisplayName, x));
            }).ToArray();
    }

    private KeyValuePair<string, Type>[] ResponseTypes { get; } = ResponseTypesAndNames();

    private void Init()
    {
        if (initd)
            return;

        databases = Resources.FindObjectsOfTypeAll<aaEventsDatabase>();
    }

    private float toolbarHeight = 0;
    private int sideWindowWidth = 300;
    private Vector2 handlerListScrollPosition;
    private Vector2 handlerScrollPosition;

    private void OnGUI()
    {
        Init();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        selectedDatabaseIndex = EditorGUILayout.Popup(selectedDatabaseIndex, databases.ToList().Select(x => x.Name).ToArray());
        if (aaEditorUtil.ColoredButton("Save", Color.white, GUILayout.ExpandWidth(false)))
            Save();
        EditorGUILayout.EndHorizontal();

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

        if (selectedDatabase != null)
            DrawDB();

        if (GUI.Button(new Rect(0, 0, position.width, position.height), "", GUIStyle.none))
            Defocus();
    }

    private void DrawDB()
    {
        if (Event.current.type == EventType.Repaint)
            toolbarHeight = GUILayoutUtility.GetLastRect().yMax + 6;

        Rect sideWindowRect = new Rect(3, toolbarHeight, sideWindowWidth, position.height - toolbarHeight - 3);
        GUILayout.BeginArea(sideWindowRect, GUI.skin.box);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Handlers");
        if (aaEditorUtil.ColoredButton("+", Color.green, GUILayout.ExpandWidth(false)))
            AddHandler();
        EditorGUILayout.EndHorizontal();

        int? removeHandleIndex = null;
        handlerScrollPosition = EditorGUILayout.BeginScrollView(handlerScrollPosition, GUILayout.ExpandHeight(true));
        for (int handleIndex = 0; handleIndex < selectedDatabase.Handlers.Count; handleIndex++)
        {
            EditorGUILayout.BeginHorizontal();
            var handle = selectedDatabase.Handlers[handleIndex];
            Color color = handleIndex == selectedHandleIndex ? Color.grey : GUI.color;

            if (aaEditorUtil.ColoredButton($"{handle.EventName} ({handle.Padding})", color))
                selectedHandleIndex = handleIndex;

            if (aaEditorUtil.ColoredButton("-", Color.red, GUILayout.ExpandWidth(false)))
                removeHandleIndex = handleIndex;

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (removeHandleIndex != null)
            RemoveHandle((int)removeHandleIndex);

        GUILayout.EndArea();

        Rect canvasRect = new Rect(sideWindowWidth + 6, toolbarHeight, position.width - sideWindowWidth - 10, position.height - toolbarHeight - 3);
        GUILayout.BeginArea(canvasRect);

        if (selectedHandler != null)
            DrawHandler();

        GUILayout.EndArea();
    }

    private void DrawHandler()
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        selectedHandler.EventName = EditorGUILayout.TextField("EventName:", selectedHandler.EventName);
        selectedHandler.Padding = EditorGUILayout.IntField("Padding:", selectedHandler.Padding);
        EditorGUILayout.EndHorizontal();

        handlerScrollPosition = EditorGUILayout.BeginScrollView(handlerScrollPosition, GUILayout.ExpandHeight(true));
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Criteria");
        EditorGUILayout.EndHorizontal();

        int? removeCriterionIndex = null;
        for (int criterionIndex = 0; criterionIndex < selectedHandler.Criteria.Count; criterionIndex++)
        {
            var criterion = selectedHandler.Criteria[criterionIndex];
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            criterion.OnCustomGUI();
            if (aaEditorUtil.ColoredButton("-", Color.red, GUILayout.ExpandWidth(false)))
                removeCriterionIndex = criterionIndex;
            EditorGUILayout.EndHorizontal();
        }
        if (removeCriterionIndex != null)
            RemoveCriterion((int)removeCriterionIndex);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (aaEditorUtil.ColoredButton("+", Color.green, GUILayout.ExpandWidth(false)))
            AddCriterion();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Responses");
        EditorGUILayout.EndHorizontal();

        int? removeResponseIndex = null;
        for (int responseIndex = 0; responseIndex < selectedHandler.Responses.Count; responseIndex++)
        {
            var response = selectedHandler.Responses[responseIndex];
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            response.OnCustomGUI();
            if (aaEditorUtil.ColoredButton("-", Color.red, GUILayout.ExpandWidth(false)))
                removeResponseIndex = responseIndex;
            EditorGUILayout.EndHorizontal();
        }
        if (removeResponseIndex != null)
            RemoveResponse((int)removeResponseIndex);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Add Response");
        foreach (var typePair in ResponseTypes)
        {
            if (aaEditorUtil.ColoredButton(typePair.Key, Color.green, GUILayout.ExpandWidth(false)))
                AddResponse(typePair.Value);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void AddHandler()
    {
        aaEventHandler handler = ScriptableObject.CreateInstance<aaEventHandler>();
        selectedDatabase.Handlers.Add(handler);

        AssetDatabase.AddObjectToAsset(handler, selectedDatabase);
        Dirty();
    }

    private void RemoveHandle(int index)
    {
        // if (!EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this handler? This will lose any unsaved work", "Yup", "NO!"))
        //     return;

        var handle = selectedDatabase.Handlers[(int)index];
        DestroyImmediate(handle, true);
        selectedDatabase.Handlers.RemoveAt((int)index);
        Dirty();
    }

    private void AddCriterion()
    {
        aaCriterion criterion = ScriptableObject.CreateInstance<aaCriterion>();
        selectedHandler.Criteria.Add(criterion);

        AssetDatabase.AddObjectToAsset(criterion, selectedHandler);
        Dirty();
    }

    private void RemoveCriterion(int index)
    {
        // if (!EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this criterion", "Yup", "NO!"))
        //     return;

        var criteria = selectedHandler.Criteria[(int)index];
        DestroyImmediate(criteria, true);
        selectedHandler.Criteria.RemoveAt((int)index);
        Dirty();
    }

    private void AddResponse(Type type)
    {
        aaResponse response = (aaResponse)ScriptableObject.CreateInstance(type);
        selectedHandler.Responses.Add(response);

        AssetDatabase.AddObjectToAsset(response, selectedHandler);
        Dirty();
    }

    private void RemoveResponse(int index)
    {
        // if (!EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this response", "Yup", "NO!"))
        //     return;

        var response = selectedHandler.Responses[(int)index];
        DestroyImmediate(response, true);
        selectedHandler.Responses.RemoveAt((int)index);
        Dirty();
    }

    private void Dirty()
    {
        EditorUtility.SetDirty(selectedDatabase);
        AssetDatabase.Refresh();
        // AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(db));
    }

    private void Save()
    {
        EditorUtility.SetDirty(selectedDatabase);
        AssetDatabase.SaveAssets();
    }

    private void Defocus()
    {
        GUIUtility.hotControl = 0;
        GUIUtility.keyboardControl = 0;
    }
}