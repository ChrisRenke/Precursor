using UnityEngine;

using System.Collections;

using System.Collections.Generic;


public class hexManagerS : MonoBehaviour {

	public enum Tiles {Grass, Desert, Forest, Farmland, Marsh, Snow, Mountain, Hills, Water, Settlement, Factory, Outpost, Junkyard, EditorTileA, EditorTileB};

	public enum Entities {Player, Enemy, Settlement, ResourceNode};

	public static HexData[,] hexes; //ADDED: changed from int to HexData array ***

	public TextAsset     level_file;
	
	//EXAMPLE
	//checkTile
	//if(hex.type == HexManager.Tiles.Mountain || hex.type = HexManager.Tiles.Water)
	//return false;
	//checkTile(Tiles.Desert);

	

	void Start(){

		Load();

	}

	private void Load(){
		//TODO: Need to create Algorithm for board initialization based on level_file
		//		hexes	= new HexData[200,200];
		
		//ADDED: I'm creating a dummy 2D array for testing purposes ***
		hexes = new HexData[,] {{new HexData(-3, 1, Tiles.Water, false), new HexData(3, 1, Tiles.Mountain, false), new HexData(2, 1, Tiles.Settlement, true), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false)},
								{new HexData(-3, 1, Tiles.Water, false), new HexData(3, 1, Tiles.Mountain, false), new HexData(2, 1, Tiles.Settlement, true), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false)},
								{new HexData(-3, 1, Tiles.Water, false), new HexData(3, 1, Tiles.Mountain, false), new HexData(2, 1, Tiles.Settlement, true), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false)},
								{new HexData(-3, 1, Tiles.Water, false), new HexData(3, 1, Tiles.Mountain, false), new HexData(2, 1, Tiles.Settlement, true), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false)},
								{new HexData(-3, 1, Tiles.Water, false), new HexData(3, 1, Tiles.Mountain, false), new HexData(2, 1, Tiles.Settlement, true), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false)},
								{new HexData(-3, 1, Tiles.Water, false), new HexData(3, 1, Tiles.Mountain, false), new HexData(2, 1, Tiles.Settlement, true), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false)},
								{new HexData(-3, 1, Tiles.Water, false), new HexData(3, 1, Tiles.Mountain, false), new HexData(2, 1, Tiles.Settlement, true), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false), new HexData(-3, 1, Tiles.Water, false)}};
	}

	//Update is called once per frame
	void Update () {

	}
	
	//ADDED: New/updated functions below ***
	
	//Return adjacent hexes for the given entity position
	public static HexData[] getAdjacent(int tag_row, int tag_col){
		//ADDED:Assumption is that outer tiles in array are marked as invalid like editor tiles ***
		//		so if we have an 200 by 200 array then at the very least row 0/col 0 and row 200/col 200 are all marked as invalid or editor tiles ***
		//		even though they won't be visible on actual board, other tiles i assume can also be marked as invalid in the board depending on how hexes are placed i guess ***
		
		//Get Upper Left
		HexData hex_uleft = hexes[tag_row - 1,tag_col];
		//Get Upper Right
		HexData hex_uright = hexes[tag_row - 1,tag_col + 1];
		//Get Left
		HexData hex_left = hexes[tag_row,tag_col - 1];
		//Get Right
		HexData hex_right = hexes[tag_row,tag_col + 1];
		//Get Down Left
		HexData hex_dleft = hexes[tag_row + 1,tag_col];
		//Get Down Right
		HexData hex_dright = hexes[tag_row + 1,tag_col + 1];
		
		return new HexData[] {hex_uleft, hex_uright, hex_left,
								hex_right, hex_dleft,hex_dright};
	}
	
	//Update hexManager map based on entity move
	public static void updateMap(int tag_row, int tag_col, GameObject entity ,hexManagerScript.HexData hex){
		//Change occupant of new hex and invalidate occupant of old hex
		hexes[hex.tag_row, hex.tag_column].occupant = entity;
		hexes[hex.tag_row, hex.tag_column].occupied = true;
		hexes[tag_row, tag_col].occupant = null;
		hexes[tag_row, tag_col].occupied = false;
	}
	
	//Get hex at given position in the map
	public static HexData getHex(int row, int col){
		return hexes[row, col];
	}
	

	public class HexData{

		public int	x; //x-axis position

		public int	z; //z-axis position

		public Tiles hex_type; //enviroment type of this hex
		
		//ADDED: Hex tag variables for locating which row/column a hex is in the matrix ***
		//		these can be tagged when initializing the hex data into the matrix (made a separate contruct function) and changed with a inner function like changeHexTags(); ***
		//		The intialization of these tages all depends on the algorithm used to populate the hexes array. Constructor function(s) can be changed later once algorithm set ***
		public int tag_row; //row hex occupies in hexes array ***
		
		public int tag_column; //column hex occupies in hexes array
		
		public bool occupied; //keeps track of whether entity occupies this hex, may be deleted when game object var working 
		
		public GameObject occupant; //entity that occupies this hex
		

		public HexData(int _x, int _z, Tiles _type, bool has_entity){
			x = _x;
			z = _z; 
			hex_type = _type;
			occupied = has_entity;
			tag_row = -1;
			tag_column = -1;
			
			occupant = null;

		}
		
		//ADDED: Separate constructor with Hex tags/Game Occupant initialized initialized ***
		public HexData(int _x, int _z, Tiles _type, bool has_entity, int row, int column, GameObject new_occupant){
			x = _x;
			z = _z; 
			hex_type = _type;
			occupied = has_entity;
			tag_row = row;
			tag_column = column;
			occupant = new_occupant;
		}
		
		//ADDED: Optional method for changing hex tags if first constructor used, can be discarded if not used***
		public void changeHexTags(int row, int column){
			tag_row = row;
			tag_column = column;
		}

	}

}



