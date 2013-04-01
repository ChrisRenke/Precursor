using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Display In Play Menu Screens
public class inPlayMenuS : MonoBehaviour {
	//text style/color, set in inspector	
	public GUIStyle gui_normal_text;
	public GUIStyle parts_text;
	
	//hp and ap bars
	public GUIStyle ap_bar;
	public GUIStyle hp_bar;
	public GUIStyle base_bar;
	
	//Menu backboards
	public Texture hp_backboard;
	public Texture parts_backboard;
	public Texture top_menu_backboard;
	public Texture upgrade_backboard;	
	
	//Custom styles for buttons
	public GUIStyle end_turn_button_style;
	public GUIStyle star_button_style;
	public GUIStyle base_button_style;
	public GUIStyle health_button_style;
	public GUIStyle mech_button_style;
	public GUIStyle transport_button_style;
	public GUIStyle default_button_style;
	
	//Window size
	private int screen_size_x;
	private int screen_size_y;
	
	//Check for whether tranport button is enabled
	bool enabled = false;
	
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
		//Menu for upgrades
		UpgradeMenuS script =  GetComponent<UpgradeMenuS>();
		
		//All Backboard variables must be based on "screen_size_x" variable and "screen_size_y" variable	 
		int button_x_start = screen_size_x/8; 
		int button_y_start = screen_size_y/6; 
		int button_size_width = screen_size_x/4; 
		int button_size_height = screen_size_y/9; 
		int button_size = button_size_height/2; 
		 
		//Backboards
  		GUI.DrawTexture(new Rect((screen_size_x/3), ((button_y_start/4)-(button_y_start*13)-(button_y_start/2)), (button_size_height*5), (button_size_width*10)), top_menu_backboard, ScaleMode.ScaleToFit, true);
		//GUI.DrawTexture(new Rect(0, (button_y_start/8), (button_size_height *4), (button_size_width*4)), upgrade_backboard, ScaleMode.ScaleToFit, true);
		GUI.DrawTexture(new Rect(((button_x_start *6) + screen_size_x/15), (screen_size_y/4), (button_size_height *3), (button_size_width*3)), parts_backboard, ScaleMode.ScaleToFit, true);
		//GUI.DrawTexture(new Rect((button_x_start*3), ((screen_size_y/2) + (screen_size_y/4)), (button_size_height *5), (screen_size_x/7)), hp_backboard, ScaleMode.ScaleToFit, true);
		
		//End Turn Button
		if(GUI.Button(new Rect((screen_size_x/2 - (screen_size_x/13)),0, (button_size_height *2), (button_size_width/7)), "", end_turn_button_style)) {
			gameManagerS.endPlayerTurn();
		}
		
		
		//Part Count Text
		int t_x_start = screen_size_x - (screen_size_x/6 + screen_size_x/115); 
		int t_y_end = screen_size_y - screen_size_y/20; 
		int t_size_width = 80; //text size
		int t_size_height = 20; //text size
		int t_spacing = screen_size_x/7 - (screen_size_x/6 + screen_size_x/45);
		
		GUI.Label(new Rect(t_x_start - (t_spacing * 3), t_y_end, t_size_width, t_size_height), entityMechS.part_count[Part.Strut].ToString(),   parts_text);
		GUI.Label(new Rect(t_x_start - (t_spacing * 2), t_y_end, t_size_width, t_size_height), entityMechS.part_count[Part.Plate].ToString(), parts_text);
		GUI.Label(new Rect(t_x_start - t_spacing, t_y_end, t_size_width, t_size_height), entityMechS.part_count[Part.Gear].ToString(),  parts_text);
		GUI.Label(new Rect(t_x_start, t_y_end, t_size_width, t_size_height), entityMechS.part_count[Part.Piston].ToString(),  parts_text);
			
		//Change size of the ap and hp bars based on bar width = (5 * screen_size_y/ 18)
		//hp bar variables
		int denominator_hp = 18 * mech.getMaxHP();
		int multiple_hp = denominator_hp/18;
		int numerator_hp = 5 * multiple_hp;
		int difference_hp = mech.getMaxHP() - numerator_hp;
		//ap bar variables
		int denominator_ap = 18 * mech.getMaxAP();
		int multiple_ap = denominator_ap/18;
		int numerator_ap = 5 * multiple_ap;
		int difference_ap = mech.getMaxAP() - numerator_ap;
		
