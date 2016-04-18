using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityBezier : ScriptableObject {

	public Vector3 Position;
	public bool Snap = true;
	public float SnapSize = .3f;
	[HideInInspector]
	public List<Node> Nodes;
	[HideInInspector]
	public bool Closed = false;

	[System.Serializable]
	public class Node {
		public Vector3 Point, LeftHandle, RightHandle;

		public void Scale(float scale){
			Point *= scale;
			LeftHandle *= scale;
			RightHandle *= scale;
		}

		public void Scale(float scaleX, float scaleY){
			Point.x *= scaleX;
			Point.y *= scaleY;

			LeftHandle.x *= scaleX;
			LeftHandle.y *= scaleY;

			RightHandle.x *= scaleX;
			RightHandle.y *= scaleY;
		}

		public void Add(Vector3 displacement){
			Point += displacement;
			LeftHandle += displacement;
			RightHandle += displacement;
		}
	}

	public void Add(Vector3 point){
		Add (point, point, point);
	}

	public void Add(Vector3 point, Vector3 left, Vector3 right){
		Node n = new Node();
		n.Point = point;
		n.LeftHandle = left;
		n.RightHandle = right;

		if (Nodes == null){
			Nodes = new List<Node>();
		}
		Nodes.Add(n);
	}

	public void ApplySnap(){
		foreach (Node n in Nodes){
			n.Point = n.Point.Snap(SnapSize);
			n.LeftHandle = n.LeftHandle.Snap(SnapSize);		
			n.RightHandle = n.RightHandle.Snap(SnapSize);
		}
	}

	public void SubdivideAt(int index){
		if (index < Nodes.Count - 1){
			Node a = Nodes[index];
			Node b = Nodes[index + 1];

			Node c = new Node();
			c.Point = (a.Point + b.Point)/2f;
			c.LeftHandle = (a.Point + c.Point)/2f;
			c.RightHandle = (c.Point + b.Point)/2f;
			Nodes.Insert(index + 1, c);
		}
	}

	public BezierCurve GetCurve(){
		BezierCurve c = new BezierCurve();
		foreach (Node n in Nodes){
			c.Add(new BezierNode(Position + n.Point, Position + n.LeftHandle, Position + n.RightHandle));
		}
		if (Closed){
			c.Close();
		}
		return c;
	}	
}