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

	public delegate void OnStateChangeHandler ();
	public event OnStateChangeHandler OnStateChange;

	public enum GameState {
		NullState,
		Intro,
		MainMenu,
		Playing,
		Paused
	}

	public GameState gameState {
		get;
		private set;
	}

	private float previousTimeScale = 1.0f;
	private GameState previousGameState = GameState.NullState;

	public void Pause () {
		if (Time.timeScale == 0.0f) {
			Time.timeScale = previousTimeScale;
			gameState = previousGameState;
		} else {
			previousGameState = gameState;
			previousTimeScale = Time.timeScale;
			Time.timeScale = 0;
			gameState = GameState.Paused;
		}
	}
		
	public void Pause (bool pauseState) {
		if (pauseState == true) {
			previousGameState = gameState;
			previousTimeScale = Time.timeScale;
			Time.timeScale = 0;
			gameState = GameState.Paused;
		} else {
			Time.timeScale = previousTimeScale;
			gameState = previousGameState;
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