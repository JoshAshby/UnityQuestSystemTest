using System;
using System.Collections.Generic;
using UnityEngine;

public class FSMMachine<T> where T : struct, IConvertible, IComparable, IFormattable {
    public FSMBaseState<T> CurrentStateObject {
        get { return _stateMap[CurrentState]; }
    }

    public T CurrentState {
        get;
        private set;
    }

    private Dictionary<T, FSMBaseState<T>> _stateMap;
    private Dictionary<T, Dictionary<T, FSMBaseTransition>> _transitionMap;

    public delegate void OnStateChangeHandler (T newState);
    public event OnStateChangeHandler OnStateChange;

    public FSMMachine () {
        if (!typeof (T).IsEnum) {
            throw new ArgumentException("T must be an enum");
        }

        _stateMap = new Dictionary<T, FSMBaseState<T>> ();
        _transitionMap = new Dictionary<T, Dictionary<T, FSMBaseTransition>> ();
    }

    public void AddState (T name, FSMBaseState<T> state) {
        if (state == null) {
            Debug.LogError ("FSM ERROR: Null reference for state object is not allowed");
            return;
        }

        if (_stateMap.ContainsKey(name)) {
            Debug.LogErrorFormat ("FSM ERROR: State name {0} is already defined in the state map!", name.ToString ());
            return;
        }

        _stateMap.Add (name, state);
        _transitionMap.Add (name, new Dictionary<T, FSMBaseTransition> ());
    }

    public void AddTransition (T from_name, T to_name, FSMBaseTransition transition = null) {
        if (!_stateMap.ContainsKey (from_name)) {
            Debug.LogErrorFormat ("FSM ERROR: From state name {0} does not exist in the state map!", from_name.ToString ());
            return;
        }

        if (!_stateMap.ContainsKey (to_name)) {
            Debug.LogErrorFormat ("FSM ERROR: From state name {0} does not exist in the state map!", to_name.ToString ());
            return;
        }

        _transitionMap[from_name].Add (to_name, transition);
    }

    public void SetEntryState (T name) {
        if (!_stateMap.ContainsKey (name)) {
            Debug.LogErrorFormat ("FSM ERROR: State name {0} does not exist in the state map!", name.ToString ());
            return;
        }

        CurrentState = name;
    }

    public bool PerformTransition (T to_name) {
        Dictionary<T, FSMBaseTransition> currentTransitionMap = _transitionMap[CurrentState];
        if (!currentTransitionMap.ContainsKey (to_name)) {
            Debug.LogErrorFormat ("FSM ERROR: No transition exists from state {0} to state {1}!", CurrentState.ToString (), to_name.ToString ());
            return false;
        }

        CurrentStateObject.DoBeforeLeaving ();

        FSMBaseTransition transition = currentTransitionMap[to_name];
        if (transition != null) {
            transition.Act ();
        }

        CurrentState = to_name;

        CurrentStateObject.DoBeforeEntering ();

        if (OnStateChange != null)
            OnStateChange (CurrentState);

        return true;
    }

    public void Reason () {
        T nextState = CurrentStateObject.Reason ();

        if (!nextState.Equals (CurrentState)) {
            if(!PerformTransition (nextState)) {
                return;
            }
        }

        CurrentStateObject.Act();
    }
}