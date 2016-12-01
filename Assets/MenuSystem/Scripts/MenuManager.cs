using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour {
	[Header("UI Canvas")]
	[SerializeField]
	public Canvas PauseMenuCanvas = null;

	private void Start () {
	}

	private void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			GameManager.Instance.Pause (true);
		}
	}

	private void OnGui () {
		if (GameManager.Instance.paused) {
			PauseMenuCanvas.enabled = true;
		}
	}

	public void ResumeGame () {
		GameManager.Instance.Pause (false);
	}
}