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
 	
	public string 	  			level_name;
	public int   				level_editor_format_version;
	
	private static HexData[,] 	hexes = new HexData[,] {{new HexData(0, 0, Hex.Perimeter), new HexData(0, 1, Hex.Perimeter), new HexData(0, 2, Hex.Perimeter), new HexData(0, 3, Hex.Perimeter), new HexData(0, 4, Hex.Perimeter), new HexData(0, 5, Hex.Perimeter), new HexData(0, 6, Hex.Perimeter)},
														{new HexData(1, 0, Hex.Perimeter), new HexData(1, 1, Hex.Mountain), new HexData(1, 2, Hex.Grass), new HexData(1, 3, Hex.Farmland), new HexData(1, 4, Hex.Grass), new HexData(1, 5, Hex.Grass), new HexData(1, 6, Hex.Perimeter)},
														{new HexData(2, 0, Hex.Perimeter), new HexData(2, 1, Hex.Grass), new HexData(2, 2, Hex.Water), new HexData(2, 3, Hex.Forest), new HexData(2, 4, Hex.Grass), new HexData(2, 5, Hex.Forest), new HexData(2, 6, Hex.Perimeter)},
														{new HexData(3, 0, Hex.Perimeter), new HexData(3, 1, Hex.Grass), new HexData(3, 2, Hex.Forest), new HexData(3, 3, Hex.Grass), new HexData(3, 4, Hex.Hills), new HexData(3, 5, Hex.Grass), new HexData(3, 6, Hex.Perimeter)},
														{new HexData(4, 0, Hex.Perimeter), new HexData(4, 1, Hex.Grass), new HexData(4, 2, Hex.Mountain), new HexData(4, 3, Hex.Snow), new HexData(4, 4, Hex.Grass), new HexData(4, 5, Hex.Grass), new HexData(4, 6, Hex.Perimeter)},
														{new HexData(5, 0, Hex.Perimeter), new HexData(5, 1, Hex.Grass), new HexData(5, 2, Hex.Grass), new HexData(5, 3, Hex.Water), new HexData(5, 4, Hex.Grass), new HexData(5, 5, Hex.Water), new HexData(5, 6, Hex.Perimeter)},
														{new HexData(6, 0, Hex.Perimeter), new HexData(6, 1, Hex.Perimeter), new HexData(6, 2, Hex.Perimeter), new HexData(6, 3, Hex.Perimeter), new HexData(6, 4, Hex.Perimeter), new HexData(6, 5, Hex.Perimeter), new HexData(6, 6, Hex.Perimeter)}};
		
	private static int 			x_max = 7; //size of hex array, used for out of bounds checking
	private static int			z_max = 7;
	
	public GameObject  	grass_hex;
	public GameObject  	desert_hex;
	public GameObject  	forest_hex;
	public GameObject  	farmland_hex;
	public GameObject  	marsh_hex; 
	public GameObject  	mountain_hex;
	public GameObject  	hills_hex;
	public GameObject  	water_hex;  
	public GameObject  	border_hex;  
	
	
	public int		random_percentage_threshold = 30;
	
	private static Dictionary<Hex, GameObject>  hex_dict    = new Dictionary<Hex, GameObject>();
	
	//used for parsing level files
	private	string[] stringSeparators = new string[] {" = "};  
	

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
		
		

		if(!Load())
			throw new MissingComponentException("Level file malformed! : (");

	}
	
	
	private bool Load(){	
		
		/*
		
	//INITIALIZE FILE TO READ\
		StreamReader reader;
		FileInfo filer = new FileInfo(Application.dataPath + "/Level_Files/" + level_name + ".pcl");
		if(filer != null && filer.Exists)
		{
		   reader = filer.OpenText();  // returns StreamReader
		} 
		else
		{
			print ("FILE DOES NOT EXIST!");
			return false;
		}
		  	
	//BEGIN PARSING DATA
		//PARSE HEADER INFO
		if(!reader.ReadLine().Equals("LEVEL{"))
		{
			print("l1 ILL FORMATED!");
			return false;
		} 
		if(getIntR(reader) != level_editor_format_version) //EDITOR VER
		{
			print ("EDITOR VERSION MISMATCH!");
			return false;
		}
		level_name 		= getStringR(reader); //NAME
		int version    		= getIntR(reader);    //VERSION
		
		int total_count, game_count, border_count;
		x_max = getIntR(reader);
		z_max = getIntR(reader);
		
		int load_x_min = getIntR(reader);
		int load_x_max = getIntR(reader);
		
		int load_z_min = getIntR(reader);
		int load_z_max = getIntR(reader);
		
		hexes = new HexData[x_max, z_max];
		
		--x_max;
		--z_max;
		
		total_count 	= getIntR(reader);  		//total count
		game_count 		= getIntR(reader);		//game count
		border_count 	= getIntR(reader);  		//border count
		
		
		//BEGIN PARSING HEXES
		if(!reader.ReadLine().Contains("HEXES{"))
		{
			print("HEXES ILL FORMATED");
			return false;
		}
		while(getHexR(reader)) //next line is a HEX{
		{
			int x = getIntR(reader) - load_x_min;
			int z = getIntR(reader) - load_z_min;
			
			print ("making hex: " + x + ", " + z);
            Hex hex_type = (Hex) Enum.Parse(typeof(Hex), getStringR(reader));
			hexes[x, z] = new HexData(x, z, hex_type);
			
			
			GameObject new_hex = (GameObject) Instantiate(hex_dict[hex_type], CoordsGameTo3D(x,z), Quaternion.identity);
			engineHexS new_hex_script = (engineHexS) new_hex.AddComponent("engineHexS"); 
			 
			new_hex_script.buildHexData(x, z, hex_type);
			
			if(!getCBR(reader))
			{
				print("MALFORMED HEX!");
				return false;
			}
		}
		 
//		print (reader.ReadLine());
		//BEGIN PARSING ENTITES
		if(!reader.ReadLine().Contains("ENTITIES{"))
		{
			print("ENTITIES ILL FORMATED");
			return false;
		}
		while(getEntR(reader)) //next line is a HEX{
		{
			int x = getIntR(reader) - load_x_min;
			int z = getIntR(reader) - load_z_min;
            EntityE ent_type = (EntityE) Enum.Parse(typeof(EntityE), getStringR(reader));
			
			int base_starting_health_percentage = 100;
			int mech_starting_health_percentage = 100;
			bool enemy_knows_base_loc 			= false;
			bool enemy_knows_mech_loc 			= false;
			int node_starting_level				= 2;
			
			if(ent_type == EntityE.Player)
			{ 
				print ("MOVING CAMERA ONTO PLAYER!");
				GameObject maincam = GameObject.FindGameObjectWithTag("MainCamera");
				maincam.transform.position = new Vector3(CoordsGameTo3D(x,z).x, 20, CoordsGameTo3D(x,z).z);
			}
			switch(ent_type)
			{
				case EntityE.Base:
					 base_starting_health_percentage = getIntR(reader);
					break;
				case EntityE.Player:
					 mech_starting_health_percentage = getIntR(reader);
					break;
				case EntityE.Enemy:
					 enemy_knows_base_loc = getBoolR(reader);
					 enemy_knows_mech_loc = getBoolR(reader);
					break;
				case EntityE.Factory:
				case EntityE.Junkyard:
				case EntityE.Outpost:
					 node_starting_level	 = getIntR(reader);
					break;
			}
			  
			if(!getCBR(reader))
			{
				print("MALFORMED HEX!");
				return false;
			}
		}
		 
		return true;
		*/
		
		
		x_max = 7;
		z_max = 7;

		return true;
		
	}
	
	public string getStringR(StreamReader reader)
	{   
    	string[] items = reader.ReadLine().Split(stringSeparators, StringSplitOptions.None);
		return items[1];
	}
	
	public int getIntR(StreamReader reader)
	{  
    	string[] items = reader.ReadLine().Split(stringSeparators, StringSplitOptions.None);
		return int.Parse(items[1]);
	}
	
	public bool getBoolR(StreamReader reader)
	{  
    	string[] items = reader.ReadLine().Split(stringSeparators, StringSplitOptions.None);
		return bool.Parse(items[1]);
	}
	
	public bool getCBR(StreamReader reader) //close bracket Reader
	{  
    	return reader.ReadLine().Contains("}"); 
	}
	
	public bool getOBR(StreamReader reader) //close bracket Reader
	{  
    	return reader.ReadLine().Contains("{"); 
	}
	
	public bool getHexR(StreamReader reader) //close bracket Reader
	{  
    	return reader.ReadLine().Contains("HEX{"); 
	}
	
	public bool getEntR(StreamReader reader) //close bracket Reader
	{  
    	return reader.ReadLine().Contains("ENTITY{"); 
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
	public static HexData getHex(int x, int z){

		if(x < 0 || x > x_max || z < 0 || z > z_max)
		{
			throw new KeyNotFoundException("Accessing out of bounds!"); 
		}
		else
		{ 
			return hexes[x, z];
		}
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


/* 
public class HexData{ 
	
	public readonly int 	x;   	  //level x coord (NE / SW)
	public readonly int 	z;  	  //level z coord  (N / S)
	public readonly Hex 	hex_type; //enviroment type of this hex
	
	public HexData(int _x, int _z, Hex _type){
		x = _x;
		z = _z; 
		hex_type = _type;
	}
} 
 */
