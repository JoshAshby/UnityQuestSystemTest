using UnityEngine;
using System.Collections;

namespace Adnc.Quest {
	[System.Serializable]
	public abstract class QuestTaskBase : QuestBase {
		public abstract bool CheckRequirements ();
	}
}
