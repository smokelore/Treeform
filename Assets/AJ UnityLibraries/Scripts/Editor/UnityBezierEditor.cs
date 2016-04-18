using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(UnityBezier))]
public class UnityBezierEditor : Editor {

	Texture2D text;
	int currentControl, currentNode;

	void OnEnable(){
		SceneView.onSceneGUIDelegate += DrawSceneGUI;
		if (text == null){
			text = new Texture2D(1,1);
			text.SetPixel(0,0, Color.white);
			text.Apply();
		}
	}

	void OnDisable(){
		SceneView.onSceneGUIDelegate -= DrawSceneGUI;
		AssetDatabase.SaveAssets();
	}

	void DrawSceneGUI(SceneView view){
		UnityBezier curve = (UnityBezier) target;
		if (GUIUtility.hotControl != 0) currentControl = GUIUtility.hotControl;
		if (curve.Nodes != null){
			for (int i = 0; i < curve.Nodes.Count; i++){
				UnityBezier.Node node = curve.Nodes[i];
				Vector3 newPoint = Handles.Slider2D(3 * i + 1, curve.Position + node.Point, Vector3.back, Vector3.up, Vector3.right, view.size * .03f, Handles.CylinderCap, .5f * Vector2.one) - curve.Position;
				if (curve.Snap){
					newPoint = snapPoint(newPoint, curve.SnapSize);
				}
				Vector3 diff = newPoint - node.Point;
				node.Point = newPoint;

				if ((currentControl >= (3 * i + 1) && currentControl <= (3 * i + 3))){
					currentNode = i;
					if (curve.Snap){
						node.LeftHandle = snapPoint(diff + Handles.Slider2D(3 * i + 2, curve.Position + node.LeftHandle, Vector3.back, Vector3.up, Vector3.right, view.size * .01f, Handles.CircleCap, .5f * Vector2.one) - curve.Position, curve.SnapSize);
						node.RightHandle = snapPoint(diff + Handles.Slider2D(3 * i + 3, curve.Position + node.RightHandle, Vector3.back, Vector3.up, Vector3.right, view.size * .01f, Handles.CircleCap, .5f * Vector2.one) - curve.Position, curve.SnapSize);
					}
					else {
						node.LeftHandle = diff + Handles.Slider2D(3 * i + 2, curve.Position + node.LeftHandle, Vector3.back, Vector3.up, Vector3.right, view.size * .01f, Handles.CircleCap, .5f * Vector2.one) - curve.Position;
						node.RightHandle = diff + Handles.Slider2D(3 * i + 3, curve.Position + node.RightHandle, Vector3.back, Vector3.up, Vector3.right, view.size * .01f, Handles.CircleCap, .5f * Vector2.one) - curve.Position;
					}
					Handles.DrawDottedLine(curve.Position + node.Point, curve.Position + node.LeftHandle, view.size);
					Handles.DrawDottedLine(curve.Position + node.Point, curve.Position + node.RightHandle, view.size);

//					Handles.Label(node.Point + view.size * .065f * Vector3.up + view.size * .025f * Vector3.right, "Smooth");
				}
			}
			for (int i = 0; i < (curve.Closed ? curve.Nodes.Count : curve.Nodes.Count - 1); i++){
				UnityBezier.Node node = curve.Nodes[i];
				UnityBezier.Node otherNode = curve.Nodes[(i + 1)%curve.Nodes.Count];
				Handles.DrawBezier(curve.Position + node.Point, curve.Position + otherNode.Point, curve.Position + node.RightHandle, curve.Position + otherNode.LeftHandle, Color.white, text, .5f);
			}
			EditorUtility.SetDirty(target);
		}
	}

	public override void OnInspectorGUI (){
		base.OnInspectorGUI ();
		UnityBezier curve = (UnityBezier) target;
		GUILayout.Space(5f);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Node")){
			if (curve.Nodes == null){
				curve.Nodes = new List<UnityBezier.Node>();
			}
			UnityBezier.Node node = new UnityBezier.Node();
			if (curve.Nodes.Count == 0){
				node.Point = Vector3.zero;
				node.LeftHandle = Vector3.left;
				node.RightHandle = Vector3.right;
			}
			else {
				UnityBezier.Node lastNode = curve.Nodes[curve.Nodes.Count - 1];
				node.Point = curve.Nodes.Count > 0 ? 2 * curve.Nodes[curve.Nodes.Count - 1].RightHandle - curve.Nodes[curve.Nodes.Count - 1].Point : Vector3.zero;
				node.LeftHandle = node.Point + (lastNode.Point - node.Point).normalized;
				node.RightHandle = node.Point - (lastNode.Point - node.Point).normalized;
			}
			curve.Nodes.Add(node);
		}
		if (GUILayout.Button(curve.Closed ? "Open" : "Close")){
			curve.Closed = !curve.Closed;
		}
		if (GUILayout.Button("Apply Snap")){
			curve.ApplySnap();
		}
		GUILayout.EndHorizontal();
		if (curve.Nodes != null){
			GUILayout.Space(5f);
			for (int i = 0; i < curve.Nodes.Count; i++){
				UnityBezier.Node node = curve.Nodes[i];
				if (i == currentNode){
					node.Point = EditorGUILayout.Vector3Field("Node (Selected)", node.Point);
				}
				else {
					node.Point = EditorGUILayout.Vector3Field("Node", node.Point);
				}
				node.LeftHandle = EditorGUILayout.Vector3Field("    Left Handle", node.LeftHandle);
				node.RightHandle = EditorGUILayout.Vector3Field("    Right Handle", node.RightHandle);
				GUILayout.BeginHorizontal();
				GUILayout.Space(EditorGUIUtility.currentViewWidth/5f);
				if (GUILayout.Button("Insert Node After")){
					curve.SubdivideAt(i);
					break;
				}
				if (GUILayout.Button("Smooth")){
					Vector3 average = (node.LeftHandle + node.RightHandle)/2f;
					if (average != node.Point){
						average = average - node.Point;
						node.LeftHandle -= Vector3.Project(node.LeftHandle - node.Point, average);
						node.RightHandle -= Vector3.Project(node.RightHandle - node.Point, average);
					}
				}
				if (GUILayout.Button("Delete")){
					curve.Nodes.RemoveAt(i);
					i--;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
			}
			EditorUtility.SetDirty(target);
		}
	}

	Vector3 snapPoint(Vector3 point, float snap){
		point.x = Mathf.Round(point.x / snap) * snap;
		point.y = Mathf.Round(point.y / snap) * snap;
		point.z = Mathf.Round(point.z / snap) * snap;
		return point;
	}

	[MenuItem("Assets/Create/Bezier Curve")]
	public static void CreateAsset(){
		ScriptableObjectUtility.CreateAsset<UnityBezier>();
	}
}