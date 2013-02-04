using UnityEngine;
using System.Collections;
using System.Collections.Generic; 


public class editorHexManagerS : MonoBehaviour {
	
	public static Dictionary<int, Dictionary<int, editorHexManagerS.HexData>> hex_db = new Dictionary<int, Dictionary<int, editorHexManagerS.HexData>>();
	
	public GameObject  	initial_hex;
	public GameObject  	border_hex;
	
		
	public enum 		Hex    {Grass, Desert, Forest, Farmland, Marsh, Snow, Mountain, Hills, Water, EditorTileA};
	 
	public GameObject  	grass_hex;
	public GameObject  	desert_hex;
	public GameObject  	forest_hex;
	public GameObject  	farmland_hex;
	public GameObject  	marsh_hex; 
	public GameObject  	mountain_hex;
	public GameObject  	hills_hex;
	public GameObject  	water_hex; 
	
	public static Dictionary<Hex, GameObject>  hex_dict    = new Dictionary<Hex, GameObject>();
	
	
	// Use this for initialization
	void Start () {
		
		CreateHex(true, 1, initial_hex, new Vector3(0, 0, 0), initial_hex.GetComponent<editorHexS>().hex_type, 0, 0);
		
		hex_dict.Add(Hex.Grass, grass_hex);
		hex_dict.Add(Hex.Desert, desert_hex);
		hex_dict.Add(Hex.Forest, forest_hex);
		hex_dict.Add(Hex.Farmland, farmland_hex);
		hex_dict.Add(Hex.Marsh, marsh_hex); 
		hex_dict.Add(Hex.Mountain, mountain_hex);
		hex_dict.Add(Hex.Hills, hills_hex);
		hex_dict.Add(Hex.Water, water_hex);
		hex_dict.Add(Hex.EditorTileA, border_hex);
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void playSelectTile()
	{
		audio.Play();
	}
//	
//	public void CloneHex(GameObject hex_type, editorHexManagerS.Tiles data_type, float x_dist, float z_dist, int x_coord_in, int z_coord_in)
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
//	public void BuildHex(GameObject go, editorHexManagerS.Tiles data_type,  Vector3 pos, int x_coord_in, int z_coord_in)
//	{
//		print ("enter BuildHex");
//		print ("x: " + x_coord_in + " z: " + z_coord_in);
//		
//		
//			print ("attempting to replace a hex now...");
//			GameObject new_hex = (GameObject) Instantiate(go, pos, Quaternion.identity);
//			editorHexS new_hex_script = new_hex.GetComponent<editorHexS>();
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
		if(hex_db.ContainsKey(x))
		{
			if(hex_db[x].ContainsKey(z))
			{
				return true;
			}
		}
		return false;
	}
	
	private GameObject InstantiateHex(GameObject hex_type, Vector3 pos, editorHexManagerS.Hex data_type, int x, int z)
	{
		
		GameObject new_hex = (GameObject) Instantiate(hex_type, pos, Quaternion.identity);
		editorHexS new_hex_script = new_hex.GetComponent<editorHexS>();
		
//		new_hex_script.tile_num = ++editorUserS.tile_counter;
		new_hex_script.x_coord = x;
		new_hex_script.z_coord = z;
		new_hex_script.name = "hex("+new_hex_script.x_coord +"," + new_hex_script.z_coord+")";
		new_hex_script.hex_type = data_type;
		
		return new_hex;
	}
	
