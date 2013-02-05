using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System;
using System.IO; 

public class editorIOS : MonoBehaviour {
	
	
	public string       level_name = "untitled level";  
	public int 			version; 
	public int          level_editor_format_version = 2;
	 
	
	private	string[] stringSeparators = new string[] {" = "};  
	
	// Use this for initialization
	void Start () 
	{ 
	
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
        TextWriter tw = new StreamWriter("Assets/Level_Files/" + level_name + ".pcl");
		
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
				
				if(entry_2.Value.hex_type == editorHexManagerS.Hex.EditorTileA)
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
        tw.WriteLine("\tX_dim        = " + x_dim ); 
        tw.WriteLine("\tZ_dim        = " + z_dim );  
        tw.WriteLine("\tTotal_Hexes  = " + count ); 
        tw.WriteLine("\tGame_Hexes   = " + (count - border_count)); 
        tw.WriteLine("\tBorder_Hexes = " + border_count);  
        tw.WriteLine("\tHEXES{");
		
		foreach(KeyValuePair<int, Dictionary<int, editorHexManagerS.HexData>> entry in editorHexManagerS.hex_db)
		{
		    foreach(KeyValuePair<int, editorHexManagerS.HexData> entry_2 in entry.Value)
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
		
		foreach(KeyValuePair<int, Dictionary<int, editorEntityManagerS.EntityData>> entry_1 in editorEntityManagerS.entity_db)
		{
		    foreach(KeyValuePair<int, editorEntityManagerS.EntityData> entry_2 in entry_1.Value)
			{
				
        		tw.WriteLine("\t\tENTITY{ " );
//        		tw.WriteLine("\t\t\tName       = " + entry_2.Value.name);
        		tw.WriteLine("\t\t\tX          = " + entry_2.Value.x_coord);
        		tw.WriteLine("\t\t\tZ          = " + entry_2.Value.z_coord);
        		tw.WriteLine("\t\t\tType       = " + entry_2.Value.entity_type);
				
				editorEntityS ent_s = entry_2.Value.occupier.GetComponent<editorEntityS>();
				if(ent_s.entity_type == editorEntityManagerS.Entity.Base)
				{					//  Z          = "
        			tw.WriteLine("\t\t\tHP Perc    = " + ent_s.base_starting_health_percentage); 
				}
				else if(ent_s.entity_type == editorEntityManagerS.Entity.Player)
				{
        			tw.WriteLine("\t\t\tHP Perc    = " + ent_s.mech_starting_health_percentage); 
				}
				else if(ent_s.entity_type == editorEntityManagerS.Entity.Factory ||
						ent_s.entity_type == editorEntityManagerS.Entity.Junkyard ||
						ent_s.entity_type == editorEntityManagerS.Entity.Outpost)
				{ 
        			tw.WriteLine("\t\t\tNode Lvl   = " + ent_s.node_starting_level); 
				}
				else if(ent_s.entity_type == editorEntityManagerS.Entity.Enemy)
				{
        			tw.WriteLine("\t\t\tOmnsc Base = " + ent_s.enemy_knows_base_loc); 
        			tw.WriteLine("\t\t\tOmnsc Mech = " + ent_s.enemy_knows_mech_loc);  
				} 
        		tw.WriteLine("\t\t}");
			}
			// do something with entry.Value or entry.Key
		}
		
        tw.WriteLine("\t}");
		
		
        tw.WriteLine("}");
		
		
		
		// Store keys in a List
//		List<int> list_x = new List<int>(hex_dict.Keys);
//		// Loop through list
//		foreach (int x in list_x)
//		{
//			
//			List<int> list_z = new List<int>(hex_dict[x].Keys);
//			foreach (int z in list_z)
//			{
//			}
//		}

		

        // close the stream
        tw.Close();
	}
	
	
	public bool Load()
	{
		// 1 - delete EVERYTHING that exists in map
		// 2 - start drawing hexes from file
		// 3 - draw and set props of entiteis from file
		//  CoordsGameTo3D(x, z) returns a vector3 hell ya

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
		version    		= getIntR(reader);    //VERSION
		
		int x_dim, z_dim, total_count, game_count, border_count;
		x_dim 			= getIntR(reader);  		//X_dim
		z_dim 			= getIntR(reader);		//Z_dim
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
			int x = getIntR(reader);
			int z = getIntR(reader);
            editorHexManagerS.Hex hex_type = (editorHexManagerS.Hex) Enum.Parse(typeof(editorHexManagerS.Hex), getStringR(reader));
			editorUserS.tms.LoadHex(hex_type, x, z);
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
			int x = getIntR(reader);
			int z = getIntR(reader);
            editorEntityManagerS.Entity ent_type = (editorEntityManagerS.Entity) Enum.Parse(typeof(editorEntityManagerS.Entity), getStringR(reader));
			
			editorUserS.ems.base_starting_health_percentage = 100;
			editorUserS.ems.mech_starting_health_percentage = 100;
			editorUserS.ems.enemy_knows_base_loc 			= false;
			editorUserS.ems.enemy_knows_mech_loc 			= false;
			editorUserS.ems.node_starting_level				= 2;
			
			switch(ent_type)
			{
				case editorEntityManagerS.Entity.Base:
					int base_hp_percentage = getIntR(reader);
					break;
				case editorEntityManagerS.Entity.Player:
					int mech_hp_percentage = getIntR(reader);
					break;
				case editorEntityManagerS.Entity.Enemy:
					bool enemy_base_know = getBoolR(reader);
					bool enemy_mech_know = getBoolR(reader);
					break;
				case editorEntityManagerS.Entity.Factory:
				case editorEntityManagerS.Entity.Junkyard:
				case editorEntityManagerS.Entity.Outpost:
					int node_lvl = getIntR(reader);
					break;
			}
			 
			editorUserS.ems.LoadEntity(ent_type, x, z);
			if(!getCBR(reader))
			{
				print("MALFORMED HEX!");
				return false;
			}
		}
		 
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
	
}
