using UnityEngine;
using Zenject;

public class MenuManager : MonoBehaviour {
    [Header("UI Canvas")]
    [SerializeField]
    public Canvas PauseMenuCanvas = null;

    [Inject]
    private IGameManager _gameManager;

    private void OnGUI () {
        PauseMenuCanvas.gameObject.SetActive (_gameManager.fsm.CurrentState == GameStates.Menu);
    }

    public void ResumeGame () {
        _gameManager.SetTransition (GameStates.Playing);
    }

    public void QuitGame () {
        _gameManager.Quit ();
    }
}