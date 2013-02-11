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
	
	private static HexData[,] 	hexes; 
	private static int 			x_max, z_max = 0; //size of hex array, used for out of bounds checking
	
	
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

		if(!Load("awesome"))
			throw new MissingComponentException("Level file malformed! : (");

	}
	
	
	private bool Load(String level_key_name){	
		
		String level_data = PlayerPrefs.GetString(level_key_name);
		string[] level_lines = level_data.Split(new char[] {'\n'});
		int index = 0;
		
	//INITIALIZE FILE TO READ\
//		StreamReader reader;
//		FileInfo filer = new FileInfo(Application.dataPath + "/Level_Files/" + level_name + ".pcl");
//		if(filer != null && filer.Exists)
//		{
//		   reader = filer.OpenText();  // returns StreamReader
//		} 
//		else
//		{
//			print ("FILE DOES NOT EXIST!");
//			return false;
//		}
		  	
	//BEGIN PARSING DATA
		//PARSE HEADER INFO
		if(!level_lines[index++].Equals("LEVEL{"))
		{
			print("l1 ILL FORMATED!");
			return false;
		} 
		if(getIntR(level_lines[index++]) != level_editor_format_version) //EDITOR VER
		{
			print ("EDITOR VERSION MISMATCH!");
			return false;
		}
		level_name 		= getStringR(level_lines[index++]); //NAME
		int version    		= getIntR(level_lines[index++]);    //VERSION
		
		int total_count, game_count, border_count;
		x_max = getIntR(level_lines[index++]);
		z_max = getIntR(level_lines[index++]);
		
		int load_x_min = getIntR(level_lines[index++]);
		int load_x_max = getIntR(level_lines[index++]);
		
		int load_z_min = getIntR(level_lines[index++]);
		int load_z_max = getIntR(level_lines[index++]);
		
		hexes = new HexData[x_max, z_max];
		
		--x_max;
		--z_max;
		
		total_count 	= getIntR(level_lines[index++]);  		//total count
		game_count 		= getIntR(level_lines[index++]);		//game count
		border_count 	= getIntR(level_lines[index++]);  		//border count
		
		
		//BEGIN PARSING HEXES
		if(!level_lines[index++].Contains("HEXES{"))
		{
			print("HEXES ILL FORMATED");
			return false;
		}
		while(getHexR(level_lines[index++])) //next line is a HEX{
		{
			int x = getIntR(level_lines[index++]) - load_x_min;
			int z = getIntR(level_lines[index++]) - load_z_min;
			
			print ("making hex: " + x + ", " + z);
            Hex hex_type = (Hex) Enum.Parse(typeof(Hex), getStringR(level_lines[index++]));
			hexes[x, z] = new HexData(x, z, hex_type);
			
			
			GameObject new_hex = (GameObject) Instantiate(hex_dict[hex_type], CoordsGameTo3D(x,z), Quaternion.identity);
			engineHexS new_hex_script = (engineHexS) new_hex.AddComponent("engineHexS"); 
			 
			new_hex_script.buildHexData(x, z, hex_type);
			
			if(!getCBR(level_lines[index++]))
			{
				print("MALFORMED HEX!");
				return false;
			}
		}
		 
//		print (reader.ReadLine());
		//BEGIN PARSING ENTITES
		if(!level_lines[index++].Contains("ENTITIES{"))
		{
			print("ENTITIES ILL FORMATED");
			return false;
		}
		while(getEntR(level_lines[index++])) //next line is a HEX{
		{
			int x = getIntR(level_lines[index++]) - load_x_min;
			int z = getIntR(level_lines[index++]) - load_z_min;
            EntityE ent_type = (EntityE) Enum.Parse(typeof(EntityE), getStringR(level_lines[index++]));
			
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
					 base_starting_health_percentage = getIntR(level_lines[index++]);
					break;
				case EntityE.Player:
					 mech_starting_health_percentage = getIntR(level_lines[index++]);
					break;
				case EntityE.Enemy:
					 enemy_knows_base_loc = getBoolR(level_lines[index++]);
					 enemy_knows_mech_loc = getBoolR(level_lines[index++]);
					break;
				case EntityE.Factory:
				case EntityE.Junkyard:
				case EntityE.Outpost:
					 node_starting_level	 = getIntR(level_lines[index++]);
					break;
			}
			  
			if(!getCBR(level_lines[index++]))
			{
				print("MALFORMED HEX!");
				return false;
			}
		}
		 
		return true;
	}
	
	public string getStringR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return items[1];
	}
	 
	public int getIntR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return int.Parse(items[1]);
	}
	
	public bool getBoolR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return bool.Parse(items[1]);
	}
	
	public bool getCBR(String line) //close bracket Reader
	{  
    	return line.Contains("}"); 
	}
	
	public bool getOBR(String reader) //close bracket Reader
	{  
    	return reader.Contains("{"); 
	}
	
	public bool getHexR(String reader) //close bracket Reader
	{  
    	return reader.Contains("HEX{"); 
	}
	
	public bool getEntR(String reader) //close bracket Reader
	{  
    	return reader.Contains("ENTITY{"); 
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
		
		if(x < 0 || x > x_max || z < 0 || z > z_max || getHex(x, z).hex_type == Hex.Perimeter)
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

