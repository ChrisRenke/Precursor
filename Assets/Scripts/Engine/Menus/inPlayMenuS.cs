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
	
	//Custom styles for buttons
	public GUIStyle end_turn_button_style;
	public GUIStyle objective_button_style;
	public GUIStyle base_button_style;
	public GUIStyle health_button_style;
	public GUIStyle mech_button_style;
	public GUIStyle transport_button_style;
	public GUIStyle default_button_style_transport;
	
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

	void Update() {
		screen_size_x = Screen.width;
    	screen_size_y = Screen.height;
	}
	
	
	void OnGUI(){
		//Menu for upgrades
		UpgradeMenuS script =  GetComponent<UpgradeMenuS>();
		enableEnemyHealthBars();
		
		//All Backboard variables must be based on "screen_size_x" variable and "screen_size_y" variable	 
		int button_x_start = screen_size_x/8; 
		int button_y_start = screen_size_y/6; 
		int button_size_width = screen_size_x/4; 
		int button_size_height = screen_size_y/9; 
		int button_size = button_size_height/2; 
		 
		//Backboards
  		GUI.DrawTexture(new Rect((screen_size_x/3), ((button_y_start/4)-(button_y_start*10)), (button_size_height*6), (button_size_width*8)), top_menu_backboard, ScaleMode.ScaleToFit, true);
		GUI.DrawTexture(new Rect(0, (button_y_start/80), (button_size_height*4), (button_size_width*6)), upgrade_backboard, ScaleMode.ScaleToFit, true);
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
		int width_denominator = 18;
		int width_numerator = 5;
		int denominator_hp = width_denominator * mech.getMaxHP();
		int multiple_hp = denominator_hp/width_denominator;
		int numerator_hp = width_numerator * multiple_hp;
		int difference_hp = mech.getMaxHP() - numerator_hp;
		//ap bar variables
		int width_denominator_ap = 9;
		int width_numerator_ap = 5;
		int denominator_ap = width_denominator_ap * mech.getMaxAP();
		int multiple_ap = denominator_ap/width_denominator_ap;
		int numerator_ap = width_numerator_ap * multiple_ap;
		int difference_ap = mech.getMaxAP() - numerator_ap;
		
		//HP & AP Button
		if(GUI.Button(new Rect((screen_size_x/3),((button_y_start*5) + (screen_size_y/8)), (((numerator_hp - (mech.getMaxHP() - mech.getCurrentHP() - difference_hp)) * (screen_size_y * width_numerator))/denominator_hp), (screen_size_x/40)), mech.getCurrentHP() + "/" + mech.getMaxHP() + "HP", hp_bar)) {
		}
		
		if(GUI.Button(new Rect((screen_size_x/3),((button_y_start*5) + (screen_size_y/14)), (((numerator_ap - (mech.getMaxAP() - mech.getCurrentAP() - difference_ap)) * (screen_size_y * width_numerator_ap))/denominator_ap), (screen_size_x/40)), mech.getCurrentAP() + "/" + mech.getMaxAP() + "AP", ap_bar)) {
		}
		
		//HP and AP bar text
		GUI.Label(new Rect((screen_size_x/2- (screen_size_x/8)), screen_size_y - screen_size_y/22, (t_size_width), (t_size_width)), mech.getCurrentHP() + "/" + mech.getMaxHP(),  gui_normal_text);
		GUI.Label(new Rect((screen_size_x/2 - (screen_size_x/25)), screen_size_y - screen_size_y/10, (t_size_width), (t_size_width)), mech.getCurrentAP() + "/" + mech.getMaxAP(),  gui_normal_text);
	
		
		 //Objective  Button
		 if(GUI.Button(new Rect((((button_size_width/6)*3) + (button_x_start/3)), ((button_y_start*5) + (screen_size_y/11)),(button_size_height-(button_size_height/3)), (button_size_width/6)), "", objective_button_style)) {
			//disable enemy health bars
			disableEnemyHealthBars();
			//pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.Objective;
		    script.enabled = true;
		 }
		 	
		 //Mech Button  
		 if(GUI.Button(new Rect((button_x_start/10),((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "", mech_button_style)) {
		    //disable enemy health bars
			disableEnemyHealthBars();
			//pause the game
			Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.MechUpgrade1;
		    script.enabled = true;
		}
		 
		 //Health button
		 if(GUI.Button(new Rect((button_x_start/2),((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "",health_button_style)) {
		    //disable enemy health bars
			disableEnemyHealthBars();
			//pause the game
		    Time.timeScale = 0;
		    //show the pause menu
			script.menu_choice = Menu.HealthUpgrade;
		    script.enabled = true;
		 }
		
		//Base Button
		if(!entityMechS.moving_on_path){
			entityBaseS script_base =  entityManagerS.getBase();
			if(true || script_base.mechNextToBase(mech.x,mech.z)){
				 if(GUI.Button(new Rect(((button_size_width/5) + (button_x_start/2)),((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "", base_button_style)) {
				    //disable enemy health bars
					disableEnemyHealthBars();
					//pause the game
				    Time.timeScale = 0;
				    //show the pause menu
					script.menu_choice = Menu.BaseUpgrade1;
				    script.enabled = true;
				 }
			}else{
				//"Can't upgrade, you are not by the base"
			}
		}
		 
		//Transport button
		if(enable_transport_button){
			 if(GUI.Button(new Rect((((button_size_width/5)*2) + (button_x_start/2)), ((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "", transport_button_style)) {
			    if(mech.applyUpgrade(ap_cost_transport)){
					mech.applyAPCost(ap_cost_transport);
					//TODO: add code for transporting mech
				}else{
					print ("not enough ap");
				}
			 }	
		}
		else{
			GUI.Button(new Rect((((button_size_width/5)*2) + (button_x_start/2)), ((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "", default_button_style_transport);
		}
		
		//Objective Button
		 if(GUI.Button(new Rect((((button_size_width/5)*3) + (button_x_start/2)), ((button_y_start*5) + (screen_size_y/11)), ((button_size_height)-(button_size_height/5)), (button_size_width/5)), "", objective_button_style)) {
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