	public GameObject CreateHex(bool overwrite, int depth, GameObject hex_type, Vector3 pos, editorHexManagerS.Hex data_type, int x, int z)
	{
		depth--;
		
		if(data_type != Hex.EditorTileA)
		{
			editorUserS.last_created_hex_type = data_type; 
		}
		
		GameObject created_hex;
		editorHexS created_hex_script;
		print ("creating : " + x + ", " + z);
		if(hex_db.ContainsKey(x))
		{ 
			if(hex_db[x].ContainsKey(z))
			{
				if((overwrite && hex_db[x][z].getDataType() != data_type) || (hex_db[x][z].getDataType() == Hex.EditorTileA && data_type != Hex.EditorTileA))  //hex_db[x][z].getDataType() == Hex.EditorTileA && data_type != Hex.EditorTileA
				{
					//if there is already an entry there and we want to overwrite, overwrite it
					print ("if _ if _ if _ replacing : " + x + ", " + z + "with a " + data_type);
					
					if(hex_db[x][z].getOccupier() != editorHexManagerS.hex_dict[editorUserS.last_created_hex_type])
					{
						created_hex = InstantiateHex(hex_type, pos, data_type, x, z);
						created_hex_script = created_hex.GetComponent<editorHexS>();
						
						//now delete what is already there
						Destroy(hex_db[x][z].getOccupier());
						hex_db[x][z]= new HexData(created_hex_script.name, created_hex, data_type, x, z);
						
						if(data_type != Hex.EditorTileA && depth == 0)
						{
							SurroundHex(border_hex,  created_hex_script.transform.position, Hex.EditorTileA, 0, x, z);
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
				created_hex_script = created_hex.GetComponent<editorHexS>();
				
				hex_db[x].Add(z, new HexData(created_hex_script.name, created_hex, data_type, x, z));
//				if(data_type != Hex.EditorTileA)
//				{
//					created_hex_script.SurroundHex(border_hex, Hex.EditorTileA, depth);
//				}
				
				if(data_type != Hex.EditorTileA && depth == 0)
				{
					SurroundHex(border_hex, created_hex_script.transform.position, Hex.EditorTileA, 0, x, z);
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
			created_hex_script = created_hex.GetComponent<editorHexS>();
			
			hex_db.Add(x, new Dictionary<int, HexData>());
			hex_db[x].Add(z, new HexData(created_hex_script.name, created_hex, data_type,x, z));
//			if(data_type != Hex.EditorTileA)
//			{
//				created_hex_script.SurroundHex(border_hex, Hex.EditorTileA);
//			}
			
			if(data_type != Hex.EditorTileA && depth == 0)
			{
				SurroundHex(border_hex, created_hex_script.transform.position,  Hex.EditorTileA, 0, x, z);
			}
			else
			if(depth > 0)
			{
				SurroundHex(created_hex, created_hex_script.transform.position, data_type, depth, x, z);
			}
			return created_hex;
		}
	}
	

	
	public void SurroundHex(GameObject hex_type, Vector3 center_pos, editorHexManagerS.Hex data_type, int depth, int x_coord, int z_coord)
	{
		bool overwrite_enabled;
		if(data_type != editorHexManagerS.Hex.EditorTileA && editorUserS.overwrite_mode)
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
		 
		editorUserS.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans), data_type, x_coord, z_coord + 1);
		
		//CloneNorthEast
		x_trans = 2.30024F 	  + center_pos.x;
		z_trans = 1.3280592F  + center_pos.z;
		editorUserS.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans), data_type, x_coord + 1, z_coord);
		
		//CloneSouthEast
		x_trans = 3.14219F    + center_pos.x;
		z_trans = -0.486092F  + center_pos.z; 
		editorUserS.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans), data_type, x_coord + 1, z_coord - 1);
		
		//CloneSouth
		x_trans =  0.841947F + center_pos.x;
		z_trans = -1.81415F  + center_pos.z;
		editorUserS.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans),  data_type, x_coord, z_coord - 1);
		
		// CloneSouthWest
		x_trans = -2.30024F    + center_pos.x;
		z_trans = -1.3280592F  + center_pos.z;
		editorUserS.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans), data_type, x_coord - 1, z_coord);
		
		//CloneNorthWest
		x_trans = -3.14219F  + center_pos.x;
		z_trans = 0.486092F  + center_pos.z;
		editorUserS.tms.CreateHex(overwrite_enabled,  depth, hex_type, new Vector3(x_trans, 0, z_trans),  data_type, x_coord - 1, z_coord + 1);
		
		
	}
	
	public class HexData {
		
		public string tile_name;
		public int x_coord;
		public int z_coord;
		public editorHexManagerS.Hex data_type;
		public string hex_type;
		public string[] contents;
		public GameObject occupier;
		
		public HexData(string _tile_name,
						GameObject _occupier,
						editorHexManagerS.Hex    _data_type,
						int    _x_coord,
						int    _z_coord)
		{
			x_coord   = _x_coord;
			z_coord   = _z_coord;
			tile_name = _tile_name;
			occupier  = _occupier;
			data_type = _data_type;
		} 
		
		public editorHexManagerS.Hex getDataType()
		{
			return data_type;
		}
		public GameObject getOccupier()
		{
			return occupier;
		}
		
	}	
}
