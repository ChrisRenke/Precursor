using UnityEngine;
using System.Collections;
using System.Collections.Generic; 


public class editorHexManagerS : MonoBehaviour {
	
	public static Dictionary<int, Dictionary<int, editorHexManagerS.HexData>> hex_db = new Dictionary<int, Dictionary<int, editorHexManagerS.HexData>>();
	
	public GameObject  	initial_hex;
	public GameObject  	border_hex;
	public bool 		debug_prints = true;
		
	public enum 		Hex    {Grass, Desert, Forest, Farmland, Marsh, Snow, Mountain, Hills, Water, EditorTileA};
	 
	public GameObject  	grass_hex;
	public GameObject  	desert_hex;
	public GameObject  	forest_hex;
	public GameObject  	farmland_hex;
	public GameObject  	marsh_hex; 
	public GameObject  	mountain_hex;
	public GameObject  	hills_hex;
	public GameObject  	water_hex;  
	
	public int		random_percentage_threshold = 30;
	
	public static Dictionary<Hex, GameObject>  hex_dict    = new Dictionary<Hex, GameObject>();
	
	public static Hex   clicked_hex_type;
	
	// Use this for initialization
	void Start () {
		
		
		hex_dict.Add(Hex.Grass, grass_hex);
		hex_dict.Add(Hex.Desert, desert_hex);
		hex_dict.Add(Hex.Forest, forest_hex);
		hex_dict.Add(Hex.Farmland, farmland_hex);
		hex_dict.Add(Hex.Marsh, marsh_hex); 
		hex_dict.Add(Hex.Mountain, mountain_hex);
		hex_dict.Add(Hex.Hills, hills_hex);
		hex_dict.Add(Hex.Water, water_hex);
		hex_dict.Add(Hex.EditorTileA, border_hex);
		
		BrushHex(true, Hex.EditorTileA, 1, new Vector3(0, 0, 0), initial_hex.GetComponent<editorHexS>().hex_type, 0, 0); 
		
//		BrushHex(true, Hex.EditorTileA, 1, editorUserS.CoordsGameTo3D(2, 2), initial_hex.GetComponent<editorHexS>().hex_type, 2,2); 
//		BrushHex(true, Hex.EditorTileA, 1, editorUserS.CoordsGameTo3D(-1, 2), initial_hex.GetComponent<editorHexS>().hex_type, -1, 2); 
//		BrushHex(true, Hex.EditorTileA, 1, editorUserS.CoordsGameTo3D(3, -1), initial_hex.GetComponent<editorHexS>().hex_type, 3, -1); 
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void playSelectTile()
	{
		audio.Play();
	}
	
	
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
	
	private GameObject InstantiateHex(Vector3 pos, Hex hex_type, int x, int z)
	{ 
		GameObject new_hex = (GameObject) Instantiate(hex_dict[hex_type], pos, Quaternion.identity);
		editorHexS new_hex_script = new_hex.GetComponent<editorHexS>();
		 
		new_hex_script.x_coord = x;
		new_hex_script.z_coord = z;
		new_hex_script.name = "hex("+new_hex_script.x_coord +"," + new_hex_script.z_coord+")";
		new_hex_script.hex_type = hex_type;
		
		return new_hex;
	}
	 
	
	public void debug(string str)
	{
		if(debug_prints)
		{
			print (str);
		}
	}
	
	
	public GameObject BrushHex(bool overwrite, Hex clicked_hex_type, int brush_size, Vector3 pos, Hex draw_hex_type, int x, int z)
	{
		GameObject current_hex; 
		bool border_mode = false;
		GameObject origin_hex;
		debug("BrushHex called");
		
		//draw center hex, the one clicked on
		current_hex = AddHex(overwrite, clicked_hex_type, brush_size, pos, draw_hex_type, x, z);
		origin_hex = current_hex;
		//enter loop for surrounding hexes
		for(int ring = 1; ring < brush_size + 1; ring++)
		{
			
			debug("entering ring: ring level " + ring);
			//if we're in the extra iteration of the loop (hence the +1 to brush size
			if(ring == brush_size)
			{
				
				debug("setting ring to border mode");
				
				//set border_mode to true
				border_mode = true;
				
				//set it to draw BorderTileAs as outerring
				draw_hex_type = Hex.EditorTileA;
				
				//turn off override so that it doesn't destroy existing tiles to place the BorderTileAs
				overwrite = false;
			}
				
			//draw the first "northeast" edge hex
			debug("    drawing a northeast edge before initial from");
			current_hex = AddHexNE(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex));
			 
			//draw the "northeast" portion
			for(int edge_hexes_drawn = 1; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				debug("    drawing a northeast edge.");
				current_hex = AddHexSE(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex)); 
			}
			
			//draw the "southeast" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				debug("    drawing a southeast edge.");
				current_hex = AddHexS(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex)); 
			}
			
			//draw the "south" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				debug("    drawing a south edge.");
				current_hex = AddHexSW(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex)); 
			}
			
			//draw the "southwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				debug("    drawing a southwest edge.");
				current_hex = AddHexNW(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex)); 
			}
			
			//draw the "northwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				debug("    drawing a northwest edge.");
				current_hex = AddHexN(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex)); 
			}
			
			//draw the "north" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				debug("    drawing a north edge.");
				current_hex = AddHexNE(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex)); 
			}
		}
		return origin_hex;
	}
		
	
	
	public int xcrd(GameObject hex)
	{
		return hex.GetComponent<editorHexS>().x_coord;
	}
	
	public int zcrd(GameObject hex)
	{
		return hex.GetComponent<editorHexS>().z_coord;
	}
	
	public GameObject AddHexSE(bool overwrite, bool border_mode, Hex clicked_hex_type, int brush_size, Vector3 center_pos, Hex draw_hex_type, int x, int z)
	{
		float x_trans = 3.14219F    + center_pos.x;
		float z_trans = -0.486092F  + center_pos.z; 
		return AddHex(overwrite, border_mode, clicked_hex_type, brush_size, new Vector3(x_trans, 0, z_trans), draw_hex_type, x + 1, z - 1 );
		
	}
		
	public GameObject AddHexS(bool overwrite, bool border_mode, Hex clicked_hex_type, int brush_size, Vector3 center_pos, Hex draw_hex_type, int x, int z)
	{
		float x_trans =  0.841947F + center_pos.x;
		float z_trans = -1.81415F  + center_pos.z;
		return AddHex(overwrite, border_mode, clicked_hex_type, brush_size, new Vector3(x_trans, 0, z_trans), draw_hex_type, x , z - 1 );
		
	}
		
	public GameObject AddHexSW(bool overwrite, bool border_mode, Hex clicked_hex_type, int brush_size, Vector3 center_pos, Hex draw_hex_type, int x, int z)
	{
		float x_trans = -2.30024F    + center_pos.x;
		float z_trans = -1.3280592F  + center_pos.z;
		return AddHex(overwrite, border_mode, clicked_hex_type, brush_size, new Vector3(x_trans, 0, z_trans), draw_hex_type, x - 1, z); 
	}
		
	public GameObject AddHexNE(bool overwrite, bool border_mode, Hex clicked_hex_type, int brush_size, Vector3 center_pos, Hex draw_hex_type, int x, int z)
	{
		float x_trans = 2.30024F 	  + center_pos.x;
		float z_trans = 1.3280592F  + center_pos.z;
		return AddHex(overwrite, border_mode, clicked_hex_type, brush_size, new Vector3(x_trans, 0, z_trans), draw_hex_type, x+ 1, z );
		
	}
		
	public GameObject AddHexNW(bool overwrite, bool border_mode, Hex clicked_hex_type, int brush_size, Vector3 center_pos, Hex draw_hex_type, int x, int z)
	{
		float x_trans = -3.14219F  + center_pos.x;
		float z_trans = 0.486092F  + center_pos.z;
		return AddHex(overwrite, border_mode, clicked_hex_type, brush_size, new Vector3(x_trans, 0, z_trans), draw_hex_type, x - 1, z + 1);
		
	}
	
	public GameObject AddHexN(bool overwrite, bool border_mode, Hex clicked_hex_type, int brush_size, Vector3 center_pos, Hex draw_hex_type, int x, int z)
	{
		float x_trans = -0.841947F + center_pos.x;
		float z_trans =  1.81415F  + center_pos.z;
		return AddHex(overwrite, border_mode, clicked_hex_type, brush_size, new Vector3(x_trans, 0, z_trans), draw_hex_type, x, z + 1);
	}
			
	
	public GameObject LoadHex(Hex draw_hex_type, int x, int z)
	{  
			return AddHex(true, false, Hex.EditorTileA, 1, editorUserS.CoordsGameTo3D(x,z), draw_hex_type, x, z);
	}	
	
	/*
	 * Attempts to add a hex at the indicated X,Z location.
	 * 		if there is already a hex there that is of the same type as what is being drawn, method returns a reference to existing hext
	 * 		if not, it creates (and if necessecary deletes the existing one at that location) a new hex and returns it
 	 *
	 */
	public GameObject AddHex(bool overwrite, Hex clicked_hex_type, int brush_size, Vector3 pos, Hex draw_hex_type, int x, int z)
	{  
			return AddHex(overwrite, false, clicked_hex_type, brush_size, pos, draw_hex_type, x, z);
	}	
	
	public GameObject AddHex(bool overwrite, bool border_mode, Hex clicked_hex_type, int brush_size, Vector3 pos, Hex draw_hex_type, int x, int z)
	{  
		
		GameObject created_hex;
		editorHexS created_hex_script;
		
		//
		// THIS IS THE CASE WHERE WE ARE CHANGING AN EXISTING TILE
		//
		if( //if we're putting a hex where there already is one
			(hex_db.ContainsKey(x) && hex_db[x].ContainsKey(z))  
			&& //and
			//either an EditorTileA OR its the type we clicked on OR overwrite is on
		  	(hex_db[x][z].hex_type == Hex.EditorTileA || hex_db[x][z].hex_type == clicked_hex_type || overwrite) 
			&& //and
			//the existing hex isn't the same as the type we're wanting to make
			(hex_db[x][z].hex_type != draw_hex_type)
			&& //and
			//we're not in border mode, as border mode will never replace existing tiles
			(!border_mode)
		)
		{
			//create and render the new hex
			created_hex = InstantiateHex(pos, draw_hex_type, x, z);
			created_hex_script = created_hex.GetComponent<editorHexS>();
			
			//now delete what is already there
			Destroy(hex_db[x][z].occupier);
			
			hex_db[x][z]= new HexData(created_hex_script.name, created_hex, draw_hex_type, x, z);
			
			debug("    created new hex at " + x + "," + z + "by altering existing");
			return created_hex;
		}
		
		//
		// THIS IS THE CASE WHERE THERE IS NO PREXISTING TILE AT THE COORDINATE
		// 
		if(//if there does not already exist a hex at this spot of any type
			!(hex_db.ContainsKey(x) && hex_db[x].ContainsKey(z))  	
		)
		{ 
			//create and render the new hex
			created_hex = InstantiateHex(pos, draw_hex_type, x, z);
			created_hex_script = created_hex.GetComponent<editorHexS>();
			
			HexData db_entry   = new HexData(created_hex_script.name, created_hex, draw_hex_type, x, z);
			
			//if the x coord does NOT already exists
			if(!hex_db.ContainsKey(x))
			{ 
				hex_db.Add(x, new Dictionary<int, HexData>());
			}
			
			//add the entry to the corresponding z dict 
			hex_db[x].Add(z, db_entry);
			
			debug("    created new hex at " + x + "," + z + " from nothing");
			return created_hex;
		}
		
			debug("    returned existing hex at " + x + "," + z);
		return hex_db[x][z].occupier;
	}
		
