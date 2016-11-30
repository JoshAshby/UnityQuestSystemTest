using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager {
	// make sure the constructor is private, so it can only be instantiated here
	private static GameManager instance = new GameManager();
	private GameManager() { }

	public static GameManager Instance {
		get { return instance; }
	}

	private bool _paused = Time.timeScale == 0 ? true : false;
	public bool paused {
		get { return _paused; }
	}

	private float previousTimeScale = 1.0f;

	public void Pause () {
		if (Time.timeScale == 0.0f) {
			Time.timeScale = previousTimeScale;
			_paused = true;
		} else {
			previousTimeScale = Time.timeScale;
			Time.timeScale = 0;
			_paused = false;
		}
	}
		
	public void Pause (bool pauseState) {
		if (pauseState == true) {
			Time.timeScale = previousTimeScale;
			_paused = true;
		} else {
			previousTimeScale = Time.timeScale;
			Time.timeScale = 0;
			_paused = false;
		}
	}

	public void Quit () {
		#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}
}