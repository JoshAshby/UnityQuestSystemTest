using UnityEngine;
using System.Collections;

public class QuestObserver : MonoBehaviour {
	void Start () {
	}
	
	void Update () {
	}

	void OnTriggerEnter(Collider collision) {
		if (collision.tag == "Player")
			return;

		Debug.Log ("Encountered object");
	}
}