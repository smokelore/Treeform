using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public Vector3 Direction;
	public float Speed;

	void Update () {
		transform.Rotate(Direction * Speed * Time.deltaTime / .02f, Space.Self);
	}
}
