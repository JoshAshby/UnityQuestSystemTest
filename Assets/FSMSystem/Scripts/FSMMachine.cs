using System;
using System.Collections.Generic;
using UnityEngine;

public class FSMMachine<T> where T : struct, IConvertible, IComparable, IFormattable {
    public FSMBaseState<T> CurrentState {
        get { return _stateMap[CurrentStateName]; }
    }

    public T CurrentStateName {
        get;
        private set;
    }

    private Dictionary<T, FSMBaseState<T>> _stateMap;
    private Dictionary<T, Dictionary<T, FSMBaseTransition>> _transitionMap;

    public delegate void OnStateChangeHandler ();
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

        CurrentStateName = name;
    }

    public bool PerformTransition (T to_name) {
        Dictionary<T, FSMBaseTransition> currentTransitionMap = _transitionMap[CurrentStateName];
        if (!currentTransitionMap.ContainsKey (to_name)) {
            Debug.LogErrorFormat ("FSM ERROR: No transition exists from state {0} to state {1}!", CurrentStateName.ToString (), to_name.ToString ());
            return false;
        }

        CurrentState.DoBeforeLeaving ();

        FSMBaseTransition transition = currentTransitionMap[to_name];
        if (transition != null) {
            transition.Act ();
        }

        CurrentStateName = to_name;

        CurrentState.DoBeforeEntering ();

        return true;
    }

    public void Reason () {
        T nextState = CurrentState.Reason ();

        if (!nextState.Equals (CurrentStateName)) {
            if(!PerformTransition (nextState)) {
                return;
            }
        }

        CurrentState.Act();
    }
}