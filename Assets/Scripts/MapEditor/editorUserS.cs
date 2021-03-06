using UnityEngine;
using System.Collections;

public class editorUserS : MonoBehaviour {
	
	public static string 						TAG     	= "hex_tile";
	public int 									maxZoom 	= 2;
	public int 									minZoom 	= 25;
	public GameObject 							border_tile;
	
	public string								HexManagerTag;
	public string								EntityManagerTag;
	public string								HexTag;
	public string								EntTag;
	
	public GUIStyle								tooltip;
	public static editorHexManagerS 			tms;
	public static editorEntityManagerS 			ems;
	
	public static GameObject 					selected_hex;
	public static bool       					selection_menu_displayed;
	public static bool 							overwrite_mode = false;
	public static int							brush_size = 1;
	public static int 							tile_counter = 0;
	
	public Texture  							dropper_icon;
	public Texture  							brush_icon;
	public Texture								eraser_icon;
	public Texture								spray_icon;
	
	private GameObject 							terrain_menu;
	
	public static bool 							entity_mode     = false;
	public string 								draw_mode_label = "Draw Hexes";
	public string 								over_mode_label = "Fill Empty";
	public string 								current_brush   = "";
	
	public GameObject 							entMenuPrefab;
	public GameObject 							hexMenuPrefab;
	 
	public static Hex         					last_created_hex_type; 
	public static EntityE  						last_created_entity_type;
	
	public int 									min_brush_size;
	public int 									max_brush_size;
	
	public static int 							spray_prob = 70;
	
	public float								vSensitivity = 1.0F; 
	public float 								hSensitivity = 1.0F;
	public float 								zoomSensitivity = 1.0F;
	
	public GUIStyle getTooltip()
	{
		return tooltip;
	}
	
