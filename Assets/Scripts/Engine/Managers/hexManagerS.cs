using UnityEngine; 
using System.Collections; 
using System.Collections.Generic;
using System;
using System.IO; 


public class hexManagerS : MonoBehaviour {

	
	/*
		  		north 
      northwest  ___   northeast
                /    \
      southwest \___/  southeast
				south
		*/
 	
	public static string        level_name;
	public static HexData[,] 	hexes; 
	public static int 			x_max = 0; //size of hex array, used for out of bounds checking
	public static int			z_max = 0;
	  
	public 		   GameObject  	hex_display_init;
	
	public  static GameObject  	hex_display;
	 
	

	void Awake(){
		
		 hex_display = hex_display_init;
		 
		engineIOS ios      = GameObject.FindGameObjectWithTag("io_manager").GetComponent<engineIOS>();
		if(!ios.LoadFromTextAsset())
		{
			throw new System.Exception("Level file malformed! : (");
			Debug.Break();
		}
	}
	
	
	//Return adjacent hexes for the given entity position
	public static HexData[] getAdjacentHexes(int x, int z){
		
		
		//if we're out of bounds or if we're trying to get adjacency from a perimeter hex, return false
		if(x < 0 || x > x_max || z < 0 || z > z_max || getHex(x, z).hex_type == Hex.Perimeter)
		{
			throw new KeyNotFoundException("Accessing out of bounds!"); 
		}
		
		//ALWAYS BUILD THIS ARRAY "CLOCKWISE" STARTING AT NORTH
		HexData[] output = new HexData[6];

		//Get North						//Get Northeast
		output[0] = hexes[x, z +1];		output[1] = hexes[x+1, z];
		output[0].direction_from_central_hex = Facing.North;
		output[1].direction_from_central_hex = Facing.NorthEast;

		//Get Southeast                 //Get South
		output[2] = hexes[x+1, z-1];	output[3] = hexes[x, z-1]; 
		output[2].direction_from_central_hex = Facing.SouthEast;
		output[3].direction_from_central_hex = Facing.South;

		//Get Southwest					//Get Northwest
		output[4] = hexes[x-1, z];		output[5] = hexes[x-1, z+1];
		output[4].direction_from_central_hex = Facing.SouthWest;
		output[5].direction_from_central_hex = Facing.NorthWest;
		
		return output;
	}
	
	
	
	//Get hex at given position in the map
	public static HexData getHex(int hex_x, int hex_z){
		
		if(hex_x < 0 || hex_x > x_max || hex_z < 0 || hex_z > z_max)  
			throw new KeyNotFoundException("Accessing out of bounds!");  
		
		return hexes[hex_x, hex_z];
	} 
	
	//converts engine coordinates into 3D space cordinates
	public static Vector3 CoordsGameTo3D(int x, int z)
	{  
		return new Vector3(x * 2.30024F + z * -0.841947F, 0, z * 1.81415F + x * 1.3280592F);
	}
	
	
	private GameObject InstantiateHex(int x, int z)
	{ 
		GameObject new_hex = (GameObject) Instantiate(hexManagerS.hex_display, CoordsGameTo3D(x, z), Quaternion.identity);
		editorHexS new_hex_script = new_hex.GetComponent<editorHexS>();
		 
		return new_hex;
	}
	 
}


 
