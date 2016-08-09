using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Quest;

public class QuestManager : MonoBehaviour {
	public static QuestManager instance = null;

	public List<IQuest> availableQuests;

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

		availableQuests = new List<IQuest> ();
	}

	public void addQuest (IQuest _quest) {
		availableQuests.Add (_quest);
	}

	public bool hasQuest (string questName) {
		IQuest quest = availableQuests.Find (x => x.name == questName);

		if (quest == nil)
			return false;

		return quest.state != States.Unstarted;
	}

	public void assignQuest (string questName) {
		IQuest quest = availableQuests.Find (x => x.name == questName);

		if (quest == null)
			return;

		if (!quest.canStart ())
			return;

		quest.start ();
	}
}
