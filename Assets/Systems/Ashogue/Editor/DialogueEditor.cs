using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using Ashogue;
using Ashogue.Extensions;

// AppDomain.CurrentDomain.GetAssemblies()
//                        .SelectMany(assembly => assembly.GetTypes())
//                        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Foo)));
public static class ReflectiveEnumerator
{
    static ReflectiveEnumerator() { }

    public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
    {
        List<T> objects = new List<T>();
        foreach (Type type in
            Assembly.GetAssembly(typeof(T)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
        {
            objects.Add((T)Activator.CreateInstance(type, constructorArgs));
        }
        objects.Sort();
        return objects;
    }
}

public class OldDialogueEditor : EditorWindow
{
    private DialogueContainer database;
    private string path;

    private Dialogue currentDialogue;
    private INode currentNode;

    private GUIStyle selectedStyle;

    [MenuItem("Window/Old Dialogue Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(OldDialogueEditor));
    }

    List<Type> metadataTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(IMetadata)))
        .OrderBy(x => x.Name)
        .ToList();

    List<Type> choiceTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(IChoice)))
        .OrderBy(x => x.Name)
        .ToList();

    List<Type> nodeTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(INode)))
        .OrderBy(x => x.Name)
        .ToList();

    private void Awake()
    {
        EnsureDatabase();
    }

    private void EnsureDatabase()
    {
        if (database != null)
            return;

        path = Path.Combine(Application.dataPath, DialogueController.DatabaseLocation);

        if (File.Exists(path))
            LoadDatabase();
        else
            CreateDatabase();
    }

    private void CreateDatabase()
    {
        Debug.Log("Dialogue database doesn't exist, creating");
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        database = new DialogueContainer();
        database.Save(path);
    }

    private void LoadDatabase()
    {
        database = DialogueContainer.Load(path);
        currentNode = null;
        currentDialogue = null;
    }

    private void ReloadDatabase()
    {
        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to reload the database? This will lose any unsaved work", "Yup", "NO!"))
        {
            LoadDatabase();
        }
    }

    private void SelectDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        currentNode = null;
    }

    private void DeleteDialogue()
    {
        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this dialogue?", "Yup", "NO!"))
        {
            database.RemoveDialogue(currentDialogue.ID);
            currentDialogue = null;
            currentNode = null;
        }
    }

    private void SelectNode(INode node)
    {
        currentNode = node;
    }

    private void DeleteNode()
    {
        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this node?", "Yup", "NO!"))
        {
            currentDialogue.RemoveNode(currentNode.ID);
            currentNode = null;
        }
    }

    private void OnGUI()
    {
        EnsureDatabase();
        selectedStyle = new GUIStyle(GUI.skin.button);
        selectedStyle.normal.textColor = Color.blue;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Dialogue"))
            database.AddDialogue();

        if (GUILayout.Button("Save Database"))
            database.Save(path);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Reload Database"))
            ReloadDatabase();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        ListDialogues();
        ShowDialogue();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private Vector2 dialogueScrollPosition;
    private void ListDialogues()
    {
        dialogueScrollPosition = GUILayout.BeginScrollView(dialogueScrollPosition, GUILayout.Width(200), GUILayout.ExpandHeight(true));

        foreach (Dialogue dialogue in database.Dialogues.Values)
        {
            GUIStyle style = dialogue == currentDialogue ? selectedStyle : new GUIStyle(GUI.skin.button);

            if (GUILayout.Button(dialogue.ID, style))
                SelectDialogue(dialogue);
        }

        GUILayout.EndScrollView();
    }

    private int nodeChoiceIdx = 0;
    private void ShowDialogue()
    {
        if (currentDialogue == null)
            return;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        string newID = EditorGUILayout.DelayedTextField(currentDialogue.ID);
        if (EditorGUI.EndChangeCheck())
            database.RenameDialogue(currentDialogue.ID, newID);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Delete Dialogue"))
            DeleteDialogue();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        nodeChoiceIdx = EditorGUILayout.Popup(nodeChoiceIdx, nodeTypes.Select(x => x.Name).ToArray());
        if (GUILayout.Button("Add Node"))
            currentDialogue.AddNode(nodeTypes[nodeChoiceIdx]);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        ListNodes();
        ShowNode();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private Vector2 nodeScrollPosition;
    private void ListNodes()
    {
        if (currentDialogue == null || currentDialogue.Nodes == null)
            return;

        nodeScrollPosition = GUILayout.BeginScrollView(nodeScrollPosition, GUILayout.Width(200), GUILayout.ExpandHeight(true));

        foreach (INode node in currentDialogue.Nodes.Values)
        {
            GUIStyle style = node == currentNode ? selectedStyle : new GUIStyle(GUI.skin.button);
            if (GUILayout.Button(node.ID, style))
                SelectNode(node);
        }

        GUILayout.EndScrollView();
    }

    private void ShowNode()
    {
        if (currentNode == null)
            return;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        string newID = EditorGUILayout.DelayedTextField(currentNode.ID);
        if (EditorGUI.EndChangeCheck())
            currentDialogue.RenameNode(currentNode.ID, newID);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Delete Node"))
            DeleteNode();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        ChoiceEditor();

        GUILayout.Space(20);
        MetadataEditor();

        GUILayout.Space(20);
        GUILayout.Label("Custom Node Fields");
        FieldEditor.DeclaredFieldsEditor(currentNode);

        GUILayout.EndVertical();
    }

    private bool choiceShow = false;
    private int choiceChoiceIdx = 0;
    private void ChoiceEditor()
    {
        if (!(currentNode is IChoiceNode))
            return;

        IChoiceNode node = (currentNode as IChoiceNode);

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        choiceShow = EditorGUILayout.Foldout(choiceShow, "Choices");
        if (choiceShow) {
            choiceChoiceIdx = EditorGUILayout.Popup(choiceChoiceIdx, choiceTypes.Select(x => x.Name).ToArray());
            if (GUILayout.Button("Add Choice"))
                node.AddChoice(choiceTypes[choiceChoiceIdx]);
        }
        GUILayout.EndHorizontal();

        if (!choiceShow)
        {
            GUILayout.EndVertical();
            return;
        }

        EditorGUI.indentLevel++;

        IChoice removalChoice = null;
        List<INode> nodeList = new List<INode>(currentDialogue.Nodes.Values)
            .OrderBy(x => x.ID)
            .ToList();

        foreach (IChoice choice in node.Choices.Values)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Displayed Text");
            choice.Text = EditorGUILayout.DelayedTextField(choice.Text);
            GUILayout.Label("Next Node ID");

            int choiceIdx = nodeList.FindIndex(x => x.ID == choice.NextNodeID);
            if (choiceIdx == -1)
                choiceIdx = 0;
            choiceIdx = EditorGUILayout.Popup(choiceIdx, new List<string>(currentDialogue.Nodes.Keys).ToArray());
            choice.NextNodeID = nodeList[choiceIdx].ID;

            if (GUILayout.Button("X"))
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove this choice?", "Yup", "NO!"))
                    removalChoice = choice;
            EditorGUILayout.EndHorizontal();

            FieldEditor.DeclaredFieldsEditor(choice);

            EditorGUILayout.EndVertical();
        }

        if (removalChoice != null)
            node.RemoveChoice(removalChoice.ID);

        EditorGUI.indentLevel--;

        GUILayout.EndVertical();
    }

    private bool metadataShow = false;
    private int metadataChoiceIdx = 0;
    private void MetadataEditor()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        metadataShow = EditorGUILayout.Foldout(metadataShow, "Metadata");
        if (metadataShow) {
            metadataChoiceIdx = EditorGUILayout.Popup(metadataChoiceIdx, metadataTypes.Select(x => x.Name).ToArray());
            if (GUILayout.Button("Add Choice"))
                currentNode.AddMetadata(metadataTypes[metadataChoiceIdx]);
        }
        GUILayout.EndHorizontal();

        if (!metadataShow)
        {
            GUILayout.EndVertical();
            return;
        }

        EditorGUI.indentLevel++;

        IMetadata removalMetadata = null;

        foreach (IMetadata metadata in currentNode.Metadata.Values)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Key");
            metadata.ID = EditorGUILayout.DelayedTextField(metadata.ID);

            FieldEditor.DeclaredFieldsEditor(metadata);

            if (GUILayout.Button("X"))
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove this metadata?", "Yup", "NO!"))
                    removalMetadata = metadata;
            EditorGUILayout.EndHorizontal();
        }

        if (removalMetadata != null)
            currentNode.RemoveMetadata(removalMetadata.ID);

        EditorGUI.indentLevel--;

        GUILayout.EndVertical();
    }
}

