using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

	public static T Instance{
		get {
			if (instance == null){
				instance = FindObjectOfType<T>();
			}
			return instance;
		}
	}

	protected static T instance;
}