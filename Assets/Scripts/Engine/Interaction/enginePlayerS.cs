using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enginePlayerS : MonoBehaviour {
	
	public int 									maxZoom 	= 2;
	public int 									minZoom 	= 25;
	 
	public static GUIStyle						hover_text;
	public static GUIStyle						hp_bar_for_enemy;
	public static GUIStyle						hp_bar_for_base;
	public GUIStyle								hp_bar;
	public GUIStyle								hp_bar_base;
	public static Texture								hp_backboard;
	public Texture								hp_backboard_test;	
	public GUIStyle								selection_hover;
	
	
	
	public GUIStyle								gui_norm_text; 
	public GUIStyle								gui_norm_text_black;
	public GUIStyle								gui_bold_text;
	
	public GUIStyle								gui_upgrade_button;
	
	public static GUIStyle						gui_norm_text_static; 
	public static GUIStyle						gui_norm_text_black_static; 
	public static GUIStyle						gui_bold_text_static;
	
	public float								vSensitivity = 1.0F; 
	public float 								hSensitivity = 1.0F;
	public float 								zoomSensitivity = 1.0F;
	
	public static float  						camera_min_x_pos = 9999999999;
	public static float  						camera_max_x_pos = -999999999;
	public static float  						camera_min_z_pos = 9999999999;
	public static float  						camera_max_z_pos = -999999999;
	
	private static GameObject maincam;
	private static entityMechS mech;
	 
	
	public GUISkin hud_style;
	 
	public Texture chris_hp_bg_in;
	public Texture chris_hp_in; 
	public static Texture chris_hp_bg;
	public static Texture chris_hp; 
	
	//gui part
	public Texture bar_part_piston_bg;
	public Texture bar_part_gear_bg;
	public Texture bar_part_plate_bg;
	public Texture bar_part_strut_bg;
	public Texture bar_part_scale; 
	
	//gui hp
	public Texture bar_hud_hd;
//	public Texture bar_hp_town_bg;
//	public Texture bar_hp_mech;
//	public Texture bar_hp_town;
		
	public Color easy;
	public Color medium;
	public Color hard;
	public Color disable;
	public Color idle;
	public Color attack;
	public Color scavenge;
	public Color select_;
	public Color glow;
	public Color upgrade;
	
	public static Color easy_color;
	public static Color medium_color;
	public static Color hard_color;
	public static Color disable_color;
	public static Color idle_color;
	public static Color attack_color;
	public static Color scavenge_color;
	public static Color select_color;
	public static Color glow_color;
	public static Color upgrade_color;
	 
	private float northwest_angle ;
	private float northeast_angle ;
	private float west_angle ;
	private float east_angle ;
	private float southeast_angle ;
	private float southwest_angle ;
	 
//	private static LineRenderer lr;
	
	// Use this for initialization
	void Awake () {  
		maincam 		= GameObject.FindGameObjectWithTag("MainCamera");
		gui_bold_text_static = gui_bold_text;
		gui_norm_text_static = gui_norm_text;
		gui_norm_text_black_static = gui_norm_text_black;
		hover_text 		= selection_hover;
		hp_bar_for_enemy = hp_bar;
		hp_bar_for_base = hp_bar_base;
		hp_backboard = hp_backboard_test; 
		
		
		chris_hp_bg = chris_hp_bg_in;
			chris_hp = chris_hp_in;
		
		easy_color = easy;
		medium_color = medium;
		hard_color = hard;  
		idle_color = idle;
		attack_color = attack;
		scavenge_color = scavenge;
		disable_color  = disable;
		glow_color = glow;
		select_color = select_;
		upgrade_color = upgrade;
		
		
		Vector2 northwest = new Vector2(   -1F,  .557F);
		Vector2 northeast = new Vector2( .464F,     1F);
		Vector2 west 	  = new Vector2(   -1F, -.155F);
		Vector2 east 	  = new Vector2(    1F,  .155F);
		Vector2 southeast = new Vector2(    1F, -.557F);
		Vector2 southwest = new Vector2(-.464F,    -1F);
		
		northwest_angle	 = Vector2.Angle(Vector2.up, northwest);
		northeast_angle	 = Vector2.Angle(Vector2.up, northeast);
		west_angle		 = Vector2.Angle(Vector2.up, west);
		east_angle		 = Vector2.Angle(Vector2.up, east);
		southeast_angle	 = Vector2.Angle(Vector2.up, southeast);
		southwest_angle	 = Vector2.Angle(Vector2.up, southwest); 
		
		print("northwest_angle " + northwest_angle); //60.9
		print("northeast_angle " + northeast_angle); //25
		print("west_angle " + west_angle);           //99
		print("east_angle " + east_angle);           //81  
		print("southeast_angle " + southeast_angle); //119
		print("southwest_angle " + southwest_angle); //155
	}
	 
	public static entityBaseS town;
	public static void setMech()
	{
		mech    = GameObject.FindGameObjectWithTag("player_mech").GetComponent<entityMechS>(); 
		hovering_hex = hexManagerS.getHex(mech.x,mech.z);
		hovering_hex.hex_script.ControllerSelect();
		Debug.LogWarning("LOG: CREATING CONTROLLERSELECT");
	}
	
	public static void setBase(){ 
		town    = entityManagerS.getBase();
	}
	
	private void inputMac360(){
			
	}
	private static HexData hovering_hex;
	private float  move_time = 0;
	private float  move_delay = .2F;
	public static bool drawn_path = false;
	
	// Update is called once per frame
	void Update() { 
		
		Vector3 screenBottomLeft = maincam.camera.ScreenToWorldPoint(new Vector3(0,0,0));
		Vector3 screenTopRight = maincam.camera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0));
		
		Vector3 screenCenter = (screenBottomLeft + screenTopRight ) / 2;
		   
		
		float w = Input.GetAxis("Mouse ScrollWheel");
		float zoom_adjust = w * zoomSensitivity;
		
		
		
