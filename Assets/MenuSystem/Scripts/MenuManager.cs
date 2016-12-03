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
			_gameManager.Pause (!PauseMenuCanvas.gameObject.activeSelf);
		}
	}	

	private void OnGUI () {
		PauseMenuCanvas.gameObject.SetActive (_gameManager.gameState == GameState.Paused);
	}

	public void ResumeGame () {
		_gameManager.Pause (false);
	}

	public void QuitGame () {
		_gameManager.Quit ();
	}
}