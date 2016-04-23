using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
	public FractalNode root;
	public float rotationSpeed;
	public Vector3 offset;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		if (root != null)
		{
			this.transform.LookAt(root.transform.position + offset);
			this.transform.RotateAround(root.transform.position + offset, Vector3.up, rotationSpeed * Time.deltaTime);
		}
	}
}
