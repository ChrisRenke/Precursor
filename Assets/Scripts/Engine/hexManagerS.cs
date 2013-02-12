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
	public static int 			x_max, z_max = 0; //size of hex array, used for out of bounds checking
	public static Dictionary<Hex, GameObject>  hex_dict    = new Dictionary<Hex, GameObject>();
	
	
	public   GameObject  	grass_hex;
	public   GameObject  	desert_hex;
	public   GameObject  	forest_hex;
	public   GameObject  	farmland_hex;
	public   GameObject  	marsh_hex; 
	public   GameObject  	mountain_hex;
	public   GameObject  	hills_hex;
	public   GameObject  	water_hex;  
	public   GameObject  	border_hex;  
	
	 
	
	

	void Start(){
		
		hex_dict.Add(Hex.Grass, grass_hex);
		hex_dict.Add(Hex.Desert, desert_hex);
		hex_dict.Add(Hex.Forest, forest_hex);
		hex_dict.Add(Hex.Farmland, farmland_hex);
		hex_dict.Add(Hex.Marsh, marsh_hex); 
		hex_dict.Add(Hex.Mountain, mountain_hex);
		hex_dict.Add(Hex.Hills, hills_hex);
		hex_dict.Add(Hex.Water, water_hex);
		hex_dict.Add(Hex.Perimeter, border_hex);
		
//		if(!engineIOS.LoadFromTextAsset())
		if(!GameObject.FindGameObjectWithTag("io_manager").GetComponent<engineIOS>().LoadFromTextAsset())
			throw new MissingComponentException("Level file malformed! : (");

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

		//Get Southeast                 //Get South
		output[2] = hexes[x+1, z-1];	output[3] = hexes[x, z-1]; 

		//Get Southwest					//Get Northwest
		output[4] = hexes[x-1, z];		output[5] = hexes[x-1, z+1];
		
		return output;
	}
	
	
	
	//Get hex at given position in the map
	public static HexData getHex(int hex_x, int hex_z){
		
		if(hex_x < 0 || hex_x > x_max || hex_z < 0 || hex_z > z_max) //|| hexes[hex_x, hex_z].hex_type == Hex.Perimeter 
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
		GameObject new_hex = (GameObject) Instantiate(hex_dict[hexes[x, z].hex_type], CoordsGameTo3D(x, z), Quaternion.identity);
		editorHexS new_hex_script = new_hex.GetComponent<editorHexS>();
		 
		return new_hex;
	}
	 
}


 
