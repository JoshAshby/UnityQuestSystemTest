using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GameStates {
	NullState,
	Playing,
	Paused,
	Cutscene,
	Menu
}

public interface IFSMStateExt {
	void DoBeforeEntering ();
}

public interface IFSMState : IFSMStateExt {
}

public abstract class BaseFSMState : IFSMState {

}

public class FSMBase {
}

public class PauseState : IFSMState {
	private float previousTimeScale = 1.0f;

	public void DoBeforeEntering () {
		previousTimeScale = Time.timeScale;
		Time.timeScale = 0;
	}

	public override void DoBeforeLeaving () {
		Time.timeScale = previousTimeScale;
	}

	public override void Reason (GameObject actor, GameObject actee) {
	}

	public override void Act (GameObject actor, GameObject actee) {
	}
}

public class PlayingState : IFSMState {
	public override void Reason (GameObject actor, GameObject actee) {
	}

	public override void Act (GameObject actor, GameObject actee) {
	}
}
	
public class GameManager : IFixedTickable {
	public delegate void OnStateChangeHandler ();
	public event OnStateChangeHandler OnStateChange;
	
	private FSMSystem fsm;
 
    public void SetTransition (Transition t) { fsm.PerformTransition(t); }
 
    public void Start () {
        MakeFSM();
    }
 
    public void FixedTick () {
        fsm.CurrentState.Reason(null, null);
        fsm.CurrentState.Act(null, null);
    }
 
    private void MakeFSM () {
        fsm = new FSMSystem();

        fsm.AddState(GameStates.Paused, new PauseState());
		fsm.AddState(GameStates.Playing, new PlayingState());

		fsm.AddTransition(GameStates.Paused, new UnPauseTransition(), GameStates.Playing);
		fsm.AddTransition(GameStates.Playing, new PauseTransition(), GameStates.Paused);

		fsm.PerformTransition(GameStates.Playing);
    }

	public void Quit () {
		#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}
}