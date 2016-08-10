using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QuestSystem {
	public interface IQuestStep {
//		bool is_complete { get; }
//
//		List<IQuestObjective> objectives { get; }

		bool IsComplete ();
		void UpdateProgress (IQuest quest);

		void OnStart ();
		void OnComplete ();
	}
}