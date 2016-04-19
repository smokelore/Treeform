using UnityEngine;
using System.Collections;

public class SimpleRotate : MonoBehaviour 
{
	public float rotationSpeed;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		this.transform.RotateAround(this.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
	}
}
