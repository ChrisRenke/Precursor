using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enginePlayerS : MonoBehaviour {
	
	public int 									maxZoom 	= 2;
	public int 									minZoom 	= 25;
	 
	public static GUIStyle						hover_text;
	public GUIStyle								selection_hover;
	
	
	
	public GUIStyle								gui_norm_text; 
	public GUIStyle								gui_bold_text;
	public static GUIStyle								gui_norm_text_static; 
	public static GUIStyle								gui_bold_text_static;
	
	public float								vSensitivity = 1.0F; 
	public float 								hSensitivity = 1.0F;
	public float 								zoomSensitivity = 1.0F;
	
	public static float  						camera_min_x_pos = 9999999999;
	public static float  						camera_max_x_pos = -999999999;
	public static float  						camera_min_z_pos = 9999999999;
	public static float  						camera_max_z_pos = -999999999;
	
	private static GameObject maincam;
	private static entityMechS mech;
			
	public Texture part_piston;
	public Texture part_plate;
	public Texture part_strut;
	public Texture part_gear;
	
	public Texture icon_traverse;
	public Texture icon_repair;
	public Texture icon_scavenge;
	public Texture icon_attack;
	public Texture icon_upgrade;
	public Texture icon_end;
	
	public static Dictionary<Action, Texture> action_images;
	 
	
	// Use this for initialization
	void Awake () {  
		maincam 		= GameObject.FindGameObjectWithTag("MainCamera");
		gui_bold_text_static = gui_bold_text;
		gui_norm_text_static = gui_norm_text;
		hover_text 		= selection_hover;
		action_images = new Dictionary<Action, Texture>();
		action_images.Add(Action.Repair, icon_repair);
		action_images.Add(Action.Traverse, icon_traverse);
		action_images.Add(Action.Scavenge, icon_scavenge);
		action_images.Add(Action.Attack, icon_attack);
		action_images.Add(Action.UpgradeBase, icon_upgrade);
		action_images.Add(Action.UpgradeMech, icon_upgrade);
		action_images.Add(Action.End, icon_end);
		
	}
	
	public static void setMech()
	{
		mech    = GameObject.FindGameObjectWithTag("player_mech").GetComponent<entityMechS>();
	}
	
	// Update is called once per frame
	void Update() {
		Vector3 screenBottomLeft = maincam.camera.ScreenToWorldPoint(new Vector3(0,0,0));
		Vector3 screenTopRight = maincam.camera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0));
		
		Vector3 screenCenter = (screenBottomLeft + screenTopRight ) / 2;
		  
