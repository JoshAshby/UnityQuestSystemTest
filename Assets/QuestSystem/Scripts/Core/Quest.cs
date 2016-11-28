using UnityEngine;
using System;
using System.Collections.Generic;

namespace QuestSystem {
	[Serializable]
	public struct Quest {
		public int Id;

		public string Title;
		public string Description;

		public string CompletionDialogue;
		public AudioClip CompletionAudio;

		public string FailureDialogue;
		public AudioClip FailureAudio;

		public List<QuestStep> Steps;
	}
}