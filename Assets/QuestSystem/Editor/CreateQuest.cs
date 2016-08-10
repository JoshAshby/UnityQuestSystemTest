using UnityEngine;
using System.Collections;
using UnityEditor;

using QuestSystem;

public class CreateQuestUtils {
	[MenuItem("Assets/Create/Quest/Quest")]
	public static Quest CreateQuest() {
		string absPath = EditorUtility.OpenFolderPanel ("Select Quest Folder", "Assets/QuestSystem/Quests", "New Quest");

		Quest quest = ScriptableObject.CreateInstance<Quest> ();

		if (absPath.StartsWith (Application.dataPath)) {
			string relPath = absPath.Substring (Application.dataPath.Length - "Assets".Length);

			AssetDatabase.CreateAsset (quest, System.String.Format ("{0}/Quest.asset", relPath));
			AssetDatabase.SaveAssets ();
		}

		return quest;
	}

	[MenuItem("Assets/Create/Quest/Step")]
	public static QuestStep CreateQuestStep() {
		QuestStep asset = ScriptableObject.CreateInstance<QuestStep> ();

		AssetDatabase.CreateAsset (asset, "Assets/QuestStep.asset");
		AssetDatabase.SaveAssets ();

		return asset;
	}
}