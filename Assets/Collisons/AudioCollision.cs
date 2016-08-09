using UnityEngine;
using System.Collections;

[AddComponentMenu ("Collision/Audio Trigger")]
[RequireComponent (typeof (BoxCollider))]
[RequireComponent (typeof (AudioSource))]
public class AudioCollision : MonoBehaviour {
	[SerializeField] AudioClip audioClip;
	[SerializeField] float volume = 1f;

	private AudioSource audioSource;

	void Start () {
		audioSource = GetComponent<AudioSource> ();
	}

	void OnTriggerEnter (Collider other) {
		if (other.tag != "Player")
			return;

		Debug.LogFormat ("Encountered player: playing clip {0}", audioClip.name);
		audioSource.PlayOneShot (audioClip, volume);
	}
}
