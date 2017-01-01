using UnityEditor;
using UnityEngine;
using System.IO;
using Ashogue;

public class DialogueEditor : EditorWindow
{
    private DialogueContainer database;
    private string path;

    private Vector2 dialogueScrollPosition;
    private Vector2 nodeScrollPosition;

    private Dialogue currentDialogue;
    private INode currentNode;

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
        {
            LoadDatabase();
        }
        else
        {
            CreateDatabase();
        }
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
    }

    private void OnGUI()
    {
        EnsureDatabase();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("+"))
            database.Dialogues.Add(new Dialogue("Untitled Dialogue"));

        if (GUILayout.Button("Save"))
            database.Save(path);

        if (GUILayout.Button("Reload"))
            LoadDatabase();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        ListDialogues();
        ShowDialogue();
        GUILayout.EndHorizontal();
    }

    private void ListDialogues()
    {
        dialogueScrollPosition = GUILayout.BeginScrollView(dialogueScrollPosition, GUILayout.Width(200), GUILayout.ExpandHeight(true));

        foreach (Dialogue dialogue in database.Dialogues)
        {
            if (GUILayout.Button(dialogue.ID))
                currentDialogue = dialogue;
        }

        GUILayout.EndScrollView();
    }

    private void ShowDialogue()
    {
        ListNodes();
    }

    private void ListNodes()
    {
        nodeScrollPosition = GUILayout.BeginScrollView(nodeScrollPosition, GUILayout.Width(200), GUILayout.ExpandHeight(true));

        foreach (INode node in currentDialogue.Nodes)
        {
            if (GUILayout.Button(node.ID))
                currentNode = node;
        }

        GUILayout.EndScrollView();
    }

    private void ShowNode()
    { }
}