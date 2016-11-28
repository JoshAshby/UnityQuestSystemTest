using UnityEngine;
using System.Collections.Generic;

namespace Adnc.Quest {
	public class QuestEntryDetails : QuestEntryDetailsBase<QuestEntry> {
		public string Title {
			get {
				return defenition.displayName;
			}
		}
	}
}
