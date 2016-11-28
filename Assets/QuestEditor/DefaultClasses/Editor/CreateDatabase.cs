using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

namespace Adnc.Quest {
	public class CreateDatabaseQuest : CreateDatabaseBase {
		[MenuItem("Assets/Create/Quest Database")]
		public static void NewDatabase () {
			CreateDatabase(typeof(QuestDatabase), "QuestDatabase");
		}
	}
}

