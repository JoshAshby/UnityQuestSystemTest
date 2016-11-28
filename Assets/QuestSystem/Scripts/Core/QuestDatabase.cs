using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace QuestSystem {
	[Serializable]
	public class QuestDatabase : ScriptableObject {
		[SerializeField]

		public List<Quest> Quests;
	}
}