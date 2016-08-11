using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using QuestSystem;

public class QuestEditWindow : EditorWindow {
	public Quest quest;

	[MenuItem ("Window/Quest Editor")]
	static void  Init () {
		QuestEditWindow window = (QuestEditWindow)EditorWindow.GetWindow (typeof (QuestEditWindow));
		window.Show ();
	}

	void  OnEnable () {
		if(EditorPrefs.HasKey ("ObjectPath")) {
			string objectPath = EditorPrefs.GetString ("ObjectPath");
			quest = AssetDatabase.LoadAssetAtPath (objectPath, typeof (Quest)) as Quest;
		}
	}

	void  OnGUI () {
		GUILayout.BeginHorizontal ();

		GUILayout.Label ("Quest Editor", EditorStyles.boldLabel);

		GUILayout.FlexibleSpace ();

		if (GUILayout.Button ("Open Quest", GUILayout.ExpandWidth (false)))
			OpenQuest();

		if (GUILayout.Button ("New Quest", GUILayout.ExpandWidth (false)))
			CreateNewQuest ();

		GUILayout.EndHorizontal ();

		if (quest != null) {
			if (GUILayout.Button ("Show Quest", GUILayout.ExpandWidth (false))) {
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = quest;
			}

			Editor editor = Editor.CreateEditor (quest);
			if (editor != null)
				editor.OnInspectorGUI ();
		}
	}

	public void OnInspectorUpdate () {
		this.Repaint ();
	}

	void CreateNewQuest () {
		string absPath = EditorUtility.OpenFolderPanel ("Select Quest Folder", "Assets/QuestSystem/Quests", "New Quest");

		Quest asset = ScriptableObject.CreateInstance<Quest> ();

		if (absPath.StartsWith (Application.dataPath))
			absPath = absPath.Substring (Application.dataPath.Length - "Assets".Length);

		AssetDatabase.CreateAsset (asset, System.String.Format ("{0}/Quest.asset", absPath));
		AssetDatabase.SaveAssets ();

		quest = asset;

		if (quest) {
			quest.Steps = new List<QuestStep> ();
			string relPath = AssetDatabase.GetAssetPath (quest);
			EditorPrefs.SetString ("ObjectPath", relPath);

			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = quest;
		}
	}

	void OpenQuest () {
		string absPath = EditorUtility.OpenFilePanel ("Select Quest", "Assets/QuestSystem/Quests/", "");

		if (absPath.StartsWith (Application.dataPath))
			absPath = absPath.Substring (Application.dataPath.Length - "Assets".Length);

		quest = AssetDatabase.LoadAssetAtPath (absPath, typeof (Quest)) as Quest;

		if (quest != null) {
			EditorPrefs.SetString ("ObjectPath", absPath);

			if (quest.Steps == null)
				quest.Steps = new List<QuestStep> ();
		}
	}
}