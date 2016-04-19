using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
	public FractalNode root;
	public float rotationSpeed;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		if (root != null)
		{
			this.transform.LookAt(root.transform.position);
			this.transform.RotateAround(root.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
		}
	}
}
