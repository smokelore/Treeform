using UnityEngine;
using System.Collections;

public class SimpleRotate : MonoBehaviour 
{
	public float rotationSpeed;
	public Vector3 rotationAxis;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		this.transform.RotateAround(this.transform.position, rotationAxis, rotationSpeed * Time.deltaTime);
	}
}
