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
	
	private GameObject 							terrain_menu;
	
	public static bool 							entity_mode     = false;
	public string 								draw_mode_label = "Draw Hexes";
	public string 								over_mode_label = "Fill Empty";
	public string 								current_brush   = "";
	
	public GameObject 							entMenuPrefab;
	public GameObject 							hexMenuPrefab;
	 
	public static editorHexManagerS.Hex         last_created_hex_type; 
	public static editorEntityManagerS.Entity   last_created_entity_type;
	
	
	
	// Use this for initialization
	void Start () {
		tms = GameObject.FindGameObjectWithTag(HexManagerTag).GetComponent<editorHexManagerS>();
		ems = GameObject.FindGameObjectWithTag(EntityManagerTag).GetComponent<editorEntityManagerS>();
	}
	 
	// Update is called once per frame
	void Update() {
		 
		if(( (Input.mousePosition.x > 30 &&  Input.mousePosition.x < 240 ) 
					||
			(Input.mousePosition.x > Screen.width - 240 && Input.mousePosition.x < Screen.width - 30))
			&&
			 (Input.mousePosition.y < Screen.height - 30 &&  Input.mousePosition.y > Screen.height - 100 ))
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
		float zSensitivity = 1.0F;
		float zoom_adjust = w * zSensitivity;
		
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
		
		if(Input.GetKeyDown(KeyCode.Q))
		{
			overwrite_mode = !overwrite_mode;
		} 
		if(Input.GetKeyDown(KeyCode.W))
		{
			entity_mode = !entity_mode;
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
		
		if(Input.GetMouseButton(2))
		{
			float h = Input.GetAxis("Mouse X");
			float hSensitivity = -1.0F;
			maincam.transform.Translate(Vector3.right * h * hSensitivity);
			
			float v  = Input.GetAxis("Mouse Y");
			float vSensitivity = -1.0F; 
			maincam.transform.Translate(Vector3.up * v * vSensitivity);
			  
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
				editorUserS.tms.CreateHex(true, 1,  clicked_game_object.transform.position,  
											editorHexManagerS.Hex.EditorTileA, hex_script.x_coord, hex_script.z_coord);
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
				editorUserS.tms.CreateHex(true, editorUserS.brush_size,  clicked_game_object.transform.position,  
											editorUserS.last_created_hex_type, hex_script.x_coord, hex_script.z_coord);		
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
				editorUserS.tms.CreateHex(true, 1,  clicked_game_object.transform.position,  
											editorHexManagerS.Hex.EditorTileA, hex_script.x_coord, hex_script.z_coord);
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
				editorUserS.tms.CreateHex(true, editorUserS.brush_size,  clicked_game_object.transform.position,  
											editorUserS.last_created_hex_type, hex_script.x_coord, hex_script.z_coord);		
			}
		}
		else
		//right click
		if(Input.GetMouseButton(1) && !selection_menu_displayed)
		{ 
//			editorUserS.selected_hex = this.gameObject;
//			print (editorUserS.selected_hex);
//			if(terrain_menu != null)
//			{
//				Destroy(terrain_menu);
//			}
//			if(!menu_item)
//			{
			Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
			p.y = 5;
			terrain_menu = (GameObject) Instantiate(hexMenuPrefab, 
				p,
				Quaternion.identity);	//new Vector3(transform.position.x, transform.position.y + .5F, transform.position.z), 
//			}
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
		
//		//ctrl + shift + left click
//		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
//		{
//			clicked_game_object = RaycastMouse(EntTag); 
//			if(clicked_game_object != null)
//			{
//				entity_s          = clicked_game_object.GetComponent<editorEntityS>();
//				editorUserS.tms.CreateHex(true, 1,  clicked_game_object.transform.position,  
//											editorHexManagerS.Hex.EditorTileA, entity_s.x_coord, entity_s.z_coord);
//			}
//		}
//		else
//		//shift + left click
//		if(Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
//		{
//			clicked_game_object = RaycastMouse(EntTag);
//			if(clicked_game_object != null)
//			{
//				entity_s          = clicked_game_object.GetComponent<editorEntityS>();
//				editorUserS.tms.CreateHex(true, editorUserS.brush_size,  clicked_game_object.transform.position,  
//											editorUserS.last_created_hex_type, entity_s.x_coord, entity_s.z_coord);		
//			}
//		}
//		else
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
//				clicked_game_object = RaycastMouse(HexTag);
//				entity_s          = clicked_game_object.GetComponent<editorEntityS>();
//				editorUserS.tms.CreateHex(true, editorUserS.brush_size,  clicked_game_object.transform.position,  
//											editorUserS.last_created_hex_type, entity_s.x_coord, entity_s.z_coord);		
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
////				if(!hex_tile.menu_item && hex_tile.tile_type != tileManagerS.Tiles.EditorTileA)
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
//        toggleTxt = GUI.Toggle(new Rect(10, 10, 100, 30), toggleTxt, "A Toggle text");
		
//		overwrite_mode = GUI.Toggle(new Rect(30, 30, 200, 30), overwrite_mode, "Overwrite Mode");	 
//		entity_mode = GUI.Toggle(new Rect(30, Screen.height - 30, 200, 30), entity_mode, "Entiy Editing");
			
//	    var mousePos : Vector3 = Input.mousePosition;
//	    var pos : Rect = Rect(mousePos.x,Screen.height - mousePos.y,cursorImage.width,cursorImage.height);
//		 
		
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
		
		brush_size = (int)GUI.HorizontalSlider(new Rect(30, 70,  210, 30), brush_size, 1.0F, 6.0F);
		
		GUI.Label(new Rect(250, 65, 70, 30),  "" + brush_size);
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
		{
			Screen.showCursor = false;
	    	GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y - brush_icon.height, brush_icon.width, brush_icon.height),brush_icon);
		}
	}
	
	
}

//
//					string nm = hit.transform.name;
//					GameObject hex = GameObject.Find(nm);
//					editorHexS hex_tile = hex.GetComponent<editorHexS>();
//					hex_tile.CloneNorth();

//-0.841947, 0, 1.81415