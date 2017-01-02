using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using Ashogue;

// AppDomain.CurrentDomain.GetAssemblies()
//                        .SelectMany(assembly => assembly.GetTypes())
//                        .Where(type => type.IsSubclassOf(typeof(Foo)));
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

public class DialogueEditor : EditorWindow
{
    private DialogueContainer database;
    private string path;

    private Vector2 dialogueScrollPosition;
    private Vector2 nodeScrollPosition;

    private Dialogue currentDialogue;
    private INode currentNode;

    private GUIStyle selectedStyle;

    [MenuItem("Window/Dialogue Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueEditor));
    }

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
        // Should find a better way to do this since this is a bit messy right now
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

    private void AddDialogue(string ID = "Untitled")
    {
        database.Dialogues.Add(new Dialogue(ID));
    }

    private void AddNode(string ID = "Untitled")
    {
        currentDialogue.Nodes.Add(new TextNode());
    }

    private void SelectDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        currentNode = null;
    }

    private void SelectNode(INode node)
    {
        currentNode = node;
    }

    private void ReloadDatabase()
    {
        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to reload the database? This will lose any unsaved work", "Yup", "NO!"))
        {
            LoadDatabase();
        }
    }

    private void DeleteDialogue()
    {
        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this dialogue?", "Yup", "NO!"))
        {
            database.Dialogues.Remove(currentDialogue);
            currentDialogue = null;
            currentNode = null;
        }
    }

    private void DeleteNode()
    {
        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this node?", "Yup", "NO!"))
        {
            currentDialogue.Nodes.Remove(currentNode);
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
            AddDialogue();

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

    private void ListDialogues()
    {
        dialogueScrollPosition = GUILayout.BeginScrollView(dialogueScrollPosition, GUILayout.Width(200), GUILayout.ExpandHeight(true));

        foreach (Dialogue dialogue in database.Dialogues)
        {
            bool button = false;

            if (dialogue == currentDialogue)
                button = GUILayout.Button(dialogue.ID, selectedStyle);
            else
                button = GUILayout.Button(dialogue.ID);

            if (button)
                SelectDialogue(dialogue);
        }

        GUILayout.EndScrollView();
    }

    private void ShowDialogue()
    {
        if (currentDialogue == null)
            return;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        string newID = EditorGUILayout.DelayedTextField(currentDialogue.ID);
        if (EditorGUI.EndChangeCheck())
            currentDialogue.ID = newID;

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Delete Dialogue"))
            DeleteDialogue();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Node"))
            AddNode();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        ListNodes();
        ShowNode();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void ListNodes()
    {
        if (currentDialogue == null || currentDialogue.Nodes == null)
            return;

        nodeScrollPosition = GUILayout.BeginScrollView(nodeScrollPosition, GUILayout.Width(200), GUILayout.ExpandHeight(true));

        foreach (INode node in currentDialogue.Nodes)
        {
            bool button = false;

            if (node == currentNode)
                button = GUILayout.Button(node.ID, selectedStyle);
            else
                button = GUILayout.Button(node.ID);

            if (button)
                SelectNode(node);
        }

        GUILayout.EndScrollView();
    }

    private bool choiceShow = false;
    private void ChoiceEditor()
    {
        if (!(currentNode is IChoiceNode))
            return;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        choiceShow = EditorGUILayout.Foldout(choiceShow, "Choices");
        if(choiceShow)
            if (GUILayout.Button("Add Choice"))
                (currentNode as IChoiceNode).Choices.Add(new Choice());
        GUILayout.EndHorizontal();

        if (choiceShow)
        {
            EditorGUI.indentLevel++;

            IChoice removalChoice = null;

            foreach (IChoice choice in (currentNode as IChoiceNode).Choices)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Displayed Text");
                choice.Text = EditorGUILayout.DelayedTextField(choice.Text);
                GUILayout.Label("Next Node ID");

                int choiceIdx = currentDialogue.Nodes.FindIndex(x => x.ID == choice.NextNodeID);
                if(choiceIdx == -1)
                    choiceIdx = 0;
                choiceIdx = EditorGUILayout.Popup(choiceIdx, currentDialogue.Nodes.Select(x => x.ID).ToArray());
                choice.NextNodeID = currentDialogue.Nodes[choiceIdx].ID;

                if (GUILayout.Button("X"))
                    if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove this choice?", "Yup", "NO!"))
                        removalChoice = choice;
                EditorGUILayout.EndHorizontal();
            }

            if (removalChoice != null)
                (currentNode as IChoiceNode).Choices.Remove(removalChoice);

            EditorGUI.indentLevel--;
        }

        GUILayout.EndVertical();
    }

    private bool metadataShow = false;
    private void MetadataEditor()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        metadataShow = EditorGUILayout.Foldout(metadataShow, "Metadata");
        if(metadataShow)
            if (GUILayout.Button("Add Metadata"))
                currentNode.Metadata.Add(new StringMetadata());
        GUILayout.EndHorizontal();

        if (metadataShow)
        {
            EditorGUI.indentLevel++;

            IMetadata removalMetadata = null;

            foreach (IMetadata metadata in currentNode.Metadata)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Key");
                metadata.ID = EditorGUILayout.DelayedTextField(metadata.ID);
                GUILayout.Label("Value");
                (metadata as StringMetadata).Value = EditorGUILayout.DelayedTextField((metadata as StringMetadata).Value);
                if (GUILayout.Button("X"))
                    if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove this metadata?", "Yup", "NO!"))
                        removalMetadata = metadata;
                EditorGUILayout.EndHorizontal();
            }

            if (removalMetadata != null)
                currentNode.Metadata.Remove(removalMetadata);

            EditorGUI.indentLevel--;
        }

        GUILayout.EndVertical();
    }

    private void ShowNode()
    {
        if (currentNode == null)
            return;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        currentNode.ID = EditorGUILayout.DelayedTextField(currentNode.ID);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Delete Node"))
            DeleteDialogue();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        ChoiceEditor();

        GUILayout.Space(20);
        MetadataEditor();

        GUILayout.EndVertical();
    }
}