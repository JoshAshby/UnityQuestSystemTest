using UnityEngine;
using System;

namespace QuestSystem {
	public class QuestObjective : ScriptableObject {
		[SerializeField] public string title;
		[SerializeField] public bool is_complete;

		[SerializeField] public string pillar_name;

		public bool IsComplete () {
			if (is_complete)
				return true;

			return false;
//
//			is_complete = true;
//			OnComplete ();
//
//			return true;
		}

		public void UpdateProgress(Quest quest) {
			if(GameManager.instance.LastPillar == pillar_name) {
				is_complete = true;
			}
		}
	}
}