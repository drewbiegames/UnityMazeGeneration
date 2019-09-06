using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Class for Maze Generation
//depedant classes - gridManagerGUI, cell (derived)
//by Andrew Bowden
//Last Updated - 27/07/2017

[ExecuteInEditMode]
public class mazeManager : MonoBehaviour {

	//Amount of cells on each axis
	public int Width = 10;
	public int Height = 10;

	//size of each cell
	public float Size = 1.0f;

	//Seed for random generation
	public int Seed = 1;
	//Likelyhood of a dead end being removed
	public float Braiding = 0.0f;

	//Enum for algorithm selection
	public enum SelectAlgorithm
	{
		None,
		RecursiveBacktracker,
		Prims,
		Kuskals
	};

	//Algorithm Selector
	public SelectAlgorithm Algorithm = SelectAlgorithm.RecursiveBacktracker;

	//Prefabs for game objects
	public GameObject CellPrefab;
	public GameObject WallPrefab;

	//list of cells for algorithms and cleanup functions
	private List<cell> mCells;
	//list of walls for cleanup
	private List<GameObject> mWalls;

	//Called on button press
	//Creates the grid, runs the maze generation algorithm, 
	//braids the maze and initialises walls
	public void RunGeneration(){
		if (CellPrefab != null && WallPrefab != null) {

			//initialise cells
			mCells = new List<cell> ();

			for (int jj = 0; jj < Height; jj++) {
				for (int ii = 0; ii < Width; ii++) {
					GameObject obj = Instantiate (CellPrefab, transform.position, Quaternion.identity, this.transform);
					cell cCell = obj.GetComponent<cell> ();
					cCell.Initialise (Size, ii, jj, Width, Height);
					mCells.Add (cCell);
				}
			}

			//Run chosen algorithm - switch statement
			switch (Algorithm) {
			case SelectAlgorithm.None:
				break;
			case SelectAlgorithm.RecursiveBacktracker:
				recursiveBacktracker ();
				break;
			case SelectAlgorithm.Prims:
				primsAlgorithm ();
				break;
			case SelectAlgorithm.Kuskals:
				kruskalsAlgorithm ();
				break;
			default:
				this.ClearMaze ();
				return;
			}

			//Remove deadends (if applicable)
			braid();
	

			//Create walls
			mWalls = new List<GameObject> ();
			foreach (cell cCell in mCells) {
				if (cCell != null) {
					List<GameObject> walls = cCell.CreateWalls (WallPrefab);
					foreach (GameObject wall in walls) {
						mWalls.Add (wall);
					}
				}
			}
		}
	}

	//Clear all objects, before generating new maze
	//Removes all walls and cells from this maze
	//Removes all child objects
	public void ClearMaze() {
		if (mWalls != null) {
			//destroy all walls
			if (mWalls.Count > 0) {
				foreach (GameObject ww in mWalls) {
					DestroyImmediate (ww);
				}
				mWalls.Clear ();
			}
		}

		if (mCells != null) {
			//destroy all cells
				if (mCells.Count > 0) {
				foreach (cell cc in mCells) {
					DestroyImmediate (cc.gameObject);
				}
				mCells.Clear ();
			}
		}
	}

	//Given list of Indices (iIndices)
	//Outputs list of neighbouring cells
	private List<cell> CellsFromIndices(List<int> iIndices) {
		List<cell> oList = new List<cell> ();

		foreach (int ii in iIndices) {
			if (ii > -1 && ii < mCells.Count) {
				if (mCells[ii] != null &&  mCells [ii].Visited == false) {
					oList.Add (mCells [ii]);
				}
			}
		}
		return oList;
	}


	//Martin Foltin - Automated Maze Generation and Human Interaction - 2011 - p20-22 - Recursive Backtracker
	//Recursive Backtracker
	private void recursiveBacktracker(){
		Stack<int> stack = new Stack<int> ();
		Random.InitState(Seed);

		cell current = mCells [0];
		if (current != null) {
			current.Visited = true;
			stack.Push (current.Index);

			while (stack.Count > 0){
				List<int> adjacentIndices = current.NeighbourIndices ();
				List<cell> adjacentCells = CellsFromIndices (adjacentIndices);

				int count = adjacentCells.Count;
				if (count == 0) { //no unvisted neighbours
					if (stack.Count > 0) {
						current = mCells[stack.Pop ()];
					} else {
						break;
					}
				} else {
					int next = Random.Range(0, count -1);
					stack.Push (adjacentCells [next].Index);
					current.RemoveWall (adjacentCells [next]);
					current = adjacentCells[next];
					current.Visited = true;
				}
			}
		}
	}


