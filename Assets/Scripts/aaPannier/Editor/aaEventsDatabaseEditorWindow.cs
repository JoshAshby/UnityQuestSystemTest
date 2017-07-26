using System.Collections.Generic;
using System.Linq;
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

    private void Init()
    {
        if (initd)
            return;

        databases = Resources.FindObjectsOfTypeAll<aaEventsDatabase>();
    }

    private float toolbarHeight = 0;
    private int sideWindowWidth = 300;
    private Vector2 scrollPosition;

    private void OnGUI()
    {
        Init();

        EditorGUILayout.BeginHorizontal();
        selectedDatabaseIndex = EditorGUILayout.Popup(selectedDatabaseIndex, databases.ToList().Select(x => x.Name).ToArray());
        EditorGUILayout.EndHorizontal();

        if (Event.current.type == EventType.Repaint)
            toolbarHeight = GUILayoutUtility.GetLastRect().yMax + 6;

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

        if (selectedDatabase != null)
            DrawDB();
    }

    private void DrawDB()
    {
        Rect sideWindowRect = new Rect(3, toolbarHeight, sideWindowWidth, position.height - toolbarHeight - 3);
        GUILayout.BeginArea(sideWindowRect, GUI.skin.box);

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.FlexibleSpace();
        if (aaEditorUtil.ColoredButton("+", Color.green, EditorStyles.toolbarButton))
            AddHandler();
        EditorGUILayout.EndHorizontal();

        int? removeHandleIndex = null;
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
        for (int handleIndex = 0; handleIndex < selectedDatabase.Handlers.Count; handleIndex++)
        {
            EditorGUILayout.BeginHorizontal();
            var handle = selectedDatabase.Handlers[handleIndex];
            Color color = handleIndex == selectedHandleIndex ? Color.grey : GUI.color;

            if (aaEditorUtil.ColoredButton($"{handle.EventName} ({handle.Padding})", color, EditorStyles.miniButtonLeft))
                selectedHandleIndex = handleIndex;

            if (aaEditorUtil.ColoredButton("-", Color.red, EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
                removeHandleIndex = handleIndex;

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (removeHandleIndex != null)
            RemoveHandle((int)removeHandleIndex);

        GUILayout.EndArea();

        Rect canvasRect = new Rect(sideWindowWidth + 3, toolbarHeight, position.width - sideWindowWidth - 3, position.height - toolbarHeight - 3);
        GUILayout.BeginArea(canvasRect);

        if (selectedHandler != null)
            DrawHandler();

        GUILayout.EndArea();
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
        if (!EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this handler? This will lose any unsaved work", "Yup", "NO!"))
            return;

        var handle = selectedDatabase.Handlers[(int)index];
        DestroyImmediate(handle, true);
        selectedDatabase.Handlers.RemoveAt((int)index);
        Dirty();
    }

    private void DrawHandler()
    {

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

    // class DatabaseDetail
    // {
    //     public aaEventsDatabase asset;

    //     public DatabaseDetail(aaEventsDatabase database)
    //     {
    //         this.asset = database;
    //     }

    //     public void Draw()
    //     {
    //         asset.Name = EditorGUILayout.TextField(asset.Name);

    //         GUILayout.Button("Add Handler");

    //         for (int handleIndex = 0; handleIndex < asset.Handlers.Count; handleIndex++)
    //         {
    //             var handle = asset.Handlers[handleIndex];
    //         }
    //     }
    // }

    // private class CriterionDetail
    // {
    //     public aaCriterion asset;

    //     public CriterionDetail(aaCriterion criterion)
    //     {
    //         this.asset = criterion;
    //     }

    //     public void Draw()
    //     {
    //         EditorGUILayout.BeginHorizontal();
    //         asset.OnCustomGUI();
    //         EditorGUILayout.EndHorizontal();
    //     }
    // }

    // private class ResponseDetail
    // {
    //     public aaResponse asset;

    //     public ResponseDetail(aaResponse response)
    //     {
    //         this.asset = response;
    //     }

    //     public void Draw()
    //     {
    //         EditorGUILayout.BeginHorizontal();
    //         asset.OnCustomGUI();
    //         EditorGUILayout.EndHorizontal();
    //     }
    // }

    // private class HandleDetail
    // {
    //     public aaEventHandler asset;

    //     public HandleDetail(aaEventHandler handle)
    //     {
    //         this.asset = handle;
    //     }

    //     public void DrawHandle()
    //     {
    //         EditorGUILayout.BeginVertical();

    //         EditorGUILayout.BeginHorizontal();

    //         asset.EventName = EditorGUILayout.TextField(asset.EventName);
    //         asset.Padding = EditorGUILayout.IntField(asset.Padding);

    //         bool shouldRemove = aaEditorUtil.RemoveButton("-");

    //         EditorGUILayout.EndHorizontal();

    //         DrawCriteria();
    //         DrawResponses();

    //         EditorGUILayout.EndVertical();
    //     }

    //     private void DrawCriteria()
    //     {
    //     }

    //     private void DrawResponses()
    //     {
    //     }
    // }
}