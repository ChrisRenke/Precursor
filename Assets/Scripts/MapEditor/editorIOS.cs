using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System;
using System.Text; 
using System.IO;
//using System.;

public class editorIOS : MonoBehaviour {
	
	
	public static string       level_name = "untitled level";  
	public static int 			version; 
	public static int          level_editor_format_version = 7;
	public static string 		LAST_EDITED  = "LASTEDITED";
	private static String      key_list_key = "LEVELKEYS";
	
	private	static string[] stringSeparators = new string[] {" = "};  
	
	// Use this for initialization
	void Start () 
	{ 
//		level_name = PlayerPrefs.GetString(LAST_EDITED, "untitled level");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	
	void OnGUI()
	{
		 
		if(GUI.Button(new Rect(Screen.width - 240, 30, 100, 30), "Save"))
		{
			 Save();
		}
		
		if(GUI.Button(new Rect(Screen.width - 130, 30, 100, 30), "Load"))
		{
			 Load();
		}
		
		level_name = GUI.TextField(new Rect(Screen.width - 240, 70, 210, 30), level_name, 30);
	}
	
	public void Save()
	{
//		System.IO.File.WriteAllText(Application.dataPath + "/yourtextfile.txt", "shitshittest");
		int x_dim, z_dim;
		print ("SAVE!");
        // create a writer and open the file
		
//		Application.dataPath + "/data/world/
		TextWriter tw = new StreamWriter(Application.dataPath + "/" + level_name + ".txt");
		
//		//EDITOR VERSION
//        TextWriter tw = new StreamWriter("Assets/Level_Files/" + level_name + ".txt");
		
//		StringBuilder sb = new StringBuilder();
		
		
		
		int x_max = -99999, x_min = 99999, z_max = -99999, z_min = 99999, count = 0, border_count = 0;
		foreach(KeyValuePair<int, Dictionary<int, editorHexManagerS.editorHexData>> entry in editorHexManagerS.hex_db)
		{ 
		    foreach(KeyValuePair<int, editorHexManagerS.editorHexData> entry_2 in entry.Value)
			{
				if(entry_2.Value.x_coord < x_min)
					x_min = entry_2.Value.x_coord;
				if(entry_2.Value.x_coord > x_max)
					x_max = entry_2.Value.x_coord; 
				
				if(entry_2.Value.z_coord < z_min)
					z_min = entry_2.Value.z_coord;
				if(entry_2.Value.z_coord > z_max)
					z_max = entry_2.Value.z_coord; 
				
				count++;
				
				if(entry_2.Value.hex_type == Hex.Perimeter)
					border_count++;
			}
		}
		x_dim = x_max - x_min + 1; //+1 to account for 0 space
		z_dim = z_max - z_min + 1; //+1 to account for 0 space

        // write a line of text to the file
        tw.WriteLine("LEVEL{");
		
        tw.WriteLine("\tEDITOR VER   = " + level_editor_format_version );
        tw.WriteLine("\tLevel Name   = " + level_name );
        tw.WriteLine("\tLevel Ver    = " + version ); 
		
        tw.WriteLine("\tRound Num    = " + "1" ); 
		tw.WriteLine("\tTurn         = " + "Player");
		tw.WriteLine("\tCurrent_ap   = " + "18");
		
        tw.WriteLine("\tX_dim        = " + x_dim ); 
        tw.WriteLine("\tZ_dim        = " + z_dim );  
		
        tw.WriteLine("\tX_min        = " + x_min );  
        tw.WriteLine("\tX_max        = " + x_max );  
        tw.WriteLine("\tZ_min        = " + z_min );  
        tw.WriteLine("\tZ_max        = " + z_max ); 
		
        tw.WriteLine("\tTotal_Hexes  = " + count ); 
        tw.WriteLine("\tGame_Hexes   = " + (count - border_count)); 
        tw.WriteLine("\tBorder_Hexes = " + border_count);  
        tw.WriteLine("\tHEXES{");
		
		foreach(KeyValuePair<int, Dictionary<int, editorHexManagerS.editorHexData>> entry in editorHexManagerS.hex_db)
		{
		    foreach(KeyValuePair<int, editorHexManagerS.editorHexData> entry_2 in entry.Value)
			{
				
        		tw.WriteLine("\t\tHEX{ " );
//        		tw.WriteLine("\t\t\tName = " + entry_2.Value.tile_name);
        		tw.WriteLine("\t\t\tX    = " + entry_2.Value.x_coord);
        		tw.WriteLine("\t\t\tZ    = " + entry_2.Value.z_coord);
        		tw.WriteLine("\t\t\tType = " + entry_2.Value.hex_type);
        		tw.WriteLine("\t\t}");
			}
			// do something with entry.Value or entry.Key
		}
		
        tw.WriteLine("\t}");
		
		
        tw.WriteLine("\tENTITIES{");
		
		foreach(KeyValuePair<int, Dictionary<int, GameObject>> entry_1 in editorEntityManagerS.entity_db)
		{
		    foreach(KeyValuePair<int, GameObject> entry_2 in entry_1.Value)
			{
				
				entity_core new_entity = entry_2.Value.GetComponent<entity_core>();
				tw.Write(new_entity.getOutput());
			}
			// do something with entry.Value or entry.Key
		}
		
        tw.WriteLine("\t}");		
        tw.WriteLine("}");
		tw.Flush();
		tw.Close();
		
		
		//store level data to prefs
//		PlayerPrefs.SetString (level_name, sb.ToString());
//		
//		//store level name as a level key
//		PlayerPrefs.SetString (key_list_key, PlayerPrefs.GetString(key_list_key, "") + level_name + "\n");
//		
//		//store last edited level key
//		PlayerPrefs.SetString (LAST_EDITED, level_name);
		
	}
	
	
	public static bool Load()
	{
		// 1 - delete EVERYTHING that exists in map
		// 2 - start drawing hexes from file
		// 3 - draw and set props of entiteis from file
		//  CoordsGameTo3D(x, z) returns a vector3 hell ya

	//INITIALIZE FILE TO READ\
		StreamReader reader;
		FileInfo filer = new FileInfo(Application.dataPath + "/" + level_name + ".txt");
		if(filer != null && filer.Exists)
		{
		   reader  = null;//= filer.OpenText();  // returns StreamReader
		} 
		else
		{
			print ("FILE DOES NOT EXIST!");
			return false;
		}
		  
//		String level_data = PlayerPrefs.GetString(level_name, "DIDNTLOAD");
//		string[] level_lines = level_data.Split(new char[] {'\n'});
//		int index = 0;
//		
//		if(level_data.Equals("DIDNTLOAD"))
//			return false;
		
		
	//DELETE EXISTING DATA FROM EDITOR
		//remove existing hexes
		foreach(KeyValuePair<int, Dictionary<int, editorHexManagerS.editorHexData>> entry in editorHexManagerS.hex_db)
		{ 
		    foreach(KeyValuePair<int, editorHexManagerS.editorHexData> entry_2 in entry.Value)
			{
				Destroy(entry_2.Value.occupier);
			}
		}
		editorHexManagerS.hex_db = new Dictionary<int, Dictionary<int, editorHexManagerS.editorHexData>>();
		
		//remove existing entities 
		foreach(KeyValuePair<int, Dictionary<int, GameObject>> entry_1 in editorEntityManagerS.entity_db)
		{
		    foreach(KeyValuePair<int, GameObject> entry_2 in entry_1.Value)
			{
				Destroy(entry_2.Value);
			}
		}
		editorEntityManagerS.entity_db = new Dictionary<int, Dictionary<int, GameObject>>();
			
		
		int x_dim, z_dim, total_count, game_count, border_count;
	
//	//BEGIN PARSING DATA
		//PARSE HEADER INFO
		if(!reader.ReadLine().Equals("LEVEL{"))
		{
			print("l1 ILL FORMATED!");
			return false;
		} 
		if(getIntR(reader.ReadLine()) != level_editor_format_version) //EDITOR VER
		{
			print ("EDITOR VERSION MISMATCH!");
			return false;
		}
		level_name 		= getStringR(reader.ReadLine()); //NAME
		version    		= getIntR(reader.ReadLine());    //VERSION
		
		getIntR(reader.ReadLine());
		getStringR(reader.ReadLine());
		getIntR(reader.ReadLine());
		
		x_dim 			= getIntR(reader.ReadLine());  		//X_dim
		z_dim 			= getIntR(reader.ReadLine());		//Z_dim
				 
		getIntR(reader.ReadLine());
		getIntR(reader.ReadLine());
		getIntR(reader.ReadLine());
		getIntR(reader.ReadLine());
		
		total_count 	= getIntR(reader.ReadLine());  		//total count
		game_count 		= getIntR(reader.ReadLine());		//game count
		border_count 	= getIntR(reader.ReadLine());  		//border count
		
		 
		
		//BEGIN PARSING HEXES
		if(!reader.ReadLine().Contains("HEXES{"))
		{
			print("HEXES ILL FORMATED");
			return false;
		}
		while(getHexR(reader.ReadLine())) //next line is a HEX{
		{
			int x = getIntR(reader.ReadLine());
			int z = getIntR(reader.ReadLine());
            Hex hex_type = (Hex) Enum.Parse(typeof(Hex), getStringR(reader.ReadLine()));
			editorUserS.tms.LoadHex(hex_type, x, z);
			if(!getCBR(reader.ReadLine()))
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
		while(getEntR(reader.ReadLine())) //next line is a HEX{
		{
			int x = getIntR(reader.ReadLine());
			int z = getIntR(reader.ReadLine());
            editor_entity ent_type = (editor_entity) Enum.Parse(typeof(editor_entity), getStringR(reader.ReadLine())); 
			
			switch(ent_type){
					
				case editor_entity.Enemy:
					editorUserS.ems.enemy_current_hp = getIntR(reader.ReadLine());
					editorUserS.ems.enemy_max_hp = getIntR(reader.ReadLine());
					editorUserS.ems.enemy_spawner_owner_id   = getIntR(reader.ReadLine());
					editorUserS.ems.enemy_know_mech_location = getBoolR(reader.ReadLine());
					editorUserS.ems.enemy_know_base_location = getBoolR(reader.ReadLine());
				break;
				
				case editor_entity.Spawn:
//					editorUserS.ems.spawner_max_enemies_from_this_spawn = getIntR(reader.ReadLine());
					editorUserS.ems.spawner_id_number = getIntR(reader.ReadLine());
					editorUserS.ems.spawner_cadence = getStringR(reader.ReadLine());
					editorUserS.ems.spawned_enemies_know_mech_location = getBoolR(reader.ReadLine());
					editorUserS.ems.spawned_enemies_know_base_location = getBoolR(reader.ReadLine()); 
				break;
				
				case editor_entity.Mech:
					editorUserS.ems.mech_current_hp  =  getIntR(reader.ReadLine());
					editorUserS.ems.mech_max_hp = getIntR(reader.ReadLine()); 	
					editorUserS.ems.gun_range	= getBoolR(reader.ReadLine());
					editorUserS.ems.gun_cost	= getBoolR(reader.ReadLine());
					editorUserS.ems.gun_damage	= getBoolR(reader.ReadLine());
					editorUserS.ems.mobi_cost	= getBoolR(reader.ReadLine());
					editorUserS.ems.mobi_water	= getBoolR(reader.ReadLine());
					editorUserS.ems.mobi_mntn	= getBoolR(reader.ReadLine());
					editorUserS.ems.ex_tele		= getBoolR(reader.ReadLine());
					editorUserS.ems.ex_armor	= getBoolR(reader.ReadLine());
					editorUserS.ems.ex_scav		= getBoolR(reader.ReadLine()); 
				break;
				
				case editor_entity.Town:
					editorUserS.ems.town_current_hp =  getIntR(reader.ReadLine());
					editorUserS.ems.town_max_hp     =  getIntR(reader.ReadLine());
					editorUserS.ems.town_wall_level = getBaseUpgrade(reader.ReadLine());
					editorUserS.ems.town_defense_level = getBaseUpgrade(reader.ReadLine()); 
					editorUserS.ems.town_structure_level = getBaseUpgrade(reader.ReadLine()); 
				break;
					
				case editor_entity.Junkyard:
				case editor_entity.Factory: 
				case editor_entity.Outpost: 
					editorUserS.ems.node_level = getNodeLevelR(reader.ReadLine());
				break;
			} 
			 
			editorUserS.ems.LoadEntity(ent_type, x, z);
			
			if(!getCBR(reader.ReadLine()))
			{
				print("MALFORMED HEX!");
				return false;
			}
		}
		 
		//store last edited level key
//		PlayerPrefs.SetString (LAST_EDITED, level_name);
		return true;
	}
	
	public static BaseUpgrade getBaseUpgrade(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return (BaseUpgrade)BaseUpgrade.Parse(typeof(BaseUpgrade), items[1]);
	}
	public static NodeLevel getNodeLevelR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return (NodeLevel)NodeLevel.Parse(typeof(NodeLevel), items[1]);
	}
	
	public static string getStringR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return items[1];
	}
	 
	public static int getIntR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return int.Parse(items[1]);
	}
	
	public static bool getBoolR(String line)
	{  
    	string[] items = line.Split(stringSeparators, StringSplitOptions.None);
		return bool.Parse(items[1]);
	}
	
	public static bool getCBR(String line) //close bracket Reader
	{  
		print ("CBR CHECK: " + line);
    	return line.Contains("}"); 
	}
	
	public static  bool getOBR(String reader) //close bracket Reader
	{  
    	return reader.Contains("{"); 
	}
	
	public static bool getHexR(String reader) //close bracket Reader
	{  
    	return reader.Contains("HEX{"); 
	}
	
	public static bool getEntR(String reader) //close bracket Reader
	{  
    	return reader.Contains("ENTITY{"); 
	} 
	
}
