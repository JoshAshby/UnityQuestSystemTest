using UnityEngine;
using System.Collections;

[AddComponentMenu ("Collision/Quest Assigner")]
[RequireComponent (typeof (BoxCollider))]
public class QuestAssigner : MonoBehaviour {
	[SerializeField] string questName;

	void Start () {
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag != "Player")
			return;

		if (QuestManager.instance.hasQuest (questName))
		{
			Debug.LogFormat ("Encountered player: skipping assignment of quest {0} since they already have it", questName);
			return;
		}

		Debug.LogFormat ("Encountered player: assigning quest {0}", questName);
		QuestManager.instance.assignQuest (questName);
	}
}
