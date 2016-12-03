using UnityEngine;
using Zenject;

public class TriggerQuest : InteractiveBehaviour {
    [Inject]
    private IGameManager _gameManager;

    override public void OnInteract () {
        Debug.Log ("Triggering quest element {0}");
        _gameManager.Quit ();
    }
}