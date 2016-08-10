using UnityEngine;
using System.Collections.Generic;

namespace QuestSystem {
	public class Quest : ScriptableObject {
		[SerializeField] public string title;
		[SerializeField] public string description;

		[SerializeField] public bool is_complete;

		[SerializeField] public List<QuestStep> steps;
		[SerializeField] public QuestStep current_step;

		public string Title {
			get { return title; }
		}

		public string Description {
			get { return description; }
		}

		public List<QuestStep> Steps {
			get { return steps; }
			set { steps = value; }
		}

		public QuestStep CurrentStep {
			get { return current_step; }
		}

		public bool IsComplete () {
			if (is_complete)
				return true;
			
			foreach (QuestStep step in steps) {
				if (!step.IsComplete ())
					return false;
			}

			is_complete = true;

			return true;
		}

		public void UpdateProgress () {
			if (is_complete)
				return;
			
			foreach (QuestStep step in steps) {
				if(!step.IsComplete ())
					step.UpdateProgress (this);
			}

			IsComplete ();
		}
	}
}