	// Use this for initialization
	void Start () {
		tms = GameObject.FindGameObjectWithTag(HexManagerTag).GetComponent<editorHexManagerS>();
		ems = GameObject.FindGameObjectWithTag(EntityManagerTag).GetComponent<editorEntityManagerS>();
	}
	
	
	//converts engine coordinates into 3D space cordinates
	public static Vector3 CoordsGameTo3D(int x, int z)
	{  
		/*
		//north
		float x_trans = -0.841947F + center_pos.x;
		float z_trans =  1.81415F  + center_pos.z;
		
		//northeast
		float x_trans = 2.30024F 	  + center_pos.x;
		float z_trans = 1.3280592F  + center_pos.z;
		
		//southeast
		float x_trans = 3.14219F    + center_pos.x;
		float z_trans = -0.486092F  + center_pos.z; 
				
		//south
		float x_trans =  0.841947F + center_pos.x;
		float z_trans = -1.81415F  + center_pos.z;
		 
		//southwest
		float x_trans = -2.30024F    + center_pos.x;
		float z_trans = -1.3280592F  + center_pos.z;
		
		//northwest
		float x_trans = -3.14219F  + center_pos.x;
		float z_trans = 0.486092F  + center_pos.z;
		
		*/
		return new Vector3(x * 2.30024F + z * -0.841947F, 0, z * 1.81415F + x * 1.3280592F);
	}
		 
	 
	// Update is called once per frame
	void Update() {
		 
		if(( (Input.mousePosition.x > 30 &&  Input.mousePosition.x < 240 ) 
					&&
			 (Input.mousePosition.y < Screen.height - 30 &&  Input.mousePosition.y > Screen.height - 140 ))
			||
			((Input.mousePosition.x > Screen.width - 240 && Input.mousePosition.x < Screen.width - 30)
			&&
			 (Input.mousePosition.y < Screen.height - 30 &&  Input.mousePosition.y > Screen.height - 100 ))
			)
		{
			//do nothing if the mouse is over the gui element areas
		}
		else{
			if(!entity_mode)
			{
				TerrainUpdate();
			}
			else
			{
				EntityUpdate();
			}
		}
		
		
		GameObject maincam = GameObject.FindGameObjectWithTag("MainCamera");
		float w = Input.GetAxis("Mouse ScrollWheel");
		float zoom_adjust = w * zoomSensitivity;
		
		
		if(Input.GetKeyDown(KeyCode.Q))
		{
			overwrite_mode = !overwrite_mode;
		} 
		if(Input.GetKeyDown(KeyCode.E))
		{
			entity_mode = !entity_mode;
		} 
		if(Input.GetKeyDown(KeyCode.LeftBracket) || Input.GetKeyDown(KeyCode.Z))
		{
			if(brush_size > min_brush_size)
				brush_size--;
		} 
		if(Input.GetKeyDown(KeyCode.RightBracket) || Input.GetKeyDown(KeyCode.X)) 
		{
			if(brush_size < max_brush_size)
				brush_size++;
		} 
		
		if(Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Semicolon))
		{
			if(spray_prob > 0)
				spray_prob-= 5;
			
			if(spray_prob < 1)
				spray_prob = 1;
		} 
		if(Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.Quote)) 
		{
			if(spray_prob < 100)
				spray_prob +=5;
			
			if(spray_prob > 100)
				spray_prob = 100;
		} 
		
		if(Input.GetKey(KeyCode.W))
		{
			Vector3 deltaPos = transform.forward * Time.deltaTime * 20;
			maincam.transform.position += deltaPos;
		} 
		if(Input.GetKey(KeyCode.S))
		{
			Vector3 deltaPos = transform.forward * Time.deltaTime * -20;
			maincam.transform.position += deltaPos;
		} 
		if(Input.GetKey(KeyCode.D))
		{
			Vector3 deltaPos = transform.right * Time.deltaTime * 20;
			maincam.transform.position += deltaPos;
		} 
		if(Input.GetKey(KeyCode.A))
		{
			Vector3 deltaPos = transform.right * Time.deltaTime * -20;
			maincam.transform.position += deltaPos;
		} 
		
		
		
		//zoom out
		if(Input.GetKey(KeyCode.Minus))
		{ 
			zoom_adjust += .5F * zoomSensitivity;
		}
		
		//zoom in
		if(Input.GetKey(KeyCode.Equals))
		{
			zoom_adjust -= .5F * zoomSensitivity;	
		}
		
		if(minZoom > maincam.camera.orthographicSize - zoom_adjust)
		{
			if(maxZoom < maincam.camera.orthographicSize - zoom_adjust)
			{
				maincam.camera.orthographicSize -= zoom_adjust;
			}
			else
			{
				maincam.camera.orthographicSize = maxZoom;
			}
		}
		else
		{	
			maincam.camera.orthographicSize = minZoom;
		}
		
		if(Input.GetMouseButton(2))
		{
			float h = Input.GetAxis("Mouse X");
			maincam.transform.Translate(Vector3.right * h * -1 * hSensitivity);
			
			float v  = Input.GetAxis("Mouse Y");
			maincam.transform.Translate(Vector3.up * v * -1 * vSensitivity);
			  
		}
    }
	
	void TerrainUpdate()
	{
		GameObject 	clicked_game_object;
		editorHexS 	hex_script;
		
		//ctrl + shift + left click
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
		{
			clicked_game_object = RaycastMouse(HexTag); 
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<editorHexS>();
				
				editorUserS.tms.BrushHex(overwrite_mode, hex_script.hex_type, brush_size, clicked_game_object.transform.position,  
										Hex.Perimeter, hex_script.x_coord, hex_script.z_coord);	 
				
//				editorUserS.tms.CreateHex(true, 1,  clicked_game_object.transform.position,  
//											editorHexManagerS.Hex.Perimeter, hex_script.x_coord, hex_script.z_coord);
			}
		}
		else
		//shift + left click
		if(Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
		{
			clicked_game_object = RaycastMouse(HexTag);
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<editorHexS>();
				editorUserS.tms.BrushHex(overwrite_mode, hex_script.hex_type, brush_size, clicked_game_object.transform.position,  
										last_created_hex_type, hex_script.x_coord, hex_script.z_coord);		
			}
		}
		else
		//alt + left click
		if(Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0))
		{
			clicked_game_object 				= RaycastMouse(HexTag);
			
			if(clicked_game_object != null)
			{
				hex_script          				= clicked_game_object.GetComponent<editorHexS>();
				if(hex_script.hex_type != Hex.Perimeter)
					editorUserS.last_created_hex_type 	= hex_script.hex_type; 
			}
		}
		else
		//ctrl + left click
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
		{	
			clicked_game_object = RaycastMouse(HexTag);
			 
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<editorHexS>();
				
				editorUserS.tms.BrushHex(true, hex_script.hex_type, brush_size, clicked_game_object.transform.position,  
										Hex.Perimeter, hex_script.x_coord, hex_script.z_coord);	  
			}
		}
		else
		//LeftCommand + left click
		if((Input.GetKey(KeyCode.LeftCommand)|| Input.GetKey(KeyCode.BackQuote)) && Input.GetMouseButtonDown(0)) 
		{	
			clicked_game_object = RaycastMouse(HexTag);
			 
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<editorHexS>();
				
				editorUserS.tms.SprayHex(overwrite_mode, hex_script.hex_type, brush_size, clicked_game_object.transform.position,  
										last_created_hex_type, hex_script.x_coord, hex_script.z_coord);	  
			}
		}
		else
		//just left click
		if(Input.GetMouseButtonDown(0))
		{
			clicked_game_object = RaycastMouse(HexTag);
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<editorHexS>();
				print ("derp");
				print (hex_script.x_coord  + " ," +  hex_script.z_coord);
				print (editorUserS.brush_size  + " |" +  clicked_game_object.transform.position);
				bool bs = editorUserS.tms == null;
				print ("stuff! " + bs.ToString()); 
				editorUserS.tms.BrushHex(overwrite_mode, hex_script.hex_type, brush_size, clicked_game_object.transform.position,  
										last_created_hex_type, hex_script.x_coord, hex_script.z_coord);	 
			}
		}
		else
		//right click
		if(Input.GetMouseButton(1) && !selection_menu_displayed)
		{  
			Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
			p.y = 5;
			terrain_menu = (GameObject) Instantiate(hexMenuPrefab, p, Quaternion.identity);	 
			selection_menu_displayed = true;
			print ("displaying menu");
		}
		else
		if(!Input.GetMouseButton(1) && selection_menu_displayed)
		{
			selection_menu_displayed  = false;
			clicked_game_object = RaycastMouse(HexTag);
			
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<editorHexS>();
				
				if(hex_script.menu_item)
				{ 
					clicked_game_object 				= RaycastMouse(HexTag);
					hex_script          				= clicked_game_object.GetComponent<editorHexS>();
					editorUserS.last_created_hex_type 	= hex_script.hex_type; 
				}
			}
			
			Destroy(terrain_menu);
			terrain_menu 		= null;
			print ("destroying menu");
		}
		
	}
	
	void EntityUpdate()
	{
		
		GameObject 	    clicked_game_object;
		editorEntityS 	entity_s;
		
		//alt + left click
		if(Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0))
		{
			clicked_game_object 				= RaycastMouse(EntTag);
			
			if(clicked_game_object != null)
			{
				entity_s          				     = clicked_game_object.GetComponent<editorEntityS>();
				editorUserS.last_created_entity_type = entity_s.entity_type; 
			}
		}
		else
		//ctrl + left click
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
		{	
			clicked_game_object = RaycastMouse(EntTag);
			 
			if(clicked_game_object != null)
			{
				entity_s          = clicked_game_object.GetComponent<editorEntityS>();
				ems.deleteEntity(entity_s);
			}
		}
		else
		//just left click
		if(Input.GetMouseButtonDown(0))
		{
			clicked_game_object = RaycastMouse(EntTag);
			if(clicked_game_object == null)
			{ 
				clicked_game_object = RaycastMouse(HexTag);
				editorHexS hex_script          = clicked_game_object.GetComponent<editorHexS>();
				editorUserS.ems.AddEntity(
					new Vector3(clicked_game_object.transform.position.x, clicked_game_object.transform.position.y + 1F , clicked_game_object.transform.position.z + .5F), 
					last_created_entity_type, hex_script.x_coord, hex_script.z_coord);	
			}
		}
		else
		//right click
		if(Input.GetMouseButton(1) && !selection_menu_displayed)
		{  
			Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
			p.y = 5;
			terrain_menu = (GameObject) Instantiate(entMenuPrefab, 
				p,
				Quaternion.identity);	 
			selection_menu_displayed = true;
			print ("displaying menu");
		}
		else
		if(!Input.GetMouseButton(1) && selection_menu_displayed)
		{
			selection_menu_displayed  = false;
			clicked_game_object = RaycastMouse(EntTag);
			
			if(clicked_game_object != null)
			{
				entity_s          = clicked_game_object.GetComponent<editorEntityS>();
				
				if(entity_s.menu_item)
				{ 
					clicked_game_object 				 = RaycastMouse(EntTag);
					entity_s          				     = clicked_game_object.GetComponent<editorEntityS>();
					editorUserS.last_created_entity_type = entity_s.entity_type; 
				}
			}
			
			Destroy(terrain_menu);
			terrain_menu 		= null;
			print ("destroying menu");
		}
		
	}
		
