using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QuestSystem {
	public class QuestStep : ScriptableObject {
		[SerializeField] private bool is_complete;

		[SerializeField] public AudioClip audio;
		[SerializeField] public string dialogue;

		[SerializeField] public List<QuestObjective> objectives;

		public bool IsComplete () {
			if (is_complete)
				return true;

			foreach (QuestObjective objective in objectives) {
				if (!objective.IsComplete ())
					return false;
			}

			is_complete = true;

			return true;
		}

		public void UpdateProgress (Quest quest) {
			if (is_complete)
				return;

			foreach (QuestObjective objective in objectives) {
				if(!objective.IsComplete ())
					objective.UpdateProgress (quest);
			}

			IsComplete ();
		}
	}
}