using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;

	public QuestManager questManager;
	public PlayerState playerState;

	void Awake () {
		if (instance == null)
			instance = this;

		else if (instance != this)
			Destroy (gameObject);    

		DontDestroyOnLoad (gameObject);

		questManager = GetComponent<QuestManager> ();
		playerState = GetComponent<PlayerState> ();
	}
}