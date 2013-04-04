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
	
	//Menu backboards
	//public Texture hp_backboard;
	public Texture parts_backboard;
	public Texture top_menu_backboard;
	public Texture upgrade_backboard;
	public Texture default_button_backboard;
	public GUIStyle hp_backboard;
	
	public Texture chris_parts;
	public Texture chris_top;
	public Texture chris_ap_bg;
	public Texture chris_ap;
	public Texture chris_hp_town_bg;
	public Texture chris_hp_town;
	public Texture chris_hp_mech_bg;
	public Texture chris_hp_mech; 
	
	//Custom styles for buttons
	public GUIStyle end_turn_button_style;
	public GUIStyle objective_button_style;
	public GUIStyle base_button_style;
	public GUIStyle health_button_style;
	public GUIStyle mech_button_style;
	public GUIStyle transport_button_style;
	public GUIStyle default_button_style_transport;
	
	public static popUpMenu popup; 
	
	//transport upgrade cost
	int ap_cost_transport = 4;
	
	//Window size
	private int screen_size_x;
	private int screen_size_y;
	
	//Check for whether transport button is enabled
	public bool enable_transport_button = false;
	
	private static entityMechS mech;
	
	public static void setMech()
	{
		mech    = GameObject.FindGameObjectWithTag("player_mech").GetComponent<entityMechS>();
	}
 
	
	void Awake()
	{
		popup =  GetComponent<popUpMenu>();
	}
	void OnGUI(){
		//Menu for upgrades
		UpgradeMenuS script =  GetComponent<UpgradeMenuS>();
		enableEnemyHealthBars();
		screen_size_x = Screen.width;
    	screen_size_y = Screen.height; 
		
		GUI.DrawTexture(new Rect(415, screen_size_y - 78, 450,36 ), chris_ap_bg, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(415, screen_size_y - 36, 218, 36 ), chris_hp_mech_bg, ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(647, screen_size_y - 36, 218, 36 ), chris_hp_town_bg, ScaleMode.StretchToFill);
		
		GUI.DrawTexture(new Rect(424, 0, 432, 42), chris_top, ScaleMode.StretchToFill);
		
		GUI.DrawTexture(new Rect(screen_size_x - 245, screen_size_y - 104, 245, 104 ), chris_parts, ScaleMode.StretchToFill); 
		 
		GUI.BeginGroup (new Rect (420, screen_size_y - 75, (int)440*(float)entityManagerS.getMech().current_ap/(float)entityManagerS.getMech().max_ap,24));
		GUI.DrawTexture(new Rect (0, 0, 440, 24), chris_ap, ScaleMode.StretchToFill);
		GUI.EndGroup (); 
	 	GUI.Label(new Rect(420, screen_size_y - 75, 440, 24), entityManagerS.getMech().current_ap + "/" + entityManagerS.getMech().max_ap,  enginePlayerS.gui_norm_text_static);
		
		GUI.BeginGroup (new Rect (420, screen_size_y - 33, (int)210*(float)entityManagerS.getMech().current_hp/(float)entityManagerS.getMech().max_hp, 24));
		GUI.DrawTexture(new Rect (0,0, 210, 24), chris_hp_mech, ScaleMode.StretchToFill);
		GUI.EndGroup (); 
	 	GUI.Label(new Rect(420, screen_size_y - 33, 210, 24), entityManagerS.getMech().current_hp + "/" + entityManagerS.getMech().max_hp,  enginePlayerS.gui_norm_text_static);
		
		GUI.BeginGroup (new Rect (650, screen_size_y - 33, (int)210*(float)entityManagerS.getBase().current_hp/(float)entityManagerS.getBase().max_hp,24));
		GUI.DrawTexture(new Rect (0, 0, 210, 24), chris_hp_town, ScaleMode.StretchToFill);
		GUI.EndGroup (); 
	 	GUI.Label(new Rect(650, screen_size_y - 33, 210, 24), entityManagerS.getBase().current_hp + "/" + entityManagerS.getBase().max_hp,  enginePlayerS.gui_norm_text_static);
		
	 	GUI.Label(new Rect(432, 0, 151, 27), gameManagerS.current_turn.ToString() + " Turn",  enginePlayerS.gui_norm_text_static);
		GUI.Label(new Rect(698, 0, 151, 27), "Round " + gameManagerS.current_round.ToString() , enginePlayerS.gui_norm_text_static); //+ current_round.ToString() + ""
	
	
	 	GUI.Label(new Rect(screen_size_x - 233, screen_size_y - 52, 43, 43 ), entityMechS.part_count[Part.Piston].ToString(), enginePlayerS.gui_norm_text_black_static);
	 	GUI.Label(new Rect(screen_size_x - 173, screen_size_y - 52, 43, 43 ), entityMechS.part_count[Part.Gear].ToString(),   enginePlayerS.gui_norm_text_black_static);
	 	GUI.Label(new Rect(screen_size_x - 112, screen_size_y - 52, 43, 43 ), entityMechS.part_count[Part.Plate].ToString(),  enginePlayerS.gui_norm_text_black_static);
	 	GUI.Label(new Rect(screen_size_x - 52 , screen_size_y - 52, 43, 43 ), entityMechS.part_count[Part.Strut].ToString(),  enginePlayerS.gui_norm_text_black_static);
//		GUI.DrawTexture(new Rect(screen_size_x - 245, screen_size_y - 104, 245, 104 ), chris_parts, ScaleMode.StretchToFill); 
		 
		//End Turn Button
		if(GUI.Button(new Rect(582, 0, 116, 46), "", end_turn_button_style)) {
			gameManagerS.endPlayerTurn();
		}
		   
		
		 //Mech Button  
		 if(GUI.Button(new Rect(160, screen_size_y - 54, 50, 50), "", mech_button_style)) {
		    
			if(gameManagerS.current_level != Level.Level0){
				//disable enemy health bars
				disableEnemyHealthBars();
				//pause the game
				Time.timeScale = 0;
			    //show the pause menu
				script.menu_choice = Menu.MechUpgrade1;
			    script.enabled = true;
			}else{
					//get popup menu
				
				popup.custom_rect = new Rect(screen_size_x /2 - screen_size_x /9 - screen_size_x /25, screen_size_y/2 - screen_size_y/10, screen_size_x - (screen_size_x /2 + screen_size_x /5), screen_size_y/4);
				popup.custom_text = "This menu looks much too cool for now.\nYou decide you'll check it out later"; //not enough ap	
				popup.custom_popup = true;
			}
		}
		 
		 //Health button
		 if(GUI.Button(new Rect(108, screen_size_y - 54, 50, 50), "",health_button_style)) {
		    //disable enemy health bars
			disableEnemyHealthBars();
			//pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.HealthUpgrade;
		    script.enabled = true;
		 }
		
		//Base Button
		entityBaseS script_base =  entityManagerS.getBase();
		if(GUI.Button(new Rect(212, screen_size_y - 54, 50, 50), "", base_button_style)) {  
			if(script_base.mechNextToBase(mech.x,mech.z)){
			 	//disable enemy health bars
				disableEnemyHealthBars();
				//pause the game
			    Time.timeScale = 0;
			    //show the pause menu
				script.menu_choice = Menu.BaseUpgrade1;
			    script.enabled = true;
				}else{
					//get popup menu
				
				popup.custom_rect = new Rect(screen_size_x /2 - screen_size_x /9 - screen_size_x /25, screen_size_y/2 - screen_size_y/10, screen_size_x - (screen_size_x /2 + screen_size_x /5), screen_size_y/4);
				popup.custom_text = "You can't upgrade the base, \n you are too far away"; //not enough ap	
				popup.custom_popup = true;
			}
		}
		 
		//Transport button
		if(GUI.Button(new Rect(56, screen_size_y - 54, 50, 50), "", transport_button_style)) {
			popUpMenu script_popup =  popup;
			if(enable_transport_button){
			 	if(mech.applyUpgrade(ap_cost_transport)){
					mech.applyAPCost(ap_cost_transport);
					//TODO: add code for transporting mech
				}else{
					script_popup.custom_rect = new Rect(screen_size_x /2 - screen_size_x /9 - screen_size_x /25, screen_size_y/2 - screen_size_y/10, screen_size_x - (screen_size_x /2 + screen_size_x /5), screen_size_y/4);
					script_popup.custom_text = "You don't have enough ap /n to transport"; //not enough ap	
					script_popup.custom_popup = true;
				}
			 }else{
				
					script_popup.custom_rect = new Rect(screen_size_x /2 - screen_size_x /9 - screen_size_x /25, screen_size_y/2 - screen_size_y/10, screen_size_x - (screen_size_x /2 + screen_size_x /5), screen_size_y/4);
					script_popup.custom_text = "You haven't gotten this upgrade yet"; //not enough ap	
					script_popup.custom_popup = true;
			}
		}
		
		//Objective Button
	 	if(GUI.Button(new Rect(4, screen_size_y - 54, 50,50), "", objective_button_style)) {
			//disable enemy health bars
			disableEnemyHealthBars();
			//pause the game
	    	Time.timeScale = 0;
	    	//show the pause menu
			script.menu_choice = Menu.Objective;
	    	script.enabled = true;
	 	}
	}
	
	private void disableEnemyHealthBars(){
		entityEnemyS.show_health_bar = false;	
		entityBaseS.show_health_bar  = false;	
	}
	
	private void enableEnemyHealthBars(){
		entityEnemyS.show_health_bar = true;
		entityBaseS.show_health_bar  = true;
	}
	
}
