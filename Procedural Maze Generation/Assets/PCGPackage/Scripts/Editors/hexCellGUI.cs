using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Class to create GUI elements for gridManager Class
//allows the attaching of scripts to buttons
//Dependant Classes - hexCell
//by Andrew Bowden
//Last Updated - 29/07/2017

[CustomEditor(typeof(hexCell))]
public class hexCellGUI : Editor {

	//Create Editor GUI elements
	public override void OnInspectorGUI() {
		hexCell cell = (hexCell)target;

		EditorGUILayout.IntField ("X Coordinate:", cell.X);
		EditorGUILayout.IntField ("Y Coordinate:", cell.Y);
	}
}
