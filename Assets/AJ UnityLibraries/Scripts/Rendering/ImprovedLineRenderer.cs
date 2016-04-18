using System; 
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ImprovedLineRenderer : MonoBehaviour { 

	private Vector3[] vertices = new Vector3[0]{};
	[HideInInspector]
	public LineRenderer Line;

	void Awake(){
		Line = GetComponent<LineRenderer>();
	}

	public void SetPosition(int index, Vector3 position)
	{
		if (index > vertices.Length - 1){
			SetVertexCount(index + 1);
		}
		vertices[index] = position;
		Line.SetPosition(index, position);
	}
	
	public void SetVertexCount(int count)
	{
		Array.Resize(ref vertices, count);
		Line.SetVertexCount(count);
	}
	
	public Vector3 GetPosition(int index){
		return vertices[index];
	}

	public float GetLength(){
		float length = 0;
		for (int i = 0; i < vertices.Length - 1; i++){
			length += Vector3.Distance(vertices[i], vertices[i + 1]);
		}
		return length;
	}
}