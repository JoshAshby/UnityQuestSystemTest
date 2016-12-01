using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour {
	[Header("UI Canvas")]
	[SerializeField]
	public Canvas PauseMenuCanvas = null;

	private void Start () {
	}

	private void Update () {
		if (Input.GetButtonDown ("Cancel")) {
			GameManager.Instance.Pause (!PauseMenuCanvas.gameObject.activeSelf);
		}
	}

	private void OnGUI () {
		PauseMenuCanvas.gameObject.SetActive (GameManager.Instance.gameState == GameManager.GameState.Paused);
	}

	public void ResumeGame () {
		GameManager.Instance.Pause (false);
	}

	public void QuitGame () {
		GameManager.Instance.Quit ();
	}
}