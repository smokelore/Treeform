using UnityEngine;
using System.Collections;

public abstract class MonoState<T> : MonoBehaviour, IState<T> {
	protected StateMachine<T> currentMachine;
	protected T parentObject {
		get {
			return currentMachine.ParentObject;
		}
	}
	public virtual IEnumerator OnEnter(){ yield break; }
	public virtual void OnUpdate(){}
	public virtual IEnumerator OnExit(){ yield break; }
	public void SetMachine(StateMachine<T> machine){
		currentMachine = machine;
	}

	public new Coroutine StartCoroutine(IEnumerator routine){
		return base.StartCoroutine(routine);
	}
}
