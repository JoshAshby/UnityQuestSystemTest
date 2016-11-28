using UnityEngine;
using System.Collections.Generic;

namespace Adnc.Quest {
	[System.Serializable]
	public abstract class QuestDatabaseBase<T> : ScriptableObject {
		public string title = "Untitled";
		
		[TextArea(3, 5)]
		public string description;

		[HideInInspector]
		public List<T> _quests = new List<T>();
		public virtual List<T> Quests {
			get {
				return _quests;
			}
			set {
				_quests = value;
			}
		}
	}
}
