#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SideScrollActionPlayer))]
public class SideScrollActionPlayerEditor : Editor
{
	bool[] isOpen=new bool[5];
	int counter=0;
	public override void OnInspectorGUI(){
		serializedObject.Update();
		
		counter=0;
		var script=target as SideScrollActionPlayer;
		isOpen[counter]=EditorGUILayout.Foldout(isOpen[counter],"基本データ");
		if(isOpen[counter]){
			var walkSpeed=serializedObject.FindProperty("walkSpeed");
			var jumpPow=serializedObject.FindProperty("jumpPow");
			var gravity=serializedObject.FindProperty("gravity");
			var additionalJumpMaxTime=serializedObject.FindProperty("additionalJumpMaxTime");
			var additionalJumpPow=serializedObject.FindProperty("additionalJumpPow");
			EditorGUILayout.PropertyField(walkSpeed);
			EditorGUILayout.PropertyField(jumpPow);
			EditorGUILayout.PropertyField(gravity);
			EditorGUILayout.PropertyField(additionalJumpMaxTime);
			EditorGUILayout.PropertyField(additionalJumpPow);
		}
		EditorGUILayout.Space();
		
		counter++;
		isOpen[counter]=EditorGUILayout.Foldout(isOpen[counter],"接地判定のRaycast");
		if(isOpen[counter]){
			var rayType=serializedObject.FindProperty("rayType");
			var rayOrigin=serializedObject.FindProperty("rayOrigin");
			var rayDirection=serializedObject.FindProperty("rayDirection");
			var rayLength=serializedObject.FindProperty("rayLength");
			var rayBoxSize=serializedObject.FindProperty("rayBoxSize");
			var raySphereRadius=serializedObject.FindProperty("raySphereRadius");
			var targetLayerNum=serializedObject.FindProperty("targetLayerNum");
			EditorGUILayout.PropertyField(rayType);
			EditorGUILayout.PropertyField(rayOrigin);
			EditorGUILayout.PropertyField(rayDirection);
			EditorGUILayout.PropertyField(rayLength);
			EditorGUILayout.PropertyField(rayBoxSize);
			EditorGUILayout.PropertyField(raySphereRadius);
			EditorGUILayout.PropertyField(targetLayerNum);
		}
		EditorGUILayout.Space();
		
		counter++;
		isOpen[counter]=EditorGUILayout.Foldout(isOpen[counter],"画面の進行方向");
		if(isOpen[counter]){
			var rayDirectionForward=serializedObject.FindProperty("rayDirectionForward");
			EditorGUILayout.PropertyField(rayDirectionForward);
		}
		EditorGUILayout.Space();
		
		counter++;
		isOpen[counter]=EditorGUILayout.Foldout(isOpen[counter],"進行方向の障害物判定のRaycast");
		if(isOpen[counter]){
			var rayOriginForward=serializedObject.FindProperty("rayOriginForward");
			var rayLengthForward=serializedObject.FindProperty("rayLengthForward");
			EditorGUILayout.PropertyField(rayOriginForward);
			EditorGUILayout.PropertyField(rayLengthForward);
		}
		EditorGUILayout.Space();
		
		counter++;
		isOpen[counter]=EditorGUILayout.Foldout(isOpen[counter],"坂道判定のRaycast");
		if(isOpen[counter]){
			var rayOriginHill=serializedObject.FindProperty("rayOriginHill");
			var rayLengthHill=serializedObject.FindProperty("rayLengthHill");
			EditorGUILayout.PropertyField(rayOriginHill);
			EditorGUILayout.PropertyField(rayLengthHill);
		}
		
		serializedObject.ApplyModifiedProperties();
	}
}
#endif