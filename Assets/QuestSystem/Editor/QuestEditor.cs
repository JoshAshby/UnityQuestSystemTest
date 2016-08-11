using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using QuestSystem;

[CustomEditor (typeof (Quest))]
[CanEditMultipleObjects]
public class QuestEditor : Editor {
	Quest quest;

	bool show_position = true;

	void  OnEnable () {
		quest = target as Quest;
	}

	public override void OnInspectorGUI () {
		EditorGUI.BeginChangeCheck ();

		quest.title = EditorGUILayout.TextField ("Title", quest.title);
		quest.description = EditorGUILayout.TextField ("Description", quest.description);

		int step_count = quest.steps.Count;

		List<QuestStep> to_delete = new List<QuestStep> ();
		show_position = EditorGUILayout.Foldout (show_position, System.String.Format ("Steps ({0})", step_count));

		if (show_position) {
			EditorGUI.indentLevel++;
			List<QuestStep> steps = quest.Steps;

			if (steps.Count > 0) {
				foreach (QuestStep step in steps) {
					if (GUILayout.Button ("Delete Step", GUILayout.ExpandWidth (false))) {
						to_delete.Add (step);
						GUI.changed = true;
					}

					Editor editor = Editor.CreateEditor (step);
					if (editor != null)
						editor.OnInspectorGUI ();
				}
			} else {
				GUILayout.Label ("This Quest has no Steps yet.");
			}

			if (GUILayout.Button ("Add Step", GUILayout.ExpandWidth (false))) {
				AddStep ();
			}

			EditorGUI.indentLevel--;
		}

		if (EditorGUI.EndChangeCheck ()) {
			foreach (QuestStep step in to_delete) {
				DeleteStep (step);
			}

			to_delete.Clear ();

			EditorUtility.SetDirty (quest);
		}
	}

	void AddStep () {
		string object_path = EditorPrefs.GetString ("ObjectPath");
		string dir_path = System.String.Format ("{0}/Steps", Path.GetDirectoryName (object_path));

		if (dir_path.StartsWith (Application.dataPath))
			dir_path = dir_path.Substring (Application.dataPath.Length - "Assets".Length);

		if (!Directory.Exists (dir_path))
			Directory.CreateDirectory (dir_path);

		QuestStep new_step = ScriptableObject.CreateInstance<QuestStep> ();

		quest.Steps.Add (new_step);

		string step_path = System.String.Format ("{0}/Step {1}.asset", dir_path, quest.Steps.Count);

		AssetDatabase.CreateAsset (new_step, step_path);
		AssetDatabase.SaveAssets ();
	}

	void DeleteStep (QuestStep step) {
		string path = AssetDatabase.GetAssetPath (step);
		AssetDatabase.DeleteAsset (path);

		quest.Steps.Remove (step);
	}
}