//XBOX CONTROLLER STUFF||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
		
//			print("//////////////////////////////////////////////");
				
		float cam_lr_axis = Input.GetAxisRaw("controller-right-leftright");
		float cam_ud_axis = Input.GetAxisRaw("controller-right-updown");
//	
//		print("cam_lr_axis " + cam_lr_axis);
//		print("cam_ud_axis " + cam_ud_axis);
	
		Vector3 motion_r_lr = Vector3.right * hSensitivity * -12F * Time.deltaTime * -cam_lr_axis; 
		Vector3 motion_r_up = Vector3.up * hSensitivity * -12F * Time.deltaTime * cam_ud_axis;  
	
		if(motion_r_lr.x + screenTopRight.x > camera_max_x_pos)
			maincam.transform.position.Set(camera_max_x_pos, screenCenter.y, screenCenter.z);
		else if(motion_r_lr.x + screenBottomLeft.x < camera_min_x_pos)
			maincam.transform.position.Set(camera_min_x_pos, screenCenter.y, screenCenter.z);
		else
			maincam.transform.Translate(motion_r_lr);
 
		if(motion_r_up.y + screenTopRight.z > camera_max_z_pos)
			maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_max_z_pos);
		else
		if(motion_r_up.y + screenBottomLeft.z < camera_min_z_pos)
			maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_min_z_pos);
		else
			maincam.transform.Translate(motion_r_up);
		
		
		if(move_time + move_delay < Time.time)
		{
		  	float cursor_lr_axis = Input.GetAxisRaw("controller-left-leftright");
			float cursor_ud_axis = Input.GetAxisRaw("controller-left-updown"); 
		 	Vector2 move_direction = new Vector2(cursor_lr_axis, -cursor_ud_axis);
//		 	
			float move_angle = Vector2.Angle(Vector2.up, move_direction);
			print("move_angle " + move_angle);
		print("cursor_lr_axis " + cursor_lr_axis);
		print("cursor_ud_axis " + cursor_ud_axis);
			if(cursor_ud_axis >  .5F || cursor_lr_axis >  .5F
			|| cursor_ud_axis < -.5F || cursor_lr_axis < -.5F)
			{
					
				Facing direction_to_move = Facing.North;
				
				if(cursor_lr_axis < 0)
				{
					if(move_angle < northwest_angle)
						direction_to_move = Facing.North;
					else
					if(move_angle < west_angle)
						direction_to_move = Facing.NorthWest;
					else
					if(move_angle < southwest_angle)
						direction_to_move = Facing.SouthWest;
					else 
						direction_to_move = Facing.South; 
				}
				else
				{
					if(move_angle < northeast_angle)
						direction_to_move = Facing.North;
					else
					if(move_angle < east_angle)
						direction_to_move = Facing.NorthEast;
					else
					if(move_angle < southeast_angle)
						direction_to_move = Facing.SouthEast;
					else
						direction_to_move = Facing.South; 
				}
				
				
				
	//		print("northwest_angle " + northwest_angle); //60.9
	//		print("northeast_angle " + northeast_angle); //25
	//		print("west_angle " + west_angle);           //99
	//		print("east_angle " + east_angle);           //81  
	//		print("southeast_angle " + southeast_angle); //119
	//		print("southwest_angle " + southwest_angle); //155
				
				
				
				
				
				print ("DIRECTION LEFT STICK:   " + direction_to_move.ToString());
				HexData new_selection = hexManagerS.getHex(hovering_hex.x, hovering_hex.z, direction_to_move);
				
//				if(new_selection.x != hovering_hex.x && new_selection.z != hovering_hex.z)
//				{
					hovering_hex.hex_script.ControllerDeselect();
					
					new_selection.hex_script.ControllerSelect();
					
					hovering_hex = new_selection;
					move_time = Time.time; 
					print ("MOVING TO A NEW HEX @ " + new_selection.x + ", " + new_selection.z);
//				}
				
			}
			
		}
