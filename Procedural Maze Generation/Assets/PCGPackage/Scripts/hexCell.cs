using System;
using System.Collections.Generic;
using UnityEngine;

//Class: Hex Cell Object structure
//by Andrew Bowden
//Last Updated 31/07/2017
//REF: http://www.redblobgames.com/grids/hexagons/

[ExecuteInEditMode]
public class hexCell : cell {
	
	private static float sSqrt3Half = Mathf.Sqrt(3) / 2;
	private static float sThreeForths = 3.0f / 4.0f;

	//Initalise all values
	//Input1: flaot: size of hexagon (half width)
	//Input2: int: X coordinate
	//Input3: int: Y coordinate
	//Input4: int: number of columns in grid
	//Input5: int: number of rows in grid
	public override void Initialise(float iSize, int iX, int iY, int iCols, int iRows) {
		mSize = iSize; 
		X = iX;
		Y = iY;

		Index = X + (Y * iCols);
		Vector3 pos = transform.position;

		mWidth = sSqrt3Half * (mSize * 2);
		mHeight = mSize * 2;

		pos.y -= (float) (Y * (mHeight * sThreeForths));

		//find neighbours
		// Amit - Red Blob Games - Hexagonal Grids - Odd Offseet Coord
		// [ Hex(+1,  0), Hex( 0, -1), Hex(-1, -1),
		//	 Hex(-1,  0), Hex(-1, +1), Hex( 0, +1) ],
		// [ Hex(+1,  0), Hex(+1, -1), Hex( 0, -1),
		//	 Hex(-1,  0), Hex( 0, +1), Hex(+1, +1) ]

		initialiseNeighbours (iCols, iRows);
		if (Y % 2 == 1) {	//Odd Row
			pos.x += (float) ((mWidth / 2) + (mWidth * X));
		}
		else {			//Even Row
			pos.x += (float) mWidth * X;
		}
		transform.position = pos;

		//initail walls
		initialiseWalls(iCols);

	}

	//Intialise indexes of neighbouring cells
	//Input1: int: number of columns in grid
	//Input2: int: number of rows in grid
	protected override void initialiseNeighbours(int iCols, int iRows){
		mNeighbours = new List<int>();

		//find neighbours
		// Amit - Red Blob Games - Hexagonal Grids - Odd Offseet Coord
		// [ Hex(+1,  0), Hex( 0, -1), Hex(-1, -1),
		//	 Hex(-1,  0), Hex(-1, +1), Hex( 0, +1) ],
		// [ Hex(+1,  0), Hex(+1, -1), Hex( 0, -1),
		//	 Hex(-1,  0), Hex( 0, +1), Hex(+1, +1) ]

		bool top = Y == 0;
		bool bot = Y == iRows - 1;

		bool left = X == 0;
		bool right = X == iCols - 1;


		if (Y % 2 == 1) {	//Odd Row

			if (!right) { 
				if (!top) {
					mNeighbours.Add (Index + 1 - iCols);
				}
				mNeighbours.Add (Index + 1);
				if (!bot) {
					mNeighbours.Add (Index + 1 + iCols); 
				}
			}

			if (!left) {
				if (!top) {
					mNeighbours.Add (Index - iCols); 
				}
				mNeighbours.Add (Index - 1);
				if (!bot) {
					mNeighbours.Add (Index + iCols); 
				}
			} else {
				if (!top) {
					mNeighbours.Add (Index - iCols);
				}
				if (!bot) {
					mNeighbours.Add (Index + iCols); 
				}
			}


		} else {			//Even Row

			if (!right) {
				if (!top) {
					mNeighbours.Add (Index - iCols); 
				}
				mNeighbours.Add (Index + 1);
				if (!bot) {
					mNeighbours.Add (Index + iCols); 
				}
			} else {
				if (!top) {
					mNeighbours.Add (Index - iCols);
				}
				if (!bot) {
					mNeighbours.Add (Index + iCols);
				}
			}


			if (!left) {
				if (!top) {
					mNeighbours.Add (Index - 1 - iCols); 
				}
				mNeighbours.Add (Index - 1);
				if (!bot) {
					mNeighbours.Add ((Index - 1) + iCols); 
				}
			}
		}
	}

