using UnityEngine;
using Zenject;

public class MenuManager : MonoBehaviour {
    [Header("UI Canvas")]
    [SerializeField]
    public Canvas PauseMenuCanvas = null;

    [Inject]
    private IGameManager _gameManager;

    private void Start () {
    }

    private void Update () {
    }

    private void OnGUI () {
        bool shouldShowPause = _gameManager.fsm.CurrentStateName == GameStates.Paused;
        PauseMenuCanvas.gameObject.SetActive (shouldShowPause);
    }

    public void ResumeGame () {
        _gameManager.SetTransition (GameStates.Playing);
    }

    public void QuitGame () {
        _gameManager.Quit ();
    }
}