	//Martin Foltin - Automated Maze Generation and Human Interaction - 2011 - p15-17 - Randomized Prim's Algorithm#
	// Randomised Prim's Algorithm
	private void primsAlgorithm(){
		Random.InitState(Seed);

		//initialise current inCells
		int currentIndex = 0;
		cell current = mCells [currentIndex];
		current.Visited = true;

		List<int> inCells = new List<int> ();
		inCells.Add (currentIndex);
		//initialise current frontier
		List<int> frontierCells = current.NeighbourIndices ();

		while (!entireGridVisited()) {
			int fIndex = -1;
			cell fCell = null;

			int iIndex = -1;
			cell iCell = null;
			bool find = true;

			do {
				//choose least weight (or Random) frontier cell (cF)
				fIndex = frontierCells[Random.Range(0, frontierCells.Count)];
				fCell = mCells [fIndex];

				//choose adjacent incell (cI) to frontier cell
				List<int> fCellNeighbours = fCell.NeighbourIndices ();
				List<int> adjInCells = new List<int> ();
				foreach (int nI in fCellNeighbours) {
					if (mCells [nI].Visited) {
						adjInCells.Add (nI);
					}
				}
				if (adjInCells.Count > 0) {
					iIndex = adjInCells [Random.Range (0, adjInCells.Count)];
					iCell = mCells [iIndex];
					find = false;
				}
			} while (find);

			//Add cF to inCells
			fCell.Visited = true;
			inCells.Add(fIndex);

			//mark all out-cells around cF as fontier
			//remove visited
			List<int> fNeighbours = fCell.NeighbourIndices();
			foreach (int fInd in fNeighbours) {
				if (!mCells[fInd].Visited && !frontierCells.Contains(fInd)){
					frontierCells.Add(fInd);
				}
			}

			//carve path between cI and cF
			iCell.RemoveWall (fCell);


			//remove duplicates
			//frontierCells.Sort();
			//for (int ii = 1; ii <= frontierCells.Count - 1; ii++) {
			//	if (frontierCells [ii] == frontierCells [ii - 1]) {
			//		frontierCells.RemoveAt (ii);
			//	}
			//}

			//Remove cF from frontierCells
			for(int ind = 0; ind < frontierCells.Count; ind++) {
				if (frontierCells[ind] == fIndex) {
					frontierCells.RemoveAt(ind);
					break;
				}
			}

		}
	}

	//REF: http://www.personal.kent.edu/~rmuhamma/Algorithms/MyAlgorithms/GraphAlgor/kruskalAlgor.htm
	//For use in Kruskal's Algorithm
	//defines the wakll between two cells
	private struct cellEdge {
		public int Cell1Index, Cell2Index;
		public int Weight;
	}

	//Sort method for cellEdge struct
	static int EdgeSortByWeight(cellEdge e1, cellEdge e2){
		return e1.Weight.CompareTo (e2.Weight);
	}

	//defines a tree for use in Kruskal's algorithm
	private struct treeSet {
		public int ID;
		public List<int> TreeSet;
	}

	//Given index of cell and List of treeSet
	//Output Indices of treeSet containing the given cell index
	private int findSetIndex(int find, List<treeSet> sets) {
		for (int ss = sets.Count - 1; ss >= 0; ss--) {
			for(int sI = sets[ss].TreeSet.Count - 1; sI >= 0; sI--) {
				if (sets [ss].TreeSet[sI] == find) {
					return ss;
				}
			}
		}
		return -1;
	}

	// Kruskal's Implement with Priority Queue
	private void kruskalsAlgorithm() {
		Random.InitState (Seed);

		//initialise sets
		int id = -1;
		List<treeSet> sets = new List<treeSet>();
		foreach (cell cC in mCells) {
			treeSet tSet = new treeSet ();
			tSet.ID = id++;
			tSet.TreeSet = new List<int> ();
			tSet.TreeSet.Add (cC.Index);
			sets.Add (tSet);
		}
			
		//initialise edges
		List<cellEdge> edges = new List<cellEdge>();
		for (int ii = mCells.Count - 1; ii >= 0; ii--) {
			List<int> neighbourIndices = mCells[ii].NeighbourIndices ();
			foreach (int ind in neighbourIndices) {
				if (mCells [ind].Index < ii) {
					cellEdge edge = new cellEdge ();
					edge.Cell1Index = ii;
					edge.Cell2Index = ind;
					edge.Weight = Random.Range (0, mCells.Count);
					edges.Add (edge);
				}
			}
		}
		edges.Sort (EdgeSortByWeight); //makes prioty list

		int currentEdge = 0;
		cellEdge current;

		//End case: Only one set exists
		//Continue case: more than one set exists
		while (sets.Count > 1) {
			//define next edge
			current = edges [currentEdge];

			//find sets containing each cell adjacent to edge
			int set1Index = findSetIndex (current.Cell1Index, sets);
			int set2Index = findSetIndex (current.Cell2Index, sets);

			if (set1Index > -1 && set2Index > -1) {
				//if the ID of set is different
				if (sets [set1Index].ID != sets [set2Index].ID) {
					//carve path
					mCells [current.Cell1Index].RemoveWall (mCells [current.Cell2Index]);
	
					//merge two sets
					sets [set1Index].TreeSet.AddRange (sets [set2Index].TreeSet);
					//remove other set from sets
					sets [set2Index].TreeSet.Clear ();
					sets.Remove (sets [set2Index]);
				}
			}
			//define next edge
			currentEdge++;
		}

	}

	//Removes dead end given the Braiding chance
	private void braid() {
		//if braiding chance above 0
		if (Braiding > 0.0f) {

			//create list of all deadends
			List<cell> deadEnds = new List<cell> ();
			foreach (cell lCell in mCells) {
				if (lCell.DeadEnd ()) {
					deadEnds.Add (lCell);
				}
			}


			foreach (cell cC in deadEnds) {
				//if random < Braiding chance
				if (Random.value < Braiding) {
					//find neighbours
					List<int> iNeighbours = cC.NeighbourIndices ();
					//do until a wall is removed
					bool wallRemoved = false;
					do {
						//pick random wall
						int iI = iNeighbours [Random.Range (0, iNeighbours.Count)];
						cell nN = mCells [iI]; //neighbouring cell
						//remove wall, returns true if a wal is removed
						wallRemoved = cC.RemoveWall (nN);
					} while (!wallRemoved);
				}
			}
		}
	}

	//Output: Returns false if it finds an invisted cell
	//        or true if all cells are marked visited
	private bool entireGridVisited() {
		foreach (cell lCell in mCells) {
			if (!lCell.Visited) {
				return false;
			}
		}
		return true;
	}

	//Resets the visited flag of all cells
	public void resetVisited() {
		foreach (cell lCell in mCells) {
			lCell.Visited = false;
		}
	}
}
