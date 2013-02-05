using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System;
using System.IO;

public class editorIOS : MonoBehaviour {
	
	
	public string       level_name = "untitled level";  
	public int 			version; 
	public int          level_editor_format_version = 2;
	 
	
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
        tw.WriteLine("LEVEL\n{");
		
        tw.WriteLine("\tEDITOR VER. = " + level_editor_format_version );
        tw.WriteLine("\tName        = " + level_name );
        tw.WriteLine("\tVersion     = " + version ); 
        tw.WriteLine("\tX_dim       = " + x_dim ); 
        tw.WriteLine("\tZ_dim       = " + z_dim );  
        tw.WriteLine("\tTot_Hex_num = " + count ); 
        tw.WriteLine("\tGameHex_num = " + (count - border_count)); 
        tw.WriteLine("\tBourder_num = " + border_count);  
        tw.WriteLine("\tHEXES\n\t{");
		
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
		
		
        tw.WriteLine("\tENTITIES\n\t{");
		
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
	
	
	public void Load()
	{
		// 1 - delete EVERYTHING that exists in map
		// 2 - start drawing hexes from file
		// 3 - draw and set props of entiteis from file
	}
	
}