static class FieldEditor
{
    private static Dictionary<Type, Action<object, FieldInfo>> typeLookup = new Dictionary<Type, Action<object, FieldInfo>> {
        { typeof(bool),   BoolField },
        { typeof(float),  FloatField },
        { typeof(string), StringField }
    };

    public static void DeclaredFieldsEditor(object obj)
    {
        List<FieldInfo> fields = obj.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).ToList();
        if (!fields.Any())
            return;

        GUILayout.BeginVertical();
        foreach (FieldInfo field in fields)
        {
            GUILayout.BeginHorizontal();
            typeLookup[field.FieldType](obj, field);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private static void BoolField(object obj, FieldInfo field)
    {
        bool val = (bool)field.GetValue(obj);
        val = GUILayout.Toggle(val, field.Name);
        field.SetValue(obj, val);
    }

    private static void FloatField(object obj, FieldInfo field)
    {
        GUILayout.Label(field.Name);
        float val = (float)field.GetValue(obj);
        val = EditorGUILayout.DelayedFloatField(val);
        field.SetValue(obj, val);
    }

    private static void StringField(object obj, FieldInfo field)
    {
        GUILayout.Label(field.Name);
        string val = (string)field.GetValue(obj);
        val = EditorGUILayout.DelayedTextField(val);
        field.SetValue(obj, val);
    }
}