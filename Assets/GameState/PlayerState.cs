using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerState : MonoBehaviour {
	public static PlayerState instance = null;

	public Dictionary<string, Quest> assignedQuests;

	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);    

		DontDestroyOnLoad (gameObject);

		assignedQuests = new Dictionary<string, Quest> ();
	}
}
