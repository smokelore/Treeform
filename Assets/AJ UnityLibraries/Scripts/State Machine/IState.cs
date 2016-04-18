using UnityEngine;
using System.Collections;

public interface IState<T> {
	IEnumerator OnEnter();
	void OnUpdate();
	IEnumerator OnExit();
	void SetMachine(StateMachine<T> machine);
	Coroutine StartCoroutine(IEnumerator routine);
}
