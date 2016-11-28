using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace QuestSystem { 
	[Serializable]
	public struct QuestStep {
		public string Title;

		public AudioClip Audio;
		public string Dialogue;

		public List<QuestObjective> Objectives;
	}
}