//		
//		
//		
//		
//		
//		
//		if(data_type != Hex.EditorTileA)
//		{
//			editorUserS.last_created_hex_type = data_type; 
//		}
//		
//		GameObject hex_type = hex_dict[data_type];
//		
//		
//		GameObject created_hex;
//		editorHexS created_hex_script;
//		print ("creating : " + x + ", " + z);
//		
//		
//		if(hex_db.ContainsKey(x))
//		{ 
//			if(hex_db[x].ContainsKey(z))
//			{
//				if((overwrite 
//					&&
//				 hex_db[x][z].getDataType() != data_type) 
//					|| 
//				(hex_db[x][z].getDataType() == Hex.EditorTileA 
//					&& 
//				data_type != Hex.EditorTileA))  //hex_db[x][z].getDataType() == Hex.EditorTileA && data_type != Hex.EditorTileA
//				{
//					//if there is already an entry there and we want to overwrite, overwrite it
//					print ("if _ if _ if _ replacing : " + x + ", " + z + "with a " + data_type);
//					
//					if(hex_db[x][z].getOccupier() != editorHexManagerS.hex_dict[editorUserS.last_created_hex_type])
//					{
//						created_hex = InstantiateHex(hex_type, pos, data_type, x, z);
//						created_hex_script = created_hex.GetComponent<editorHexS>();
//						
//						//now delete what is already there
//						Destroy(hex_db[x][z].getOccupier());
//						hex_db[x][z]= new HexData(created_hex_script.name, created_hex, data_type, x, z);
//						
//						if(data_type != Hex.EditorTileA && depth == 0)
//						{
//							SurroundHex(created_hex_script.transform.position, Hex.EditorTileA, 0, x, z);
//						}
//						else
//						if(depth > 0)
//						{
//							SurroundHex(created_hex_script.transform.position,data_type, depth, x, z);
//						}
//						return created_hex; 
//					}
//					else
//					{
//						print("same type already there bro");	
//					}
//					return null;
//					
//				}
//				else
//				{
//					//there is something there and we dont have overwrite permission, so do nothing 
//					print ("if _ if _ else _ Something already there, surroundin'. "); 
//					if(data_type != Hex.EditorTileA && depth == 0)
//					{
//						SurroundHex(pos, Hex.EditorTileA, 0, x, z);
//					}
//					else
//					if(depth > 0)
//					{
//						SurroundHex(pos,data_type, depth, x, z);
//					}
//					return null;
//				}
//			}
//			else
//			{
//				//we've just gotta make a new z entry since nothing has ever been made in this z spot
//				print ("if _ else _ occupying : " + x + ", " + z);
//				
//				
//				
//				created_hex = InstantiateHex(hex_type, pos, data_type, x, z);
//				created_hex_script = created_hex.GetComponent<editorHexS>();
//				
//				hex_db[x].Add(z, new HexData(created_hex_script.name, created_hex, data_type, x, z)); 
//				
//				if(data_type != Hex.EditorTileA && depth == 0)
//				{
//					SurroundHex(created_hex_script.transform.position, Hex.EditorTileA, 0, x, z);
//				}
//				else
//				if(depth > 0)
//				{
//					SurroundHex(created_hex_script.transform.position, data_type, depth, x, z);
//				}
//				return created_hex;
//			}
//		}
//		else
//		{
//			//nothing has ever been made in this x row, so make the z dict and the z entry
//			print ("else ________ occupying : " + x + ", " + z);
//			
//			
//			created_hex = InstantiateHex(hex_type, pos, data_type, x, z);
//			created_hex_script = created_hex.GetComponent<editorHexS>();
//			
//			hex_db.Add(x, new Dictionary<int, HexData>());
//			hex_db[x].Add(z, new HexData(created_hex_script.name, created_hex, data_type,x, z)); 
//			
//			if(data_type != Hex.EditorTileA && depth == 0)
//			{
//				print ("drawing border...");
//				SurroundHex(created_hex_script.transform.position,  Hex.EditorTileA, 0, x, z);
//			}
//			else
//			if(depth > 0)
//			{
//				print ("drawing another layer...");
//				SurroundHex(created_hex_script.transform.position, data_type, depth, x, z);
//			}
//			return created_hex;
//		}
//	}

