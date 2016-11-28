using UnityEngine;
using System.Collections.Generic;

namespace Adnc.Quest {
	[System.Serializable]
	public class QuestEntry : QuestEntryBase<QuestTask> {
		public override void Reward () {
			// Distribute rewards here
		}
	}
}
