using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


public class tileManagerScript : MonoBehaviour {
	
	private Dictionary<int, Dictionary<int, TileData>> tile_db = new Dictionary<int, Dictionary<int, TileData>>();
	
	public GameObject  	initial_hex;
	public GameObject  	border_hex;
	
	public int 			version;
	public string 		level_name;
		
	public enum 		Tiles {Grass, Desert, Forest, Farmland, Marsh, Snow, Mountain, Hills, Water, Settlement, Factory, Outpost, Junkyard, EditorTileA, EditorTileB};
	public enum 		Entities {Player, Enemy};
	 
	public GameObject  	grass_hex;
	public GameObject  	desert_hex;
	public GameObject  	forest_hex;
	public GameObject  	farmland_hex;
	public GameObject  	marsh_hex;
	public GameObject  	snow_hex;
	public GameObject  	mountain_hex;
	public GameObject  	hills_hex;
	public GameObject  	water_hex; 
	
	public static Dictionary<Tiles, GameObject> hex_dict = new Dictionary<Tiles, GameObject>();
	
	
	// Use this for initialization
	void Start () {
//		tile_db  
		
		CreateHex(true, 1, initial_hex, new Vector3(0, 0, 0), initial_hex.GetComponent<hexScript>().tile_type, 0, 0);
		//GameObject origin_hex = 
		//hexScript origin_hex_script = origin_hex.GetComponent<hexScript>();
		
		//origin_hex_script.SurroundHex(border_hex, border_hex.GetComponent<hexScript>().tile_type); //-1 denotes tempborders
		
//		hex_dict = new Dictionary<int, TileData>();
		hex_dict.Add(Tiles.Grass, grass_hex);
		hex_dict.Add(Tiles.Desert, desert_hex);
		hex_dict.Add(Tiles.Forest, forest_hex);
		hex_dict.Add(Tiles.Farmland, farmland_hex);
		hex_dict.Add(Tiles.Marsh, marsh_hex);
		hex_dict.Add(Tiles.Snow, snow_hex);
		hex_dict.Add(Tiles.Mountain, mountain_hex);
		hex_dict.Add(Tiles.Hills, hills_hex);
		hex_dict.Add(Tiles.Water, water_hex);
		hex_dict.Add(Tiles.EditorTileA, border_hex);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void playSelectTile()
	{
		audio.Play();
	}
//	
//	public void CloneHex(GameObject hex_type, tileManagerScript.Tiles data_type, float x_dist, float z_dist, int x_coord_in, int z_coord_in)
//	{ 
//		print ("enter CloneHex");
//		print ("x: " + x_coord_in + " z: " + z_coord_in);
//		
//		if(!occupied(x_coord_in, z_coord_in))
//		{
//			print ("attempting to occupy a hex now...");
//			occupy(new_hex_script.name, new_hex, data_type, new Vector3(x_dist, 0, z_dist), new_hex_script.x_coord, new_hex_script.z_coord);
//		} 
//	}
//	
//	public void BuildHex(GameObject go, tileManagerScript.Tiles data_type,  Vector3 pos, int x_coord_in, int z_coord_in)
//	{
//		print ("enter BuildHex");
//		print ("x: " + x_coord_in + " z: " + z_coord_in);
//		
//		
//			print ("attempting to replace a hex now...");
//			GameObject new_hex = (GameObject) Instantiate(go, pos, Quaternion.identity);
//			hexScript new_hex_script = new_hex.GetComponent<hexScript>();
//			
//			new_hex_script.x_coord = x_coord_in;
//			new_hex_script.z_coord = z_coord_in;
//			new_hex_script.name = "hex("+new_hex_script.x_coord +"," + new_hex_script.z_coord+")";
//			
//			replace(new_hex_script.name, new_hex, data_type, new_hex_script.x_coord, new_hex_script.z_coord);
//		
//		
//	}
	
	
	public bool occupied(int x, int z)
	{
		if(tile_db.ContainsKey(x))
		{
			if(tile_db[x].ContainsKey(z))
			{
				return true;
			}
		}
		return false;
	}
	
	private GameObject InstantiateHex(GameObject hex_type, Vector3 pos, tileManagerScript.Tiles data_type, int x, int z)
	{
		
		GameObject new_hex = (GameObject) Instantiate(hex_type, pos, Quaternion.identity);
		hexScript new_hex_script = new_hex.GetComponent<hexScript>();
		
//		new_hex_script.tile_num = ++playerScript.tile_counter;
		new_hex_script.x_coord = x;
		new_hex_script.z_coord = z;
		new_hex_script.name = "hex("+new_hex_script.x_coord +"," + new_hex_script.z_coord+")";
		new_hex_script.tile_type = data_type;
		
		return new_hex;
	}
	
	public void Save()
	{
		int x_dim, z_dim;
		print ("SAVE!");
        // create a writer and open the file
        TextWriter tw = new StreamWriter("Assets/Level_Files/lvl.pcl");
		
		int x_max = -99999, x_min = 99999, z_max = -99999, z_min = 99999, count = 0;
		foreach(KeyValuePair<int, Dictionary<int, TileData>> entry in tile_db)
		{ 
		    foreach(KeyValuePair<int,TileData> entry_2 in entry.Value)
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
		
		foreach(KeyValuePair<int, Dictionary<int, TileData>> entry in tile_db)
		{
		    foreach(KeyValuePair<int,TileData> entry_2 in entry.Value)
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
	
	public GameObject CreateHex(bool overwrite, int depth, GameObject hex_type, Vector3 pos, tileManagerScript.Tiles data_type, int x, int z)
	{
		depth--;
		
		if(data_type != Tiles.EditorTileA)
		{
			playerScript.last_created_hex_type = data_type;
			playerScript.last_created_hex_go   = hex_type;
		}
		
		GameObject created_hex;
		hexScript created_hex_script;
		print ("creating : " + x + ", " + z);
		if(tile_db.ContainsKey(x))
		{ 
			if(tile_db[x].ContainsKey(z))
			{
				if((overwrite && tile_db[x][z].getDataType() != data_type) || (tile_db[x][z].getDataType() == Tiles.EditorTileA && data_type != Tiles.EditorTileA))  //tile_db[x][z].getDataType() == Tiles.EditorTileA && data_type != Tiles.EditorTileA
				{
					//if there is already an entry there and we want to overwrite, overwrite it
					print ("if _ if _ if _ replacing : " + x + ", " + z + "with a " + data_type);
					
					if(tile_db[x][z].getOccupier() != playerScript.last_created_hex_go)
					{
						created_hex = InstantiateHex(hex_type, pos, data_type, x, z);
						created_hex_script = created_hex.GetComponent<hexScript>();
						
						//now delete what is already there
						Destroy(tile_db[x][z].getOccupier());
						tile_db[x][z]= new TileData(created_hex_script.name, created_hex, data_type, x, z);
						
						if(data_type != Tiles.EditorTileA && depth == 0)
						{
							SurroundHex(border_hex,  created_hex_script.transform.position, Tiles.EditorTileA, 0, x, z);
						}
						else
						if(depth > 0)
						{
							SurroundHex(created_hex,  created_hex_script.transform.position,data_type, depth, x, z);
						}
						return created_hex; 
					}
					else
					{
						print("same type already there bro");	
					}
					return null;
					
				}
				else
				{
					//there is something there and we dont have overwrite permission, so do nothing
					print ("if _ if _ else _ Something already there, doin' nothin'. ");
					return null;
				}
			}
			else
			{
				//we've just gotta make a new z entry since nothing has ever been made in this z spot
				print ("if _ else _ occupying : " + x + ", " + z);
				created_hex = InstantiateHex(hex_type, pos, data_type, x, z);
				created_hex_script = created_hex.GetComponent<hexScript>();
				
				tile_db[x].Add(z, new TileData(created_hex_script.name, created_hex, data_type, x, z));
//				if(data_type != Tiles.EditorTileA)
//				{
//					created_hex_script.SurroundHex(border_hex, Tiles.EditorTileA, depth);
//				}
				
				if(data_type != Tiles.EditorTileA && depth == 0)
				{
					SurroundHex(border_hex, created_hex_script.transform.position, Tiles.EditorTileA, 0, x, z);
				}
				else
				if(depth > 0)
				{
					SurroundHex(created_hex, created_hex_script.transform.position, data_type, depth, x, z);
				}
				return created_hex;
			}
		}
		else
		{
			//nothing has ever been made in this x row, so make the z dict and the z entry
			print ("else ________ occupying : " + x + ", " + z);
			created_hex = InstantiateHex(hex_type, pos, data_type, x, z);
			created_hex_script = created_hex.GetComponent<hexScript>();
			
			tile_db.Add(x, new Dictionary<int, TileData>());
			tile_db[x].Add(z, new TileData(created_hex_script.name, created_hex, data_type,x, z));
//			if(data_type != Tiles.EditorTileA)
//			{
//				created_hex_script.SurroundHex(border_hex, Tiles.EditorTileA);
//			}
			
			if(data_type != Tiles.EditorTileA && depth == 0)
			{
				SurroundHex(border_hex, created_hex_script.transform.position,  Tiles.EditorTileA, 0, x, z);
			}
			else
			if(depth > 0)
			{
				SurroundHex(created_hex, created_hex_script.transform.position, data_type, depth, x, z);
			}
			return created_hex;
		}
	}
	

	
	public void SurroundHex(GameObject hex_type, Vector3 center_pos, tileManagerScript.Tiles data_type, int depth, int x_coord, int z_coord)
	{
		bool overwrite_enabled;
		if(data_type != tileManagerScript.Tiles.EditorTileA && playerScript.overwrite_mode)
		{
			overwrite_enabled = true;
		}
		else
		{
			overwrite_enabled = false;
		}
		
		//CloneNorth
		float x_trans = -0.841947F + center_pos.x;
		float z_trans =  1.81415F  + center_pos.z;
		playerScript.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans), data_type, x_coord, z_coord + 1);
		
		//CloneNorthEast
		x_trans = 2.30024F 	  + center_pos.x;
		z_trans = 1.3280592F  + center_pos.z;
		playerScript.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans), data_type, x_coord + 1, z_coord);
		
		//CloneSouthEast
		x_trans = 3.14219F    + center_pos.x;
		z_trans = -0.486092F  + center_pos.z; 
		playerScript.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans), data_type, x_coord + 1, z_coord - 1);
		
		//CloneSouth
		x_trans =  0.841947F + center_pos.x;
		z_trans = -1.81415F  + center_pos.z;
		playerScript.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans),  data_type, x_coord, z_coord - 1);
		
		// CloneSouthWest
		x_trans = -2.30024F    + center_pos.x;
		z_trans = -1.3280592F  + center_pos.z;
		playerScript.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans), data_type, x_coord - 1, z_coord);
		
		//CloneNorthWest
		x_trans = -3.14219F  + center_pos.x;
		z_trans = 0.486092F  + center_pos.z;
		playerScript.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans),  data_type, x_coord - 1, z_coord + 1);
		
		
	}
	
	private class TileData {
		
		public string tile_name;
		public int x_coord;
		public int z_coord;
		public tileManagerScript.Tiles data_type;
		public string hex_type;
		public string[] contents;
		public GameObject occupier;
		
		public TileData(string _tile_name,
						GameObject _occupier,
						tileManagerScript.Tiles    _data_type,
						int    _x_coord,
						int    _z_coord)
		{
			x_coord   = _x_coord;
			z_coord   = _z_coord;
			tile_name = _tile_name;
			occupier  = _occupier;
			data_type = _data_type;
		} 
		
		public tileManagerScript.Tiles getDataType()
		{
			return data_type;
		}
		public GameObject getOccupier()
		{
			return occupier;
		}
		
	}	
}
