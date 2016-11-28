using UnityEngine;
using System;

namespace QuestSystem {
	public enum TriggerTypes {
		Collision,
		Interaction
	};

	[Serializable]
	public struct QuestObjective {
		public string Title;

		public GameObject GameTrigger;

		public TriggerTypes TriggerType;

		public bool FailsQuest;
	}
}