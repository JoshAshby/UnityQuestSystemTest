using System.Collections.Generic;

namespace QuestSystem {
	public interface IQuest {
		string Title { get; }
		string Description { get; }

		bool IsComplete { get; }

		List<IQuestObjective> Objectives { get; }

		void OnStart ();
		void OnComplete ();

		void UpdateProgress ();
	}
}