//NORMAL STUFF |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
		
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
		
		
		
		if(Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftControl))
		{
			gameManagerS.forcePlayerTurn();
		}
		if(Input.GetKeyDown(KeyCode.Space))
		{
			gameManagerS.endPlayerTurn();
		}
		
		
		Vector3 trans_x = Vector3.back;
		Vector3 trans_y = Vector3.back;
		
		 
		if(Input.mousePosition.x < 5)
		{
			trans_x = Vector3.right * hSensitivity * -12F * Time.deltaTime;  
		}
		
		if(Input.mousePosition.x > Screen.width - 5)
		{
			trans_x = Vector3.right * hSensitivity * 12F * Time.deltaTime;  
		}
	
		if(Input.mousePosition.y > Screen.height - 5)
		{
			trans_y = Vector3.up * vSensitivity * 12F * Time.deltaTime; 
		}  
		if(Input.mousePosition.y < 5)
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
		
		if(Input.GetMouseButton(2) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
		{  
			  trans_x = Vector3.back;
			  trans_y = Vector3.back;
			
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
	
	public Texture menu_upgrade_over;
	public Texture menu_upgrade_norm;
	public Texture menu_upgrade_down;
	public Texture menu_upgrade_owned;
	
	
	
	private static bool mech_menu_displayed = true;
	private static bool base_menu_displayed = true;
	 
	public static void enableMechUpgradeMenu(){ mech_menu_displayed = true;	}
	public static void enableBaseUpgradeMenu(){ base_menu_displayed = true; }
	public static void disableMechUpgradeMenu(){ mech_menu_displayed = false; }
	public static void disableBaseUpgradeMenu(){ base_menu_displayed = false; }
	  
	public int gui_element_size = 80;
	public int gui_text_element_size =  20;
	public int gui_spacing      = 10;
	
	void OnGUI()
	{		
		Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (hexManagerS.CoordsGameTo3D(display_text_at.x, display_text_at.z));  
		ShadowAndOutline.DrawOutline(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15, 200,30),
			display_text, enginePlayerS.hover_text,Color.black, Color.white, 2F); 
			
			 
		drawHUDPartBar(203, 109,  bar_part_plate_bg, Part.Plate); 
		drawHUDPartBar(203,  65,  bar_part_strut_bg, Part.Strut); 
		drawHUDPartBar( 29, 109,   bar_part_gear_bg, Part.Gear); 
		drawHUDPartBar( 29,  65, bar_part_piston_bg, Part.Piston); 
			
		drawHUDHPBar(311,  60, 276, town.getCurrentHP(), town.getMaxHP(), Color.red, Color.yellow, Color.gray);
		drawHUDHPBar(311, 104, 178, mech.getCurrentHP(), mech.getMaxHP(), Color.red, Color.yellow, Color.green);
		
		drawButtonBool(122, 105, 94, "Repair"); 
		drawButtonBool(408, Screen.height - 28, 104,"Round 30"); 
		drawButtonBool(976, Screen.height - 28, 104,"End Turn"); 
		
		drawHUDEnergyBar(417,29,446,mech.getCurrentAP(), mech.getMaxAP());
		
		if(mech_menu_displayed)
			drawUpgradeMenu(UpgradeMenu.Mech);
			
		if(base_menu_displayed)
			drawUpgradeMenu(UpgradeMenu.Base);
			
//	  	ShadowAndOutline.DrawShadow(new Rect(36, 8, 128, 24), new GUIContent(entityMechS.getPartCount(Part.Plate) + "/12"),  enginePlayerS.gui_norm_text_static,Color.white, new Color(0,0,0,.5F),Vector2.up);

	}

	
	private MechUpgradeMode mech_upgrade_tab = MechUpgradeMode.Movement;
	private BaseUpgradeMode base_upgrade_tab = BaseUpgradeMode.Perimeter;
	 
	private void drawUpgradeMenu(UpgradeMenu upgrade_menu_type){ 
		switch(upgrade_menu_type)
		{
			case UpgradeMenu.Base: drawBaseUpgradeMenu(); break;
			case UpgradeMenu.Mech: drawMechUpgradeMenu(); break;
		}
	}
	
	private void drawMechUpgradeMenu(){ 
		GUI.BeginGroup (new Rect (30, 28, 378, 576));   
		
			//background
			GUI.DrawTexture(new Rect (0,0, 378, 576), menu_background);	
			
				//menu title
				ShadowAndOutline.DrawOutline(new Rect(18,18, 342, 47), "Mech Upgrades", menu_heading, new Color(0,0,0,.5F),Color.white, 3F);
		
				//menu tabs
				GUI.BeginGroup (new Rect (18, 80, 342, 29));   
					drawMechMenuFilterButtons(0,0,"Movement", MechUpgradeMode.Movement );
					drawMechMenuFilterButtons(89,0,"Combat", MechUpgradeMode.Combat );   
					drawMechMenuFilterButtons(178,0,"Scavenge", MechUpgradeMode.Scavenge);   
					drawMechMenuFilterButtons(267,0,"Utility", MechUpgradeMode.Utility) ;
				GUI.EndGroup (); 
			
				//upgrade items
				GUI.BeginGroup (new Rect (18, 124, 342, 430)); 
				GUI.EndGroup (); 
				
		
		
		GUI.EndGroup (); 
	}
	
	
	
	
//	private void drawMenuUpgradeItem(int from_left, int from_top, )
	
	private void drawBaseUpgradeMenu(){ 
		GUI.BeginGroup (new Rect (Screen.width - 378 - 30, 28, 378, 576));   
			GUI.DrawTexture(new Rect (0,0, 378, 576), menu_background);	
			ShadowAndOutline.DrawOutline(new Rect(18,18, 342, 47), "Base Upgrades", menu_heading, new Color(0,0,0,.5F),Color.white, 3F);
			GUI.BeginGroup (new Rect (18, 80, 342, 29));   
				drawBaseMenuFilterButtons(0,0,"Perimeter", BaseUpgradeMode.Perimeter );
				drawBaseMenuFilterButtons(89,0,"Armament", BaseUpgradeMode.Armament );   
				drawBaseMenuFilterButtons(178,0,"Structure", BaseUpgradeMode.Structure);   
				drawBaseMenuFilterButtons(267,0,"Utility", BaseUpgradeMode.Utility) ;
			GUI.EndGroup (); 
		GUI.EndGroup (); 
	}
	
	
	private bool drawButtonBool(int from_right, int from_bottom, int width, string text)
	{  
		GUI.BeginGroup (new Rect (Screen.width - from_right, Screen.height - from_bottom, width, 35));  
			bool result = GUI.Button(new Rect (0,0, width, 35),"", HUD_button); 
			ShadowAndOutline.DrawOutline(new Rect(0, 0, width, 31), text, 
				 menu_filter_text, new Color(0,0,0,.5F),Color.white, 3F);
		GUI.EndGroup (); 
		return result;
	}
	  
	public GUIStyle HUD_button;
	public GUIStyle HUD_bar;
	public Texture  energy_bar;
	public Texture  menu_background;
	public GUIStyle menu_filter_button;
	public GUIStyle menu_filter_button_selected;
	public GUIStyle menu_filter_text;
	public GUIStyle menu_heading;
	public GUIStyle menu_item_heading;
	public GUIStyle menu_item_description;
	public GUIStyle menu_item_costs;
	
	
	private void drawHUDPartBar(int from_left, int from_bottom, Texture background, Part part_type)
	{  
		GUI.BeginGroup (new Rect (from_left, Screen.height - from_bottom, 168, 44));
			GUI.DrawTexture(new Rect (0,0, 168, 44), background);
			GUI.BeginGroup (new Rect (36, 8, (int)128*(float)entityMechS.getPartCount(part_type)/(float)12, 24));
				GUI.DrawTexture(new Rect (0,0, 128, 24), bar_part_scale);
			GUI.EndGroup (); 
			ShadowAndOutline.DrawOutline(new Rect(36, 8, 128, 24), entityMechS.getPartCount(part_type) + "/12", 
				enginePlayerS.gui_norm_text_static, new Color(0,0,0,.5F),Color.white, 3F);
		GUI.EndGroup (); 
	}
	 
		
	private void drawHUDHPBar(int from_right, int from_bottom, int bar_width, int current, int max, Color empty, Color med, Color full)
	{ 
		Color bar;
		Color prev; 
		
		float hp_percent = (float)current/(float)max;
		if(hp_percent >= .5)
			bar = Color.Lerp(med, full, (hp_percent-.5f)/.5f);
		else
			bar = Color.Lerp(empty, med, (hp_percent)/.5f);
		
		GUI.BeginGroup (new Rect (Screen.width - from_right, Screen.height - from_bottom, bar_width, 34));
			GUI.Box(new Rect (0,0, bar_width, 34),"", HUD_bar);
			GUI.BeginGroup (new Rect (3, 3, (int)((float)bar_width*hp_percent), 24));
				prev = GUI.color;
				GUI.color = bar;
				bar_width -= 6;
				GUI.DrawTexture(new Rect (0,0, bar_width, 24), bar_hud_hd);
				GUI.color = prev;
			GUI.EndGroup (); 
			ShadowAndOutline.DrawOutline(new Rect(3, 3, bar_width, 24),  current + "/" + max, 
				enginePlayerS.gui_norm_text_static, new Color(0, 0, 0, .5F),Color.white, 3F);
		GUI.EndGroup (); 
	}
	
	private void drawHUDEnergyBar(int from_left, int from_top, int bar_width, int current, int max)
	{   	 
		
		float hp_percent = (float)current/(float)max; 
		
		GUI.BeginGroup (new Rect (from_left, from_top, bar_width, 34));
			GUI.Box(new Rect (0,0, bar_width, 34),"", HUD_bar);
			GUI.BeginGroup (new Rect (3, 3, (int)((float)bar_width*hp_percent), 24)); 
				bar_width -= 6;
				GUI.DrawTexture(new Rect (0,0, bar_width, 24), energy_bar); 
			GUI.EndGroup (); 
			ShadowAndOutline.DrawOutline(new Rect(3, 3, bar_width, 24),  current + "/" + max + " AP", 
				enginePlayerS.gui_norm_text_static, new Color(0, 0, 0, .5F),Color.white, 3F);
		GUI.EndGroup (); 
	}
	
	private void drawMechMenuFilterButtons(int from_left, int from_top,  string text, MechUpgradeMode filter_control)
	{   		 
		GUI.BeginGroup (new Rect (from_left, from_top, 75, 29));  
			if(mech_upgrade_tab == filter_control) 
				GUI.Toggle(new Rect (0,0, 75, 29), mech_upgrade_tab == filter_control, "", menu_filter_button_selected);
			else
				if(GUI.Toggle(new Rect (0,0, 75, 29), mech_upgrade_tab == filter_control, "", menu_filter_button))
					mech_upgrade_tab = filter_control;
			ShadowAndOutline.DrawOutline(new Rect(0, 0, 75, 29), text, 
				menu_filter_text, new Color(0,0,0,.5F),Color.white, 3F);
		GUI.EndGroup ();   
	}
	
	private void drawBaseMenuFilterButtons(int from_left, int from_top,  string text, BaseUpgradeMode filter_control)
	{   		 
		GUI.BeginGroup (new Rect (from_left, from_top, 75, 29));  
			if(base_upgrade_tab == filter_control) 
				GUI.Toggle(new Rect (0,0, 75, 29), base_upgrade_tab == filter_control, "", menu_filter_button_selected);
			else
				if(GUI.Toggle(new Rect (0,0, 75, 29), base_upgrade_tab == filter_control, "", menu_filter_button))
					base_upgrade_tab = filter_control;
			ShadowAndOutline.DrawOutline(new Rect(0, 0, 75, 29), text, 
				menu_filter_text, new Color(0,0,0,.5F),Color.white, 3F);
		GUI.EndGroup ();   
	}
	
}
//			GUI.DrawTexture(new Rect (0,0, 168, 44), background);
//				GUITexture bar_texture = new GUITexture();
//				bar_texture.color = Color.Lerp(empty, full, hp_percent);
//				bar_texture.texture = bar_hp_town;

//
//					string nm = hit.transform.name;
//					GameObject hex = GameObject.Find(nm);
//					editorHexS hex_tile = hex.GetComponent<editorHexS>();
//					hex_tile.CloneNorth();

//-0.841947, 0, 1.81415
