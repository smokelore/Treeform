using UnityEngine;
using System.Collections;

public class GameObjectPool : ObjectPool<GameObject> {

	public override void Init (){
		base.Init ();
		foreach (GameObject g in pool){
			g.SetActive(false);
		}
	}

	public override GameObject RequestObject (){
		GameObject g = base.RequestObject ();
		g.SetActive(true);
		return g;
	}

	public override void ReturnObject (GameObject obj){
		if (obj.activeSelf){
			obj.SetActive(false);
		}
		base.ReturnObject (obj);
	}
}
