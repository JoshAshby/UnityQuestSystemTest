using UnityEditor;
using UnityEngine;
using System.IO;
using Ashogue;

public class DialogueEditor : EditorWindow
{
    private DialogueContainer database;
    private string path;

    [MenuItem("Window/Dialogue Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueEditor));
    }

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, DialogueController.DatabaseLocation);

        bool exists = File.Exists(path);

        Debug.Assert(exists, "Dialogue database doesn't exist, creating");

        if(exists) {
            database = DialogueContainer.Load(path);
        } else {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Ashogue"));

            database = new DialogueContainer();
            database.Save(path);
        }
    }

    private void OnGUI()
    {}
}