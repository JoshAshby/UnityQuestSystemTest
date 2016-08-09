using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour {
	public static QuestManager instance = null;

	public Dictionary<string, Quest> allQuests;

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

		allQuests = new Dictionary<string, Quest> ();
	}

	void Start () {
		Quest _quest = new Quest();
		_quest.questName = "Test";

		allQuests.Add ("Test", _quest);
	}

	public void addQuest (string questName, Quest _quest) {
		allQuests.Add (questName, _quest);
	}

	public bool hasQuest (string questName) {
		return GameManager.instance.playerState.instance.assignedQuests.ContainsKey (questName);
	}

	public void assignQuest (string questName) {
		Quest quest = allQuests [questName];
		GameManager.instance.playerState.instance.assignedQuests.Add (questName, quest);
	}
}
