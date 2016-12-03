using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FSMMachine<T> {
    public FSMBaseState<T> CurrentState {
        get;
        private set;
    }

    public T CurrentStateName {
        get;
        private set;
    }

    private Dictionary<T, FSMBaseState<T>> _stateMap;

    public delegate void OnStateChangeHandler ();
    public event OnStateChangeHandler OnStateChange;

    public void AddState (T name, FSMBaseState<T> state) {
        // Check for Null reference before deleting
        if (state == null) {
            Debug.LogError ("FSM ERROR: Null reference for state object is not allowed");
        }

        _stateMap.Add (name, state);
    }

    public void AddTransition (T from, T to, FSMBaseTransition transition = null) { }

    public void PerformTransition (T to_name) { }
}

public abstract class FSMBaseState<T> {
    public virtual void DoBeforeEntering () { }
    public virtual void DoBeforeLeaving () { }

    public abstract T Reason ();
    public abstract void Act ();
}

public abstract class FSMBaseTransition {
    public abstract void Act ();
}

public enum GameStates {
    NullState,
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
        return GameStates.Paused;
    }

    public override void Act () {
    }
}

public class PlayingState : FSMBaseState<GameStates> {
    public override GameStates Reason () {
        return GameStates.Playing;
    }

    public override void Act () {
    }
}

public class PauseTransition : FSMBaseTransition {
    public override void Act () { }
}

public class UnPauseTransition : FSMBaseTransition {
    public override void Act () { }
}

public class GameManager : IFixedTickable {
    public FSMMachine<GameStates> fsm {
        get;
        private set;
    }

    public void SetTransition (GameStates t) { fsm.PerformTransition(t); }

    public void Start () {
        BuildFSM ();
    }

    public void FixedTick () {
        fsm.CurrentState.Reason ();
        fsm.CurrentState.Act ();
    }

    private void BuildFSM () {
        fsm = new FSMMachine<GameStates> ();

        fsm.AddState (GameStates.Paused, new PauseState ());
        fsm.AddState (GameStates.Playing, new PlayingState ());

        fsm.AddTransition (GameStates.Paused, GameStates.Playing, new UnPauseTransition ());
        fsm.AddTransition (GameStates.Playing, GameStates.Paused, new PauseTransition ());

        fsm.PerformTransition (GameStates.Playing);
    }

    public void Quit () {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit ();
        #endif
    }
}