using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using QuestSystem;

public class QuestManager : MonoBehaviour {
	public static QuestManager instance = null;

	private List<IQuest> available_quests;
	private List<IQuest> completed_quests;

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

		available_quests = new List<IQuest> ();
		completed_quests = new List<IQuest> ();
	}

	public void Assign (IQuest quest) {
		available_quests.Add (quest);
		quest.OnStart ();
	}

	void Update () {
		foreach (IQuest quest in available_quests) {
			quest.UpdateProgress ();

			if (quest.IsComplete) {
				available_quests.Remove (quest);
				completed_quests.Add (quest);
			}
		}
	}
}