using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using UnityEditorInternal;

using QuestSystem;

[CustomEditor (typeof (QuestStep))]
[CanEditMultipleObjects]
public class QuestStepEditor : Editor {
	QuestStep step;

	void  OnEnable () {
		step = target as QuestStep;
	}

	public override void OnInspectorGUI () {
		EditorGUI.BeginChangeCheck ();

		step.audio = EditorGUILayout.ObjectField ("Audio", step.audio, typeof(AudioClip)) as AudioClip;
		step.dialogue = EditorGUILayout.TextField ("Dialogue", step.dialogue);

		if (EditorGUI.EndChangeCheck ())
			EditorUtility.SetDirty (step);
	}
}