//	
//	public void SurroundHex(Vector3 center_pos, editorHexManagerS.Hex hex_type, int depth, int x_coord, int z_coord)
//	{
//		bool overwrite_enabled;
//		
//		if(hex_type != editorHexManagerS.Hex.EditorTileA && editorUserS.overwrite_mode)
//		{
//			overwrite_enabled = true;
//		}
//		else
//		{
//			overwrite_enabled = false;
//		}
//		
//		//CloneNorth
//		float x_trans = -0.841947F + center_pos.x;
//		float z_trans =  1.81415F  + center_pos.z;
//		CreateHex(overwrite_enabled, depth, new Vector3(x_trans, 0, z_trans), hex_type, x_coord, z_coord + 1);
//		
//		//CloneNorthEast
//		x_trans = 2.30024F 	  + center_pos.x;
//		z_trans = 1.3280592F  + center_pos.z;
//		CreateHex(overwrite_enabled,  depth, new Vector3(x_trans, 0, z_trans), hex_type, x_coord + 1, z_coord);
//		
//		//CloneSouthEast
//		x_trans = 3.14219F    + center_pos.x;
//		z_trans = -0.486092F  + center_pos.z; 
//		CreateHex(overwrite_enabled,  depth, new Vector3(x_trans, 0, z_trans), hex_type, x_coord + 1, z_coord - 1);
//		
//		//CloneSouth
//		x_trans =  0.841947F + center_pos.x;
//		z_trans = -1.81415F  + center_pos.z;
//		CreateHex(overwrite_enabled,  depth, new Vector3(x_trans, 0, z_trans),  hex_type, x_coord, z_coord - 1);
//		
//		// CloneSouthWest
//		x_trans = -2.30024F    + center_pos.x;
//		z_trans = -1.3280592F  + center_pos.z;
//		CreateHex(overwrite_enabled,  depth, new Vector3(x_trans, 0, z_trans), hex_type, x_coord - 1, z_coord);
//		
//		//CloneNorthWest
//		x_trans = -3.14219F  + center_pos.x;
//		z_trans = 0.486092F  + center_pos.z;
//		CreateHex(overwrite_enabled,  depth, new Vector3(x_trans, 0, z_trans),  hex_type, x_coord - 1, z_coord + 1);
//		
//		
//	}
	
	public class HexData {
		
		public string tile_name;
		public int x_coord;
		public int z_coord;
		public editorHexManagerS.Hex hex_type;
		public string[] contents;
		public GameObject occupier;
		
		public HexData(string _tile_name,
						GameObject _occupier,
						editorHexManagerS.Hex    _hex_type,
						int    _x_coord,
						int    _z_coord)
		{
			x_coord   = _x_coord;
			z_coord   = _z_coord;
			tile_name = _tile_name;
			occupier  = _occupier;
			hex_type = _hex_type;
		} 
		
		public GameObject getOccupier()
		{
			return occupier;
		}
		
	}
	
	private int sprayNum()
	{
		return Random.Range(0, 100);
	}
	
	public void SprayHex(bool overwrite, Hex clicked_hex_type, int brush_size, Vector3 pos, Hex draw_hex_type, int x, int z)
	{
		GameObject current_hex; 
		bool border_mode = false;
		
		debug("BrushHex called");
		
		//draw center hex, the one clicked on
		
		if(sprayNum() < (editorUserS.spray_prob))
			current_hex = AddHex(overwrite, clicked_hex_type, brush_size, pos, draw_hex_type, x, z);
		else
			current_hex = hex_db[x][z].occupier;
		
		//enter loop for surrounding hexes
		for(int ring = 1; ring < brush_size; ring++)
		{ 
			//draw the first "northeast" edge hex
			debug("    drawing a northeast edge before initial from");
			
				x += 1;
				z += 0;
				if(sprayNum() < (editorUserS.spray_prob)) 
					current_hex = BrushHex(overwrite, clicked_hex_type, 1, editorUserS.CoordsGameTo3D(x, z), draw_hex_type, x, z);
				
			//draw the "northeast" portion
			for(int edge_hexes_drawn = 1; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{ 
				x += 1;
				z -= 1;
				debug("    drawing a northeast edge.");
				if(sprayNum() < (editorUserS.spray_prob)) 
					current_hex = BrushHex(overwrite, clicked_hex_type, 1, editorUserS.CoordsGameTo3D(x, z), draw_hex_type, x, z);
			}
			
			//draw the "southeast" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				x += 0;
				z -= 1;
				debug("    drawing a southeast edge.");
				if(sprayNum() < (editorUserS.spray_prob)) 
					current_hex = BrushHex(overwrite, clicked_hex_type, 1, editorUserS.CoordsGameTo3D(x, z), draw_hex_type, x, z);
			}
			
			//draw the "south" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				x -= 1;
				z += 0;
				debug("    drawing a south edge.");
				if(sprayNum() < (editorUserS.spray_prob)) 
					current_hex  = BrushHex(overwrite, clicked_hex_type, 1, editorUserS.CoordsGameTo3D(x, z), draw_hex_type, x, z);
			}
			
			//draw the "southwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				x -= 1;
				z += 1;
				debug("    drawing a southwest edge.");
				if(sprayNum() < (editorUserS.spray_prob)) 
					current_hex  = BrushHex(overwrite, clicked_hex_type, 1, editorUserS.CoordsGameTo3D(x, z), draw_hex_type, x, z);
			}
			
			//draw the "northwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				x -= 0;
				z += 1;
				debug("    drawing a northwest edge.");
				if(sprayNum() < (editorUserS.spray_prob)) 
					current_hex  = BrushHex(overwrite, clicked_hex_type, 1, editorUserS.CoordsGameTo3D(x, z), draw_hex_type, x, z);
			}
			
			//draw the "north" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				x += 1;
				z += 0;
				debug("    drawing a north edge.");
				if(sprayNum() < (editorUserS.spray_prob)) 
					current_hex  = BrushHex(overwrite, clicked_hex_type, 1, editorUserS.CoordsGameTo3D(x, z), draw_hex_type, x, z);
			}
		}
	}
}
