using UnityEngine;
using System.Collections;

public class playerScript : MonoBehaviour {
	
	public static string 					TAG     	= "hex_tile";
	public int 								maxZoom 	= 2;
	public int 								minZoom 	= 20;
	public GameObject 						border_tile;
	public string							tileManagerTag;
	
	public static GameObject 				selected_hex;
	public static bool       				selection_menu_displayed;
	public static 							tileManagerScript tms;
	public static bool 						overwrite_mode = false;
	public static GameObject      		  	last_created_hex_go;
	public GameObject 						circleMenuPrefab;
	public static tileManagerScript.Tiles   last_created_hex_type;
	public static int						brush_size = 1;
	public static int 						tile_counter = 0;
	
	public Texture  						dropper_icon;
	public Texture  						brush_icon;
	public Texture							eraser_icon;
	
	private GameObject 						terrain_menu;
	
	// Use this for initialization
	void Start () {
		tms = GameObject.FindGameObjectWithTag(tileManagerTag).GetComponent<tileManagerScript>();
	}
	
	// Update is called once per frame
	void Update() {
		
		GameObject 	clicked_game_object;
		hexScript 	hex_script;
		
		//ctrl + shift + left click
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
		{
			clicked_game_object = RaycastMouse(); 
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<hexScript>();
				playerScript.tms.CreateHex(true, 1,
											playerScript.tms.border_hex, clicked_game_object.transform.position,  
											tileManagerScript.Tiles.EditorTileA, hex_script.x_coord, hex_script.z_coord);
			}
		}
		else
		//shift + left click
		if(Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
		{
			clicked_game_object = RaycastMouse();
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<hexScript>();
				playerScript.tms.CreateHex(true, playerScript.brush_size, 
											playerScript.last_created_hex_go, clicked_game_object.transform.position,  
											playerScript.last_created_hex_type, hex_script.x_coord, hex_script.z_coord);		
			}
		}
		else
		//alt + left click
		if(Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0))
		{
			clicked_game_object 				= RaycastMouse();
			
			if(clicked_game_object != null)
			{
				hex_script          				= clicked_game_object.GetComponent<hexScript>();
				playerScript.last_created_hex_type 	= hex_script.tile_type;
				playerScript.last_created_hex_go   	= tileManagerScript.hex_dict[hex_script.tile_type];
			}
		}
		else
		//ctrl + left click
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
		{	
			clicked_game_object = RaycastMouse();
			 
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<hexScript>();
				playerScript.tms.CreateHex(true, 1,
											playerScript.tms.border_hex, clicked_game_object.transform.position,  
											tileManagerScript.Tiles.EditorTileA, hex_script.x_coord, hex_script.z_coord);
			}
		}
		else
		//just left click
		if(Input.GetMouseButtonDown(0))
		{
			clicked_game_object = RaycastMouse();
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<hexScript>();
				playerScript.tms.CreateHex(true, playerScript.brush_size, 
											playerScript.last_created_hex_go, clicked_game_object.transform.position,  
											playerScript.last_created_hex_type, hex_script.x_coord, hex_script.z_coord);		
			}
		}
		else
		//right click
		if(Input.GetMouseButton(1) && !selection_menu_displayed)
		{ 
//			playerScript.selected_hex = this.gameObject;
//			print (playerScript.selected_hex);
//			if(terrain_menu != null)
//			{
//				Destroy(terrain_menu);
//			}
//			if(!menu_item)
//			{
			Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15));
			terrain_menu = (GameObject) Instantiate(circleMenuPrefab, 
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
			clicked_game_object = RaycastMouse();
			
			if(clicked_game_object != null)
			{
				hex_script          = clicked_game_object.GetComponent<hexScript>();
				
				if(hex_script.menu_item)
				{ 
					clicked_game_object 				= RaycastMouse();
					hex_script          				= clicked_game_object.GetComponent<hexScript>();
					playerScript.last_created_hex_type 	= hex_script.tile_type;
					playerScript.last_created_hex_go   	= tileManagerScript.hex_dict[hex_script.tile_type];
				}
			}
			
			Destroy(terrain_menu);
			terrain_menu 		= null;
			print ("destroying menu");
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
	
	GameObject RaycastMouse()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
		if (Physics.Raycast(ray, out hit, 100))
		{
			if(hit.transform.tag == TAG)
			{
            	print("Hit something with proper tag = " + TAG);
				return hit.transform.gameObject;
//				hexScript hex_tile = hit.transform.gameObject.GetComponent<hexScript>();
//				if(!hex_tile.menu_item && hex_tile.tile_type != tileManagerScript.Tiles.EditorTileA)
//				{
//					audio.Play();
//					hex_tile.CloneRing(1);
//				}
			}
		} 
		print ("nothing");
		return null; 
	}
	
	void OnGUI()
	{
//        toggleTxt = GUI.Toggle(new Rect(10, 10, 100, 30), toggleTxt, "A Toggle text");
		
		overwrite_mode = GUI.Toggle(new Rect(30, 30, 200, 30), overwrite_mode, "Overwrite Mode");
		 
		if(GUI.Button(new Rect(Screen.width - 220, 30, 200, 30), "SAVE!"))
			tms.Save();
		
//	    var mousePos : Vector3 = Input.mousePosition;
//	    var pos : Rect = Rect(mousePos.x,Screen.height - mousePos.y,cursorImage.width,cursorImage.height);
		
		brush_size = (int)GUI.VerticalSlider(new Rect(32, 60, 30, 200), brush_size, 6.0F, 1.0F);
		GUI.Label(new Rect(30, 270, 200, 30),  "Brush Size: " + brush_size);
		
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
//					hexScript hex_tile = hex.GetComponent<hexScript>();
//					hex_tile.CloneNorth();

//-0.841947, 0, 1.81415