using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool<T> : MonoBehaviour where T : Object {

	public T PooledObject;
	public int StartCapacity;

	[HideInInspector]
	public int Capacity;

	protected Stack<T> pool;

	void Awake () {
		Init ();
	}

	public virtual void Init(){
		pool = new Stack<T>();
		for (int i = 0; i < StartCapacity; i++){
			pool.Push((T) Instantiate(PooledObject));
		}
	}

	public virtual T RequestObject(){
		if (pool.Count > 0){
			return pool.Pop();
		}
		else {
			return (T) Instantiate(PooledObject);
		}
	}

	public virtual void ReturnObject(T obj){
		pool.Push(obj);
	}
}
