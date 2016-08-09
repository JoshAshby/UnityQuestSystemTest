using UnityEngine;
using System.Collections;

[AddComponentMenu ("Collision/Base")]
[RequireComponent (typeof (BoxCollider))]
public class Collision : MonoBehaviour {

	void Start () {
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag != "Player")
			return;

		Debug.Log ("Encountered player");
	}
}
