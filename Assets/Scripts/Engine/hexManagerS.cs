using UnityEngine; 
using System.Collections; 
using System.Collections.Generic;


public class hexManagerS : MonoBehaviour {

	
 
	public static HexData[,] hexes; 
	

	void Start(){

		Load();

	}

	private void Load(){
		//TODO: Port Load code from editorIOS
		
		//ADDED: I'm creating a dummy 2D array for testing purposes ***
		hexes = new HexData[,] {{new HexData(0, 2, Hex.Grass), new HexData(1, 2, Tiles.Mountain), new HexData(2, 2, Hex.Grass, true)},
								{new HexData(0, 1, Hex.Grass), new HexData(1, 1, Tiles.Grass),    new HexData(2, 1, Hex.Grass, true)},
								{new HexData(0, 0, Hex.Grass), new HexData(1, 0, Tiles.Mountain), new HexData(2, 0, Hex.Grass, true)}};
	}
	
	//ADDED: New/updated functions below ***
	
	//Return adjacent hexes for the given entity position
	public static HexData[] getAdjacent(Vector3 ina){
//	public static HexData[] getAdjacent(int x, int z){
		//ADDED:Assumption is that outer tiles in array are marked as invalid like editor tiles ***
		//		so if we have an 200 by 200 array then at the very least row 0/col 0 and row 200/col 200 are all marked as invalid or editor tiles ***
		//		even though they won't be visible on actual board, other tiles i assume can also be marked as invalid in the board depending on how hexes are placed i guess ***
		
		
		
		/*
		  		north 
      northwest  ___   northeast
                /    \
      southwest \___/  southeast
				south
		*/
		
		//Get North
//		HexData n = hexes[x, z +1];
//		
//		//Get Northeast
//		HexData s = hexes[x+1, z];
//		
//		//Get Southeast
//		HexData s = hexes[x+1, z-1];
//		
//		//Get South
//		HexData s = hexes[x, z-1];
//		
//		//Get Southwest
//		HexData s = hexes[x-1, z];
//		
//		//Get Northwest
//		HexData s = hexes[x-1, z+1];
		
		
		
//		//Get Upper Left
//		HexData hex_uleft = hexes[tag_row - 1,tag_col];
//		//Get Upper Right
//		HexData hex_uright = hexes[tag_row - 1,tag_col + 1];
//		//Get Left
//		HexData hex_left = hexes[tag_row,tag_col - 1];
//		//Get Right
//		HexData hex_right = hexes[tag_row,tag_col + 1];
//		//Get Down Left
//		HexData hex_dleft = hexes[tag_row + 1,tag_col];
//		//Get Down Right
//		HexData hex_dright = hexes[tag_row + 1,tag_col + 1];
//		
//		return new HexData[] {hex_uleft, hex_uright, hex_left,
//								hex_right, hex_dleft,hex_dright};
//	}
		return null;
	}
//	
//	//Update hexManager map based on entity move
//	public static void updateMap(int tag_row, int tag_col, GameObject entity ,hexManagerScript.HexData hex){
//		//Change occupant of new hex and invalidate occupant of old hex
//		hexes[hex.tag_row, hex.tag_column].occupant = entity;
//		hexes[hex.tag_row, hex.tag_column].occupied = true;
//		hexes[tag_row, tag_col].occupant = null;
//		hexes[tag_row, tag_col].occupied = false;
//	}
	
	//Get hex at given position in the map
	public static HexData getHex(int row, int col){
		return hexes[row, col];
	}
	
 
	public class HexData{

		public int	x; 
		public int	z;  

		public Hex hex_type; //enviroment type of this hex
		
		public HexData(int _x, int _z, Tiles _type){
			x = _x;
			z = _z; 
			hex_type = _type;
		}
		
		
//		//ADDED: Optional method for changing hex tags if first constructor used, can be discarded if not used***
//		public void changeHexTags(int row, int column){
//			tag_row = row;
//			tag_column = column;
//		}

	} 

}



