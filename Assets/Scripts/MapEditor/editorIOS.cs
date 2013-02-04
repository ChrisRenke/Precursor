using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System;
using System.IO;

public class editorIOS : MonoBehaviour {
	
	
	public string       level_name; 
	public int 			version; 
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public void Save()
	{
		int x_dim, z_dim;
		print ("SAVE!");
        // create a writer and open the file
        TextWriter tw = new StreamWriter("Assets/Level_Files/" + level_name + ".pcl");
		
		int x_max = -99999, x_min = 99999, z_max = -99999, z_min = 99999, count = 0;
		foreach(KeyValuePair<int, Dictionary<int, editorHexManagerS.HexData>> entry in editorHexManagerS.hex_db)
		{ 
		    foreach(KeyValuePair<int, editorHexManagerS.HexData> entry_2 in entry.Value)
			{
				
			}
		}

        // write a line of text to the file
        tw.WriteLine("LEVEL\n{");
        tw.WriteLine("\tVName       = " + level_name );
        tw.WriteLine("\tVersion     = " + version ); 
//        tw.WriteLine("\tx_dimension = " + x_dim ); 
//        tw.WriteLine("\tz_dimension = " + z_dim );  
        tw.WriteLine("\tTILES\n\t{");
		
		foreach(KeyValuePair<int, Dictionary<int, editorHexManagerS.HexData>> entry in editorHexManagerS.hex_db)
		{
		    foreach(KeyValuePair<int, editorHexManagerS.HexData> entry_2 in entry.Value)
			{
				
        		tw.WriteLine("\t\tHEX{ " );
        		tw.WriteLine("\t\t\tName = " + entry_2.Value.tile_name);
        		tw.WriteLine("\t\t\tX    = " + entry_2.Value.x_coord);
        		tw.WriteLine("\t\t\tZ    = " + entry_2.Value.z_coord);
        		tw.WriteLine("\t\t\tType = " + entry_2.Value.data_type);
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
        tw.WriteLine(DateTime.Now);
        tw.WriteLine(DateTime.Now);

		

        // close the stream
        tw.Close();
	}
	
}
