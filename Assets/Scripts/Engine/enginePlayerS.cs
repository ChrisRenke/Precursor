using UnityEngine;
using System.Collections;

public class enginePlayerS : MonoBehaviour {
	
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
	
	public static float  						camera_min_x_pos = 9999999999;
	public static float  						camera_max_x_pos = -999999999;
	public static float  						camera_min_z_pos = 9999999999;
	public static float  						camera_max_z_pos = -999999999;
	
	public GUIStyle getTooltip()
	{
		return tooltip;
	}
	
	// Use this for initialization
	void Start () { 
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
		else
		{
			//mouse elsewhere in screen
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
			
			Vector3 screenBottomLeft = maincam.camera.ScreenToWorldPoint(new Vector3(0,0,0));
			Vector3 screenTopRight = maincam.camera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0));
			
			Vector3 screenCenter = (screenBottomLeft + screenTopRight ) / 2;
			
			float h = Input.GetAxis("Mouse X");
			Vector3 trans_x = Vector3.right * h * -1 * hSensitivity; 
			float v  = Input.GetAxis("Mouse Y");
			Vector3 trans_y = Vector3.up * v * -1 * vSensitivity; 
			
			
			print (screenBottomLeft.x + "   |   "+ maincam.transform.position.x +"   |   "+ camera_max_x_pos);
			if(trans_x.x + screenTopRight.x > camera_max_x_pos)
				maincam.transform.position.Set(camera_max_x_pos, screenCenter.y, screenCenter.z);
			else if(trans_x.x + screenBottomLeft.x < camera_min_x_pos)
				maincam.transform.position.Set(camera_min_x_pos, screenCenter.y, screenCenter.z);
			else
				maincam.transform.Translate(trans_x);
			 
//			
//			if(trans_y.y + screenTopRight.y > camera_max_z_pos)
//				maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_max_z_pos);
////			else if(trans_y.y + screenBottomLeft.y < camera_min_z_pos)
////				maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_min_z_pos);
//			else
//				maincam.transform.Translate(trans_y);
//				
				
			if(trans_y.y + screenTopRight.z > camera_max_z_pos)
				maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_max_z_pos);
			else
			if(trans_y.y + screenBottomLeft.z < camera_min_z_pos)
				maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_min_z_pos);
			else
				maincam.transform.Translate(trans_y);
			  
			
//			
//			float h = Input.GetAxis("Mouse X");
//			
//			Vector3 trans = Vector3.right * h * -1 * hSensitivity; 
//			if(trans.x + maincam.transform.position.x > camera_max_x_pos)
//				maincam.transform.position.Set(camera_max_x_pos, maincam.transform.position.y, maincam.transform.position.z);
//			else if(trans.x + maincam.transform.position.x < camera_min_x_pos)
//				maincam.transform.position.Set(camera_min_x_pos, maincam.transform.position.y, maincam.transform.position.z);
//			else
//				maincam.transform.Translate(trans);
//			
//			float v  = Input.GetAxis("Mouse Y");
//			Vector3 trans_z = Vector3.up * v * -1 * vSensitivity; 
//			print (trans_z.y + maincam.transform.position.z +"   |   "+ maincam.transform.position.z +"   |   "+ camera_max_z_pos);
//			if(trans_z.y + maincam.transform.position.z > camera_max_z_pos)
//				maincam.transform.position.Set(maincam.transform.position.x, maincam.transform.position.y, camera_max_z_pos);
//			else
//			if(trans_z.y + maincam.transform.position.z < camera_min_z_pos)
//				maincam.transform.position.Set(maincam.transform.position.x, maincam.transform.position.y, camera_min_z_pos);
//			else
//				maincam.transform.Translate(trans_z);
			  
		}
    }
	
	
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
	 
	
	
}

//
//					string nm = hit.transform.name;
//					GameObject hex = GameObject.Find(nm);
//					editorHexS hex_tile = hex.GetComponent<editorHexS>();
//					hex_tile.CloneNorth();

//-0.841947, 0, 1.81415