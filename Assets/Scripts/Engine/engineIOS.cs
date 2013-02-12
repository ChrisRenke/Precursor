using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO; 

public class engineIOS : MonoBehaviour {
	 
	public static int   				level_editor_format_version = 3;
	public TextAsset                    level_string_asset;
	
	//used for parsing level files
	private	static  string[] stringSeparators = new string[] {" = "};  
	
	public bool LoadFromTextAsset()
	{
		return LoadFromString(level_string_asset.ToString());
	}
	
	public static bool LoadFromPref(string level_key_name)
	{
		return LoadFromString(PlayerPrefs.GetString(level_key_name));
	}
	
	public static bool LoadFromString(string level_data){	
		
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
		hexManagerS.level_name   = getStringR(level_lines[index++]); //NAME
		int version    		= getIntR(level_lines[index++]);    //VERSION
		
		int total_count, game_count, border_count;
		hexManagerS.x_max = getIntR(level_lines[index++]);
		hexManagerS.z_max = getIntR(level_lines[index++]);
		
		int load_x_min = getIntR(level_lines[index++]);
		int load_x_max = getIntR(level_lines[index++]);
		
		int load_z_min = getIntR(level_lines[index++]);
		int load_z_max = getIntR(level_lines[index++]);
		
		hexManagerS.hexes = new HexData[hexManagerS.x_max, hexManagerS.z_max];
		
		--hexManagerS.x_max;
		--hexManagerS.z_max;
		
		total_count 	= getIntR(level_lines[index++]);  		//total count
		game_count 		= getIntR(level_lines[index++]);		//game count
		border_count 	= getIntR(level_lines[index++]);  		//border count
		
		
		//BEGIN PARSING hexManagerS.hexes
		if(!level_lines[index++].Contains("HEXES{"))
		{
			print("hexManagerS.hexes ILL FORMATED");
			return false;
		}
		while(getHexR(level_lines[index++])) //next line is a HEX{
		{
			int x = getIntR(level_lines[index++]) - load_x_min;
			int z = getIntR(level_lines[index++]) - load_z_min;
			
			print ("making hex: " + x + ", " + z);
            Hex hex_type = (Hex) Enum.Parse(typeof(Hex), getStringR(level_lines[index++]));
			hexManagerS.hexes[x, z] = new HexData(x, z, hex_type);
			
			Vector3 pos = hexManagerS.CoordsGameTo3D(x,z);
			if(pos.x < enginePlayerS.camera_min_x_pos)
				enginePlayerS.camera_min_x_pos = pos.x;
			if(pos.x > enginePlayerS.camera_max_x_pos)
				enginePlayerS.camera_max_x_pos = pos.x;
			
			if(pos.z > enginePlayerS.camera_max_z_pos)
				enginePlayerS.camera_max_z_pos = pos.z;
			
			if(pos.z < enginePlayerS.camera_min_z_pos)
				enginePlayerS.camera_min_z_pos = pos.z;
			
			GameObject new_hex = (GameObject) Instantiate(hexManagerS.hex_dict[hex_type], pos, Quaternion.identity);
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
				maincam.transform.position = new Vector3(hexManagerS.CoordsGameTo3D(x,z).x, 20, hexManagerS.CoordsGameTo3D(x,z).z);
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
	
	private static string getStringR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return items[1];
	}
	 
	private static int getIntR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return int.Parse(items[1]);
	}
	
	private static bool getBoolR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return bool.Parse(items[1]);
	}
	
	private static bool getCBR(String line) //close bracket Reader
	{  
    	return line.Contains("}"); 
	}
	
	private static bool getOBR(String reader) //close bracket Reader
	{  
    	return reader.Contains("{"); 
	}
	
	private static bool getHexR(String reader) //close bracket Reader
	{  
    	return reader.Contains("HEX{"); 
	}
	
	private static bool getEntR(String reader) //close bracket Reader
	{  
    	return reader.Contains("ENTITY{"); 
	} 


}