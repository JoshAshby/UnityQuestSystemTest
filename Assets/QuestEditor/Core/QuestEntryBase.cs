using UnityEngine;
using System.Collections.Generic;

namespace Adnc.Quest {
	[System.Serializable]
	public abstract class QuestEntryBase<T> : QuestBase {
		public string notes = "";
		public string successMessage = "";
		public string failMessage = "";
		public bool sideQuest;
		public string description = "";

		public bool expanded = false;

		public List<T> _tasks = new List<T>();
		public virtual List<T> Tasks { 
			get {
				return _tasks;
			}

			set {
				_tasks = value;
			}
		}

		public abstract void Reward ();
	}
}
