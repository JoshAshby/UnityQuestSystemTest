using System.Collections.Generic;

namespace QuestSystem {
	public class MainQuest : IQuest {
		protected string title;
		protected string description;

		protected List<IQuestObjective> objectives;

		public MainQuest () {
			title = "Chapter 1";
			description = "Learn about the Sleeping Trouble";

			objectives = new List<IQuestObjective> ();
			objectives.Add (new PillarObjective("Find The Intro Pillar", "Main.Intro"));
			objectives.Add (new PillarObjective("Find The Mid Pillar", "Main.Mid"));
			objectives.Add (new PillarObjective("Find The Fin Pillar", "Main.Fin"));
		}

		public string Title {
			get { return title; }
		}

		public string Description {
			get { return description; }
		}

		public bool IsComplete {
			get {
				foreach (IQuestObjective objective in objectives) {
					if (!objective.IsComplete)
						return false;
				}

				return true;
			}
		}

		public List<IQuestObjective> Objectives {
			get { return objectives; }
		}

		public void UpdateProgress () {
			foreach (IQuestObjective objective in objectives) {
				if(!objective.IsComplete)
					objective.UpdateProgress ();
			}
		}

		public void OnStart () {
		}

		public void OnComplete () {
		}
	}
}