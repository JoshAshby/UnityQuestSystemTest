using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using QuestSystem;

[AddComponentMenu ("Quest System/Quest Assigner")]
[RequireComponent (typeof (BoxCollider))]
public class QuestAssigner : MonoBehaviour {
	[SerializeField] string pillar_name;

	void OnTriggerEnter (Collider other) {
		if (other.tag != "Player")
			return;

		Debug.Log ("Encountered player: assigning main quest");

		IQuest quest = new MainQuest ();

		QuestManager.instance.Assign (quest);

		GameManager.instance.LastPillar = pillar_name;

		Destroy (gameObject);
	}
}