	//Intialise booleans of walls list
	//Input1: int: number of columns in grid
	protected override void initialiseWalls(int iCols) {
		Walls = new List<bool>();

		Walls.Add (true);   //right wall
		Walls.Add (true);	//bottom right wall
		Walls.Add (true);	//bottom left wall
		Walls.Add (false);	//left wall
		Walls.Add (false);	//top left wall
		Walls.Add (false);	//top right wall

		bool top = Y == 0;			//top row
		bool left = X == 0;			//first column
		bool right = X == iCols - 1;	//last column

		bool oddRow = Y % 2 == 1;

		if (left) {	
			Walls [3] = true;
			if (!oddRow) Walls [4] = true;
		}

		if (right && oddRow) { 
			Walls [5] = true;
		}

		if (right && !oddRow) { 
			Walls[5] = false;
		}

		if (top) { 	//first row
			Walls [4] = true;
			Walls [5] = true;
		}
	}

	//Removes wall between adjacent cell (sets wall values to false)
	//Output: bool: returns true if all removed
	//			increments wallRemoved
	//Output: bool: returns false if no wall removed
	//Input: cell: the adjacent cell
	public override bool RemoveWall(cell iOther){
		if (iOther != null && typeof(hexCell) == iOther.GetType()) {

			//the other grid ref is to the...
			bool above = Y > iOther.Y;
			bool below = Y < iOther.Y;

			bool left = X > iOther.X;
			bool right = X < iOther.X;

			if (Y % 2 == 1) { // odd row
				if (left) {
					if (this.Walls [3] || iOther.Walls [0]) {
						this.Walls [3] = false;
						iOther.Walls [0] = false;
						this.WallsRemoved++;
						iOther.WallsRemoved++;
						return true;
					}
				} else if (right) {
					if (above) {
						if (this.Walls [5] || iOther.Walls [2]) {
							this.Walls [5] = false;
							iOther.Walls [2] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					} else if (below) {
						if (this.Walls [1] || iOther.Walls [4]) {
							this.Walls [1] = false;
							iOther.Walls [4] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					} else {
						if (this.Walls [0] || iOther.Walls [3]) {
							this.Walls [0] = false;
							iOther.Walls [3] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					}
				} else {
					if (above) {
						if (this.Walls [4] || iOther.Walls [1]) {
							this.Walls [4] = false;
							iOther.Walls [1] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					} else if (below) {
						if (this.Walls [2] || iOther.Walls [5]) {
							this.Walls [2] = false;
							iOther.Walls [5] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					}
				}
			} else { //even row
				if (left) {
					if (above) {
						if (this.Walls [4] || iOther.Walls [1]) {
							this.Walls [4] = false;
							iOther.Walls [1] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					} else if (below) {
						if (this.Walls [2] || iOther.Walls [5]) {
							this.Walls [2] = false;
							iOther.Walls [5] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					} else {
						if (this.Walls [3] || iOther.Walls [0]) {
							this.Walls [3] = false;
							iOther.Walls [0] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					}
				} else if (right) {
					if (this.Walls [0] || iOther.Walls [3]) {
						this.Walls [0] = false;
						iOther.Walls [3] = false;
						this.WallsRemoved++;
						iOther.WallsRemoved++;
						return true;
					}
				} else {
					if (above) {
						if (this.Walls [5] || iOther.Walls [2]) {
							this.Walls [5] = false;
							iOther.Walls [2] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					} else if (below) {
						if (this.Walls [1] || iOther.Walls [4]) {
							this.Walls [1] = false;
							iOther.Walls [4] = false;
							this.WallsRemoved++;
							iOther.WallsRemoved++;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	//Places walls around cell
	//Input1: GameObject: Wall Prefab, the wall to be copied
	//Output: List of GameObjects, created from iWallPrefab
	public override List<GameObject> CreateWalls(GameObject iWallPrefab){
		List<GameObject> oList = new List<GameObject> ();
		for (int ii = 0; ii < Walls.Count; ii++) {
			if (Walls [ii]) {
				int rot = -(ii * 60);

				Vector3 euler = new Vector3 (0, 0, rot);
				Quaternion rotation = Quaternion.Euler(euler);
				Vector3 position = this.transform.position + (rotation * (Vector3.right * (mWidth / 2)));
				GameObject wall = Instantiate (iWallPrefab, position, rotation, this.transform);
				oList.Add (wall);
			}
		}
		return oList;
	}
}
