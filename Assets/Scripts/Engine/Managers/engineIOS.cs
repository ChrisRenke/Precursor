using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO; 
//
//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]

public class engineIOS : MonoBehaviour {
	 
	public static int   				level_editor_format_version = 8;
	public TextAsset                    level_string_asset;
	
	//used for parsing level files
	private	static  string[] stringSeparators = new string[] {" = "};  
	
//	private static entityManagerS ems;
	
	void Awake()
	{
//		ems = GameObject.FindWithTag("entity_manager").GetComponent<entityManagerS>();
	}
	
	public bool LoadFromTextAsset()
	{
		return LoadFromString(level_string_asset.ToString());
	}
	
	public static bool LoadFromPref(string level_key_name)
	{
		return LoadFromString(PlayerPrefs.GetString(level_key_name));
	}
	
	public static bool LoadFromString(string level_data){	
		
		string[] level_lines = level_data.Split(new string[] {"\n","\r\n"},StringSplitOptions.None);
		int index = 0; 
		  	
	//BEGIN PARSING DATA
		//PARSE HEADER INFO
		print ("DEBUG " + level_lines[index]);
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
		int round    		= getIntR(level_lines[index++]);    //VERSION
		Turn current_turn   = getTurnR(level_lines[index++]);
		int current_ap 		= getIntR(level_lines[index++]);    //VERSION
		
		gameManagerS.current_turn = current_turn;
		gameManagerS.current_level = (Level)version;
		gameManagerS.current_round = round;
		
		
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
			
			Vector3 pos = hexManagerS.CoordsGameTo3D(x,z);
			GameObject new_hex = (GameObject) Instantiate(hexManagerS.hex_display, pos, Quaternion.identity);
			engineHexS new_hex_script = (engineHexS) new_hex.GetComponent("engineHexS"); 
			
//			print ("making hex: " + x + ", " + z);
            Hex hex_type = (Hex) Enum.Parse(typeof(Hex), getStringR(level_lines[index++]));
			HexData new_hex_data = new HexData(x, z, hex_type, new_hex, new_hex_script, Vision.Unvisted);
			new_hex_script.assignHexData_IO_LOADER_ONLY(new_hex_data);
			new_hex_script.SetVisiual();
			
			hexManagerS.hexes[x, z] = new_hex_data;
			
			new_hex_script.updateFoWState();
			
			
			if(pos.x < enginePlayerS.camera_min_x_pos)
				enginePlayerS.camera_min_x_pos = pos.x;
			if(pos.x > enginePlayerS.camera_max_x_pos)
				enginePlayerS.camera_max_x_pos = pos.x;
			
			if(pos.z > enginePlayerS.camera_max_z_pos)
				enginePlayerS.camera_max_z_pos = pos.z;
			
			if(pos.z < enginePlayerS.camera_min_z_pos)
				enginePlayerS.camera_min_z_pos = pos.z;
			
			
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
            editor_entity ent_type = (editor_entity) Enum.Parse(typeof(editor_entity), getStringR(level_lines[index++]));
//			print (ent_type);
			if(ent_type == editor_entity.Mech)
			{ 
//				print ("MOVING CAMERA ONTO PLAYER!");
				GameObject maincam = GameObject.FindGameObjectWithTag("MainCamera");
				maincam.transform.position = new Vector3(hexManagerS.CoordsGameTo3D(x,z).x, 60, hexManagerS.CoordsGameTo3D(x,z).z);
			}
			
			switch(ent_type)
			{
				case editor_entity.Town: 
					int town_current_hp =  getIntR(level_lines[index++]);
					int town_max_hp     =  getIntR(level_lines[index++]);
					BaseUpgradeLevel town_wall_level = getBaseUpgrade(level_lines[index++]);
					BaseUpgradeLevel town_defense_level = getBaseUpgrade(level_lines[index++]); 
					BaseUpgradeLevel town_structure_level = getBaseUpgrade(level_lines[index++]);  
				
					if(!entityManagerS.instantiateBase(x, z, town_current_hp, town_max_hp, town_wall_level,	town_defense_level, town_structure_level))
						throw new System.Exception("There is already one base, cannot have two! D: Go edit the level file you're loading to only have one!");
					break;
				
				case editor_entity.Mech:
//				print("player case");
					int mech_current_hp =  getIntR(level_lines[index++]);
					int mech_max_hp     =  getIntR(level_lines[index++]);
					bool gun_range	= getBoolR(level_lines[index++]);
					bool gun_cost	= getBoolR(level_lines[index++]);
					bool gun_damage	= getBoolR(level_lines[index++]);
					bool mobi_cost	= getBoolR(level_lines[index++]);
					bool mobi_water	= getBoolR(level_lines[index++]);
					bool mobi_mntn	= getBoolR(level_lines[index++]);
					bool ex_tele	= getBoolR(level_lines[index++]);
					bool ex_armor	= getBoolR(level_lines[index++]);
					bool ex_scav    = getBoolR(level_lines[index++]);
				
					if(!entityManagerS.instantiatePlayer(x, z, mech_current_hp, mech_max_hp, 
					 gun_range,
					 gun_cost	,
					 gun_damage	,
					 mobi_cost	,
					 mobi_water	,
					 mobi_mntn	,
					 ex_tele	,
					 ex_armor	,
					 ex_scav))
						throw new System.Exception("There is already one player mech, cannot have two! D: Go edit the level file you're loading to only have one!");
				
//					hexManagerS.getAdjacentHexes(entityManagerS.getBase().sentityManagerS.getBase().sight_range)updateFoWState()
					break;
				
				case editor_entity.Spawn:
					int spawner_id_number =  getIntR(level_lines[index++]);
					string spawner_cadence = getStringR(level_lines[index++]);
					bool spawned_enemies_know_mech_location = getBoolR(level_lines[index++]);
					bool spawned_enemies_know_base_location = getBoolR(level_lines[index++]); 
					EntityE spawned_enemy_type = getEnemyUpgrade(level_lines[index++]); 
					entityManagerS.instantiateSpawn(x, z, spawner_id_number, spawner_cadence, spawned_enemies_know_mech_location, spawned_enemies_know_base_location, spawned_enemy_type);
					break;
				
				case editor_entity.Enemy:
//				print("enemy case");
					int enemy_current_hp =  getIntR(level_lines[index++]);
					int enemy_max_hp     =  getIntR(level_lines[index++]);
					int enemy_spawner_id_number =  getIntR(level_lines[index++]);
					bool enemy_knows_mech_loc = getBoolR(level_lines[index++]);
					bool enemy_knows_base_loc = getBoolR(level_lines[index++]);
					EntityE enemy_type = getEnemyUpgrade(level_lines[index++]); 
					if(!entityManagerS.instantiateEnemy(x, z, enemy_current_hp, enemy_max_hp, enemy_spawner_id_number, enemy_knows_base_loc, enemy_knows_mech_loc, enemy_type))
						throw new System.Exception("Issue adding enemy!");
					break;
				
				case editor_entity.Flyer:
//				print("enemy case");
					int flyer_current_hp =  getIntR(level_lines[index++]);
					int flyer_max_hp     =  getIntR(level_lines[index++]);
					int flyer_spawner_id_number =  getIntR(level_lines[index++]);
					bool flyer_knows_mech_loc = getBoolR(level_lines[index++]);
					bool flyer_knows_base_loc = getBoolR(level_lines[index++]);
					EntityE flyer_type = getEnemyUpgrade(level_lines[index++]); 
					if(!entityManagerS.instantiateEnemy(x, z, flyer_current_hp, flyer_max_hp, flyer_spawner_id_number, flyer_knows_base_loc, flyer_knows_mech_loc, flyer_type))
						throw new System.Exception("Issue adding flyer!");
					break;
				
				case editor_entity.Factory: 
				case editor_entity.Junkyard: 
				case editor_entity.Outpost:  
					Node node_type                = (Node) Enum.Parse(typeof(Node), ent_type.ToString()); 
					NodeLevel node_starting_level = getNodeLevelR(level_lines[index++]);
					entityManagerS.instantiateResourceNode(x, z, node_type, node_starting_level);
					break;
			}
			  
			if(!getCBR(level_lines[index++]))
			{
				print("MALFORMED HEX!");
				return false;
			}
			
			
		} 

		entityManagerS.getMech().current_ap = current_ap; 
				enginePlayerS.setMech();
		inPlayMenuS.setMech();
		UpgradeMenuS.setMech();
		hexManagerS.setNodePresenseOnHexes();
		entityManagerS.buildEnemySpawnCounts();
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
	
	private static Turn getTurnR(String reader) //close bracket Reader
	{  
          return (Turn) Enum.Parse(typeof(Turn), getStringR(reader)); 
	} 
	
	private static NodeLevel getNodeLevelR(String reader) //close bracket Reader
	{  
          return (NodeLevel) Enum.Parse(typeof(NodeLevel), getStringR(reader));
	} 
	private static Node getNodeR(String reader) //close bracket Reader
	{  
          return (Node) Enum.Parse(typeof(Node), getStringR(reader));
	} 
	
	
	public static BaseUpgradeLevel getBaseUpgrade(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return (BaseUpgradeLevel)BaseUpgradeLevel.Parse(typeof(BaseUpgradeLevel), items[1]);
	} 
	public static EntityE getEnemyUpgrade(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return (EntityE)EntityE.Parse(typeof(EntityE), items[1]);
	} 
}
