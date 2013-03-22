using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Display In Play Menu Screens
public class inPlayMenuS : MonoBehaviour {
	//text style/color, set in inspector	
	public GUIStyle gui_normal_text;
	public GUIStyle parts_text;
	
	//hp/ap bars
	public GUIStyle ap_bar;
	public GUIStyle hp_bar;
	
	//menu backboards
	public Texture hp_backboard;
	public Texture parts_backboard;
	public Texture top_menu_backboard;
	public Texture upgrade_backboard;	
	
	//custom styles for buttons
	public GUIStyle end_turn_button_style;
	public GUIStyle star_button_style;
	public GUIStyle base_button_style;
	public GUIStyle health_button_style;
	public GUIStyle mech_button_style;
	public GUIStyle transport_button_style;
	
	//custom styles for boxes
	public GUIStyle top_menu_backboard_style;
	
	//window size
	private int screen_size_x;
	private int screen_size_y;
	
	private static entityMechS mech;
	
	public static void setMech()
	{
		mech    = GameObject.FindGameObjectWithTag("player_mech").GetComponent<entityMechS>();
	}

	void Update() {
		screen_size_x = Screen.width;
    	screen_size_y = Screen.height;
	}
	
	
	void OnGUI(){
		//menu for upgrades
		UpgradeMenuS script =  GetComponent<UpgradeMenuS>();
		
		//All Backboard variables must be based on "screen_size_x" variable and "screen_size_y" variable	 
		int button_x_start = screen_size_x/8; 
		int button_y_start = screen_size_y/6; 
		int button_size_width = screen_size_x/4; 
		int button_size_height = screen_size_y/9; 
		int button_size = button_size_height/2; 
		 
		//End Turn Button
		if(GUI.Button(new Rect((screen_size_x/2 - (screen_size_x/20)),(button_y_start - (screen_size_y/20)), (button_size_height *2), (button_size_width/7)), "", end_turn_button_style)) {
			gameManagerS.endPlayerTurn();
		}
		
		//add variables here for adjusting backboards
		//GUI.DrawTexture(new Rect(((button_x_start*4)-(button_x_start/5)), ((button_y_start/3)-(button_y_start*10)), (button_size_height *6), (button_size_width*8)), top_menu_backboard, ScaleMode.ScaleToFit, true);
		GUI.DrawTexture(new Rect((screen_size_x/4 + (screen_size_x/13)), ((button_y_start/3)-(button_y_start*11)+screen_size_x/24), (button_size_height *6), (button_size_width*8)), top_menu_backboard, ScaleMode.ScaleToFit, true);
		GUI.DrawTexture(new Rect(0, (button_y_start/8), (button_size_height *4), (button_size_width*4)), upgrade_backboard, ScaleMode.ScaleToFit, true);
		GUI.DrawTexture(new Rect(((button_x_start *6) + screen_size_x/60), (button_y_start/15), (button_size_height *4), (button_size_width*4)), parts_backboard, ScaleMode.ScaleToFit, true);
		GUI.DrawTexture(new Rect((button_x_start*3), ((screen_size_y/2) + (screen_size_y/4)), (button_size_height *5), (screen_size_x/7)), hp_backboard, ScaleMode.ScaleToFit, true);

		//Part Count Text
		int t_x_start = screen_size_x - (screen_size_x/6 + screen_size_x/115); 
		int t_y_end = screen_size_y - screen_size_y/24; 
		int t_size_width = 80; 
		int t_size_height = 20;
		int t_spacing = screen_size_x/7 - (screen_size_x/6 + screen_size_x/40);
		
		GUI.Label(new Rect(t_x_start - (t_spacing * 3), t_y_end, t_size_width, t_size_height), entityMechS.part_count[Part.Strut].ToString(),   parts_text);
		GUI.Label(new Rect(t_x_start - (t_spacing * 2), t_y_end, t_size_width, t_size_height), entityMechS.part_count[Part.Plate].ToString(), parts_text);
		GUI.Label(new Rect(t_x_start - t_spacing, t_y_end, t_size_width, t_size_height), entityMechS.part_count[Part.Gear].ToString(),  parts_text);
		GUI.Label(new Rect(t_x_start, t_y_end, t_size_width, t_size_height), entityMechS.part_count[Part.Piston].ToString(),  parts_text);
			
		//HP/AP bar Button
		if(GUI.Button(new Rect((screen_size_x/2 - (screen_size_x/20)),((button_y_start*5) + (screen_size_y/35)), ((button_size_height *3) - (button_size_height/2)), (button_size_width/20)), "", hp_bar)) {
		}
		
		if(GUI.Button(new Rect((screen_size_x/2 - (screen_size_x/20)),((button_y_start*5) + (screen_size_y/10)), ((button_size_height *3)- (button_size_height/2)), (button_size_width/20)), "", ap_bar)) {
		}
		
		//HP/AP bar text
		GUI.Label(new Rect(screen_size_x/2, screen_size_y - screen_size_y/9, (t_size_width), (t_size_width)), mech.getCurrentHP() + "/" + mech.getMaxHP() + "HP",  gui_normal_text);
		GUI.Label(new Rect(screen_size_x/2, screen_size_y - screen_size_y/22, (t_size_width), (t_size_width)), mech.getCurrentAP() + "/" + mech.getMaxAP() + "AP",  gui_normal_text);
		
		 //Objective  Button
		 if(GUI.Button(new Rect((screen_size_x/2 - (screen_size_x/55)),(button_y_start/25), button_size_height, (button_size_width/5)), "", star_button_style)) {
			//pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.Objective;
		    script.enabled = true;
		 }
		 	
		//Base Button
		 if(GUI.Button(new Rect((button_x_start/10),((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "", base_button_style)) {
		    //pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.BaseUpgrade;
		    script.enabled = true;
		 }
		 
		 //Mech Button  
		 if(GUI.Button(new Rect((button_x_start/2),((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "", mech_button_style)) {
		    //pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.MechUpgrade;
		    script.enabled = true;
		 }
		 
		 //Health button
		 if(GUI.Button(new Rect(((button_size_width/5) + (button_x_start/2)),((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "",health_button_style)) {
		    //pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.HealthUpgrade;
		    script.enabled = true;
		 }
		 
		 //Transport button
		 if(GUI.Button(new Rect((((button_size_width/5)*2) + (button_x_start/2)), ((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "", transport_button_style)) {
		    //pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.TransportUpgrade;
		    script.enabled = true;
		 }	
	}
	
}