//		if(( (Input.mousePosition.x > 30 &&  Input.mousePosition.x < 240 ) 
//					&&
//			 (Input.mousePosition.y < Screen.height - 30 &&  Input.mousePosition.y > Screen.height - 140 ))
//			||
//			((Input.mousePosition.x > Screen.width - 240 && Input.mousePosition.x < Screen.width - 30)
//			&&
//			 (Input.mousePosition.y < Screen.height - 30 &&  Input.mousePosition.y > Screen.height - 100 ))
//			)
//		{
//			//do nothing if the mouse is over the gui element areas
//		}
//		else
//		{
//			//mouse elsewhere in screen
//		}
		
		float w = Input.GetAxis("Mouse ScrollWheel");
		float zoom_adjust = w * zoomSensitivity;
		
		
		
		
		//zoom out
		if(Input.GetKey(KeyCode.Equals))
		{ 
			zoom_adjust += .5F * zoomSensitivity; 
		}
		
		//zoom in
		if(Input.GetKey(KeyCode.Minus))
		{
			zoom_adjust -= .5F * zoomSensitivity;	 
		}
		if(zoom_adjust != 0)
		{
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
		}
		 

		
		if(Input.GetMouseButton(2) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
		{ 
			Vector3 trans_x = Vector3.back;
			Vector3 trans_y = Vector3.back;
			
			if(Input.GetMouseButton(2))
			{
				float h = Input.GetAxis("Mouse X");
				float v  = Input.GetAxis("Mouse Y");
				trans_y = Vector3.up * v * -1 * vSensitivity; 
				trans_x = Vector3.right * h * -1 * hSensitivity; 
			}
			
			if(Input.GetKey(KeyCode.UpArrow))
				print ("RIGHT ARROW!");
				
			if(Input.GetKey(KeyCode.D)  || Input.GetKey(KeyCode.RightArrow))
			{
				trans_x = Vector3.right * hSensitivity * 12F * Time.deltaTime; 
			} 
			if(Input.GetKey(KeyCode.A)  || Input.GetKey(KeyCode.LeftArrow))
			{
				trans_x = Vector3.right * hSensitivity * -12F * Time.deltaTime; 
			} 
			
			
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				trans_y = Vector3.up * vSensitivity * 12F * Time.deltaTime; 
			} 
			if(Input.GetKey(KeyCode.S)|| Input.GetKey(KeyCode.DownArrow))
			{
				trans_y = Vector3.up * vSensitivity * -12F * Time.deltaTime; 
			} 
			
			if(trans_x != Vector3.back)
			{
				if(trans_x.x + screenTopRight.x > camera_max_x_pos)
					maincam.transform.position.Set(camera_max_x_pos, screenCenter.y, screenCenter.z);
				else if(trans_x.x + screenBottomLeft.x < camera_min_x_pos)
					maincam.transform.position.Set(camera_min_x_pos, screenCenter.y, screenCenter.z);
				else
					maincam.transform.Translate(trans_x);
			}
				
			if(trans_y != Vector3.back)
			{
				if(trans_y.y + screenTopRight.z > camera_max_z_pos)
					maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_max_z_pos);
				else
				if(trans_y.y + screenBottomLeft.z < camera_min_z_pos)
					maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_min_z_pos);
				else
					maincam.transform.Translate(trans_y);
			}
		}
		
		
		if(true)
		{ 
			Vector3 adjustedScreenBottomLeft = maincam.camera.ScreenToWorldPoint(new Vector3(0,0,0));
			Vector3 adjustedScreenTopRight = maincam.camera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0));
			
			
			if(adjustedScreenTopRight.z > camera_max_z_pos)
			{
				print ("too far up"); 
				maincam.transform.position = new Vector3(maincam.transform.position.x, maincam.transform.position.y, maincam.transform.position.z -  (adjustedScreenTopRight.z - screenTopRight.z));
			}
			if(adjustedScreenBottomLeft.z <= camera_min_z_pos)
			{
				print ("too far down");
				maincam.transform.position = new Vector3(maincam.transform.position.x, maincam.transform.position.y, maincam.transform.position.z +  (adjustedScreenTopRight.z - screenTopRight.z));
			}
			
			if(adjustedScreenTopRight.x > camera_max_x_pos)
			{
				print ("too far right");
				maincam.transform.position = new Vector3(maincam.transform.position.x - (adjustedScreenTopRight.x - screenTopRight.x), maincam.transform.position.y, maincam.transform.position.z);
			}
			if(adjustedScreenBottomLeft.x <= camera_min_x_pos)
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
	 
	public int gui_element_size = 80;
	public int gui_text_element_size =  20;
	public int gui_spacing      = 10;
	void OnGUI()
	{
   		
        GUI.DrawTexture(new Rect(gui_spacing * 1 + 0 * gui_element_size, Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), part_gear, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(gui_spacing * 2 + 1 * gui_element_size, Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), part_piston, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(gui_spacing * 3 + 2 * gui_element_size, Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), part_plate, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(gui_spacing * 4 + 3 * gui_element_size, Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), part_strut, ScaleMode.ScaleToFit, true);
		
        GUI.Label(new Rect(gui_spacing * 1 + 0 * gui_element_size, Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_text_element_size), entityMechS.part_count[Part.Gear].ToString(),    gui_norm_text);
        GUI.Label(new Rect(gui_spacing * 2 + 1 * gui_element_size, Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_text_element_size), entityMechS.part_count[Part.Piston].ToString(), gui_norm_text);
        GUI.Label(new Rect(gui_spacing * 3 + 2 * gui_element_size, Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_text_element_size), entityMechS.part_count[Part.Plate].ToString(),  gui_norm_text);
        GUI.Label(new Rect(gui_spacing * 4 + 3 * gui_element_size, Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_text_element_size), entityMechS.part_count[Part.Strut].ToString(),  gui_norm_text);
    
		if(GUI.Button(new Rect(gui_spacing, gui_spacing, 180, 40), "Aquatic Fins 5gr | 2plt"))
		{ 
			if(
				(entityMechS.getPartCount(Part.Plate) >= 2) && 
				(entityMechS.getPartCount(Part.Gear) >= 5))
			{
					entityMechS.adjustPartCount(Part.Plate, -2);
					entityMechS.adjustPartCount(Part.Gear, -5);
				mech.upgrade_traverse_water = true;
					mech.destroySelectionHexes();
					mech.allowSelectionHexesDraw();
				} 
			else
					print ("no good!");
		}
		
		if(GUI.Button(new Rect(gui_spacing + 180 + gui_spacing , gui_spacing, 180, 40), "Climbing Hooks 5gr | 2pst"))
		{ 
			
				if(
					(entityMechS.getPartCount(Part.Piston) >= 2) && 
					(entityMechS.getPartCount(Part.Gear) >= 5))
				{
					entityMechS.adjustPartCount(Part.Piston, -2);
					entityMechS.adjustPartCount(Part.Gear, -5);
					mech.upgrade_traverse_mountain = true;
					mech.destroySelectionHexes();
					mech.allowSelectionHexesDraw();
				} 
			else
					print ("no good!");
		}
		
		if(GUI.Button(new Rect(gui_spacing + 180 + gui_spacing + 180 + gui_spacing, gui_spacing, 180, 40), "Leg Speed 3pst | 3str"))
		{  
				if(
					(entityMechS.getPartCount(Part.Piston) >= 3) && 
					(entityMechS.getPartCount(Part.Strut) >= 3))
				{
					entityMechS.adjustPartCount(Part.Piston, -3);
					entityMechS.adjustPartCount(Part.Strut, -3);
					mech.upgrade_traverse_cost = true;
					mech.destroySelectionHexes();
					mech.allowSelectionHexesDraw();
				} 
			else
					print ("no good!");
		}
		
		
		
		GUI.Label(new Rect(Screen.width - (1 * (gui_spacing + gui_element_size)), Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), "AP",  gui_norm_text);
        GUI.Label(new Rect(Screen.width - (2 * (gui_spacing + gui_element_size)), Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), "HP",  gui_norm_text);
		
		GUI.Label(new Rect(Screen.width - (1 * (gui_spacing + gui_element_size)), Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_element_size), mech.getCurrentAP() + "/" + mech.getMaxAP(),  gui_norm_text);
        GUI.Label(new Rect(Screen.width - (2 * (gui_spacing + gui_element_size)), Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_element_size), mech.getCurrentHP() + "/" + mech.getMaxHP(),  gui_norm_text);
    
	
	}
	
	
}

//
//					string nm = hit.transform.name;
//					GameObject hex = GameObject.Find(nm);
//					editorHexS hex_tile = hex.GetComponent<editorHexS>();
//					hex_tile.CloneNorth();

//-0.841947, 0, 1.81415