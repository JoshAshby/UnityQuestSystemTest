using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using Zenject;
#endif

public enum GameStates {
    Playing,
    Paused,
    Cutscene,
    Menu
}

public class PauseState : FSMBaseState<GameStates> {
    private float previousTimeScale = 1.0f;

    public override void DoBeforeEntering () {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public override void DoBeforeLeaving () {
        Time.timeScale = previousTimeScale;
    }

    public override GameStates Reason () {
        if (Input.GetButtonDown ("Cancel")) {
            return GameStates.Playing;
        }

        return GameStates.Paused;
    }

    public override void Act () { }
}

public class PlayingState : FSMBaseState<GameStates> {
    public override GameStates Reason () {
        if (Input.GetButtonDown ("Cancel")) {
            return GameStates.Paused;
        }

        return GameStates.Playing;
    }

    public override void Act () { }
}

public interface IGameManager {
    FSMMachine<GameStates> fsm { get; }

    void SetTransition(GameStates t);
    void Quit ();
}

public class GameManager : IGameManager, ITickable {
    public FSMMachine<GameStates> fsm {
        get;
        private set;
    }

    public GameManager () {
        BuildFSM ();
    }

    public void Tick () {
        fsm.Reason ();
    }

    private void BuildFSM () {
        fsm = new FSMMachine<GameStates> ();

        fsm.AddState (GameStates.Paused, new PauseState ());
        fsm.AddState (GameStates.Playing, new PlayingState ());

        fsm.AddTransition (GameStates.Paused, GameStates.Playing);
        fsm.AddTransition (GameStates.Playing, GameStates.Paused);

        fsm.SetEntryState (GameStates.Playing);
    }

    public void Quit () {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit ();
        #endif
    }

    public void SetTransition (GameStates t) {
        fsm.PerformTransition(t);
    }
}