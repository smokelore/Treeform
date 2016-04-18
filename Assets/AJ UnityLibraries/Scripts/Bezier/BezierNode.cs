using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierNode {
	
	public Vector3 Point, LeftControl, RightControl;
	public static BezierNode Zero {
		get {
			return new BezierNode(Vector3.zero, Vector3.zero, Vector3.zero);
		}
	}
	bool mapped;
	
	public BezierNode(Vector3 Point, Vector3 LeftControl, Vector3 RightControl){
		this.Point = Point;
		this.LeftControl = LeftControl;
		this.RightControl = RightControl;
	}

	public BezierNode(Vector3 Point) : this (Point, Point, Point){}	
}