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
	
	
	public static List<HexData> getAdjacentHexes(int x, int z, int sight_range)
	{
		return hexManagerS.getAdjacentHexes(getHex(x, z), sight_range);
	}
	
	public static List<HexData> getAdjacentHexes(HexData center, int sight_range)
	{ 
		List<HexData> hexes_in_range = new List<HexData>();
		
		//get the hex standing on
		HexData current_hex = center; 
		hexes_in_range.Add(current_hex);
		
		//enter loop for surrounding hexes
		for(int ring = 1; ring <= sight_range; ring++)
		{
			 
			//draw the first "northeast" edge hex 
			current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.NorthEast);
			hexes_in_range.Add(current_hex); 
			
			//draw the "northeast" portion
			for(int edge_hexes_drawn = 1; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{ 
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.SouthEast);// = AddHexSE(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex)); 
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "southeast" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.South);
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "south" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.SouthWest);
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "southwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.NorthWest);
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "northwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.North);
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "north" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.NorthEast);
				hexes_in_range.Add(current_hex); 
			}
		}
		
		Debug.LogWarning("hexes_in_range size = " + hexes_in_range.Count);
		return hexes_in_range;
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
	
	public static HexData getHex(int x, int z, Facing direction)
	{
		switch(direction)
		{
			case Facing.North: 		return hexes[x, z +1];
			case Facing.NorthEast:	return hexes[x+1, z];
			case Facing.SouthEast:	return hexes[x+1, z-1];
			case Facing.South:		return hexes[x, z-1]; 
			case Facing.SouthWest:	return hexes[x-1, z];	
			case Facing.NorthWest:	return hexes[x-1, z+1];
		default: throw new System.Exception("Wtf, how'd you get this?  getHex(facing)");
		}
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


 
