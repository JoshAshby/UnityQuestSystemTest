using UnityEngine;

namespace QuestSystem {
	public class PillarObjective : IQuestObjective {
		private string title;

		private string pillar_name;

		private bool is_complete;

		public PillarObjective (string title, string pillar_name) {
			this.title = title;
			this.pillar_name = pillar_name;
		}

		public string Title {
			get { return title; }
		}

		public bool IsComplete {
			get { return is_complete; }
		}

		public void OnComplete() {
			Debug.LogFormat ("Completed pillar objective {0}", title);
		}

		public void UpdateProgress() {
			if(GameManager.instance.LastPillar == pillar_name) {
				is_complete = true;
				OnComplete ();
			}
		}
	}
}