		//HP & AP Button
		if(GUI.Button(new Rect((screen_size_x/3),((button_y_start*5) + (screen_size_y/8)), (((numerator_hp - (mech.getMaxHP() - mech.getCurrentHP() - difference_hp)) * (screen_size_y * 5))/denominator_hp), (screen_size_x/40)), "", hp_bar)) {
		}
		
		if(GUI.Button(new Rect((screen_size_x/3),((button_y_start*5) + (screen_size_y/14)), (((numerator_ap - (mech.getMaxAP() - mech.getCurrentAP() - difference_ap)) * (screen_size_y * 10))/denominator_ap), (screen_size_x/40)), "", ap_bar)) {
		}
		
		//Base bar NEED TO REPLACE NUMBERS!! 
		if(GUI.Button(new Rect(((screen_size_x/2)),((button_y_start*5) + (screen_size_y/8)), (((numerator_ap - (mech.getMaxAP() - mech.getCurrentAP() - difference_ap)) * (screen_size_y * 5))/denominator_ap), (screen_size_x/40)), "", base_bar)) {
		}
		
		//HP and AP bar text
		GUI.Label(new Rect((screen_size_x/2- (screen_size_x/8)), screen_size_y - screen_size_y/22, (t_size_width), (t_size_width)), mech.getCurrentHP() + "/" + mech.getMaxHP(),  gui_normal_text);
		GUI.Label(new Rect((screen_size_x/2 - (screen_size_x/25)), screen_size_y - screen_size_y/10, (t_size_width), (t_size_width)), mech.getCurrentAP() + "/" + mech.getMaxAP(),  gui_normal_text);
		GUI.Label(new Rect((screen_size_x/2 +(screen_size_x/25)), screen_size_y - screen_size_y/22, (t_size_width), (t_size_width)), mech.getCurrentAP() + "/" + mech.getMaxAP() + "B",  gui_normal_text);

		
		 //Objective  Button
		 if(GUI.Button(new Rect((((button_size_width/6)*3) + (button_x_start/3)), ((button_y_start*5) + (screen_size_y/11)),(button_size_height-(button_size_height/3)), (button_size_width/6)), "", star_button_style)) {
			//pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.Objective;
		    script.enabled = true;
		 }
		 	
		//Base Button
		 if(GUI.Button(new Rect(0,((button_y_start*5) + (screen_size_y/11)), (button_size_height-(button_size_height/3)), (button_size_width/6)), "", base_button_style)) {
		    //pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.BaseUpgrade1;
		    script.enabled = true;
		 }
		 
		 //Mech Button  
		 if(GUI.Button(new Rect(((button_x_start/2)-(button_size_width/12)),((button_y_start*5) + (screen_size_y/11)), (button_size_height-(button_size_height/3)), (button_size_width/6)), "", mech_button_style)) {
		    //pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.MechUpgrade1;
		    script.enabled = true;
		 }
		 
		 //Health button
		 if(GUI.Button(new Rect(((button_size_width/6) + (button_x_start/3)),((button_y_start*5) + (screen_size_y/11)), (button_size_height-(button_size_height/3)), (button_size_width/6)), "",health_button_style)) {
		    //pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.HealthUpgrade;
		    script.enabled = true;
		 }
		 
		//TODO: add check for whether transport button should be enabled, currently set as disabled
		//Transport button
		if(enabled){
		
			 if(GUI.Button(new Rect((((button_size_width/6)*2) + (button_x_start/3)), ((button_y_start*5) + (screen_size_y/11)),(button_size_height-(button_size_height/3)), (button_size_width/6)), "", transport_button_style)) {
			    //TODO: add code for transporting mech
			 }	
			
		}else{
			GUI.Button(new Rect((((button_size_width/6)*2) + (button_x_start/3)), ((button_y_start*5) + (screen_size_y/11)), (button_size_height-(button_size_height/3)), (button_size_width/6)), "", default_button_style);
		}
	}
	
}
