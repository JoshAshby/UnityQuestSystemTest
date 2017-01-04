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

    private Dialogue currentDialogue;
    private ANode currentNode;

    private GUIStyle selectedStyle;

    [MenuItem("Window/Old Dialogue Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(OldDialogueEditor));
    }

    private static List<Type> AllTypes<T>()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
            .OrderBy(x => x.Name)
            .ToList();
    }

    List<Type> metadataTypes = AllTypes<IMetadata>();
    List<Type> branchTypes   = AllTypes<IBranch>();
    List<Type> nodeTypes     = AllTypes<ANode>();

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

    private void SelectNode(ANode node)
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

        foreach (ANode node in currentDialogue.Nodes.Values)
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
        GUILayout.Label("Custom Node Fields");
        FieldEditor.DeclaredFieldsEditor(currentNode);

        GUILayout.Space(20);
        List<ANode> nodeList = currentDialogue.Nodes.Values.OrderBy(x => x.ID).ToList();
        BranchesEditor(nodeList);
        ChainEditor(nodeList);

        GUILayout.Space(20);
        MetadataEditor();

        GUILayout.EndVertical();
    }

    private void ChainEditor(List<ANode> options)
    {
        if(!(currentNode is AChainedNode))
            return;
        
        AChainedNode node = (currentNode as AChainedNode);

        GUILayout.Label("Next Node ID");

        int choiceIdx = currentDialogue.XmlNodes.ToList().FindIndex(x => x.ID == node.NextNodeID);
        if (choiceIdx == -1)
            choiceIdx = 0;
        choiceIdx = EditorGUILayout.Popup(choiceIdx, currentDialogue.Nodes.Keys.ToArray());
        node.NextNodeID = options[choiceIdx].ID;
    }

    private bool branchShow = false;
    private int branchChoiceIdx = 0;
    private void BranchesEditor(List<ANode> options)
    {
        if (!(currentNode is ABranchedNode))
            return;

        ABranchedNode node = (currentNode as ABranchedNode);

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

            int choiceIdx = currentDialogue.XmlNodes.ToList().FindIndex(x => x.ID == branch.NextNodeID);
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