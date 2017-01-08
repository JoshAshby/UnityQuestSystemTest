using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using Ashogue;
using Ashogue.Data;
using Ashogue.Extensions;

public class OldDialogueEditor : EditorWindow
{
    private DialogueContainer database;
    private string path;

    private IDialogue currentDialogue;
    private INode currentNode;

    private GUIStyle selectedStyle;

    [MenuItem("Window/Old Dialogue Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(OldDialogueEditor));
    }

    private static List<Type> AllSubTypes<T>()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
            .OrderBy(x => x.Name)
            .ToList();
    }

    private static List<Type> AllImplementTypes<T>()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(T)))
            .OrderBy(x => x.Name)
            .ToList();
    }

    List<Type> metadataTypes = AllImplementTypes<IMetadata>();
    List<Type> branchTypes = AllImplementTypes<IBranch>();
    List<Type> nodeTypes = AllImplementTypes<INode>();

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

    private void SelectDialogue(IDialogue dialogue)
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

        foreach (IDialogue dialogue in database.Dialogues.Values)
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

        List<INode> nodeList = currentDialogue.Nodes.Values.OrderBy(x => x.ID).ToList();

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

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        ListNodes(nodeList);
        ShowNode(nodeList);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void FirstNode(List<INode> options)
    {
        GUILayout.Label("First Node ID");

        int choiceIdx = currentDialogue.Nodes.Values.ToList().FindIndex(x => x.ID == currentDialogue.FirstNodeID);
        if (choiceIdx == -1)
            choiceIdx = 0;
        choiceIdx = EditorGUILayout.Popup(choiceIdx, currentDialogue.Nodes.Keys.ToArray());
        currentDialogue.FirstNodeID = options[choiceIdx].ID;
    }

    private Vector2 nodeScrollPosition;
    private void ListNodes(List<INode> options)
    {
        if (currentDialogue == null || currentDialogue.Nodes == null)
            return;

        nodeScrollPosition = GUILayout.BeginScrollView(nodeScrollPosition, GUILayout.Width(200), GUILayout.ExpandHeight(true));

        FirstNode(options);

        foreach (INode node in currentDialogue.Nodes.Values)
        {
            GUIStyle style = node == currentNode ? selectedStyle : new GUIStyle(GUI.skin.button);

            if (GUILayout.Button(node.ID, style))
                SelectNode(node);
        }

        GUILayout.EndScrollView();
    }

    private void ShowNode(List<INode> options)
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
        GUILayout.Label("Custom Node Fields");
        FieldEditor.DeclaredFieldsEditor(currentNode);

        GUILayout.Space(20);
        BranchesEditor(options);
        ChainEditor(options);

        GUILayout.Space(20);
        MetadataEditor();

        GUILayout.EndVertical();
    }

    private void ChainEditor(List<INode> options)
    {
        if (!(currentNode is INextNode))
            return;

        INextNode node = (INextNode)currentNode;

        GUILayout.Label("Next Node ID");

        int choiceIdx = currentDialogue.Nodes.Values.ToList().FindIndex(x => x.ID == node.NextNodeID);
        if (choiceIdx == -1)
            choiceIdx = 0;
        choiceIdx = EditorGUILayout.Popup(choiceIdx, currentDialogue.Nodes.Keys.ToArray());
        node.NextNodeID = options[choiceIdx].ID;
    }

    private bool branchShow = false;
    private int branchChoiceIdx = 0;
    private void BranchesEditor(List<INode> options)
    {
        if (!(currentNode is IBranchedNode))
            return;

        IBranchedNode node = (IBranchedNode)currentNode;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        branchShow = EditorGUILayout.Foldout(branchShow, "Branches");
        if (branchShow)
        {
            branchChoiceIdx = EditorGUILayout.Popup(branchChoiceIdx, branchTypes.Select(x => x.Name).ToArray());
            if (GUILayout.Button("Add Branch"))
                node.AddBranch(branchTypes[branchChoiceIdx]);
        }
        GUILayout.EndHorizontal();

        if (!branchShow)
        {
            GUILayout.EndVertical();
            return;
        }

        EditorGUI.indentLevel++;

        IBranch removalBranch = null;

        foreach (IBranch branch in node.Branches.Values)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Displayed Text");
            branch.Text = EditorGUILayout.DelayedTextField(branch.Text);
            GUILayout.Label("Next Node ID");

            int choiceIdx = currentDialogue.Nodes.Values.ToList().FindIndex(x => x.ID == branch.NextNodeID);
            if (choiceIdx == -1)
                choiceIdx = 0;
            choiceIdx = EditorGUILayout.Popup(choiceIdx, currentDialogue.Nodes.Keys.ToArray());
            branch.NextNodeID = options[choiceIdx].ID;

            if (GUILayout.Button("X"))
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove this branch?", "Yup", "NO!"))
                    removalBranch = branch;
            EditorGUILayout.EndHorizontal();

            FieldEditor.DeclaredFieldsEditor(branch);

            EditorGUILayout.EndVertical();
        }

        if (removalBranch != null)
            node.RemoveBranch(removalBranch.ID);

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
        if (metadataShow)
        {
            metadataChoiceIdx = EditorGUILayout.Popup(metadataChoiceIdx, metadataTypes.Select(x => x.Name).ToArray());
            if (GUILayout.Button("Add Metadata"))
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

            if (metadata.Type == typeof(bool))
            {
                IMetadata<bool> handle = metadata.OfType<bool>();
                handle.Value = GUILayout.Toggle(handle.Value, "");
            }
            else if (metadata.Type == typeof(float))
            {
                IMetadata<float> handle = metadata.OfType<float>();
                handle.Value = EditorGUILayout.DelayedFloatField(handle.Value);
            }
            else if (metadata.Type == typeof(string))
            {
                IMetadata<string> handle = metadata.OfType<string>();
                handle.Value = EditorGUILayout.DelayedTextField(handle.Value);
            }

            GUILayout.FlexibleSpace();

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