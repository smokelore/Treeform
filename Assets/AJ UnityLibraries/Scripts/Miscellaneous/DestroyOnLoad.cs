using UnityEngine;
using System.Collections;

public class DestroyOnLoad : MonoBehaviour {

	public bool DetatchChildren;
	public float Delay;

	IEnumerator Start () {
		yield return new WaitForSeconds(Delay);
		if (DetatchChildren){
			transform.DetachChildren();
		}
		Destroy(this.gameObject);
	}
}
