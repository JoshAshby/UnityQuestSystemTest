namespace QuestSystem {
	public interface IQuestObjective {
		string Title { get; }

		bool IsComplete { get; }

		void OnComplete ();

		void UpdateProgress();
	}
}