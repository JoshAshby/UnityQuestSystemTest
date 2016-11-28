using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using QuestSystem;

public class QuestDatabaseEditor : Editor {
}

//public class QuestManagerEditor : EditorWindow {
//	public List<BaseQuest> Quests;
//
//	[MenuItem ("Window/Quest Editor")]
//	static void Init () {
//		GetWindow <QuestManagerEditor> ();
//	}
//
//	private void OnEnable() {
//		if(Quests == null)
//			Quests = new List<BaseQuest> ();
//	}
//
//	public void OnGUI() {
//		if (GUILayout.Button ("Load", GUILayout.ExpandWidth (true))) {
//			load ();
//		}
//
//		EditorGUILayout.LabelField ("Quests");
//
//		if (GUILayout.Button ("Create New Quest", GUILayout.ExpandWidth (true))) {
//			newQuest ();
//		}
//
//		if (Quests.Count == 0) {
//			EditorGUILayout.HelpBox ("No quests have been created yet, click the 'Create New Quest' button above", MessageType.Info);
//		} else {
//			if (GUILayout.Button ("Save", GUILayout.ExpandWidth (true))) {
//				save ();
//			}
//
//			EditorGUILayout.BeginVertical (GUI.skin.GetStyle ("box"));
//
//			List<BaseQuest> toRemove = new List<BaseQuest> ();
//
//			foreach (var quest in Quests) {
//				EditorGUILayout.BeginHorizontal ();
//
//				if (GUILayout.Button (quest.Title, GUILayout.ExpandWidth (true))) {
//					
//				}
//
//				if (GUILayout.Button ("X", GUILayout.ExpandWidth (false))) {
//					toRemove.Add (quest);
//				}
//
//				EditorGUILayout.EndHorizontal ();
//			}
//
//			foreach (var quest in toRemove) {
//				removeQuest (quest);
//			}
//
//			toRemove.Clear ();
//
//			EditorGUILayout.EndVertical ();
//		}
//	}
//
//	public void newQuest () {
//		BaseQuest quest = ScriptableObject.CreateInstance<BaseQuest> ();
//
//		int count = Quests.Count;
//
//		quest.Id = count;
//		quest.Title = String.Format("Untitled Quest {0}", count);
//			
//		Quests.Add (quest);
//	}
//
//	public void removeQuest (BaseQuest quest) {
//		if (EditorUtility.DisplayDialog(
//			"Warning!",
//			"Are you sure you want to delete the quest?",
//			"Yes", "No"
//		)) {
//			Quests.Remove (quest);
//		}
//	}
//
//	public void save () {
//		string path = EditorUtility.SaveFilePanel ("Save quest asset", "", "Untitled Quest.quest", ".quest");
//		Debug.Log (path);
//	}
//
//	public void load () {
//		string path = EditorUtility.OpenFilePanel ("Open quest asset", "", ".quest");
//		Debug.Log (path);
//	}
//}

//[CustomEditor (typeof (BaseQuest))]
//public class QuestEditor : Editor {
//	BaseQuest quest;
//
//	bool show_position = true;
//
//	void  OnEnable () {
//		quest = target as BaseQuest;
//	}
//
//	public override void OnInspectorGUI () {
//		EditorGUI.BeginChangeCheck ();
//
//		quest.Id = EditorGUILayout.TextField ("Id", quest.Id);
//
//		quest.Title = EditorGUILayout.TextField ("Title", quest.Title);
//		quest.Description = EditorGUILayout.TextField ("Description", quest.Description);
//
//		int step_count = quest.Steps.Count;
//
//		List<BaseQuestStep> to_delete = new List<BaseQuestStep> ();
//		show_position = EditorGUILayout.Foldout (show_position, System.String.Format ("Steps ({0})", step_count));
//
//		if (show_position) {
//			EditorGUILayout.BeginVertical();
//			EditorGUI.indentLevel++;
//
//			List<BaseQuestStep> steps = quest.Steps;
//
//			if (steps.Count > 0) {
//				foreach (var step in steps) {
//					if (GUILayout.Button ("Delete Step", GUILayout.ExpandWidth (false))) {
//						to_delete.Add (step);
//						GUI.changed = true;
//					}
//
//					step.Title = EditorGUILayout.TextField ("Title", step.Title); 
//
//					step.Audio = EditorGUILayout.ObjectField ("Audio", step.Audio, typeof (AudioClip), true) as AudioClip;
//					step.Dialogue = EditorGUILayout.TextField ("Dialogue", step.Dialogue);
//				}
//			} else {
//				GUILayout.Label ("This Quest has no Steps yet.");
//			}
//
//			if (GUILayout.Button ("Add Step", GUILayout.ExpandWidth (false))) {
//				AddStep ();
//			}
//
//			EditorGUI.indentLevel--;
//			EditorGUILayout.EndVertical();
//		}
//
//		if (EditorGUI.EndChangeCheck ()) {
//			foreach (var step in to_delete) {
//				DeleteStep (step);
//			}
//
//			to_delete.Clear ();
//
//			EditorUtility.SetDirty (quest);
//		}
//	}
//
//	void AddStep () {
//		BaseQuestStep new_step = new BaseQuestStep ();
//
//		quest.Steps.Add (new_step);
//	}
//
//	void DeleteStep (BaseQuestStep step) {
//		quest.Steps.Remove (step);
//	}
//}