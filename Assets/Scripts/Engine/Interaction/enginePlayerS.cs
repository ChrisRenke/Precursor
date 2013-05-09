using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enginePlayerS : MonoBehaviour {
	
	
	public AudioClip  _sound_button;
	public AudioClip  _sound_negative;
	public AudioClip  _sound_open_menu;
	
	public static AudioClip  sound_button;
	public static AudioClip  sound_negative;
	public static AudioClip  sound_open_menu;
	
	
	public int 									maxZoom 	= 2;
	public int 									minZoom 	= 25;
	  
	public GUIStyle								hp_bar;
	public GUIStyle								hp_bar_base; 
	public Texture								hp_backboard_test;	
	public GUIStyle								selection_hover;
	
	
	
	public GUIStyle								gui_norm_text; 
	public GUIStyle								gui_norm_text_black;
	public GUIStyle								gui_bold_text;
	
	public GUIStyle								gui_upgrade_button;
	
	
	public float								vSensitivity = 1.0F; 
	public float 								hSensitivity = 1.0F;
	public float 								zoomSensitivity = 1.0F;
	
	public   float  						camera_min_x_pos = 9999999999;
	public   float  						camera_max_x_pos = -999999999;
	public   float  						camera_min_z_pos = 9999999999;
	public   float  						camera_max_z_pos = -999999999;
	
	private static GameObject maincam;
	private   entityMechS mech;
	 
	
	public GUISkin hud_style;
	 
	public Texture chris_hp_bg_in;
	public Texture chris_hp_in;  
	
	//gui part
	public Texture bar_part_piston_bg;
	public Texture bar_part_gear_bg;
	public Texture bar_part_plate_bg;
	public Texture bar_part_strut_bg;
	public Texture bar_part_scale; 
	
	//gui hp
	public Texture bar_hud_hd; 
	
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
	
	 
	private float northwest_angle ;
	private float northeast_angle ;
	private float west_angle ;
	private float east_angle ;
	private float southeast_angle ;
	private float southwest_angle ;
	
	 
	public gameManagerS  gm; 
	public hexManagerS hm; 
	public entityManagerS em; 

//	private static LineRenderer lr;
	void Start () {
		audio.volume = 1F;
		audio.priority = 128;
		audio.ignoreListenerVolume = true;
		audio.rolloffMode = UnityEngine.AudioRolloffMode.Linear; 
		gm = GameObject.Find("engineGameManager").GetComponent<gameManagerS>(); 
		em = GameObject.Find("engineEntityManager").GetComponent<entityManagerS>();
		
	}
	// Use this for initialization
	void Awake () {  
		maincam 		= GameObject.FindGameObjectWithTag("MainCamera");
		hm = GameObject.Find("engineHexManager").GetComponent<hexManagerS>();
		 
		  
		
//		easy_color = easy;
//		medium_color = medium;
//		hard_color = hard;  
//		idle_color = idle;
//		attack_color = attack;
//		scavenge_color = scavenge;
//		disable_color  = disable;
//		glow_color = glow;
//		select_color = select_;
//		upgrade_color = upgrade;
//		 
		sound_button    = _sound_button;
		sound_negative  = _sound_negative;
		sound_open_menu = _sound_open_menu;
		
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
		
//		print("northwest_angle " + northwest_angle); //60.9
//		print("northeast_angle " + northeast_angle); //25
//		print("west_angle " + west_angle);           //99
//		print("east_angle " + east_angle);           //81  
//		print("southeast_angle " + southeast_angle); //119
//		print("southwest_angle " + southwest_angle); //155
	}
	 
	public  entityBaseS town;
	public  void setMech()
	{
		mech    = em.getMech();//GameObject.FindGameObjectWithTag("player_mech").GetComponent<entityMechS>(); 
  		hovering_hex = hm.getHex(mech.x,mech.z);
		hovering_hex.hex_script.ControllerSelect();
		Debug.LogWarning("LOG: CREATING CONTROLLERSELECT");
	}
	
	public  void setBase(){ 
		town    = em.getBase();
	}
	
	private void inputMac360(){
			
	}
	private  HexData hovering_hex;
	private float  move_time = 0;
	private float  move_delay = .2F;
	public  bool drawn_path = false;
	
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

//			print("move_angle " + move_angle);
//		print("cursor_lr_axis " + cursor_lr_axis);
//		print("cursor_ud_axis " + cursor_ud_axis);0
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
				
				
				
				
				
//				print ("DIRECTION LEFT STICK:   " + direction_to_move.ToString());
				HexData new_selection = hm.getHex(hovering_hex.x, hovering_hex.z, direction_to_move);
				
//				if(new_selection.x != hovering_hex.x && new_selection.z != hovering_hex.z)
//				{
					hovering_hex.hex_script.ControllerDeselect();
					
					new_selection.hex_script.ControllerSelect();
					
					hovering_hex = new_selection;
					move_time = Time.time; 
//					print ("MOVING TO A NEW HEX @ " + new_selection.x + ", " + new_selection.z);
//				}
				
			}
			
		}
//NORMAL STUFF |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
		
		if(Input.GetKeyDown(KeyCode.Tab))
		{ 
//			GameObject maincam = GameObject.FindGameObjectWithTag("MainCamera");
			Vector3 current_camera_pos = maincam.transform.position;
			Vector3 mech_camera_pos = new Vector3(hm.CoordsGameTo3D(mech.x,mech.z).x, 60, hm.CoordsGameTo3D(mech.x,mech.z).z);
			Vector3 town_camera_pos = new Vector3(hm.CoordsGameTo3D(town.x,town.z).x, 60, hm.CoordsGameTo3D(town.x,town.z).z);
			
			if(current_camera_pos == mech_camera_pos)
			{
				maincam.transform.position = town_camera_pos;
			}
			else
			if(current_camera_pos == town_camera_pos)
			{
				maincam.transform.position = mech_camera_pos;
			}
			else
			{
				maincam.transform.position = mech_camera_pos; 
			}
		}
		
		if(Input.GetKeyDown(KeyCode.O))
		{ 
			displayObjectivesMenu();
		}
		
		if(Input.GetKeyDown(KeyCode.LeftShift))
		{ 
			gm.advanceFST();
		}
		
		if(Input.GetKeyDown(KeyCode.R))
		{ 
			displayRepairMenu();
		}
		
		//zoom out
		if(Input.GetKey(KeyCode.Equals))
		{  
			zoom_adjust += .5F * zoomSensitivity; 
		}
		
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			
			if(!mech_menu_displayed && !base_menu_displayed && !repair_menu_displayed && !pause_menu_displayed && !objective_menu_displayed)
				displayPauseMenu();
			else
			{
				mech_menu_displayed = false;
				base_menu_displayed = false;
				repair_menu_displayed = false;
				pause_menu_displayed = false;
				objective_menu_displayed = false;
				audio.PlayOneShot(sound_open_menu);
			} 
		}
		
		if(Input.GetKeyDown(KeyCode.Q))
		{
			displayMechUpgradeMenu();
		}
