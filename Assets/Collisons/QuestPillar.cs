using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu ("Quest System/Quest Pillar")]
[RequireComponent (typeof (BoxCollider))]
public class QuestPillar : MonoBehaviour {
	[SerializeField] string pillar_name;
	[SerializeField] string previous_pillar_name;

	private bool should_act = false;
	private MeshRenderer mesh_renderer;

	void Awake () {
		mesh_renderer = GetComponent<MeshRenderer> ();
		mesh_renderer.enabled = false;
	}

	void Update () {
		if (GameManager.instance.LastPillar == previous_pillar_name) {
			should_act = true;
			mesh_renderer.enabled = true;
		}
	}

	void OnTriggerEnter (Collider other) {
		if (!should_act)
			return;
		
		if (other.tag != "Player")
			return;

		Debug.Log ("Encountered player: updating last pillar name");

		GameManager.instance.LastPillar = pillar_name;
	
		Destroy (gameObject);
	}
}