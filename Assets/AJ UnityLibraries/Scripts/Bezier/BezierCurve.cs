using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierCurve {

	public List<BezierSegment> Curve;

	public float Length {
		get {
			if (!mapped){
				Map();
			}
			return length;
		}
	}
	public bool Closed {
		get {
			return closed;
		}
	}
	protected float length;
	protected bool mapped, closed;
	protected int resolution;
	BezierNode firstNode;

	public BezierCurve(){
		Curve = new List<BezierSegment>();
	}

	public List<BezierNode> GetNodes(){

		List<BezierNode> nodes = new List<BezierNode>();
		int length = closed ? Curve.Count - 1 : Curve.Count;

		nodes.Add(Curve[0].Start);
		for (int i = 0; i < length; i++){
			nodes.Add(Curve[i].End);
		}
		return nodes;
	}

	public void Add(BezierNode Node){
		if (closed){
			if (mapped){
				length -= Curve[Curve.Count - 1].Length;
			}
			Curve.RemoveAt(Curve.Count - 1);
			closed = false;
		}
		if (Curve.Count > 0){
			BezierSegment segment = new BezierSegment(Curve[Curve.Count - 1].End, Node);
			Curve.Add(segment);
		}
		else if (firstNode != null) {
			BezierSegment segment = new BezierSegment(firstNode, Node);
			Curve.Add(segment);
		}
		else {
			firstNode = Node;
		}
		if (mapped){
			Curve[Curve.Count - 1].Map(resolution);
			length += Curve[Curve.Count - 1].Length;
		}
	}


	public void Close(){
		if (!closed){
			Curve.Add(new BezierSegment(Curve[Curve.Count - 1].End, Curve[0].Start));
			if (mapped){
				Curve[Curve.Count - 1].Map(resolution);
				length += Curve[Curve.Count - 1].Length;
			}
			closed = true;
		}
	}

	public void Smooth(float strength = .25f){
		for (int i = 0; i < Curve.Count; i++){
			BezierSegment node = Curve[i];
			Vector3 handle = NextSegment(i).Start.Point - PreviousSegment(i).End.Point;
			handle *= .5f * strength;
			node.Start.LeftControl = node.Start.Point - handle;
			node.Start.RightControl = node.Start.Point + handle;
		}
	}

	public BezierSegment NextSegment(int index){
		return Curve[(index + 1)%Curve.Count];
	}

	public BezierSegment PreviousSegment(int index){
		return Curve[index == 0 ? Curve.Count - 1 : index - 1];
	}

	public Vector3 Sample(float t){
		int node = Mathf.FloorToInt(t * Curve.Count);
		BezierSegment b = Curve[node];
		return b.Sample((t - (node/(float)Curve.Count)) * Curve.Count);
	}
	

	public Vector3 SampleConstant(float t){
		float targetLength = t * Length;
		int i = Curve.Count/2;
		int halving = 2;
		while (targetLength > arcLengths[i] || targetLength < (i == 0 ? 0 : arcLengths[i - 1])){
			halving *= 2;
			if (targetLength < arcLengths[i]){
				i = i - Mathf.Clamp(Curve.Count/halving, 1, Curve.Count);
			}
			else {
				i = i + Mathf.Clamp(Curve.Count/halving, 1, Curve.Count);
			}
		}
		return Curve[i].SampleConstant((targetLength - arcLengths[i==0 ? 0 : i-1])/Curve[i].Length);
	}

	float[] arcLengths;
	public void Map(int resolution = 25){
		this.resolution = resolution;
		length = 0;

		arcLengths = new float[Curve.Count];
		for (int i = 0; i < Curve.Count; i++){
			BezierSegment b = Curve[i];
			b.Map(resolution);
			arcLengths[i] = length + b.Length;
			length = arcLengths[i];
		}
		mapped = true;
	}

	public class PointInfo {
		public int SegmentIndex;
		public float Time;
		public Vector3 Point;
	}
	
	public PointInfo GetClosestPoint (Vector3 point, int iterations, float minDistance = 0) {

		PointInfo closest = null;
		float dist = float.MaxValue;
		
		for (int i = 0; i < Curve.Count; i++){

			PointInfo p2 = GetClosestPoint(point, iterations, i);

			float distance = Vector3.Distance(p2.Point, point);
			if (distance < dist){
				closest = p2;
				dist = distance;
				if (dist < minDistance){
					break;
				}
			}
		}

		return closest;
	}
	
	public List<PointInfo> GetClosestPoints(Vector3 point, int iterations, float minDistance) {

		List<PointInfo> closestPoints = new List<PointInfo>(); 
		PointInfo closest = null;
		float dist = float.MaxValue;
		
		for (int i = 0; i < Curve.Count; i++){
			
			PointInfo p2 = GetClosestPoint(point, iterations, i);
			//Debug.DrawRay(p2.Point, Curve[p2.SegmentIndex].GetNormalConstant(p2.Time));
			
			float distance = Vector3.Distance(p2.Point, point);
			if (distance < dist){
				closest = p2;
				dist = distance;
			}

			if (distance < minDistance){
				closestPoints.Add(p2);
			}
		}

		if (closestPoints.Count == 0){
			closestPoints.Add(closest);
		}
		
		return closestPoints;
	}

	PointInfo GetClosestPoint(Vector3 point, int iterations, int segmentIndex){

		BezierSegment seg = Curve[segmentIndex];

		PointInfo closestPoint = new PointInfo();
		closestPoint.SegmentIndex = segmentIndex;
		closestPoint.Time = 0;

		float closestDistance = float.MaxValue;
		float time = 0;

		for (int i = 1; i <= iterations; i++){
			Vector3 p2 = seg.SampleConstant(time);
			float dist = Vector3.Distance(point, p2);
			if (dist < closestDistance){
				closestPoint.Point = p2;
				closestPoint.Time = time;
				closestDistance = dist;
			}
			time += 1f / iterations;
		}
		return closestPoint;
	}

	public void Draw(){
		foreach (BezierSegment seg in Curve){
			seg.Draw();
		}
	}

	public void DrawWithNormals(){
		foreach (BezierSegment seg in Curve){
			seg.DrawWithNormals();
		}
	}
}