//		if(Input.GetKeyDown())
//		{
//			displayMechUpgradeMenu();
//		}
		if(Input.GetKeyDown(KeyCode.E))
		{
			displayBaseUpgradeMenu();
		}
		
		if(Input.GetKeyDown(KeyCode.R))
		{
			drawRepairMenu();
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
		
		
		
//		if(Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.LeftControl))
//		{
//			gameManagerS.forcePlayerTurn();
//		}
		if(Input.GetKeyDown(KeyCode.Space) & gm.current_turn == Turn.Player)
		{
			gm.endPlayerTurn();
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
	
	public   void popFrontOfRoute()
	{
		if(current_path_display!=null)
			current_path_display.removeFrontNode();
	}	
	
	public   void setRoute(PathDisplay in_path, List<string> _hex_display_text, HexData _hex_display_text_at)
	{
		hex_display_text_at = _hex_display_text_at;
		hex_display_text = _hex_display_text;
		
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
	
	public static List<string> 	  hex_display_text = new List<string>();
	public static HexData 	  hex_display_text_at;
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
	
	public Texture menu_upgrade_norm;
	public Texture menu_upgrade_over;
	public Texture menu_upgrade_down_canafford;
	public Texture menu_upgrade_down_cannotafford; 
	public Texture menu_upgrade_disabled;
	public Texture menu_upgrade_owned;
	
	public Texture menu_upgrade_tier_spacer;
	
	public Texture part_small_gear;
	public Texture part_small_strut;
	public Texture part_small_plate;
	public Texture part_small_piston;
	
	public GUIStyle menu_close_button;
	
	
	public Texture hud_indicator_box;
	
	
	private bool mech_menu_displayed = false;
	private bool base_menu_displayed = false;
	private bool repair_menu_displayed = false;
	private bool controls_menu_displayed = false;
	 
	public void displayControlsMenu(){ controls_menu_displayed = !controls_menu_displayed; audio.PlayOneShot(sound_open_menu);}
	public void displayRepairMenu(){ repair_menu_displayed = !repair_menu_displayed; audio.PlayOneShot(sound_open_menu);}
	public void displayObjectivesMenu(){ objective_menu_displayed = !objective_menu_displayed; pause_menu_displayed = false; controls_menu_displayed = false;  audio.PlayOneShot(sound_open_menu);}
	public void displayPauseMenu(){ pause_menu_displayed = !pause_menu_displayed;	  audio.PlayOneShot(sound_open_menu);}
	public void displayMechUpgradeMenu(){ mech_menu_displayed = !mech_menu_displayed;	mech_upgrade_start_time = Time.time; audio.PlayOneShot(sound_open_menu);}
	public void displayBaseUpgradeMenu(){ base_menu_displayed = !base_menu_displayed;    base_upgrade_start_time = Time.time; audio.PlayOneShot(sound_open_menu);}
	
	public void hideMechUpgradeMenu(){ mech_menu_displayed = false; audio.PlayOneShot(sound_button); audio.PlayOneShot(sound_open_menu);}
	public void hideBaseUpgradeMenu(){ base_menu_displayed = false; audio.PlayOneShot(sound_button); audio.PlayOneShot(sound_open_menu);}
	public void hideRepairMenu(){ repair_menu_displayed = false; audio.PlayOneShot(sound_button);  audio.PlayOneShot(sound_open_menu);}
	public void hideObjectivesMenu(){ objective_menu_displayed = false;  audio.PlayOneShot(sound_button);  audio.PlayOneShot(sound_open_menu);}
	public void hidePauseMenu(){ pause_menu_displayed = false; audio.PlayOneShot(sound_button);  audio.PlayOneShot(sound_open_menu);}
	public void hideControlsMenu(){controls_menu_displayed = false; audio.PlayOneShot(sound_button);  audio.PlayOneShot(sound_open_menu);}
	
	  
	
	private Rect mech_upgrade_rect;
	private Rect base_upgrade_rect;
	private Rect controls_rect = new Rect (343,81, 594, 579);
	private Rect objective_rect = new Rect (417,220, 446, 248);
	
	public int gui_element_size = 80;
	public int gui_text_element_size =  20;
	public int gui_spacing      = 10;
	
	public bool objective_menu_displayed = false;
	public bool pause_menu_displayed = false;
	
	public Texture objectives_bg;
	public Texture big_bg;
	
	private void drawObjectivesMenu(){
		 
		GUI.BeginGroup (new Rect (417,220, 446, 248));   
		
			//background
			GUI.DrawTexture(new Rect (0,0, 446, 248), objectives_bg);	
		
			if(GUI.Button(new Rect(400,17,30, 29),"",menu_close_button))
			{
				hideObjectivesMenu();
				gm.mouse_over_gui = false;
			}
			//menu title
			ShadowAndOutline.DrawOutline(new Rect(18,18, 342, 47), "Objectives", menu_heading, new Color(0,0,0,.5F),Color.white, 3F);
	 
		
			//upgrade items
			GUI.BeginGroup (new Rect (25, 80, 420, 438));  
				
				ShadowAndOutline.DrawOutline(new Rect(0,0, 342, 90),  gm.getObjectiveText(), menu_item_heading, new Color(0,0,0,.5F),Color.white, 3F);
			 
		 
			GUI.EndGroup ();  
		GUI.EndGroup (); 
	}
	
	public Texture logo_small;
	public Texture controls;
	
	private void drawPauseMenu(){
		
		
		GUI.DrawTexture(new Rect (417+13, 108, 420, 114), logo_small);
		GUI.BeginGroup (new Rect (417,220, 446, 248));   
		
			//background
			GUI.DrawTexture(new Rect (0,0, 446, 248), objectives_bg);	
		
			if(GUI.Button(new Rect(400,17,30, 29),"",menu_close_button))
			{
				hidePauseMenu();
				gm.mouse_over_gui = false;
			} 
		
		
			//upgrade items
			GUI.BeginGroup (new Rect (13, 28, 420, 248));  
				if(drawButtonBoolInGroup(105,0,210,"Restart"))
					gm.restartLevel();
		
				if(drawButtonBoolInGroup(105,50,210,"Objectives"))
				{
					pause_menu_displayed = false;
					displayObjectivesMenu();
				}
				if(drawButtonBoolInGroup(105,100,210,"Control List"))
				{
					pause_menu_displayed = false;
					displayControlsMenu();
				}
		
				if(drawButtonBoolInGroup(105,150,210,"Quit"))
					gm.quitToMenu();
		 
			GUI.EndGroup ();  
		GUI.EndGroup (); 
	}
	
	private void drawControlsMenu(){
		GUI.BeginGroup (new Rect (343,81, 594, 579));   
		
			//background
			GUI.DrawTexture(new Rect (0,0, 594, 579), controls);	
		
			if(GUI.Button(new Rect(541,28,30, 29),"",menu_close_button))
			{
				hideControlsMenu();
				gm.mouse_over_gui = false;
			} 
		GUI.EndGroup();
		
	}
	
	
	private bool drawButtonBoolInGroup(int from_left, int from_top, int width, string text)
	{  
		GUI.BeginGroup (new Rect (from_left,from_top, width, 35));  
			bool result = GUI.Button(new Rect (0,0, width, 35),"", HUD_button); 
			ShadowAndOutline.DrawOutline(new Rect(0, 0, width, 31), text, 
				 menu_filter_text, new Color(0,0,0,.5F),Color.white, 3F);
		GUI.EndGroup (); 
		
		if(result)
			audio.PlayOneShot(sound_button);
			 
		return result;
	}
	
	public GUIStyle popupbox;
	public Texture popup;
	
	private void drawInGamePopups(){
		
		List<PopupInfo> popups = gm.getPopupInfoForLevel();
		
		foreach(PopupInfo pops in popups)
		{
			drawSpecificPopup(pops.placement, pops.text, pops.widthless, pops.heightless, pops.ID);
		}
		
	}
	
	public GUIStyle popup_text;
	
	public Texture s1;
	public Texture s2;
	public Texture s3_1;
	public Texture s3_2;
	public Texture s4;
	public Texture s5;
	public Texture s6;
	public Texture s6_f;
	public Texture s6_j;
	public Texture s6_o;
	public Texture s7;
	public Texture s8;
	public Texture s9_1;
	public Texture s9_2;
	public Texture s10;
	public Texture s11;
	public Texture s14;
	
	public bool bs1   = false;
	public bool bs2   = false;
	public bool bs3_1   = false;
	public bool bs3_2   = false;
	public bool bs4   = false;
	public bool bs5   = false;
	public bool bs6   = false;
	public bool bs6_f   = false;
	public bool bs6_j   = false;
	public bool bs6_o   = false;
	public bool bs7   = false;
	public bool bs8   = false;
	public bool bs9_1   = false;
	public bool bs9_2   = false;
	public bool bs10   = false;
	public bool bs11   = false;
	public bool bs14   = false;
	
	private void drawOverlay(Texture fst, ref bool closer)
	{ 
		if(fst!= null && !closer)
		{
			game_paused_overlay = true;
			GUI.DrawTexture(new Rect (0,0, Screen.width, Screen.height), fst);	
//			ShadowAndOutline.DrawOutline(new Rect(Screen.width/2 - 100, Screen.height - 100, 200, 50), "Press [SHIFT] to continue.", popup_text, new Color(0,0,0,.7F),Color.white, 3F);
			if(drawButtonBool(740, 160, 200, "Click Here to Continue"))
			{
				closer = true;
				game_paused_overlay = false;
			}
				
		} 
	}	
	
	public Vector3 getScreenSpot(float xx, float yy)
	{
		Vector2 v2 = new Vector2(xx,yy);
		return Camera.main.WorldToScreenPoint (new Vector3(v2.x,0,v2.y)); 
	}	
	
	bool popup_visible = false;
	bool beginning_of_game = true;
	bool spot1 = false;
	bool spot2 = false;
	bool spot3 = false;
	bool spot4 = false;
	bool spot5 = false;
	bool spot6 = false;
	bool spot7 = false;
	bool spot8 = false;
	bool upgradedlegs = false;
	bool nodeflags = false; 
	
	public void drawGmaps()
	{
		
		if(gm.current_level == Level.Level0)
		{
			if(!spot1)
			{
				if(em.isPlayerAt(19,18))
				{
					spot1 = true;
				}
				PopupInfo temp  = new PopupInfo("Click here to move.", getScreenSpot(28.54951F, 57.88783F), Trigger.Visible, 1);
					temp.widthless = 100; temp.heightless = 152;
				drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
			}
			else
			if(!spot2)
			{ 
				if(em.isPlayerAt(20,22))
				{
					spot2 = true;
				} 
				PopupInfo temp  = new PopupInfo("Next, move here!", getScreenSpot(27.5F, 66.5F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 152;
				drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
			}
			else
			if(!spot3)
			{ 
				if(em.isPlayerAt(23,21))
				{
					spot3 = true;
				} 
				if(em.getNodeInfoAt(22,21).node_level == NodeLevel.Empty)
					spot3 = true;
				PopupInfo temp  = new PopupInfo("Move on to this tile,\nthen click it to <b>Scavenge</b>\nfor <b>Parts</b> until it's empty.", getScreenSpot(33F,67.3F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 106;
				drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
			}
			else
			if(!nodeflags)
			{
				
				
				PopupInfo  temp  = new PopupInfo("This is an <b>Outpost</b>.\nIt gives you <b>Plates</b> & <b>Struts</b>.\nClick here to <b>Scavenge</b>."
					, getScreenSpot(37.5F,69.9F), Trigger.Visible, 1);
					temp.widthless = 100; temp.heightless = 106;
				drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
					temp  = new PopupInfo("This is a <b>Factory</b>.\nIt gives you <b>Pistons</b> & <b>Gears</b>.\nClick here to<b> Scavenge</b>.", getScreenSpot(35F,75.4F), Trigger.Visible, 1);
					temp.widthless = 100; temp.heightless = 106;
				drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
					temp  = new PopupInfo("This is a <b>Junkyard</b>.\nIt gives you random parts.\nClick here to <b>Scavenge</b>.", getScreenSpot(43.6F,75.8F), Trigger.Visible, 1);
					temp.widthless = 100; temp.heightless = 106;
				drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
				
				if(em.mech_s.getPartCount(Part.Gear) >= 2 && em.mech_s.getPartCount(Part.Piston) >= 4 && em.mech_s.getPartCount(Part.Strut) >= 2)
				{	nodeflags = true;
					List<Enemy> enemies = em.getEnemies();
					foreach(entityEnemyS es in enemies)
					{
						((entityEnemyS)es).displayhelper = true;
					}
				}
			}
			 
			
			if(em.mech_s.checkUpgrade(MechUpgrade.Move_Mountain) && !upgradedlegs)
			{ 
				if(em.isPlayerAt(31,21))
				{
					upgradedlegs = true;
				} 
				PopupInfo temp  = new PopupInfo("Now that you have <b>Mountaineering Claws,</b>\ncross over to here!", getScreenSpot(53.6F,79.2F), Trigger.Visible, 1);
					temp.widthless = 100; temp.heightless = 106; 
				drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
				 
			}
			
			if(upgradedlegs && em.base_s.getHighestLevelUpgrade(BaseUpgradeMode.Walls) == BaseUpgradeLevel.Level0)
			{
				//Protect it at all costs!
				PopupInfo temp  = new PopupInfo("This is the <b>Town</b>.\n" +
					"It has its turn to defend\n" +
					 "after the <b>Enemy Turn</b>.\n" +
					 "Upgrade it <b>Ironplate Retrofit</b>.", getScreenSpot(52.1F,101.4F), Trigger.Visible, 1);
					temp.widthless = 100; temp.heightless = 86;
					drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
				
				 
			} 
			if(upgradedlegs && em.base_s.getHighestLevelUpgrade(BaseUpgradeMode.Walls) != BaseUpgradeLevel.Level0)
			{
				//Protect it at all costs!
				PopupInfo temp  = new PopupInfo("This is the <b>Town</b>.\n" +
					"Thanks for the town, now\nplease defend us from those \n<b>Enemy Mechs</b>!", getScreenSpot(52.1F,101.4F), Trigger.Visible, 1);
					temp.widthless = 100; temp.heightless = 86;
					drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
				
				if(!spawned_last_wave)
				{
					em.instantiateEnemy(37,33,15,15,1,true,false,EntityE.Enemy);
					em.instantiateEnemy(37,34,15,15,1,true,false,EntityE.Enemy);
					spawned_last_wave  = true;
				}
				 
			}
			
					
			{
				 
				PopupInfo temp  = new PopupInfo("This is an enemy <b>Spawner</b>.\n<b>Enemies</b> will emerge from here sometimes.", getScreenSpot(40.38F,101.5F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 106;
				drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
			
			 
				temp  = new PopupInfo("This is an enemy <b>Spawner</b>.\n<b>Enemies</b> will emerge from here sometimes.", getScreenSpot(57.48F,110.81F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 106;
				drawSpecificPopup(temp.placement, temp.text, temp.widthless, temp.heightless, temp.ID); 
				
				if(em.mech_s.gethpPercent() < .333)
				{ 
					drawOverlay(s14, ref bs14);
				}
			}
		}
//		else
	}
	
	public bool spawned_last_wave =false;
	
	public void drawPopups(){
		
		if(gm.current_level == Level.Level0)
		{
			
		if(!bs1)
		{
			drawOverlay(s1, ref bs1);
		}
		else
		if(!bs2)
		{
			drawOverlay(s2, ref bs2);
		}
		else
		if(!bs6)
		{
			drawOverlay(s6, ref bs6);
		}
		else
		if(spot2 && !bs3_1)
		{ 
			drawOverlay(s3_1, ref bs3_1);
		}
		else
		if(spot2 && !bs3_2)
		{ 
			drawOverlay(s3_2, ref bs3_2);
		}
		else
		if(spot2 && !bs4)
		{ 
			drawOverlay(s4, ref bs4);
		}
		else
		if(spot2 && !bs5)
		{ 
			drawOverlay(s5, ref bs5);
		}  
		else	//got the parts now
		if(nodeflags && !bs8 )
		{
			drawOverlay(s8, ref bs8);
		}   
		else
		if(nodeflags && !bs7)
		{
			nodeflags = true;
			drawOverlay(s7, ref bs7);
		}
		else
		if(nodeflags && em.mech_s.checkUpgrade(MechUpgrade.Move_Mountain) && !bs9_1)
		{
			drawOverlay(s9_1, ref bs9_1);
		}
		else
		if(upgradedlegs && !bs9_2)
		{
			drawOverlay(s9_2, ref bs9_2);
		}
		else
			//either killed or ran past enemy, now near base
		if(upgradedlegs && !bs10)
		{
			if(em.isPlayerAt(32,28) || em.isPlayerAt(33,27))
				drawOverlay(s10, ref bs10);
		}
		else
		if(upgradedlegs && !bs11)
		{
			drawOverlay(s11, ref bs11);
		}  
		
		}
	}
	
	
	
	private bool draw_hover_node = false;
	private PopupInfo pi;
	
	public void drawHoverNodeInfo(PopupInfo pii, bool draw_it)
	{
		if(!draw_it)
			pi.enabled = false;
		else
			pi = pii;
	}
	
	public void disablePI()
	{
		pi.enabled = false; 
	}
	
	public void drawSpecificPopup(Vector2 placement, string text, int widthless, int heightless, int ID)
	{ 
		int popup_width = 304;
		int popup_height = 271;
		int popup_content_width = 287;
		int popup_content_height = 146;
		
		
		switch(ID)
		{
		case 5: 
			if(gm.first_move_popup_display)
			{ 
				GUI.BeginGroup (new Rect (placement.x - 48,Screen.height - placement.y-popup_height + heightless, popup_width, popup_height- heightless));  
				
//					GUI.Box(new Rect(0,0, popup_width - widthless, popup_height - heightless),"",popupbox);
					GUI.DrawTexture(new Rect(0,0, popup_width - widthless, popup_height - heightless),popup);
					ShadowAndOutline.DrawOutline(new Rect(9,19, popup_content_width- widthless, popup_content_height - heightless), text, popup_text, new Color(0,0,0,.5F),Color.white, 3F);
				GUI.EndGroup (); 
			}
			break;
			
		default:
		GUI.BeginGroup (new Rect (placement.x - 48,Screen.height - placement.y-popup_height + heightless, popup_width, popup_height- heightless));  
		
//			GUI.Box(new Rect(0,0, popup_width - widthless, popup_height - heightless),"",popupbox);
			GUI.DrawTexture(new Rect(0,0, popup_width - widthless, popup_height - heightless),popup);
					ShadowAndOutline.DrawOutline(new Rect(9,19, popup_content_width- widthless, popup_content_height - heightless), text, popup_text, new Color(0,0,0,.5F),Color.white, 3F);
		GUI.EndGroup (); 
			break;
		}
	}
	
	
	void OnGUI()
	{	
		drawHexText();
		drawGmaps();
		if(pi.enabled && nodeflags)
		{
			drawSpecificPopup(pi.placement,pi.text,pi.widthless,pi.heightless,pi.ID);
		}
		
		drawPopups(); 
		
		drawInGamePopups();
		
		checkMousePlacement();
		
		if(pause_menu_displayed)
			drawPauseMenu();
		
		if(controls_menu_displayed)
			drawControlsMenu();
		
		 
		
		
		//draw Part bars
		drawHUDPartBar(203, 109,  bar_part_plate_bg, Part.Plate); 
		drawHUDPartBar(203,  65,  bar_part_strut_bg, Part.Strut); 
		drawHUDPartBar( 29, 109,   bar_part_gear_bg, Part.Gear); 
		drawHUDPartBar( 29,  65, bar_part_piston_bg, Part.Piston); 
			
		//draw HP Bars
		drawHUDHPBar(311,  60, 276, town.getCurrentHP(), town.getMaxHP(), Color.red, Color.yellow, Color.gray);
			ShadowAndOutline.DrawOutline(new Rect(Screen.width - 360, Screen.height - 104, 50, 28), "Mech", menu_filter_text, new Color(0,0,0,.7F),Color.white, 3F);
		drawHUDHPBar(311, 104, 178, mech.getCurrentHP(), mech.getMaxHP(), Color.red, Color.yellow, Color.green);
			ShadowAndOutline.DrawOutline(new Rect(Screen.width - 360, Screen.height - 60, 50, 28), "Town", menu_filter_text, new Color(0,0,0,.7F),Color.white, 3F);
		
		//draw repair menu button
		if(drawButtonBool(122, 105, 94, "Repair"))
			displayRepairMenu();
		
		//draw round counter
		if(!base_menu_displayed)
		{ 
			if(drawButtonBool(408, Screen.height - 28, 104,"Round " + gm.current_round))
				displayObjectivesMenu(); 
		}
		
		//draw end turn button5
		if(!mech_menu_displayed)
			drawEndTurnButton();
		
		//draw AP bar
		drawHUDEnergyBar(417,29,446,mech.getCurrentAP(), mech.getMaxAP());
		if(gm.display_hud_obj_area)
		{
			GUI.DrawTexture(new Rect(562,59,156,31), hud_indicator_box);
			ShadowAndOutline.DrawOutline(new Rect(562,59,156,23), gm.getHUDIndicatorText(), menu_filter_text, new Color(0,0,0,.7F),Color.white, 3F);
		}
		
		 
		//display menu mech upgrade
		if(mech_menu_displayed)
			drawUpgradeMenu(UpgradeMenu.Mech);
			
		//display menu mech upgrade
		if(base_menu_displayed)
			drawUpgradeMenu(UpgradeMenu.Base);
			 
		//display menu mech repair
		if(repair_menu_displayed) 
			drawRepairMenu(); 
//	  	 
//		drawFullscreenTips();
		
		if(objective_menu_displayed)
			drawObjectivesMenu();
		
	}
	
	
	private void checkMousePlacement(){
		
		Rect mech_menu_zone = new Rect (30, 28, 378, 576);
		Rect base_menu_zone = new Rect (Screen.width - 378 - 30, 28, 378, 576);
		Rect repair_menu_zone = new Rect (Screen.width - 311, Screen.height - 187, 282, 92);
		
		Vector2 mouse_pos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		
		
		if((base_menu_zone.Contains(mouse_pos) && base_menu_displayed) 
			|| (mech_menu_zone.Contains(mouse_pos) && mech_menu_displayed) 
			|| (objective_rect.Contains(mouse_pos) && objective_menu_displayed) 
			|| (controls_rect.Contains(mouse_pos) && controls_menu_displayed) 
			|| (repair_menu_zone.Contains(mouse_pos) && repair_menu_displayed) ) 
			gm.mouse_over_gui = true; 
		else
			gm.mouse_over_gui = false;
		
		if(game_paused_overlay)
			gm.mouse_over_gui = true; 
			
		
		
	}
	
	public bool game_paused_overlay = false;
	
	private void drawEndTurnButton(){
		
		if(gm.current_turn == Turn.Player) 
		{
			if(drawButtonBool(976, Screen.height - 28, 104,"Finish Turn?"))
			{
				gm.endPlayerTurn(); 
				audio.PlayOneShot(sound_button);
			}
		}
		else
		
		if(gm.current_turn == Turn.Enemy) 
		{
			if(drawButtonBool(976, Screen.height - 28, 104,"Enemy Turn...")) 
				audio.PlayOneShot(sound_negative); 
		}
		else
		
		if(gm.current_turn == Turn.Base) 
		{
			if(drawButtonBool(976, Screen.height - 28, 104,"Base Turn..."))
				audio.PlayOneShot(sound_negative);
		} 	
	}
	
	
	public void drawHexText(){
		if(!gm.mouse_over_gui)
		{
			
			Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (hm.CoordsGameTo3D(hex_display_text_at.x, hex_display_text_at.z)); 
			 
			
			switch(hex_display_text.Count){
			case 0: break;
			case 1: 
				ShadowAndOutline.DrawOutline(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 10, 200, 20), "<size=21>"+hex_display_text[0]+"</size>", selection_hover, Color.black, Color.white, 2F); 
				break;
			case 2:
				ShadowAndOutline.DrawOutline(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 20, 200, 20), "<size=21>"+hex_display_text[0]+"</size>", selection_hover, Color.black, Color.white, 2F); 
				ShadowAndOutline.DrawOutline(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 00, 200, 20), hex_display_text[1], selection_hover, Color.black, Color.white, 2F); 
				break;
			case 3:
				ShadowAndOutline.DrawOutline(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 30, 200, 20), "<size=21>"+hex_display_text[0]+"</size>", selection_hover, Color.black, Color.white, 2F); 
				ShadowAndOutline.DrawOutline(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 10, 200, 20), hex_display_text[1], selection_hover, Color.black, Color.white, 2F); 
				ShadowAndOutline.DrawOutline(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y + 10, 200, 20), hex_display_text[2], selection_hover, Color.black, Color.white, 2F); 
				break;
			}
			
			
		
		}else{
			if(current_path_display != null){
				current_path_display.hidePath();
				current_path_display.destroySelf();
			}
		}
	}
	
	public Color can_afford;
	public Color cannot_afford;
	public Color owned_afford;
	public Color disabled_afford;
	
	private MechUpgradeMode mech_upgrade_tab = MechUpgradeMode.Movement;
	private BaseUpgradeMode base_upgrade_tab = BaseUpgradeMode.Armament;
	 
	private void drawUpgradeMenu(UpgradeMenu upgrade_menu_type){ 
		switch(upgrade_menu_type)
		{
			case UpgradeMenu.Base: drawBaseUpgradeMenu(); break;
			case UpgradeMenu.Mech: drawMechUpgradeMenu(); break;
		}
	}
	
	
	
	private static float mech_upgrade_start_time = 0;
	private static float base_upgrade_start_time = 0;
	private static float animation_upgrade_menu_time = .14F; 
	
	private float menu_mech_origin_lerp = -408;
	private float menu_mech_destin_lerp = 30;
	
	private float menu_base_origin_lerp = Screen.width;
	private float menu_base_destin_lerp = Screen.width - 408;
	
	private void lerpMenuInMechUpgrade()
	{ 
		float current_t_param = (Time.time - mech_upgrade_start_time)/animation_upgrade_menu_time;
//		print (current_t_param);
		if(current_t_param < 1)
		{
			float result = Mathf.SmoothStep(menu_mech_origin_lerp,menu_mech_destin_lerp, current_t_param);
			mech_upgrade_rect = new Rect(result, 28, 378,576);  
		}
		else
		{
			mech_upgrade_rect =new Rect (30, 28, 378, 576);  
		}
	}
	
	private void lerpMenuInBaseUpgrade()
	{ 
		float current_t_param = (Time.time - base_upgrade_start_time)/animation_upgrade_menu_time;
//		print (current_t_param);
		if(current_t_param < 1)
		{
			float result = Mathf.SmoothStep(menu_base_origin_lerp,menu_base_destin_lerp, current_t_param);
			base_upgrade_rect = new Rect(result, 28, 378,576);  
		}else
		{
			base_upgrade_rect = new Rect (Screen.width - 378 - 30, 28, 378, 576);   
		}
	}
	
	private void drawMechUpgradeMenu(){ 
		
		lerpMenuInMechUpgrade();
		 
		GUI.BeginGroup (mech_upgrade_rect);   
		
			//background
			GUI.DrawTexture(new Rect (0,0, 378, 576), menu_background);	
		
			if(GUI.Button(new Rect(329,17,30, 29),"",menu_close_button))
			{
				hideMechUpgradeMenu();
				gm.mouse_over_gui = false;
			}
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
			GUI.BeginGroup (new Rect (14, 120, 350, 438)); 
				drawMechUpgradeMenuEntries();	 		
			GUI.EndGroup ();  
		GUI.EndGroup (); 
	}
	
	private bool[] click_started = new bool[5];
	
	private void drawMechUpgradeMenuEntries(){
		List<UpgradeEntry> upgrade_entires = mech.getUpgradeEntries(mech_upgrade_tab);
	 	bool can_afford_upgrade = false; 
		
		for(int entry_row = 0; entry_row < upgrade_entires.Count; entry_row++)
		{
			UpgradeEntry entry = upgrade_entires[entry_row];
			can_afford_upgrade = mech.canAffordUpgrade(entry.upgrade_type);
			
			
 			bool upgrade_owned = mech.checkUpgrade(entry.upgrade_type); 
			Rect hover_zone = new Rect (48, 28 + 124 + entry_row*(74+15), 342, 74);
			
			
		   if(upgrade_owned)
				GUI.DrawTexture(new Rect(0,entry_row*(74+15),350,82), menu_upgrade_owned);	
				
			GUI.BeginGroup (new Rect (4, 4 + entry_row*(74+15) , 342, 74)); 
		
				if(hover_zone.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) && !upgrade_owned)
				{
					if(Input.GetMouseButtonDown(0))  
					{
						for(int i = 0; i < click_started.Length; ++i)
							click_started[i] = false;
					
						click_started[entry_row] = true;
					}
					if(Input.GetMouseButtonUp(0))  
					{ 
					  
					 
						if(can_afford_upgrade)
						{
							mech.applyUpgrade(entry.upgrade_type);  
							audio.PlayOneShot(sound_button);
						}
						else
						{
							audio.PlayOneShot(sound_negative);	
						}
					
					}
					 
					if(Input.GetMouseButton(0) && click_started[entry_row])
					{  
						if(can_afford_upgrade)
							GUI.DrawTexture(new Rect(0,0,342,74), menu_upgrade_down_canafford);	 
						else
							GUI.DrawTexture(new Rect(0,0,342,74), menu_upgrade_down_cannotafford);	  
					}
					else
					{ 
						click_started[entry_row] = false;
						GUI.DrawTexture(new Rect(0,0,342,74), menu_upgrade_over);	 
					}
				}
				else 
				{ 
					if(!upgrade_owned)
						GUI.DrawTexture(new Rect(0,0,342,74), menu_upgrade_norm);	 
				}
				
				drawEntryContents(entry, upgrade_owned);
					
			GUI.EndGroup ();   
		}
	}
	
	
	private void drawEntryContents(UpgradeEntry entry, bool upgrade_owned){
		
		
		bool enabled = mech.checkUpgradeEnabled(entry.upgrade_type);
		
		GUI.DrawTexture(new Rect(7,7,60,60),entry.thumbnail); 					
		ShadowAndOutline.DrawOutline(new Rect(75,5,200,19), entry.title, menu_item_heading, new Color(0,0,0,.5F),Color.white, 3F);
		ShadowAndOutline.DrawOutline(new Rect(75, 24 ,200,19), entry.description, menu_item_description, new Color(0,0,0,.5F),Color.white, 3F);

		//cost stuff
		GUI.BeginGroup (new Rect (75, 47, 262, 22));
			GUI.DrawTexture(new Rect(  0,0,23,23), part_small_gear);	 
			drawPartCost(new Rect(28,0, 23, 23), entry, Part.Gear, upgrade_owned, enabled);

			GUI.DrawTexture(new Rect( 53,0,23,23), part_small_piston);	
			drawPartCost(new Rect(81,0, 23, 23), entry, Part.Piston, upgrade_owned, enabled);
		 
			GUI.DrawTexture(new Rect(107,0,23,23), part_small_plate);	
			drawPartCost(new Rect(135,0, 23, 23), entry, Part.Plate, upgrade_owned, enabled);

			GUI.DrawTexture(new Rect(160,0,23,23), part_small_strut); 
			drawPartCost(new Rect(188,0, 23, 23), entry, Part.Strut, upgrade_owned, enabled);
	 
			drawPartCostAP(new Rect(207,0, 47, 23), entry.ap_cost, upgrade_owned, enabled);

		GUI.EndGroup ();  
	}
	
	private void drawPartCostAP(Rect in_rect, int ap_cost, bool owned, bool enabled)
	{ 
		Color price_color;
		   
		if(owned)
			price_color = owned_afford;
		else
		if(!enabled)
			price_color = disabled_afford;
		else
			if(ap_cost > mech.getCurrentAP())
				price_color = cannot_afford;
			else
				price_color = can_afford; 
		  
		ShadowAndOutline.DrawOutline(in_rect, ap_cost + " AP", menu_item_ap, new Color(0,0,0,.5F), price_color, 3F);

	}
	  
	private void drawPartCost(Rect in_rect, UpgradeEntry upgrade_entry, Part part_to_price, bool owned, bool enabled)
	{
		int part_cost = 0;
		Color price_color; 
		
		switch(part_to_price)
		{
			case Part.Gear:   part_cost = upgrade_entry.gear_cost; break;
			case Part.Plate:  part_cost = upgrade_entry.plate_cost; break;
			case Part.Piston: part_cost = upgrade_entry.piston_cost; break;
			case Part.Strut:  part_cost = upgrade_entry.strut_cost; break;
		}
		
		if(owned)
			price_color = owned_afford;
		else
		if(!enabled)
			price_color = disabled_afford;
		else
			if(part_cost > em.getMech().getPartCount(part_to_price))
				price_color = cannot_afford;
			else
				price_color = can_afford;
		 
		ShadowAndOutline.DrawOutline(in_rect, part_cost.ToString(), menu_item_costs, new Color(0,0,0,.5F),price_color, 3F);
					
	}
	  
	private void drawBaseUpgradeMenu(){ 
		
		lerpMenuInBaseUpgrade();
		
		GUI.BeginGroup (base_upgrade_rect);   
			GUI.DrawTexture(new Rect (0,0, 378, 576), menu_background);	
			ShadowAndOutline.DrawOutline(new Rect(18,18, 342, 47), "Base Upgrades", menu_heading, new Color(0,0,0,.5F),Color.white, 3F);
		
			//close
			if(GUI.Button(new Rect(329,17,30, 29),"",menu_close_button))
			{
				hideBaseUpgradeMenu();
				gm.mouse_over_gui = false;
			}
		
			GUI.BeginGroup (new Rect (18, 80, 342, 29));   
				drawBaseMenuFilterButtons(0,0,"Perimeter",   BaseUpgradeMode.Walls );
				drawBaseMenuFilterButtons(119,0,"Armament",  BaseUpgradeMode.Armament );   
				drawBaseMenuFilterButtons(237,0,"Structure", BaseUpgradeMode.Structure);    
			GUI.EndGroup (); 
		
			//upgrade items
			GUI.BeginGroup (new Rect (14, 120, 350, 438)); 
				drawBaseUpgradeMenuEntries();	 		
			GUI.EndGroup ();
		
		GUI.EndGroup (); 
	}
	
	
	private void drawBaseUpgradeMenuEntries(){
		
		List<UpgradeEntry> upgrade_entires = town.getUpgradeEntries(base_upgrade_tab);
	 	bool can_afford_upgrade = false; 
		
		int highest_level_upgrade =  (int)town.getHighestLevelUpgrade(base_upgrade_tab);
		
		
		
		for(int entry_row = 0; entry_row < 3; entry_row++)
		{
			UpgradeEntry entry = upgrade_entires[entry_row];
			can_afford_upgrade = mech.canAffordUpgrade(entry);
			
			BaseUpgradeLevel bul = entry.base_level; 
	 
 			bool upgrade_owned = (highest_level_upgrade) >= (int) bul;
			 
			
			Rect hover_zone = new Rect (Screen.width - 378 - 10, 28 + 124 + entry_row*(168), 342, 74);
			
			
		    if(upgrade_owned)
				GUI.DrawTexture(new Rect(0,entry_row*(168),350,82), menu_upgrade_owned);	
				
			
			GUI.BeginGroup (new Rect (4, 4 + entry_row*(168) , 342, 165)); 
		
				if(highest_level_upgrade < entry_row)
				{
					GUI.DrawTexture(new Rect(0,0,342,74), menu_upgrade_disabled);	 
				}
				else			
				if(hover_zone.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) && !upgrade_owned)
				{
					if(Input.GetMouseButtonDown(0))  
					{
						for(int i = 0; i < click_started.Length; ++i)
							click_started[i] = false;
					
						click_started[entry_row] = true;
					}
					if(Input.GetMouseButtonUp(0))  
					{ 
					 
						if(can_afford_upgrade)
						{
							
							if( (new List<HexData>(hm.getAdjacentHexes(em.getBase().x, em.getBase().z)))
							.Contains(hm.getHex(em.getMech().x,em.getMech().z)))
							{
								em.getBase().upgradeBase(base_upgrade_tab, entry.base_level); 
								audio.PlayOneShot(sound_button);
							}
							else
							{
								audio.PlayOneShot(sound_negative); 
							}
						}
						else
						{
							audio.PlayOneShot(sound_negative);	
						}
					
					}
					 
					if(Input.GetMouseButton(0) && click_started[entry_row])
					{  
						if((can_afford_upgrade) && (new List<HexData>(hm.getAdjacentHexes(em.getBase().x, em.getBase().z)))
								.Contains(hm.getHex(em.getMech().x,em.getMech().z)))
								{
								GUI.DrawTexture(new Rect(0,0,342,74), menu_upgrade_down_canafford);	 
							}
						
						else
						{
							GUI.DrawTexture(new Rect(0,0,342,74), menu_upgrade_down_cannotafford);	  
							
						}
					}
					else
					{ 
						click_started[entry_row] = false;
						GUI.DrawTexture(new Rect(0,0,342,74), menu_upgrade_over);	 
					}
				}
				else 
				{ 
					if(!upgrade_owned)
						GUI.DrawTexture(new Rect(0,0,342,74), menu_upgrade_norm);	 
				}
				
				if(entry_row != 2)
				{
					GUI.DrawTexture(new Rect(0,94,342,56), menu_upgrade_tier_spacer);	 
					
				}
			 
				drawEntryContents(entry, upgrade_owned);
					
			GUI.EndGroup ();   
		}
	}
	
	private bool drawButtonBool(int from_right, int from_bottom, int width, string text)
	{  
		GUI.BeginGroup (new Rect (Screen.width - from_right, Screen.height - from_bottom, width, 35));  
			bool result = GUI.Button(new Rect (0,0, width, 35),"", HUD_button); 
			ShadowAndOutline.DrawOutline(new Rect(0, 0, width, 31), text, 
				 menu_filter_text, new Color(0,0,0,.5F),Color.white, 3F);
		GUI.EndGroup (); 
		
		if(result)
			audio.PlayOneShot(sound_button);
			 
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
	public GUIStyle menu_item_ap;
	
	
	
	private void drawHUDPartBar(int from_left, int from_bottom, Texture background, Part part_type)
	{  
		int part_cap = em.getMech().getPartCapacity();
		GUI.BeginGroup (new Rect (from_left, Screen.height - from_bottom, 168, 44));
			GUI.DrawTexture(new Rect (0,0, 168, 44), background);
			GUI.BeginGroup (new Rect (36, 8, (int)128*(float)em.getMech().getPartCount(part_type)/(float)part_cap, 24));
				GUI.DrawTexture(new Rect (0,0, 128, 24), bar_part_scale);
			GUI.EndGroup (); 
			ShadowAndOutline.DrawOutline(new Rect(36, 8, 128, 24), em.getMech().getPartCount(part_type) + "/" + part_cap, 
				gui_norm_text, new Color(0,0,0,.5F),Color.white, 3F);
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
				gui_norm_text, new Color(0, 0, 0, .5F),Color.white, 3F);
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
				gui_norm_text, new Color(0, 0, 0, .5F),Color.white, 3F);
		GUI.EndGroup (); 
	}
	
	private void drawMechMenuFilterButtons(int from_left, int from_top,  string text, MechUpgradeMode filter_control)
	{   		 
		GUI.BeginGroup (new Rect (from_left, from_top, 75, 29));  
			if(mech_upgrade_tab == filter_control) 
				GUI.Toggle(new Rect (0,0, 75, 29), mech_upgrade_tab == filter_control, "", menu_filter_button_selected);
			else
				if(GUI.Toggle(new Rect (0,0, 75, 29), mech_upgrade_tab == filter_control, "", menu_filter_button))
				{
					mech_upgrade_tab = filter_control; 
					audio.PlayOneShot(sound_button); 
				}
			ShadowAndOutline.DrawOutline(new Rect(0, 0, 75, 29), text, 
				menu_filter_text, new Color(0,0,0,.5F),Color.white, 3F);
		GUI.EndGroup ();   
	}
	
	private void drawBaseMenuFilterButtons(int from_left, int from_top,  string text, BaseUpgradeMode filter_control)
	{   		 
		GUI.BeginGroup (new Rect (from_left, from_top, 105, 29));  
			if(base_upgrade_tab == filter_control) 
				GUI.Toggle(new Rect (0,0, 105, 29), base_upgrade_tab == filter_control, "", menu_filter_button_selected);
			else
				if(GUI.Toggle(new Rect (0,0, 105, 29), base_upgrade_tab == filter_control, "", menu_filter_button))
				{
					base_upgrade_tab = filter_control;
					audio.PlayOneShot(sound_button);
				}
			ShadowAndOutline.DrawOutline(new Rect(0, 0, 105, 29), text, 
				menu_filter_text, new Color(0,0,0,.5F),Color.white, 3F);
		GUI.EndGroup ();   
	}
	
	
	
	public GUIStyle repair_menu_text;
	public GUIStyle repair_menu_button_canafford;
	public GUIStyle repair_menu_button_cannotafford;
	public GUIStyle repair_callout_bg;
	
	public Texture repair_menu_gear;
	public Texture repair_menu_strut;
	public Texture repair_menu_plate;
	public Texture repair_menu_piston;
	
	
	private void drawRepairMenu(){ 
		  
		GUI.BeginGroup (new Rect (Screen.width - 311, Screen.height - 187, 282, 92));
			GUI.Box(new Rect (0,0, 282, 92),"", repair_callout_bg);
			 
				ShadowAndOutline.DrawOutline(new Rect(16, 11, 47, 17), "-1 Part", repair_menu_text, new Color(0,0,0,.5F),Color.white, 3F);
				ShadowAndOutline.DrawOutline(new Rect(16, 27, 47, 17), "-1 AP", repair_menu_text, new Color(0,0,0,.5F),Color.white, 3F);
				ShadowAndOutline.DrawOutline(new Rect(16, 43, 47, 17), "+2 HP", repair_menu_text, new Color(0,0,0,.5F),Color.white, 3F);
			 
			GUI.BeginGroup (new Rect (71, 13, 198, 48)); 
				drawRepairMenuPartButton(repair_menu_gear,  0, Part.Gear);
				drawRepairMenuPartButton(repair_menu_piston,1, Part.Piston);
				drawRepairMenuPartButton(repair_menu_plate, 2, Part.Plate);
				drawRepairMenuPartButton(repair_menu_strut, 3, Part.Strut); 
			GUI.EndGroup ();  
		
		GUI.EndGroup (); 
		
	}
	
	private void drawRepairMenuPartButton(Texture button_image, int position, Part part_type)
	{ 
		if(mech.getPartCount(part_type) > 0 && mech.getCurrentAP() >= mech.getRepairAPCost() && mech.getCurrentHP() < mech.getMaxHP())
		{ 
			if(GUI.Button(new Rect(position*48 + position*2, 0, 48,48),button_image,repair_menu_button_canafford))
			{
				mech.repair(part_type);
				audio.PlayOneShot(sound_button);
			}
		}
		else
		{ 
			if(GUI.Button(new Rect(position*48 + position*2, 0, 48,48),button_image,repair_menu_button_cannotafford))  
			{	
				audio.PlayOneShot(sound_negative);
				return;
			}
		}
	
	}
	
	
	
	
} 
//-0.841947, 0, 1.81415