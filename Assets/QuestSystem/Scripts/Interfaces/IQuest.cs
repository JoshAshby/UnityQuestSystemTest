using System.Collections.Generic;

namespace QuestSystem {
	public interface IQuest {
		string Title { get; }
		string Description { get; }

		List<IQuestStep> Steps { get; }
		IQuestStep CurrentStep { get; }

		bool IsComplete ();
		void UpdateProgress ();

		void OnStart ();
		void OnComplete ();
	}
}