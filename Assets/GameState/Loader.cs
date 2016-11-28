using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {
	public GameObject gameManager;
	public GameObject questManager;

	void Awake () {
		if (GameManager.instance == null)
			Instantiate (gameManager);
		
		if (QuestManager.Instance == null)
			Instantiate (questManager);
	}
}