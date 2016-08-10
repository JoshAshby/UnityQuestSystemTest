using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using QuestSystem;

public class QuestEditor : EditorWindow {
	public Quest quest;

	private int viewIndex = 1;

	[MenuItem ("Window/Quest Editor %#e")]
	static void  Init () {
		EditorWindow.GetWindow (typeof (QuestEditor));
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

		if (quest != null) {
			if (GUILayout.Button ("Show Quest")) {
				EditorUtility.FocusProjectWindow ();
				Selection.activeObject = quest;
			}
		}

		if (GUILayout.Button ("Open Quest"))
			OpenQuest();

		if (GUILayout.Button ("New Quest"))
			CreateNewQuest ();

		GUILayout.EndHorizontal ();

		GUILayout.Space (20);

		if (quest != null)  {
			List<QuestStep> steps = quest.Steps;

			GUILayout.BeginHorizontal ();

			GUILayout.Space (10);

			if (GUILayout.Button ("Prev", GUILayout.ExpandWidth (false))) {
				if (viewIndex > 1)
					viewIndex --;
			}

			GUILayout.Space (5);

			if (GUILayout.Button ("Next", GUILayout.ExpandWidth (false))) {
				if (viewIndex < steps.Count) {
					viewIndex ++;
				}
			}

			GUILayout.Space (60);

			if (GUILayout.Button ("Add Step", GUILayout.ExpandWidth (false))) {
				AddStep ();
			}

			if (GUILayout.Button ("Delete Step", GUILayout.ExpandWidth (false))) {
				DeleteStep (viewIndex - 1);
			}

			GUILayout.EndHorizontal ();

			if (steps == null)
				Debug.Log ("wtf, steps List is empty!!!");
			
			if (steps.Count > 0) {
				GUILayout.BeginHorizontal ();

				viewIndex = Mathf.Clamp (EditorGUILayout.IntField ("Current Step", viewIndex, GUILayout.ExpandWidth (false)), 1, steps.Count);
				QuestStep step = steps [viewIndex - 1];

				EditorGUILayout.LabelField ("of   " + steps.Count.ToString () + " steps", "", GUILayout.ExpandWidth (false));
				GUILayout.EndHorizontal ();

				step.audio = EditorGUILayout.ObjectField ("Step Audio", step.audio, typeof (AudioClip), false) as AudioClip;
				step.dialogue = EditorGUILayout.TextField ("Step Dialogue", step.dialogue as string);

				GUILayout.Space (10);
			} else {
				GUILayout.Label ("This Quest has no Steps yet.");
			}
		}

		if (GUI.changed)
			EditorUtility.SetDirty (quest);
	}

	void CreateNewQuest () {
		quest = CreateQuestUtils.CreateQuest ();

		if (quest) {
			quest.Steps = new List<QuestStep> ();
			string relPath = AssetDatabase.GetAssetPath (quest);
			EditorPrefs.SetString ("ObjectPath", relPath);
		}

		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = quest;
	}

	void OpenQuest () {
		string absPath = EditorUtility.OpenFilePanel ("Select Quest", "", "");

		if (absPath.StartsWith (Application.dataPath)) {
			string relPath = absPath.Substring (Application.dataPath.Length - "Assets".Length);

			quest = AssetDatabase.LoadAssetAtPath (relPath, typeof (Quest)) as Quest;

			if (quest.Steps == null)
				quest.Steps = new List<QuestStep> ();
			
			if (quest)
				EditorPrefs.SetString ("ObjectPath", relPath);
		}
	}

	void AddStep () {
		string object_path = EditorPrefs.GetString ("ObjectPath");
		string dir_path = System.String.Format ("{0}/Steps", Path.GetDirectoryName (object_path));
			
		if (dir_path.StartsWith (Application.dataPath))
			dir_path = dir_path.Substring (Application.dataPath.Length - "Assets".Length);

		QuestStep new_step = ScriptableObject.CreateInstance<QuestStep> ();

		if (!Directory.Exists (dir_path))
			Directory.CreateDirectory (dir_path);
			
		AssetDatabase.CreateAsset (new_step, System.String.Format("{0}/Step {1}.asset", dir_path, viewIndex));
		AssetDatabase.SaveAssets ();

		quest.Steps.Add (new_step);
		viewIndex = quest.Steps.Count;
	}

	void DeleteStep (int index) {
		quest.Steps.RemoveAt (index);
	}
}