//	GameObject RaycastAllMouse()
//	{
//		RaycastHit hit;
//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
//		RaycastHit[] results = Physics.RaycastAll(ray, out hit, 100);
//		for(int i = 0; i < results.Length; ++i)
//		{
//			if(results[i].transform.tag == TAG)
//			{
//            	print("Hit something with proper tag = " + TAG);
//				return hit.transform.gameObject;
//			}
//			
//		} 
//		print ("nothing");
//		return null; 
//	}
	
	
	GameObject RaycastMouse(string _tag)
	{ 
		print (_tag);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
		RaycastHit[] results = Physics.RaycastAll(ray, 30);
		for(int i = 0; i < results.Length; ++i)
		{
			print (results[i].transform.gameObject.GetType() + " _ " + results[i].transform.gameObject.tag);
			if(results[i].transform.tag == _tag)
			{
            	print("Hit something with proper tag = " + _tag);
				return results[i].transform.gameObject;
			}
			
		} 
		print ("Nothing clicked.");
		return null; 
	}
//	{
//		RaycastHit hit;
//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
//		if (Physics.Raycast(ray, out hit, 100))
//		{
//			if(hit.transform.tag == TAG)
//			{
//            	print("Hit something with proper tag = " + TAG);
//				return hit.transform.gameObject;
////				editorHexS hex_tile = hit.transform.gameObject.GetComponent<editorHexS>();
////				if(!hex_tile.menu_item && hex_tile.tile_type != tileManagerS.Tiles.Perimeter)
////				{
////					audio.Play();
////					hex_tile.CloneRing(1);
////				}
//			}
//		} 
//		print ("nothing");
//		return null; 
//	}
	
	void OnGUI()
	{
		draw_mode_label =  overwrite_mode ? "Overwrite" : "Fill";
		over_mode_label =  entity_mode ?  "Entity"    : "Terrain"; 
		current_brush   =  entity_mode ?  last_created_entity_type.ToString()    : last_created_hex_type.ToString(); 
		
		if(GUI.Button(new Rect( 30, 30, 100, 30), draw_mode_label))
		{
			overwrite_mode = !overwrite_mode;
		} 
	
		if(GUI.Button(new Rect( 140, 30, 100, 30), over_mode_label))
		{
			entity_mode  = !entity_mode;
		} 
		
		if(!entity_mode)
		{
			brush_size = (int)GUI.HorizontalSlider(new Rect(30, 70,  210, 30), brush_size, (float) min_brush_size, (float) max_brush_size);	
			GUI.Label(new Rect(250, 65, 70, 30),  "Size: " + brush_size);
			
			spray_prob = (int)GUI.HorizontalSlider(new Rect(30, 110,  210, 30), spray_prob, (float) 1, (float) 100);	
			GUI.Label(new Rect(250, 105, 110, 30),  "Spray: " + spray_prob + "%");
		}
		
		GUI.Label(new Rect(250, 35, 70, 30),  "" + current_brush);
		
		if(Input.GetKey(KeyCode.LeftAlt))
		{
			Screen.showCursor = false;
	    	GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - dropper_icon.height, dropper_icon.width, dropper_icon.height),dropper_icon);
		}
		else
		if(Input.GetKey(KeyCode.LeftControl) || (Input.GetKey(KeyCode.LeftControl)  && Input.GetKey(KeyCode.LeftShift) ))
		{
			Screen.showCursor = false;
	    	GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - eraser_icon.height, eraser_icon.width, eraser_icon.height), eraser_icon);
		}
		else
		if(Input.GetKey(KeyCode.LeftShift))
		{
			Screen.showCursor = false;
	    	GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - brush_icon.height, brush_icon.width, brush_icon.height),brush_icon);
		}
		else
		if(Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.BackQuote))
		{
			Screen.showCursor = false;
	    	GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - spray_icon.height + 20, spray_icon.width, spray_icon.height),spray_icon);
		}
		else
		{
			Screen.showCursor = false;
	    	GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - brush_icon.height, brush_icon.width, brush_icon.height),brush_icon);
		}
	}
	
	
}
//-0.841947, 0, 1.81415