using UnityEngine; 
using System.Collections; 
using System.Collections.Generic;


public class hexManagerS : MonoBehaviour {

	
		/*
		  		north 
      northwest  ___   northeast
                /    \
      southwest \___/  southeast
				south
		*/
 
	private static HexData[,] hexes; 
	

	void Start(){

		Load();

	}

	private void Load(){
		//TODO: Port Load code from editorIOS
		
		//bottom left corner will be 0,0, at least thats the plan anyways
		
		//ADDED: I'm creating a dummy 2D array for testing purposes ***
		hexes = new HexData[,] {{new HexData(0, 2, Hex.Grass), new HexData(1, 2, Tiles.Mountain), new HexData(2, 2, Hex.Grass, true)},
								{new HexData(0, 1, Hex.Grass), new HexData(1, 1, Tiles.Grass),    new HexData(2, 1, Hex.Grass, true)},
								{new HexData(0, 0, Hex.Grass), new HexData(1, 0, Tiles.Mountain), new HexData(2, 0, Hex.Grass, true)}};
	}
	
	//ADDED: New/updated functions below ***
	
	//Return adjacent hexes for the given entity position
	public static HexData[] getAdjacentHexes(int x, int z){
		//TODO CHECK BOUNDS!
		
		//ALWAYS BUILD THIS ARRAY "CLOCKWISE" STARTING AT NORTH
		HexData[] output = new HexData[6]();
		
		//Get North						//Get Northeast
		output[0] = hexes[x, z +1];		output[1] = hexes[x+1, z];
		
		//Get Southeast                 //Get South
		output[2] = hexes[x+1, z-1];	output[3] = hexes[x, z-1]; 
		
		//Get Southwest					//Get Northwest
		output[4] = hexes[x-1, z];		output[5] = hexes[x-1, z+1];
		
		return output;
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
	public static HexData getHex(int x, int z){
		//TODO CHECK BOUNDS D: 
		return hexes[row, col];
	}
	 
}


 
public class HexData{ 
	
	public readonly int 	x;   	  //level x coord (NE / SW)
	public readonly int 	z;  	  //level z coord  (N / S)
	public readonly Hex 	hex_type; //enviroment type of this hex
	
	public HexData(int _x, int _z, Tiles _type){
		x = _x;
		z = _z; 
		hex_type = _type;
	}
} 

