namespace QuestSystem {
	public interface IQuestObjective {
//		string title { get; }
//		bool is_complete { get; }

		bool IsComplete ();
		void UpdateProgress(IQuest quest);

		void OnStart ();
		void OnComplete ();
	}
}