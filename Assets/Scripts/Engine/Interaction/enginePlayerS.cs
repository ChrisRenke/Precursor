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
	
	public GUIStyle								gui_upgrade_button;
	
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
	private static entityBaseS the_base;
			
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
	
	
	
	
	public Color easy;
	public Color medium;
	public Color hard;
	public Color disable;
	public Color idle;
	public Color attack;
	public Color scavenge;
	public Color select_;
	public Color glow;
	
	public static Color easy_color;
	public static Color medium_color;
	public static Color hard_color;
	public static Color disable_color;
	public static Color idle_color;
	public static Color attack_color;
	public static Color scavenge_color;
	public static Color select_color;
	public static Color glow_color;
	
	public static Dictionary<Action, Texture> action_images;
	 
//	private static LineRenderer lr;
	
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
		
		easy_color = easy;
		medium_color = medium;
		hard_color = hard;  
		idle_color = idle;
		attack_color = attack;
		scavenge_color = scavenge;
		disable_color  = disable;
		glow_color = glow;
		select_color = select_;
	}
	
	public static void setMech()
	{
		mech    = GameObject.FindGameObjectWithTag("player_mech").GetComponent<entityMechS>();
		the_base    = entityManagerS.getBase();
	}
	
	
	public static bool drawn_path = false;
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
	
	public static void popFrontOfRoute()
	{
		if(current_path_display!=null)
			current_path_display.removeFrontNode();
	}	
	
	public static void setRoute(PathDisplay in_path, string _display_text, HexData _display_text_at)
	{
		display_text_at = _display_text_at;
		display_text = _display_text;
		
		if(current_path_display != null)
		{ 
			if(!current_path_display.Equals(in_path))
			{ 
				current_path_display.hidePath();
				current_path_display.destroySelf();
			}
		}
		current_path_display = in_path;
		
		if(current_path_display != null)
		{
			current_path_display.displayPath();
		}
	}
	
	public static string 	  display_text;
	public static HexData 	  display_text_at;
	public static PathDisplay current_path_display;
	
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
		
		Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (hexManagerS.CoordsGameTo3D(display_text_at.x, display_text_at.z)); 
		GUI.Label(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15, 200,30),
			display_text, 
			enginePlayerS.hover_text);
   		
        GUI.DrawTexture(new Rect(gui_spacing * 1 + 0 * gui_element_size, Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), part_gear, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(gui_spacing * 2 + 1 * gui_element_size, Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), part_piston, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(gui_spacing * 3 + 2 * gui_element_size, Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), part_plate, ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(gui_spacing * 4 + 3 * gui_element_size, Screen.height - (gui_spacing + gui_element_size), gui_element_size, gui_element_size), part_strut, ScaleMode.ScaleToFit, true);
		
        GUI.Label(new Rect(gui_spacing * 1 + 0 * gui_element_size, Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_text_element_size), entityMechS.part_count[Part.Gear].ToString(),  gui_norm_text);
        GUI.Label(new Rect(gui_spacing * 2 + 1 * gui_element_size, Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_text_element_size), entityMechS.part_count[Part.Piston].ToString(), gui_norm_text);
        GUI.Label(new Rect(gui_spacing * 3 + 2 * gui_element_size, Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_text_element_size), entityMechS.part_count[Part.Plate].ToString(),  gui_norm_text);
        GUI.Label(new Rect(gui_spacing * 4 + 3 * gui_element_size, Screen.height - (gui_spacing * 2 + gui_element_size + gui_text_element_size - 10), gui_element_size, gui_text_element_size), entityMechS.part_count[Part.Strut].ToString(),  gui_norm_text);
    
		if(GUI.Button(new Rect(gui_spacing, gui_spacing, 180, 40), "Aquatic Fins 5 gear | 2 plate"))
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
		
		if(GUI.Button(new Rect(gui_spacing + 180 + gui_spacing , gui_spacing, 180, 40), "Climbing Hooks 5 gear | 2 piston"))
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
		
		if(GUI.Button(new Rect(gui_spacing + 180 + gui_spacing + 180 + gui_spacing, gui_spacing, 180, 40), "Leg Speed 3 piston | 3 strut"))
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
		
		
		
		
		
		
		
		
		if(GUI.Button(new Rect(gui_spacing, gui_spacing + gui_spacing + 40, 180, 40), "Base Guns 4 pstn | 4 struts"))
		{ 
			if(
				(entityMechS.getPartCount(Part.Piston) >= 4) && 
				(entityMechS.getPartCount(Part.Strut) >= 4)  && (int)the_base.defense_level <= 3)
			{
					entityMechS.adjustPartCount(Part.Piston, -4);
					entityMechS.adjustPartCount(Part.Strut, -4);
				the_base.upgradeBase(BaseCategories.Defense, (BaseUpgrade) the_base.defense_level + 1);
//				mech.upgrade_traverse_water = tru?e;
//					mech.destroySelectionHexes();
//					mech.allowSelectionHexesDraw();
				} 
			else
					print ("no good!");
		}
		
		if(GUI.Button(new Rect(gui_spacing + 180 + gui_spacing , gui_spacing  + gui_spacing + 40, 180, 40), "Base Struct 5gears | 4plates"))
		{ 
			
				if(
					(entityMechS.getPartCount(Part.Plate) >= 4) && 
					(entityMechS.getPartCount(Part.Gear) >= 5)  && (int)the_base.structure_level <= 3)
				{
					entityMechS.adjustPartCount(Part.Plate, -4);
					entityMechS.adjustPartCount(Part.Gear, -5);
					the_base.upgradeBase(BaseCategories.Defense, (BaseUpgrade) the_base.structure_level + 1);
				} 
			else
					print ("no good!");
		}
		
		if(GUI.Button(new Rect(gui_spacing + 180 + gui_spacing + 180 + gui_spacing, gui_spacing  + gui_spacing + 40, 180, 40), "Base Walls 3 plate | 4struts "))
		{  
				if(
					(entityMechS.getPartCount(Part.Plate) >= 3) && 
					(entityMechS.getPartCount(Part.Strut) >= 4) && (int)the_base.wall_level <= 3)
				{
					entityMechS.adjustPartCount(Part.Plate, -3);
					entityMechS.adjustPartCount(Part.Strut, -4);
					the_base.upgradeBase(BaseCategories.Walls, (BaseUpgrade) the_base.wall_level + 1);
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