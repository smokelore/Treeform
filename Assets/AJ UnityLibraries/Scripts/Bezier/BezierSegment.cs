using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierSegment {
	
	public BezierNode Start, End;
	public float Length {
		get {
			if (!mapped){
				Map ();
			}
			return internal_length();
		}
	}
	protected int resolution = 0;
	protected bool mapped;

	public bool IsStraight {
		get {
			Vector3 temp = (End.Point - Start.Point).normalized;
			return (((End.Point - End.LeftControl).normalized == temp || (End.Point == End.LeftControl)) && ((Start.RightControl - Start.Point).normalized == temp || (Start.Point == Start.RightControl)));
		}
	}
	
	public BezierSegment(BezierNode Start, BezierNode End){
		this.Start = Start;
		this.End = End;
	}

	protected float internal_length(){
		return arcLengths[arcLengths.Length - 1];
	}

	
	public Vector3 Sample(float t){
		return (1 - t) * (1 - t) * (1 - t) * Start.Point + 3 * t * (1 - t) * (1 - t) * Start.RightControl + 3 * t * t * (1 - t) * End.LeftControl + t * t * t * End.Point;
	}

	public Vector3 SampleConstant(float t, int resolution = 25){
		if (!mapped || resolution != this.resolution){
			Map(resolution);
		}
		if (t == 1) return Sample (1f);
		if (t == 0) return Sample (0f);

		float targetLength = Length * t;
		int i = 1;
		while (arcLengths[i] <= targetLength){
			i++;
		}
//		Debug.Log("Target " + targetLength + " Length " + arcLengths[i-1] + " Lenght 2 " + arcLengths[i]);
		float adjT = unlerp(arcLengths[i - 1], arcLengths[i], targetLength);
		return Sample((i - 1 + adjT)/(float)resolution);
	}

	const float NORMAL_FACTOR = 0.001f;
	public Vector3 GetNormalConstant(float t){
		Vector3 normal = Vector3.zero;
		if (t < NORMAL_FACTOR){
			normal = (SampleConstant(t + NORMAL_FACTOR) - SampleConstant(t));
		}
		else if (t > (1 - NORMAL_FACTOR)){
			normal = (SampleConstant(t) - SampleConstant(t - NORMAL_FACTOR));
		}
		else {
			normal = (SampleConstant(t) - SampleConstant(t - NORMAL_FACTOR));
			normal += (SampleConstant(t + NORMAL_FACTOR) - SampleConstant(t));
		}

		normal.Normalize();
		
		normal.z = normal.x;
		
		normal.x = -normal.y;
		normal.y = normal.z;
		normal.z = 0;

		return normal;
	}
	
	public Vector3 GetTangentConstant(float t){
		Vector3 tangent = Vector3.zero;
		if (t < NORMAL_FACTOR){
			tangent = (SampleConstant(t + NORMAL_FACTOR) - SampleConstant(t));
		}
		else if (t > (1 - NORMAL_FACTOR)){
			tangent = (SampleConstant(t) - SampleConstant(t - NORMAL_FACTOR));
		}
		else {
			tangent = (SampleConstant(t) - SampleConstant(t - NORMAL_FACTOR));
			tangent += (SampleConstant(t + NORMAL_FACTOR) - SampleConstant(t));
		}
		tangent.Normalize();
		return tangent;
	}

	float unlerp(float start, float end, float val){
		return (val - start)/(end - start);
	}

	float[] arcLengths;
	public void Map(int resolution = 25){
		this.resolution = resolution;
		mapped = true;

		arcLengths = new float[resolution + 1];

		float length = 0;
		arcLengths[0] = 0;
		for (int i = 1; i <= resolution; i++){
			arcLengths[i] = length + Vector3.Distance(Sample((i - 1f)/resolution), Sample(i/(float)resolution));
			length = arcLengths[i];
		}
	}

	const float DRAW_RESOLUTION = 100f;
	public void Draw(){
		for (int i = 0; i < DRAW_RESOLUTION; i++){
			Debug.DrawLine(Sample(Mathf.Clamp01(i / DRAW_RESOLUTION)), Sample(Mathf.Clamp01((i + 1) / DRAW_RESOLUTION)));
		}
	}
	
	public void DrawWithNormals(){
		Debug.DrawLine(Sample (0f) - GetNormalConstant(0f), Sample (0f) + GetNormalConstant(0f), Color.red);
		for (int i = 0; i < DRAW_RESOLUTION; i++){
			Debug.DrawLine(SampleConstant(Mathf.Clamp01(i / DRAW_RESOLUTION)), SampleConstant(Mathf.Clamp01((i + 1) / DRAW_RESOLUTION)), Color.white);
			Debug.DrawRay(SampleConstant(i / DRAW_RESOLUTION), GetNormalConstant(i / DRAW_RESOLUTION), Color.blue);
			//Debug.DrawRay(SampleConstant(i / DRAW_RESOLUTION), Vector3.right, Color.blue);
		}
		Debug.DrawLine(Sample (1f) - GetNormalConstant(1f), Sample (1f) + GetNormalConstant(1f), Color.yellow);
	}
}