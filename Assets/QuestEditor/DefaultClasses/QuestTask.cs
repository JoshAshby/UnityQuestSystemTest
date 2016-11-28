using UnityEngine;
using System.Collections;

namespace Adnc.Quest {
	[System.Serializable]
	public class QuestTask : QuestTaskBase {
		public override bool CheckRequirements () {
			return true;
		}
	}
}
