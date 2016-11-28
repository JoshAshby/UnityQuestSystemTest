using UnityEngine;
using System.Collections;

namespace Adnc.Quest {
	public class QuestEntryDetailsBase<T> : QuestDetailsBase<T> {
		public int currentTaskIndex = 0; // Index of the current task
	}
}

