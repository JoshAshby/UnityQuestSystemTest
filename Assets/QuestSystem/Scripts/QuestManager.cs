using UnityEngine;
using UnityEngine.Events;

using System;
using System.Collections;
using System.Collections.Generic;

using QuestSystem;

[Serializable]
public class QuestEvent : UnityEvent<Quest> {}

[Serializable]
public class QuestManager : MonoBehaviour {
    public List<Quest> Quests;

    public AudioSource audioSource;

    public QuestEvent OnQuestStart = new QuestEvent();
    public QuestEvent OnQuestUpdate = new QuestEvent();
    public QuestEvent OnQuestComplete = new QuestEvent();
    public QuestEvent OnQuestFail = new QuestEvent();

    private static QuestManager instance = null;

    public static QuestManager Instance {
    	get { return instance; }
	}

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}

	void Update () {
	}

	public Quest FindQuest(int quest_id) {
		Debug.LogFormat ("Finding quest by id `{0}'", quest_id);

		return Quests[quest_id];
	}

	public void StartQuest(int quest_id) {
		Debug.LogFormat ("Starting quest `{0}'", quest_id);

		Quest quest = FindQuest (quest_id);

		OnQuestStart.Invoke (quest);
	}

	public void UpdateQuest(int quest_id) {
		Debug.LogFormat ("Updating quest `{0}'", quest_id);

		Quest quest = FindQuest (quest_id);

		OnQuestUpdate.Invoke (quest);
	}

	public void CompleteQuest(int quest_id) {
		Debug.LogFormat ("Completing quest `{0}'", quest_id);

		Quest quest = FindQuest (quest_id);

		OnQuestComplete.Invoke (quest);
	}

	public void CompleteQuestStep(int quest_id, int step_id) {
		Debug.LogFormat ("Completing quest `{0}' step `{1}'", quest_id, step_id);

		Quest quest = FindQuest (quest_id);

		OnQuestUpdate.Invoke (quest);
	}

	public void CompleteQuestObjective(int quest_id, int step_id, int objective_id) {
		Debug.LogFormat ("Completing quest `{0}' objective `{1}' of step `{2}'", quest_id, objective_id, step_id);

		Quest quest = FindQuest (quest_id);

		OnQuestUpdate.Invoke (quest);
	}

	public void FailQuest(int quest_id) {
		Debug.LogFormat ("Updating a quest `{0}'", quest_id);

		Quest quest = FindQuest (quest_id);

		OnQuestComplete.Invoke (quest);
	}

	public void FailQuestStep(int quest_id, int step_id) {
		Debug.LogFormat ("Updating quest `{0}'", quest_id);

		Quest quest = FindQuest (quest_id);

		OnQuestUpdate.Invoke (quest);
	}

	public void FailQuestObjective(int quest_id, int step_id, int objective_id) {
		Debug.LogFormat ("Updating quest `{0}'", quest_id);

		Quest quest = FindQuest (quest_id);

		OnQuestUpdate.Invoke (quest);
	}
}