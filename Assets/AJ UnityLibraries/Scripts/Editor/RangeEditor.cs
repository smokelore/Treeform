using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer (typeof (IntRange))]
public class IntRangeDrawer : PropertyDrawer {
	public override void OnGUI (Rect pos, SerializedProperty prop, GUIContent label) {
		SerializedProperty start = prop.FindPropertyRelative ("Min");
		SerializedProperty end = prop.FindPropertyRelative ("Max");


		EditorGUI.LabelField (new Rect (pos.x, pos.y, pos.width, pos.height), label);

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		EditorGUI.PrefixLabel(
			new Rect (pos.x + pos.width/2f, pos.y, 40, pos.height),
			new GUIContent("Min"));

		EditorGUI.PropertyField (
			new Rect (pos.x + pos.width/2f + 40, pos.y, (pos.width * .25f - 40), pos.height),
			start, GUIContent.none);

		EditorGUI.PrefixLabel(
			new Rect (pos.x + pos.width/2f + pos.width * .25f, pos.y, 40, pos.height),
			new GUIContent("Max"));
		

		EditorGUI.PropertyField (
			new Rect (pos.x + pos.width/2f + pos.width * .25f + 40, pos.y, (pos.width * .25f - 40), pos.height),
			end, GUIContent.none);

		EditorGUI.indentLevel = indent;

		if (start.intValue > end.intValue){
			end.intValue = start.intValue;
		}
	}
}

[CustomPropertyDrawer (typeof (FloatRange))]
public class FloatRangeDrawer : PropertyDrawer {
	public override void OnGUI (Rect pos, SerializedProperty prop, GUIContent label) {
		SerializedProperty start = prop.FindPropertyRelative ("Min");
		SerializedProperty end = prop.FindPropertyRelative ("Max");
		
		
		EditorGUI.LabelField (new Rect (pos.x, pos.y, pos.width, pos.height), label);
		
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		EditorGUI.PrefixLabel(
			new Rect (pos.x + pos.width/2f, pos.y, 40, pos.height),
			new GUIContent("Min"));
		
		EditorGUI.PropertyField (
			new Rect (pos.x + pos.width/2f + 40, pos.y, (pos.width * .25f - 40), pos.height),
			start, GUIContent.none);
		
		EditorGUI.PrefixLabel(
			new Rect (pos.x + pos.width/2f + pos.width * .25f, pos.y, 40, pos.height),
			new GUIContent("Max"));
		
		
		EditorGUI.PropertyField (
			new Rect (pos.x + pos.width/2f + pos.width * .25f + 40, pos.y, (pos.width * .25f - 40), pos.height),
			end, GUIContent.none);
		
		EditorGUI.indentLevel = indent;
		
		if (start.floatValue > end.floatValue){
			end.floatValue = start.floatValue;
		}
	}
}