using UnityEditor;
using UnityEngine;

public class aaDefaultBlackboardEditorWindow : EditorWindow
{
    [MenuItem("Databases/Default Blackboards")]
    public static void ShowWindow()
    {
        GetWindow<aaDefaultBlackboardEditorWindow>(false, "Default Blackboards", true);
    }

    private bool initd = false;

    private aaDefaultBlackboard[] databases;

    private void Init()
    {
        if (initd)
            return;

        databases = Resources.FindObjectsOfTypeAll<aaDefaultBlackboard>();
    }

    private void OnGUI()
    {
        Init();
    }
}