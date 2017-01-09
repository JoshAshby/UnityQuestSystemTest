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

    private static OldDialogueEditor _editor;
    public static OldDialogueEditor editor { get { AssureEditor(); return _editor; } }
    public static void AssureEditor() { if (_editor == null) OpenEditor(); }

    [MenuItem("Window/Old Dialogue Editor")]
    public static OldDialogueEditor OpenEditor()
    {
        _editor = EditorWindow.GetWindow<OldDialogueEditor>();

        _editor.titleContent = new GUIContent("Dialogue Editor");

        return _editor;
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

    private string RenameField(string val, Action<string> callback)
    {
        EditorGUI.BeginChangeCheck();
        string newVal = EditorGUILayout.DelayedTextField(val);
        if (EditorGUI.EndChangeCheck())
            callback(newVal);

        return newVal;
    }

    private string ChoiceSelector(string[] options, string selected, Action<string> callback)
    {
        int choiceIndex = Array.IndexOf(options, selected);
        if (choiceIndex == -1)
            choiceIndex = 0;

        EditorGUI.BeginChangeCheck();
        choiceIndex = EditorGUILayout.Popup(choiceIndex, options, EditorStyles.toolbarPopup);
        if (EditorGUI.EndChangeCheck())
        {
            string newSelection = options[choiceIndex];
            callback(newSelection);
            return newSelection;
        }

        return selected;
    }

    private Dictionary<string, string> nodeTypeChoice = new Dictionary<string, string>();
    private string TypeField(List<Type> options, string buttonText, Action<string> callback)
    {
        if(!options.Any())
            return null;

        string[] optionNames = options.Select(x => x.Name).OrderBy(x => x).ToArray();
        if(!nodeTypeChoice.ContainsKey(buttonText))
            nodeTypeChoice.Add(buttonText, optionNames.First());

        ChoiceSelector(
            optionNames,
            nodeTypeChoice[buttonText],
            (newTypeName) => { nodeTypeChoice[buttonText] = newTypeName; }
        );

        if (GUILayout.Button(buttonText, EditorStyles.toolbarButton))
            callback(nodeTypeChoice[buttonText]);

        return nodeTypeChoice[buttonText];
    }

    private void OnGUI()
    {
        EnsureDatabase();
        selectedStyle = new GUIStyle(GUI.skin.button);
        selectedStyle.normal.textColor = Color.blue;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Add Dialogue", EditorStyles.toolbarButton))
            database.AddDialogue();

        if (GUILayout.Button("Save Database", EditorStyles.toolbarButton))
            database.Save(path);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Reload Database", EditorStyles.toolbarButton))
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

        List<IDialogue> dialogues = database.Dialogues.Values.OrderBy(x => x.ID).ToList();
        foreach (IDialogue dialogue in dialogues)
        {
            GUIStyle style = dialogue == currentDialogue ? selectedStyle : new GUIStyle(GUI.skin.button);

            if (GUILayout.Button(dialogue.ID, style))
                SelectDialogue(dialogue);
        }

        GUILayout.EndScrollView();
    }

    private string[] currentNodeIDs;
    private void ShowDialogue()
    {
        if (currentDialogue == null)
            return;

        currentNodeIDs = currentDialogue.Nodes.Keys.OrderBy(x => x).ToArray();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        RenameField(
            currentDialogue.ID,
            (newID) => { database.RenameDialogue(currentDialogue.ID, newID); }
        );

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Delete Dialogue"))
            DeleteDialogue();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        TypeField(
            nodeTypes,
            "Add Node",
            (newTypeName) => { currentDialogue.AddNode(nodeTypes.Find(type => type.Name == newTypeName)); }
        );

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        ListNodes();
        ShowNode();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void FirstNode()
    {
        GUILayout.Label("First Node ID");

        ChoiceSelector(
            currentNodeIDs,
            currentDialogue.FirstNodeID,
            (newID) => { currentDialogue.FirstNodeID = newID; }
        );
    }

    private Vector2 nodeScrollPosition;
    private void ListNodes()
    {
        if (currentDialogue == null || currentDialogue.Nodes == null)
            return;

        nodeScrollPosition = GUILayout.BeginScrollView(nodeScrollPosition, GUILayout.Width(200), GUILayout.ExpandHeight(true));

        FirstNode();

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

        RenameField(
            currentNode.ID,
            (newID) => { currentDialogue.RenameNode(currentNode.ID, newID); }
        );

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Delete Node"))
            DeleteNode();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.Label("Custom Node Fields");
        FieldEditor.DeclaredFieldsEditor(currentNode);

        GUILayout.Space(20);
        BranchesEditor();
        ChainEditor();

        GUILayout.Space(20);
        MetadataEditor();

        GUILayout.EndVertical();
    }

    private void ChainEditor()
    {
        if (!(currentNode is INextNode))
            return;

        INextNode node = (INextNode)currentNode;

        GUILayout.BeginHorizontal();

        GUILayout.Label("Next Node ID");
        ChoiceSelector(
            currentNodeIDs,
            node.NextNodeID,
            (newID) => { node.NextNodeID = newID; }
        );

        GUILayout.EndHorizontal();
    }

    private bool branchShow = false;
    private void BranchesEditor()
    {
        if (!(currentNode is IBranchedNode))
            return;

        IBranchedNode node = (IBranchedNode)currentNode;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        branchShow = EditorGUILayout.Foldout(branchShow, "Branches");
        if (branchShow)
        {
            TypeField(
                branchTypes,
                "Add Branch",
                (newTypeName) => { node.AddBranch(branchTypes.Find(type => type.Name == newTypeName)); }
            );
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

            ChoiceSelector(
                currentNodeIDs,
                branch.NextNodeID,
                (newID) => { branch.NextNodeID = newID; }
            );

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
    private void MetadataEditor()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        metadataShow = EditorGUILayout.Foldout(metadataShow, "Metadata");
        if (metadataShow)
        {
            TypeField(
                metadataTypes,
                "Add Metadata",
                (newTypeName) => { currentNode.AddMetadata(metadataTypes.Find(type => type.Name == newTypeName)); }
            );
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