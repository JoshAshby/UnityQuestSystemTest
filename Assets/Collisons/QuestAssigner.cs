using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[AddComponentMenu ("Quest System/Quest Assigner")]
[RequireComponent (typeof (BoxCollider))]
public class QuestAssigner : MonoBehaviour {
	[SerializeField] string pillar_name;

	private bool should_act = true;
	private MeshRenderer mesh_renderer;

	void Awake () {
		mesh_renderer = GetComponent<MeshRenderer> ();
	}

	void OnTriggerEnter (Collider other) {
		if (!should_act)
			return;
		
		if (other.tag != "Player")
			return;

		Debug.Log ("Encountered player: assigning main quest");

		GameManager.instance.LastPillar = pillar_name;

		should_act = false;
		mesh_renderer.enabled = false;
	}
}