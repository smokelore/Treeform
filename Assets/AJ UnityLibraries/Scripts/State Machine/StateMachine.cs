using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachine<T> {

	public delegate void StateHandler(IState<T> state);
	public event StateHandler OnStateFinished;
	public IState<T> CurrentState;
	public T ParentObject;

	public StateMachine(T parent){
		ParentObject = parent;
	}

	bool switching = false;
	public IEnumerator SwitchState(IState<T> s){
		switching = true;
		if (CurrentState != null){
			yield return CurrentState.StartCoroutine(CurrentState.OnExit());
			CurrentState.SetMachine(null);
		}
		CurrentState = s;
		if (CurrentState != null){
			CurrentState.SetMachine(this);
			yield return CurrentState.StartCoroutine(CurrentState.OnEnter());
		}
		switching = false;
	}

	public void StateFinished(IState<T> s){
		if (OnStateFinished != null){
			OnStateFinished(s);
		}
	}

	public void Update(){
		if (!switching && CurrentState != null){
			CurrentState.OnUpdate();
		}
	}
}