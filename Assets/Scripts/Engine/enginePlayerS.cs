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
	private static GameObject maincam;
	
	public GUIStyle getTooltip()
	{
		return tooltip;
	}
	
	// Use this for initialization
	void Start () { 
		
		maincam = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update() {
		
		bool zoom_adjusted = false;
		
		Vector3 screenBottomLeft = maincam.camera.ScreenToWorldPoint(new Vector3(0,0,0));
		Vector3 screenTopRight = maincam.camera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0));
		
		Vector3 screenCenter = (screenBottomLeft + screenTopRight ) / 2;
		
		
		
		
		 
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
		
		float w = Input.GetAxis("Mouse ScrollWheel");
		float zoom_adjust = w * zoomSensitivity;
		
		
		
		
		//zoom out
		if(Input.GetKey(KeyCode.Minus))
		{ 
			zoom_adjust += .5F * zoomSensitivity;
			zoom_adjusted = true;
		}
		
		//zoom in
		if(Input.GetKey(KeyCode.Equals))
		{
			zoom_adjust -= .5F * zoomSensitivity;	
			zoom_adjusted = true;
		}
		if(zoom_adjust != 0)
		{
			if(minZoom > maincam.camera.orthographicSize - zoom_adjust)
			{
				if(maxZoom < maincam.camera.orthographicSize - zoom_adjust)
				{
					maincam.camera.orthographicSize -= zoom_adjust;
					zoom_adjusted = true;
					
				}
				else
				{
					maincam.camera.orthographicSize = maxZoom;
					zoom_adjusted = true;
				}
			}
			else
			{	
				maincam.camera.orthographicSize = minZoom;
				zoom_adjusted = true;
			}
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
			Vector3 trans_x = Vector3.right * h * -1 * hSensitivity; 
			float v  = Input.GetAxis("Mouse Y");
			Vector3 trans_y = Vector3.up * v * -1 * vSensitivity; 
			
			
//			print (screenBottomLeft.x + "   |   "+ maincam.transform.position.x +"   |   "+ camera_max_x_pos);
			if(trans_x.x + screenTopRight.x > camera_max_x_pos)
				maincam.transform.position.Set(camera_max_x_pos, screenCenter.y, screenCenter.z);
			else if(trans_x.x + screenBottomLeft.x < camera_min_x_pos)
				maincam.transform.position.Set(camera_min_x_pos, screenCenter.y, screenCenter.z);
			else
				maincam.transform.Translate(trans_x);
			  
				
			if(trans_y.y + screenTopRight.z > camera_max_z_pos)
				maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_max_z_pos);
			else
			if(trans_y.y + screenBottomLeft.z < camera_min_z_pos)
				maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_min_z_pos);
			else
				maincam.transform.Translate(trans_y);
			  
		}
		
		
		if(zoom_adjusted)
		{ 
			Vector3 adjustedScreenBottomLeft = maincam.camera.ScreenToWorldPoint(new Vector3(0,0,0));
			Vector3 adjustedScreenTopRight = maincam.camera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0));
			
			Vector3 adjustedScreenCenter = (adjustedScreenBottomLeft + adjustedScreenTopRight ) / 2;
			
			if(adjustedScreenTopRight.z > camera_max_z_pos)
			{
				print ("too far up"); 
				maincam.transform.position = new Vector3(maincam.transform.position.x, maincam.transform.position.y, maincam.transform.position.z -  (adjustedScreenTopRight.z - screenTopRight.z));
			}
			if(adjustedScreenBottomLeft.z < camera_min_z_pos)
			{
				print ("too far down");
				maincam.transform.position = new Vector3(maincam.transform.position.x, maincam.transform.position.y, maincam.transform.position.z +  (adjustedScreenTopRight.z - screenTopRight.z));
			}
			
			if(adjustedScreenTopRight.x > camera_max_x_pos)
			{
				print ("too far right");
				maincam.transform.position = new Vector3(maincam.transform.position.x - (adjustedScreenTopRight.x - screenTopRight.x), maincam.transform.position.y, maincam.transform.position.z);
			}
			if(adjustedScreenBottomLeft.x < camera_min_x_pos)
			{
				print ("too far left");
				maincam.transform.position = new Vector3(maincam.transform.position.x + (adjustedScreenTopRight.x - screenTopRight.x), maincam.transform.position.y, maincam.transform.position.z);
			}
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