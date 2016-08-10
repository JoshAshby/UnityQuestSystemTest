using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;

	private string last_pillar;

	void Awake () {
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy (gameObject);    

		DontDestroyOnLoad (gameObject);
	}

	public string LastPillar {
		get { return last_pillar; }
		set { last_pillar = value; }
	}
}