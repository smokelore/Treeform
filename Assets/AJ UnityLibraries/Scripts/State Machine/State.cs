using UnityEngine;
using System.Collections;

public abstract class State<T> : IState<T> {
	private static GameObject coroutineObject;
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

	public Coroutine StartCoroutine(IEnumerator routine){
		if (coroutineObject == null){
			coroutineObject = new GameObject("State Machine Coroutines");
			GameObject.DontDestroyOnLoad(coroutineObject);
			coroutineObject.AddComponent<MonoBehaviour>();
		}
		return coroutineObject.GetComponent<MonoBehaviour>().StartCoroutine(routine);
	}
}
