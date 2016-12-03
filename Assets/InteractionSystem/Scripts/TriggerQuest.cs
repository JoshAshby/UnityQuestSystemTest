using UnityEngine;
using System.Collections;

public class TriggerQuest : InteractiveBehaviour {
	private GameManager _gameManager;

    public TriggerQuest (GameManager gameManager) {
		_gameManager = gameManager;
	}
    
	override public void OnInteract () {
		Debug.Log ("Triggering quest element {0}");
		_gameManager.Quit ();
	}
}