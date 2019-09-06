using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class: Cell abstract structure
//by Andrew Bowden
//Last Updated 31/07/2017


abstract public class cell : MonoBehaviour {

	//Size of the cell
	protected float mSize;
	//Width and Height of the cell
	protected float mWidth;
	protected float mHeight;

	//Coordinates of the cell
	public int X, Y;
	//index in the grid array for cell
	public int Index; 

	//marked as true if cell is visited by an algorithm
	public bool Visited = false;

	//indices of adjacent cells
	protected List<int> mNeighbours; 	 
	public List<bool> Walls; 
	public int WallsRemoved = 0;


	//Initalise all values
	//Input1: flaot: size of cell
	//Input2: int: X coordinate
	//Input3: int: Y coordinate
	//Input4: int: number of columns in grid
	//Input5: int: number of rows in grid
	public abstract void Initialise (float iSize, int iX, int iY, int iCols, int iRows);

	//Intialise indexes of neighbouring cells
	//Input1: int: number of columns in grid
	//Input2: int: number of rows in grid
	protected abstract void initialiseNeighbours (int iCols, int iRows);

	//Intialise booleans of walls list
	//Input1: int: number of columns in grid
	protected abstract void initialiseWalls (int iCols);

	//Returns list of neighbour indices
	public List<int> NeighbourIndices() {
		return mNeighbours;
	}

	//Removes wall between adjacent cell (sets wall values to false)
	//Output: bool: returns true if all removed
	//			increments wallRemoved
	//Output: bool: returns false if no wall removed
	//Input: cell: the adjacent cell
	public abstract bool RemoveWall (cell iOther);

	//Places walls around cell
	//Input1: GameObject: Wall Prefab, the wall to be copied
	//Output: List of GameObjects, created from iWallPrefab
	public abstract List<GameObject> CreateWalls (GameObject iWallPrefab);

	//Returns true if the cell is a dead end
	public bool DeadEnd () {
		//if only one wall is removed then cell is a dead end
		return WallsRemoved < 2;
	}
}
