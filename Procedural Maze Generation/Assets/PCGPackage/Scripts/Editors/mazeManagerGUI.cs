using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Class to create GUI elements for gridManager Class
//allows the attaching of scripts to buttons
//Dependant Classes - gridManager
//by Andrew Bowden
//Last Updated - 29/07/2017

[CustomEditor(typeof(mazeManager))]
public class mazeManagerGUI : Editor {

	//Create Editor GUI elements
	public override void OnInspectorGUI() {
		mazeManager manager = (mazeManager)target;

		manager.CellPrefab = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Cell Prefab", "Cell data structure prefab. Must inherit from cell class."), manager.CellPrefab, typeof(GameObject), false);
		manager.WallPrefab = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Wall Prefab", "Prefab to be placed as wall object."), manager.WallPrefab, typeof(GameObject), false);
		manager.Width = EditorGUILayout.IntField (new GUIContent("Width", "How many cells wide?"), manager.Width);
		manager.Height = EditorGUILayout.IntField (new GUIContent("Height", "How many cells high?"), manager.Height);
		//manager.Size = EditorGUILayout.FloatField (new GUIContent("Size", "Size of a cell."), manager.Size);

		EditorGUILayout.BeginHorizontal ();
		manager.Seed = EditorGUILayout.IntField (new GUIContent("Seed", "Seed for Random Generator"), manager.Seed);

		if (GUILayout.Button ("Random Seed", GUILayout.Width(100))) {
			manager.Seed = Random.Range(0, 1000000001);
		}

		EditorGUILayout.EndHorizontal ();



		manager.Algorithm = (mazeManager.SelectAlgorithm) EditorGUILayout.EnumPopup ("Select Algorithm:", manager.Algorithm);
		manager.Braiding = EditorGUILayout.Slider (new GUIContent("Braiding", "Chance that a dead end will be removed."), manager.Braiding, 0.0f, 1.0f);

		if(GUILayout.Button("Generate")){
			manager.ClearMaze();
			manager.RunGeneration(); 
		}
		if (GUILayout.Button ("Clear")) {
			manager.ClearMaze ();
		}
			
		if (manager.Height < 2)
			manager.Height = 2;
		if (manager.Width < 2)
			manager.Width = 2;

	}
}
