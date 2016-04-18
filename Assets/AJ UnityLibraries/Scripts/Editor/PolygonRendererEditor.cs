using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor(typeof(PolygonRenderer))]
public class PolygonRendererEditor : Editor {

	int n;
	float height;

	public override void OnInspectorGUI (){
		base.OnInspectorGUI ();
		GUILayout.BeginHorizontal();
		GUILayout.Space(EditorGUIUtility.labelWidth);
		if (GUILayout.Button("Create N-Gon")){
			Vector2[] verts = new Vector2[n];

			for (int i = 0; i < verts.Length; i++){
				verts[i] = new Vector2(Mathf.Sin(i * Mathf.PI * 2f / n), Mathf.Cos(i * Mathf.PI * 2f / n)) * height;
			}

			PolygonRenderer poly = (serializedObject.targetObject as PolygonRenderer);
			poly.Vertices = verts;
			poly.Build();
		}
		GUILayout.Space(10);
		GUILayout.BeginVertical();
		GUILayout.Label("N");
		GUILayout.Label("Radius");
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		n = EditorGUILayout.IntField(n);
		height = EditorGUILayout.FloatField(height);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Rebuild")){
			(serializedObject.targetObject as PolygonRenderer).Build();
		}
		if (GUILayout.Button("Save Mesh")){
			MeshFilter m = (serializedObject.targetObject as PolygonRenderer).GetComponent<MeshFilter>();
			if (!Directory.Exists(Application.dataPath + "/Meshes")){
				Directory.CreateDirectory(Application.dataPath + "/Meshes");
			}
			AssetDatabase.CreateAsset(m.mesh, "Assets/Meshes/" + m.gameObject.name + " Mesh.asset");
		}
	}
}
