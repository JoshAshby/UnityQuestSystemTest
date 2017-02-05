using UnityEditor;
using UnityEngine;

public enum GameStates {
    Playing,
    Menu
}

public class MenuState : FSMBaseState<GameStates> {
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

        return GameStates.Menu;
    }

    public override void Act () { }
}

public class PlayingState : FSMBaseState<GameStates> {
    public override GameStates Reason () {
        if (Input.GetButtonDown ("Cancel")) {
            return GameStates.Menu;
        }

        return GameStates.Playing;
    }

    public override void Act () { }
}

public interface IGameManager {
    GameStates State {
        get;
        set;
    }

    FSMMachine<GameStates> fsm {
        get;
    }

    void Quit ();
}

public class GameManager : MonoBehaviour, IGameManager {
    public GameStates State {
        get { return fsm.CurrentState; }
        set { fsm.PerformTransition(value); }
    }

    public FSMMachine<GameStates> fsm {
        get;
        private set;
    }

    private void Awake () {
        BuildFSM ();
    }

    private void Update () {
        fsm.Reason ();
    }

    private void BuildFSM () {
        fsm = new FSMMachine<GameStates> ();

        fsm.AddState (GameStates.Menu, new MenuState ());
        fsm.AddState (GameStates.Playing, new PlayingState ());

        fsm.AddTransition (GameStates.Menu, GameStates.Playing);
        fsm.AddTransition (GameStates.Playing, GameStates.Menu);

        fsm.SetEntryState (GameStates.Playing);
    }

    public void Quit () {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit ();
        #endif
    }
}