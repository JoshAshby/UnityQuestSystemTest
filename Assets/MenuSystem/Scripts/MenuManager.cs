using UnityEngine;

public class MenuManager : MonoBehaviour {
    [Header("UI Canvas")]
    [SerializeField]
    public Canvas PauseMenuCanvas = null;

    private GameManager _gameManager;

    public MenuManager (GameManager gameManager) {
        _gameManager = gameManager;
    }

    private void Start () {
    }

    private void Update () {
        if (Input.GetButtonDown ("Cancel")) {
            _gameManager.SetTransition (GameStates.Paused);
        }
    }

    private void OnGUI () {
        PauseMenuCanvas.gameObject.SetActive (_gameManager.fsm.CurrentStateName == GameStates.Paused);
    }

    public void ResumeGame () {
        _gameManager.SetTransition (GameStates.Playing);
    }

    public void QuitGame () {
        _gameManager.Quit ();
    }
}