using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System;
using System.Text; 

public class editorIOS : MonoBehaviour {
	
	
	public static string       level_name = "untitled level";  
	public static int 			version; 
	public static int          level_editor_format_version = 3;
	public static string 		LAST_EDITED  = "LASTEDITED";
	private static String      key_list_key = "LEVELKEYS";
	
	private	static string[] stringSeparators = new string[] {" = "};  
	
	// Use this for initialization
	void Start () 
	{ 
		level_name = PlayerPrefs.GetString(LAST_EDITED, "untitled level");
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
		int x_dim, z_dim;
		print ("SAVE!");
        // create a writer and open the file
//        TextWriter tw = new StreamWriter("Assets/Level_Files/" + level_name + ".pcl");
		
		StringBuilder sb = new StringBuilder();
		
		
		
		int x_max = -99999, x_min = 99999, z_max = -99999, z_min = 99999, count = 0, border_count = 0;
		foreach(KeyValuePair<int, Dictionary<int, editorHexManagerS.HexData>> entry in editorHexManagerS.hex_db)
		{ 
		    foreach(KeyValuePair<int, editorHexManagerS.HexData> entry_2 in entry.Value)
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
        sb.AppendLine("LEVEL{");
		
        sb.AppendLine("\tEDITOR VER   = " + level_editor_format_version );
        sb.AppendLine("\tLevel Name   = " + level_name );
        sb.AppendLine("\tLevel Ver    = " + version ); 
        sb.AppendLine("\tX_dim        = " + x_dim ); 
        sb.AppendLine("\tZ_dim        = " + z_dim );  
        sb.AppendLine("\tX_min        = " + x_min );  
        sb.AppendLine("\tX_max        = " + x_max );  
        sb.AppendLine("\tZ_min        = " + z_min );  
        sb.AppendLine("\tZ_max        = " + z_max );  
        sb.AppendLine("\tTotal_Hexes  = " + count ); 
        sb.AppendLine("\tGame_Hexes   = " + (count - border_count)); 
        sb.AppendLine("\tBorder_Hexes = " + border_count);  
        sb.AppendLine("\tHEXES{");
		
		foreach(KeyValuePair<int, Dictionary<int, editorHexManagerS.HexData>> entry in editorHexManagerS.hex_db)
		{
		    foreach(KeyValuePair<int, editorHexManagerS.HexData> entry_2 in entry.Value)
			{
				
        		sb.AppendLine("\t\tHEX{ " );
//        		sb.AppendLine("\t\t\tName = " + entry_2.Value.tile_name);
        		sb.AppendLine("\t\t\tX    = " + entry_2.Value.x_coord);
        		sb.AppendLine("\t\t\tZ    = " + entry_2.Value.z_coord);
        		sb.AppendLine("\t\t\tType = " + entry_2.Value.hex_type);
        		sb.AppendLine("\t\t}");
			}
			// do something with entry.Value or entry.Key
		}
		
        sb.AppendLine("\t}");
		
		
        sb.AppendLine("\tENTITIES{");
		
		foreach(KeyValuePair<int, Dictionary<int, editorEntityManagerS.EntityData>> entry_1 in editorEntityManagerS.entity_db)
		{
		    foreach(KeyValuePair<int, editorEntityManagerS.EntityData> entry_2 in entry_1.Value)
			{
				
        		sb.AppendLine("\t\tENTITY{ " );
//        		sb.AppendLine("\t\t\tName       = " + entry_2.Value.name);
        		sb.AppendLine("\t\t\tX          = " + entry_2.Value.x_coord);
        		sb.AppendLine("\t\t\tZ          = " + entry_2.Value.z_coord);
        		sb.AppendLine("\t\t\tType       = " + entry_2.Value.entity_type);
				
				editorEntityS ent_s = entry_2.Value.occupier.GetComponent<editorEntityS>();
				if(ent_s.entity_type == EntityE.Base)
				{					//  Z          = "
        			sb.AppendLine("\t\t\tHP Perc    = " + ent_s.base_starting_health_percentage); 
				}
				else if(ent_s.entity_type == EntityE.Player)
				{
        			sb.AppendLine("\t\t\tHP Perc    = " + ent_s.mech_starting_health_percentage); 
				}
				else if(ent_s.entity_type == EntityE.Node)
				{ 
        			sb.AppendLine("\t\t\tNode       = " + ent_s.node_type); 
        			sb.AppendLine("\t\t\tNode Lvl   = " + ent_s.node_starting_level); 
				}
				else if(ent_s.entity_type == EntityE.Enemy)
				{
        			sb.AppendLine("\t\t\tOmnsc Base = " + ent_s.enemy_knows_base_loc); 
        			sb.AppendLine("\t\t\tOmnsc Mech = " + ent_s.enemy_knows_mech_loc);  
				} 
        		sb.AppendLine("\t\t}");
			}
			// do something with entry.Value or entry.Key
		}
		
        sb.AppendLine("\t}");		
        sb.AppendLine("}");
		
		//store level data to prefs
		PlayerPrefs.SetString (level_name, sb.ToString());
		
		//store level name as a level key
		PlayerPrefs.SetString (key_list_key, PlayerPrefs.GetString(key_list_key, "") + level_name + "\n");
		
		//store last edited level key
		PlayerPrefs.SetString (LAST_EDITED, level_name);
		
	}
	
	
	public static bool Load()
	{
		// 1 - delete EVERYTHING that exists in map
		// 2 - start drawing hexes from file
		// 3 - draw and set props of entiteis from file
		//  CoordsGameTo3D(x, z) returns a vector3 hell ya

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
		  
		String level_data = PlayerPrefs.GetString(level_name, "DIDNTLOAD");
		string[] level_lines = level_data.Split(new char[] {'\n'});
		int index = 0;
		
		if(level_data.Equals("DIDNTLOAD"))
			return false;
		
		
	//DELETE EXISTING DATA FROM EDITOR
		//remove existing hexes
		foreach(KeyValuePair<int, Dictionary<int, editorHexManagerS.HexData>> entry in editorHexManagerS.hex_db)
		{ 
		    foreach(KeyValuePair<int, editorHexManagerS.HexData> entry_2 in entry.Value)
			{
				Destroy(entry_2.Value.occupier);
			}
		}
		editorHexManagerS.hex_db = new Dictionary<int, Dictionary<int, editorHexManagerS.HexData>>();
		
		//remove existing entities 
		foreach(KeyValuePair<int, Dictionary<int, editorEntityManagerS.EntityData>> entry_1 in editorEntityManagerS.entity_db)
		{
		    foreach(KeyValuePair<int, editorEntityManagerS.EntityData> entry_2 in entry_1.Value)
			{
				Destroy(entry_2.Value.occupier);
			}
		}
		editorEntityManagerS.entity_db = new Dictionary<int, Dictionary<int, editorEntityManagerS.EntityData>>();
			
		
	
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
		version    		= getIntR(level_lines[index++]);    //VERSION
		
		int x_dim, z_dim, total_count, game_count, border_count;
		x_dim 			= getIntR(level_lines[index++]);  		//X_dim
		z_dim 			= getIntR(level_lines[index++]);		//Z_dim
		
		getIntR(level_lines[index++]);
		getIntR(level_lines[index++]);
		getIntR(level_lines[index++]);
		getIntR(level_lines[index++]);
		
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
			int x = getIntR(level_lines[index++]);
			int z = getIntR(level_lines[index++]);
            Hex hex_type = (Hex) Enum.Parse(typeof(Hex), getStringR(level_lines[index++]));
			editorUserS.tms.LoadHex(hex_type, x, z);
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
			int x = getIntR(level_lines[index++]);
			int z = getIntR(level_lines[index++]);
            EntityE ent_type = (EntityE) Enum.Parse(typeof(EntityE), getStringR(level_lines[index++]));
			
			editorUserS.ems.base_starting_health_percentage = 100;
			editorUserS.ems.mech_starting_health_percentage = 100;
			editorUserS.ems.enemy_knows_base_loc 			= false;
			editorUserS.ems.enemy_knows_mech_loc 			= false;
			editorUserS.ems.node_starting_level				= 2;
			
			switch(ent_type)
			{
				case EntityE.Base:
					editorUserS.ems.base_starting_health_percentage = getIntR(level_lines[index++]);
					break;
				case EntityE.Player:
					editorUserS.ems.mech_starting_health_percentage = getIntR(level_lines[index++]);
					break;
				case EntityE.Enemy:
					editorUserS.ems.enemy_knows_base_loc = getBoolR(level_lines[index++]);
					editorUserS.ems.enemy_knows_mech_loc = getBoolR(level_lines[index++]);
					break;
				case EntityE.Node:
					editorUserS.ems.node_starting_level	 = getIntR(level_lines[index++]);
					break;
			}
			 
			editorUserS.ems.LoadEntity(ent_type, x, z);
			if(!getCBR(level_lines[index++]))
			{
				print("MALFORMED HEX!");
				return false;
			}
		}
		 
		//store last edited level key
		PlayerPrefs.SetString (LAST_EDITED, level_name);
		return true;
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
