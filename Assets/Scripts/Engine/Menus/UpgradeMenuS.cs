using UnityEngine;
using System.Collections;


public class UpgradeMenuS : MonoBehaviour {
	//Menu Skin
	public GUISkin newSkin;
	public GUISkin healthMenuSkin;
	public GUISkin objectiveMenuSkin;

	//Styles for text
	public GUIStyle text_style_parts_not_in_inventory;
	public GUIStyle text_style_parts_have_in_inventory;
	public GUIStyle text_style_grey;
	public GUIStyle title_style;

	//Styles for buttons
	//mech mobile styles
	public GUIStyle fin_upgrade_style;
	public GUIStyle moutain_upgrade_style;
	public GUIStyle leg_upgrade_style;
	//mech gun styles
	public GUIStyle gun_upgrade_style;
	public GUIStyle gun_upgrade_style2;
	public GUIStyle gun_upgrade_style3;
	//mech other styles
	public GUIStyle goggles_upgrade_style;
	public GUIStyle teleport_upgrade_style;
	public GUIStyle other_upgrade_style;
	//base wall styles
	public GUIStyle wall_upgrade_style;
	public GUIStyle wall_upgrade_style2;
	public GUIStyle wall_upgrade_style3;
	//base structure styles
	public GUIStyle structure_upgrade_style;
	public GUIStyle structure_upgrade_style2;
	public GUIStyle structure_upgrade_style3;
	//base weapon styles (defense)
	public GUIStyle weapon_upgrade_style;
	public GUIStyle weapon_upgrade_style2;
	public GUIStyle weapon_upgrade_style3;
	//part buttons
	public GUIStyle piston_button_style;
	public GUIStyle gear_button_style;
	public GUIStyle plate_button_style;
	public GUIStyle strut_button_style;
	//check mark button
	public GUIStyle check_mark_style;
	//default button style, blank backboard and blackout button style
	public GUIStyle default_button_style;
	public GUIStyle left_style;
	public GUIStyle right_style;
	public GUIStyle button_blackout_style;
	
	//hp bars
	public GUIStyle hp_bar;

	//Window size
	private int screen_size_x;
	private int screen_size_y;

	//Pick a menu
	public Menu menu_choice = Menu.Default;

	//Part count for upgrades: Column 0 = Piston / Column 1 = Gear / Column 2 = Plate / Column 3 = Strut
	//Mech Upgrades part array requirements
	private int[,] parts_count_for_mobile_upgrades = new int[3,4] 	{ {1,0,4,3}, {3,4,0,1}, {3,2,0,3} }; //Row 0 = water upgrade, Row 1 = mountan upgrade, Row 2 = leg upgrade
	private int[,] parts_count_for_gun_upgrades = new int[3,4] 		{ {1,2,3,2}, {2,1,2,3}, {1,3,2,1} }; //Row 0 = gun upgrade range , Row 1 = gun upgrade damage, Row 2 = gun upgrade cost
	private int[,] parts_count_for_other_upgrades = new int[3,4] 	{ {1,0,5,2}, {2,6,2,3}, {2,2,2,2} }; //Row 0 =  armour upgrade 1, Row 1 = armour upgrade 2 (teleport base upgrade?), Row 2 = armour upgrade 3 (other upgrade?)
	//Base Upgrades part array requirements
	private int[,] parts_count_for_wall_upgrades = new int[3,4] 	{ {0,3,4,3}, {0,4,5,4}, {0,5,6,5} }; //Row 0 = wall upgrade 1, Row 1 = wall upgrade 2, Row 2 = wall upgrade 3
	private int[,] parts_count_for_struc_upgrades = new int[3,4] 	{ {2,3,1,1}, {3,2,2,2}, {4,4,4,4} }; //Row 0 = structure upgrade 1, Row 1 = structure upgrade 2, Row 2 = structure upgrade 3
	private int[,] parts_count_for_weapon_upgrades = new int[3,4] 	{ {4,1,2,3}, {4,0,4,2}, {4,1,4,3} }; //Row 0 = defense upgrade 1, Row 1 = defense upgrade 2, Row 2 = defense upgrade 3
	
	private bool [,] show_check_boxes_mech_menu = new bool[3,3] {{false, false, false}, {false, false, false}, {false, false, false}}; //Row 0 = mobile_upgrades/Row 2 = gun_upgrades/ Row 3 = other_upgrades and each column corresponds to 3 buttons on screen menu
	private bool [,] show_check_boxes_base_menu = new bool[3,3] {{false, false, false}, {false, false, false}, {false, false, false}}; //Row 0 = wall upgrades/ row 2 = struc upgrades/ row 3 = weapon upgrades
	
	//Hold the descriptions for each button
	string description_button_one = "";
	string description_button_two = "";
	string description_button_three = "";
	
	//AP cost for base upgrade/mech upgrades/health upgrades/transport upgrade
	int ap_cost_base = 3;
	int ap_cost_mech = 3;
	int ap_cost_health = 1;
	
	private static entityMechS mech;

	public static void setMech()
	{
		mech    = GameObject.FindGameObjectWithTag("player_mech").GetComponent<entityMechS>();
	}

	//Update is called once per frame
	void Update () {
		screen_size_x = Screen.width;
    	screen_size_y = Screen.height;
		//Disable other UpdateScript so buttons don't show up
		inPlayMenuS script =  GetComponent<inPlayMenuS>();
    	script.enabled = false;
	}


	private void theUpgradeMenu(GUIStyle button_one_style, GUIStyle button_two_style, GUIStyle button_three_style) {
		GUI.skin = newSkin;
		//get popup menu
		popUpMenu script_popup =  GetComponent<popUpMenu>();
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 

	    //Layout start
	    GUI.BeginGroup(new Rect(0, 0, screen_size_x, screen_size_y));

	    //The Menu background box
	    GUI.Box(new Rect(screen_size_x /4 - screen_size_x /16, screen_size_y /14, window_size_width, window_size_height), "");

	    //Game resume button (close upgrade menu)
	    if(GUI.Button(new Rect(window_size_x + window_size_width - window_size_width/12, window_size_y + window_size_height/20, window_size_width/20, window_size_height/20), "X", default_button_style)) {
		    //resume the game
		    Time.timeScale = 1;
		    inPlayMenuS script =  GetComponent<inPlayMenuS>();
    		script.enabled = true;
		    //disable pause menu
		    UpgradeMenuS script2 =  GetComponent<UpgradeMenuS>();
    		script2.enabled = false;
	    }
		
		//continue button variables
		int continue_button_x_right = window_size_x + window_size_width - window_size_width/20;
		int continue_button_x_left = window_size_x - window_size_width/8;
		int continue_button_y = window_size_y + window_size_height/2;
		int continue_button_width = window_size_width/6;
		int continue_button_height = window_size_height/8;
		int label_window_size_width  = window_size_width; 
	    int label_window_size_height  = window_size_width; 
		
		//Var for base upgrade
		bool base_upgrade_occured = false;
		
		//Continue Buttons and part count labels
		switch(menu_choice){
			case Menu.BaseUpgrade1:
				//Next button
				if(GUI.Button(new Rect(continue_button_x_right, continue_button_y, continue_button_width, continue_button_height), "" , right_style)) {
					menu_choice = Menu.BaseUpgrade2;
	    		}
				
				//draw part labels
				part_labels(label_window_size_width, label_window_size_height, ref parts_count_for_wall_upgrades);	
			
				break;
	
			case Menu.BaseUpgrade2:
			    //Next button
				if(GUI.Button(new Rect(continue_button_x_right, continue_button_y, continue_button_width, continue_button_height), "" ,right_style)) {
					menu_choice = Menu.BaseUpgrade3;
	    		}
				//Back button
				if(GUI.Button(new Rect(continue_button_x_left, continue_button_y, continue_button_width, continue_button_height), "" ,left_style)) {
					menu_choice = Menu.BaseUpgrade1;
	    		}
			
				//draw part labels
				part_labels(label_window_size_width, label_window_size_height,  ref parts_count_for_struc_upgrades);	
			
				break;
			
			case Menu.BaseUpgrade3:
			    //Back button
				if(GUI.Button(new Rect(continue_button_x_left, continue_button_y, continue_button_width, continue_button_height), "" ,left_style)) {
					menu_choice = Menu.BaseUpgrade2;
	    		}
				
				//draw part labels
				part_labels(label_window_size_width, label_window_size_height, ref parts_count_for_weapon_upgrades);	
			
				break;
			
			case Menu.MechUpgrade1:
				//Next button
				if(GUI.Button(new Rect(continue_button_x_right, continue_button_y, continue_button_width, continue_button_height), "" ,right_style)) {
					menu_choice = Menu.MechUpgrade2;
	    		}
			
				//draw part labels
				part_labels(label_window_size_width, label_window_size_height,  ref parts_count_for_mobile_upgrades);	
			
				break;
			
			case Menu.MechUpgrade2:
				//Next button
				if(GUI.Button(new Rect(continue_button_x_right, continue_button_y, continue_button_width, continue_button_height), "" ,right_style)) {
					menu_choice = Menu.MechUpgrade3;
	    		}
				//Back button
				if(GUI.Button(new Rect(continue_button_x_left, continue_button_y, continue_button_width, continue_button_height), "" ,left_style)) {
					menu_choice = Menu.MechUpgrade1;
	    		}
				
				//draw part labels
				part_labels(label_window_size_width, label_window_size_height, ref parts_count_for_gun_upgrades);	
			
			
				break;
			
			case Menu.MechUpgrade3:
				//Back button
				if(GUI.Button(new Rect(continue_button_x_left, continue_button_y, continue_button_width, continue_button_height), "" ,left_style)) {
					menu_choice = Menu.MechUpgrade2;
	    		}
			
				//draw part labels
				part_labels(label_window_size_width, label_window_size_height, ref parts_count_for_other_upgrades);	
			
				break;
			default:
				//not on the correct menu
				break;
		}

		
		//set button descriptions
		getDescriptions();
		
	    //Upgrade Menu buttons
	    int button_x_start  = window_size_x + window_size_width/9 + window_size_x/26; 
	    int button_y_start  = window_size_y + window_size_height/4 + window_size_height/35; 
		int button_size_width = window_size_width/7; 
	    int button_size_height = window_size_height/6; 
	    int button_spacing = button_size_height/4 - button_size_height/40; 
		int label_x_start  = window_size_x + window_size_width/3 + window_size_width/13; 
		int label_width  = button_size_width + button_size_width/2 - button_size_width/20;
		
	    //upgrade button 0
	    if(GUI.Button(new Rect(button_x_start, button_y_start, button_size_width, button_size_height), "" ,button_one_style)) {
			if(((menu_choice == Menu.BaseUpgrade1 || menu_choice == Menu.BaseUpgrade2 || menu_choice == Menu.BaseUpgrade3) && mech.applyUpgrade(ap_cost_base)) 
				|| ((menu_choice == Menu.MechUpgrade1 || menu_choice == Menu.MechUpgrade2 || menu_choice == Menu.MechUpgrade3) && mech.applyUpgrade(ap_cost_mech))){
				canApplyUpgrade(0);
			}else{
				script_popup.text_number_choice = 0; //not enough ap
				script_popup.upgrade_popup = true;
			}
	    }
		//Description of button
	    GUI.Label(new Rect(label_x_start, button_y_start, label_width, button_size_height), description_button_one);

	    //upgrade button 1
		if(showButton(1)){
			if(GUI.Button(new Rect(button_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "", button_two_style)) {
	    		if(((menu_choice == Menu.BaseUpgrade1 || menu_choice == Menu.BaseUpgrade2 || menu_choice == Menu.BaseUpgrade3) && mech.applyUpgrade(ap_cost_base)) 
				|| ((menu_choice == Menu.MechUpgrade1 || menu_choice == Menu.MechUpgrade2 || menu_choice == Menu.MechUpgrade3) && mech.applyUpgrade(ap_cost_mech))){
					canApplyUpgrade(1);
				}else{
					script_popup.text_number_choice = 0; //not enough ap	
					script_popup.upgrade_popup = true;
				}
	    	}
			//Description of button
			GUI.Label(new Rect(label_x_start, button_y_start + (button_spacing) + (button_size_height), label_width, button_size_height), description_button_two);
	    	
		}else{
			//Show blackout button
			GUI.Button(new Rect(button_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "", button_blackout_style);
		}

	    //upgrade button 2
		if(showButton(2)){
	    	if(GUI.Button(new Rect(button_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "", button_three_style)) {
	    		if(((menu_choice == Menu.BaseUpgrade1 || menu_choice == Menu.BaseUpgrade2 || menu_choice == Menu.BaseUpgrade3) && mech.applyUpgrade(ap_cost_base)) 
				|| ((menu_choice == Menu.MechUpgrade1 || menu_choice == Menu.MechUpgrade2 || menu_choice == Menu.MechUpgrade3) && mech.applyUpgrade(ap_cost_mech))){
					canApplyUpgrade(2);
				}else{
					script_popup.text_number_choice = 0; //not enough ap
					script_popup.upgrade_popup = true;
				}
	    	}
			//Description of button
	    	GUI.Label(new Rect(label_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), label_width, button_size_height), description_button_three);
		}else{
			//Show blackout button
			GUI.Button(new Rect(button_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "", button_blackout_style);
	    }
		
		showCheckBoxes(button_x_start + button_x_start/4, button_y_start, button_size_width/2, button_size_height, button_spacing);

	    //layout end
	    GUI.EndGroup();

	}
	
	//Get descriptions for buttons based on menu choice
	private void getDescriptions(){

switch(menu_choice){

	case Menu.BaseUpgrade1:
		description_button_one = "Scrap Wall\n\nReduces effeciveness of enemy attacks.\n\n+1 armor";
		description_button_two = "Reinforced Barracade\n\nFurther decreased enemy attack penetration.\n\n+1 armor";
		description_button_three =  "Security Bunker\n\nSeriously decreased enemy attack usefulness.\n\n+1 armor";
	break;
	
	 
	
	case Menu.BaseUpgrade2:
	    description_button_one = "City Planning\n\nGrow the town infrastructure.\n\n+10 HP";
		description_button_two = "Vertical Spires\n\nMakin' room for more folks.\n\n+15 HP"; 
		description_button_three = "Steampunk Arcology\n\nNo need to ever leave!\n\n+20 HP";
	break;
	
	 
	
	case Menu.BaseUpgrade3:
	    description_button_one = "Enhanced Rifling\nIncreases range of cannons.\n+1 attack range";
		description_button_two = "Howitzer Emplacements\nIncreases damage of cannons.\n+2 attack damage";
		description_button_three = "Ubiquitous Annihilator\nIt hurts. A lot.\n+1 attack range & damage";
	break;
	
	 
	
	case Menu.MechUpgrade1:
		description_button_one = "Aquatic Fins\n\nFunctional and stylish.\n\nAllows water traversal";
		description_button_two = "Climbing Claws\n\nMakes molehills from mountains.\n\nAllows mountain traversal";
		description_button_three = "Reengineered Legs\n\nStrong, better, faster.\n\n-1 AP traversal cost";
	break;
	
	 
	
	case Menu.MechUpgrade2:
	
		description_button_one = "Magnum Barrel\n\nBigger bullets too.\n\n+2 damage"; 
		description_button_two = "Firing Optics\n\nA steampunk scope.\n\n+1 attack range"; 
		description_button_three = "Internal Revolver\n\nThe inside are what count.\n\n-1AP attack cost";
	
	break;
	
	 
	
	case Menu.MechUpgrade3:
		
		description_button_one = "Brass Armor\n\nDefense on the go!\n\n+2 armor"; 
		description_button_two = "Mobile Recall\n\nTeleport to town.\n\n14 AP warp to town"; 
		description_button_three = "Persistant Scavenger\n\nOne mans trash...\n\n+2 resources per scavenge";
	
	break;
	
	 
	
	default:
	
	description_button_one = "?";
	
	description_button_two = "?";
	
	description_button_three = "?";
	
	break;
	
	}

}

	
	//Show base buttons based on what levels of upgrades have been unlocked for base menu upgrades
	private bool showButton(int button_number){
		if(menu_choice == Menu.MechUpgrade1 || menu_choice == Menu.MechUpgrade2 || menu_choice == Menu.MechUpgrade3){
			return true;
		}else{
			if(menu_choice == Menu.BaseUpgrade1){
				if((button_number == 1 && show_check_boxes_base_menu[0,0] == true) || (button_number == 2 && show_check_boxes_base_menu[0,1] == true)){
					return true;
				}else{
					return false;
				}
			}else if(menu_choice == Menu.BaseUpgrade2){
				if((button_number == 1 && show_check_boxes_base_menu[1,0] == true) || (button_number == 2 && show_check_boxes_base_menu[1,1] == true)){
					return true;
				}else{
					return false;
				}
			}else{
				if((button_number == 1 && show_check_boxes_base_menu[2,0] == true) || (button_number == 2 && show_check_boxes_base_menu[2,1] == true)){
					return true;
				}else{
					return false;
				}
			}
		}
		
	}
	
	//if upgrade has occured show the check box for that button if upgrade was taken
	private void showCheckBoxes(int button_x_start, int button_y_start, int button_size_width, int button_size_height, int button_spacing){
		bool use_base_check_boxes = false;
		bool[,] check_box_array;
		int index = 0;
		
		switch(menu_choice){
			case Menu.BaseUpgrade1:
				use_base_check_boxes = true;
				index = 0;
				break;
			case Menu.BaseUpgrade2:
				use_base_check_boxes = true;
				index = 1;
				break;
			case Menu.BaseUpgrade3:
				use_base_check_boxes = true;
				index = 2;
				break;
			case Menu.MechUpgrade1:
				index = 0;				
				break;
			case Menu.MechUpgrade2:
				index = 1;				
				break;
			case Menu.MechUpgrade3:
				index = 2;				
				break;
			default:
				break;
		}
		
		if(use_base_check_boxes){
			check_box_array = show_check_boxes_base_menu;
		}else{
			check_box_array = show_check_boxes_mech_menu;
		}
		
		button_x_start = button_x_start + button_size_width;
		button_y_start = button_y_start + button_size_height/6;
		//base menu 
		if(check_box_array[index,0] == true){
			//show check box for upgrade button 
			GUI.Button(new Rect(button_x_start, button_y_start, button_size_width, button_size_height/2 + button_size_height/4), "" ,check_mark_style);
		}
		
		if(check_box_array[index,1] == true){
			//show check box for upgrade button
			GUI.Button(new Rect(button_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height/2 + button_size_height/4), "", check_mark_style);
		}
		
		if(check_box_array[index,2] == true){
			//show check box for upgrade button
			GUI.Button(new Rect(button_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height/2 + button_size_height/4), "", check_mark_style);	
		}
		
	}
	
	//draw part count for each button
	private void part_labels (int window_size_width, int window_size_height, ref int[,] parts_array){
		//part count variables
		int part_x_start  = window_size_width - (window_size_width/12); 
		int part_x_adjust = part_x_start + (window_size_width/9);
	    int part_y_start  = window_size_height/6 + window_size_height/12; 
		int part_size_width  = window_size_width/4; 
	    int part_size_height  = window_size_height/8; 
	    int part_spacing  = part_size_height/5; 
		int part_shift  = part_y_start + part_size_height + part_spacing * 2;
		int part_y_adjust = part_y_start + part_spacing * 3; 
		int part_y_adjust_one = part_shift + part_spacing * 3; 
		int part_shift_one  = part_shift + part_shift/3 + part_shift/20; 
		int part_shift_adjust = part_shift_one + part_spacing * 3;
		int row1 = 0;
		int row2 = 1;
		int row3 = 2;
		
		//first button labels
		if(entityMechS.getPartCount(Part.Piston) 	>= parts_array[row1,0]){
			GUI.Label(new Rect(part_x_start, part_y_start, part_size_width, part_size_height), "" + parts_array[row1,0], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start, part_y_start, part_size_width, part_size_height), "" + parts_array[row1,0], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Gear) 	>= parts_array[row1,1]){
			GUI.Label(new Rect(part_x_start, part_y_adjust, part_size_width, part_size_height), "" + parts_array[row1,1], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start, part_y_adjust, part_size_width, part_size_height), "" + parts_array[row1,1], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Plate) 	>= parts_array[row1,2]){
			GUI.Label(new Rect(part_x_adjust, part_y_start, part_size_width, part_size_height), "" + parts_array[row1,2], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_adjust, part_y_start, part_size_width, part_size_height), "" + parts_array[row1,2], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Strut) 	>= parts_array[row1,3]){
			GUI.Label(new Rect(part_x_adjust, part_y_adjust, part_size_width, part_size_height), "" + parts_array[row1,3], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_adjust, part_y_adjust, part_size_width, part_size_height), "" + parts_array[row1,3], text_style_parts_not_in_inventory);
		}
		//second button labels
		if(showButton(1)){
			if(entityMechS.getPartCount(Part.Piston) 	>= parts_array[row2,0]){
				GUI.Label(new Rect(part_x_start, part_shift, part_size_width, part_size_height), "" + parts_array[row2,0], text_style_parts_have_in_inventory);
			}else{
				GUI.Label(new Rect(part_x_start, part_shift, part_size_width, part_size_height), "" + parts_array[row2,0], text_style_parts_not_in_inventory);
			}
			if(entityMechS.getPartCount(Part.Gear) 	>= parts_array[row2,1]){
				GUI.Label(new Rect(part_x_start, part_y_adjust_one, part_size_width, part_size_height), "" + parts_array[row2,1], text_style_parts_have_in_inventory);
			}else{
				GUI.Label(new Rect(part_x_start, part_y_adjust_one, part_size_width, part_size_height), "" + parts_array[row2,1], text_style_parts_not_in_inventory);
			}
			if(entityMechS.getPartCount(Part.Plate) 	>= parts_array[row2,2]){
				GUI.Label(new Rect(part_x_adjust, part_shift, part_size_width, part_size_height), "" + parts_array[row2,2], text_style_parts_have_in_inventory);
			}else{
				GUI.Label(new Rect(part_x_adjust, part_shift, part_size_width, part_size_height), "" + parts_array[row2,2], text_style_parts_not_in_inventory);
			}
			if(entityMechS.getPartCount(Part.Strut) 	>= parts_array[row2,3]){
				GUI.Label(new Rect(part_x_adjust, part_y_adjust_one, part_size_width, part_size_height), "" + parts_array[row2,3], text_style_parts_have_in_inventory);
			}else{
				GUI.Label(new Rect(part_x_adjust, part_y_adjust_one, part_size_width, part_size_height), "" + parts_array[row2,3], text_style_parts_not_in_inventory);
			}
		}else{
			GUI.Label(new Rect(part_x_start, part_shift, part_size_width, part_size_height), "0", text_style_grey);
			GUI.Label(new Rect(part_x_start, part_y_adjust_one, part_size_width, part_size_height), "0", text_style_grey);
			GUI.Label(new Rect(part_x_adjust, part_shift, part_size_width, part_size_height), "0", text_style_grey);
			GUI.Label(new Rect(part_x_adjust, part_y_adjust_one, part_size_width, part_size_height), "0", text_style_grey);
		}
		
		//third button labels
		if(showButton(2)){
			if(entityMechS.getPartCount(Part.Piston) 	>= parts_array[row3,0]){
				GUI.Label(new Rect(part_x_start, part_shift_one, part_size_width, part_size_height), "" + parts_array[row3,0], text_style_parts_have_in_inventory);
			}else{
				GUI.Label(new Rect(part_x_start, part_shift_one, part_size_width, part_size_height), "" + parts_array[row3,0], text_style_parts_not_in_inventory);
			}
			if(entityMechS.getPartCount(Part.Gear) 	>= parts_array[row3,1]){
				GUI.Label(new Rect(part_x_start, part_shift_adjust, part_size_width, part_size_height), "" + parts_array[row3,1], text_style_parts_have_in_inventory);
			}else{
				GUI.Label(new Rect(part_x_start, part_shift_adjust, part_size_width, part_size_height), "" + parts_array[row3,1], text_style_parts_not_in_inventory);
			}
			if(entityMechS.getPartCount(Part.Plate) 	>= parts_array[row3,2]){
				GUI.Label(new Rect(part_x_adjust, part_shift_one, part_size_width, part_size_height), "" + parts_array[row3,2], text_style_parts_have_in_inventory);
			}else{
				GUI.Label(new Rect(part_x_adjust, part_shift_one, part_size_width, part_size_height), "" + parts_array[row3,2], text_style_parts_not_in_inventory);
			}
			if(entityMechS.getPartCount(Part.Strut) 	>= parts_array[row3,3]){
				GUI.Label(new Rect(part_x_adjust, part_shift_adjust, part_size_width, part_size_height), "" + parts_array[row3,3], text_style_parts_have_in_inventory);
			}else{
				GUI.Label(new Rect(part_x_adjust, part_shift_adjust, part_size_width, part_size_height), "" + parts_array[row3,3], text_style_parts_not_in_inventory);
			}
		}else{
			GUI.Label(new Rect(part_x_start, part_shift_one, part_size_width, part_size_height), "0", text_style_grey);
			GUI.Label(new Rect(part_x_start, part_shift_adjust, part_size_width, part_size_height), "0", text_style_grey);
			GUI.Label(new Rect(part_x_adjust, part_shift_one, part_size_width, part_size_height), "0", text_style_grey);
			GUI.Label(new Rect(part_x_adjust, part_shift_adjust, part_size_width, part_size_height), "0", text_style_grey);
		}
		
	}


	//see if an upgrade can be applied
	private void canApplyUpgrade(int button_style_number){
		//find the button style number and then menu that was chosen and see if upgrade can be applied
		//get popup menu
		popUpMenu script_popup =  GetComponent<popUpMenu>();
		
		switch(button_style_number){
			case 0:
				switch(menu_choice){
						case Menu.BaseUpgrade1:
							//see if we can apply upgrade
		    				if(show_check_boxes_base_menu[0,button_style_number] == false && part_check_for_base(button_style_number, ref parts_count_for_wall_upgrades, BaseCategories.Walls, BaseUpgrade.Level1)){
								//upgrade was successful, TODO: change look of base
								//show check box for button
								show_check_boxes_base_menu[0,button_style_number] = true;
								mech.applyAPCost(ap_cost_base);	
							}else{
								if(!show_check_boxes_base_menu[0,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
						break;
				
						case Menu.BaseUpgrade2:
							//see if we can apply upgrade
		    				if(show_check_boxes_base_menu[1,button_style_number] == false && part_check_for_base(button_style_number, ref parts_count_for_struc_upgrades, BaseCategories.Structure, BaseUpgrade.Level1)){
								//upgrade was successful, TODO: change look of base
								//show check box for button
								show_check_boxes_base_menu[1,button_style_number] = true;
								mech.applyAPCost(ap_cost_base);
							}else{
								if(!show_check_boxes_base_menu[1,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
						break;
				
						case Menu.BaseUpgrade3:
							//see if we can apply upgrade
		    				if(show_check_boxes_base_menu[2,button_style_number] == false && part_check_for_base(button_style_number, ref parts_count_for_weapon_upgrades, BaseCategories.Defense, BaseUpgrade.Level1)){
								//upgrade was successful, TODO: change look of base
								//show check box for button
								show_check_boxes_base_menu[2,button_style_number] = true;
								mech.applyAPCost(ap_cost_base);
							}else{
								if(!show_check_boxes_base_menu[2,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
						break;


						case Menu.MechUpgrade1:
							//see if we can apply upgrade water
		    				if(show_check_boxes_mech_menu[0,button_style_number] == false && part_check(button_style_number, ref parts_count_for_mobile_upgrades)){
								//show check box for button
								show_check_boxes_mech_menu[0,button_style_number] = true;
								mech.upgrade_traverse_water = true; 
								mech.applyAPCost(ap_cost_mech);
							}else{
								if(!show_check_boxes_mech_menu[0,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
						break;
				
						case Menu.MechUpgrade2:
							//see if we can apply upgrade legs
		    				if(show_check_boxes_mech_menu[1,button_style_number] == false && part_check(button_style_number, ref parts_count_for_gun_upgrades)){
								//show check box for button
								show_check_boxes_mech_menu[1,button_style_number] = true;
								mech.upgrade_weapon_damage = true; 
								mech.applyAPCost(ap_cost_mech);
							}else{
								if(!show_check_boxes_mech_menu[0,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
						break;
				
						case Menu.MechUpgrade3:
							//see if we can apply upgrade legs
		    				if(show_check_boxes_mech_menu[2,button_style_number] == false && part_check(button_style_number, ref parts_count_for_other_upgrades)){
								//show check box for button
								show_check_boxes_mech_menu[2,button_style_number] = true;
								mech.upgrade_armor = true; 
								mech.applyAPCost(ap_cost_mech);
							}else{
								if(!show_check_boxes_mech_menu[0,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
						break;

						default:
							//see if we can apply upgrade
		    				print ("wrong menu");
							break;
				}
			break;
			
			
			
			
			
			
			
			


			case 1:
				switch(menu_choice){
						case Menu.BaseUpgrade1:
							//see if we can apply upgrade
		    				if(show_check_boxes_base_menu[0,button_style_number] == false && part_check_for_base(button_style_number, ref parts_count_for_wall_upgrades, BaseCategories.Walls, BaseUpgrade.Level2)){
								//upgrade was successful, TODO: change look of base
								//show check box for button
								show_check_boxes_base_menu[0,button_style_number] = true;
								mech.applyAPCost(ap_cost_base);
							}else{
								if(!show_check_boxes_base_menu[0,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
						break;
				
						case Menu.BaseUpgrade2:
							//see if we can apply upgrade
		    				if(show_check_boxes_base_menu[1,button_style_number] == false && part_check_for_base(button_style_number, ref parts_count_for_struc_upgrades, BaseCategories.Structure,BaseUpgrade.Level2)){
								//upgrade was successful, TODO: change look of base
								//show check box for button
								show_check_boxes_base_menu[1,button_style_number] = true;
								mech.applyAPCost(ap_cost_base);
							}else{
								if(!show_check_boxes_base_menu[1,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
						break;
				
						case Menu.BaseUpgrade3:
							//see if we can apply upgrade
		    				if(show_check_boxes_base_menu[2,button_style_number] == false && part_check_for_base(button_style_number, ref parts_count_for_weapon_upgrades, BaseCategories.Defense, BaseUpgrade.Level2)){
								//upgrade was successful, TODO: change look of base
								//show check box for button
								show_check_boxes_base_menu[2,button_style_number] = true;
								mech.applyAPCost(ap_cost_base);
							}else{
								if(!show_check_boxes_base_menu[2,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
						
						break;
				
				

						case Menu.MechUpgrade1:
							//see if we can apply upgrade mountain
		    				if(show_check_boxes_mech_menu[0,button_style_number] == false && part_check(button_style_number, ref parts_count_for_mobile_upgrades)){
								//show check box for button
								show_check_boxes_mech_menu[0,button_style_number] = true;
								mech.upgrade_traverse_mountain = true; 
								mech.applyAPCost(ap_cost_mech);
							}else{
								if(show_check_boxes_mech_menu[0,button_style_number])
									script_popup.text_number_choice = 2; //missing parts
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
							break;
				
						case Menu.MechUpgrade2:
							//see if we can apply upgrade legs
		    				if(show_check_boxes_mech_menu[1,button_style_number] == false && part_check(button_style_number, ref parts_count_for_gun_upgrades)){
								//show check box for button
								show_check_boxes_mech_menu[1,button_style_number] = true;
								mech.upgrade_weapon_range = true; 
								mech.applyAPCost(ap_cost_mech);
							}else{
								if(show_check_boxes_mech_menu[1,button_style_number])
									script_popup.text_number_choice = 2; //missing parts
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
							break;
				
						case Menu.MechUpgrade3:
							//see if we can apply upgrade legs
		    				if(show_check_boxes_mech_menu[2,button_style_number] == false && part_check(button_style_number, ref parts_count_for_other_upgrades)){
								//show check box for button
								inPlayMenuS script =  GetComponent<inPlayMenuS>(); //enable transport button
								script.enable_transport_button = true;
								show_check_boxes_mech_menu[2,button_style_number] = true;
								mech.upgrade_teleport = true; 
								mech.applyAPCost(ap_cost_mech);
							}else{
								if(show_check_boxes_mech_menu[2,button_style_number])
									script_popup.text_number_choice = 2; //missing parts
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
							break;


						default:
							//see if we can apply upgrade
								script_popup.text_number_choice = 1; //missing parts
								script_popup.upgrade_popup = true;
							break;
				}
			break;


			case 2:
				switch(menu_choice){
						case Menu.BaseUpgrade1:
							//see if we can apply upgrade
		    				if(show_check_boxes_base_menu[0,button_style_number] == false && part_check_for_base(button_style_number, ref parts_count_for_wall_upgrades, BaseCategories.Walls, BaseUpgrade.Level3)){
								//upgrade was successful, TODO: change look of base
								//show check box for button
								show_check_boxes_base_menu[0,button_style_number] = true;
								mech.applyAPCost(ap_cost_base);
							}else{
								if(!show_check_boxes_base_menu[0,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
							break;
				
						case Menu.BaseUpgrade2:
							//see if we can apply upgrade
		    				if(show_check_boxes_base_menu[1,button_style_number] == false && part_check_for_base(button_style_number, ref parts_count_for_struc_upgrades, BaseCategories.Structure, BaseUpgrade.Level3)){
								//upgrade was successful, TODO: change look of base
								//show check box for button
								show_check_boxes_base_menu[1,button_style_number] = true;
								mech.applyAPCost(ap_cost_base);
							}else{
								if(!show_check_boxes_base_menu[1,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
							break;
				
						case Menu.BaseUpgrade3:
							//see if we can apply upgrade 
		    				if(show_check_boxes_base_menu[2,button_style_number] == false && part_check_for_base(button_style_number, ref parts_count_for_weapon_upgrades, BaseCategories.Defense, BaseUpgrade.Level3)){
								//upgrade was successful, TODO: change look of base
								//show check box for button
								show_check_boxes_base_menu[2,button_style_number] = true;
								mech.applyAPCost(ap_cost_base);
							}else{
								if(!show_check_boxes_base_menu[2,button_style_number])
									script_popup.text_number_choice = 2; //already done
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
							break;
				
				
				

						case Menu.MechUpgrade1:
							//see if we can apply
		    				if(show_check_boxes_mech_menu[0,button_style_number] == false && part_check(button_style_number, ref parts_count_for_mobile_upgrades)){
								//show check box for button
								show_check_boxes_mech_menu[0,button_style_number] = true;
								mech.upgrade_traverse_cost = true;
								mech.applyAPCost(ap_cost_mech);
							}else{
								if(show_check_boxes_mech_menu[0,button_style_number])
									script_popup.text_number_choice = 2; //missing parts
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							} 
							break;
				
						case Menu.MechUpgrade2:
							//see if we can apply 
		    				if(show_check_boxes_mech_menu[1,button_style_number] == false && part_check(button_style_number, ref parts_count_for_gun_upgrades)){
								show_check_boxes_mech_menu[1,button_style_number] = true;
								//show check box for button
								mech.upgrade_weapon_cost = true; 
								mech.applyAPCost(ap_cost_mech);
							}else{
								if(show_check_boxes_mech_menu[1,button_style_number])
									script_popup.text_number_choice = 2; //missing parts
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
							break;
				
						case Menu.MechUpgrade3:
							//see if we can apply upgrade legs
		    				if(show_check_boxes_mech_menu[2,button_style_number] == false && part_check(button_style_number, ref parts_count_for_other_upgrades)){
								//show check box for button
								show_check_boxes_mech_menu[2,button_style_number] = true;
								mech.upgrade_teleport = true; 
								mech.applyAPCost(ap_cost_mech);
							}else{
								if(show_check_boxes_mech_menu[2,button_style_number])
									script_popup.text_number_choice = 2; //missing parts
								else
									script_popup.text_number_choice = 1; //missing parts
								
								script_popup.upgrade_popup = true;	
							}
							break;


						default:
							//see if we can apply upgrade
		    				print ("wrong menu");
							break;
				}
			break;


			default:
				//nothing to do
				break;
		}
	}
	
	//check to see if player has enough parts to do the mech upgrade
	private bool part_check(int row_number, ref int [,] parts_array){
		if(
			(entityMechS.getPartCount(Part.Piston) 	>= parts_array[row_number,0]) &&
			(entityMechS.getPartCount(Part.Gear)	>= parts_array[row_number,1]) &&
			(entityMechS.getPartCount(Part.Plate) 	>= parts_array[row_number,2]) &&
			(entityMechS.getPartCount(Part.Strut) 	>= parts_array[row_number,3]))
		{
			entityMechS.adjustPartCount(Part.Piston,	-parts_array[row_number,0]);
			entityMechS.adjustPartCount(Part.Gear, 		-parts_array[row_number,1]);
			entityMechS.adjustPartCount(Part.Plate,	 	-parts_array[row_number,2]);
			entityMechS.adjustPartCount(Part.Strut, 	-parts_array[row_number,3]);
			return true;
		}  
		return false;
	}
	
	//check to see if player has enough parts to do the base upgrade
	private bool part_check_for_base(int row_number, ref int [,] parts_array,BaseCategories category, BaseUpgrade upgrade_type){
		if(
			(entityMechS.getPartCount(Part.Piston) 	>= parts_array[row_number,0]) &&
			(entityMechS.getPartCount(Part.Gear)	>= parts_array[row_number,1]) &&
			(entityMechS.getPartCount(Part.Plate) 	>= parts_array[row_number,2]) &&
			(entityMechS.getPartCount(Part.Strut) 	>= parts_array[row_number,3]))
		{
			//get base and check to see if this upgrade can be applied or already has been applied
			entityBaseS base_s = entityManagerS.getBase();
			if(base_s.upgradeBase(category, upgrade_type)){
				entityMechS.adjustPartCount(Part.Piston,	-parts_array[row_number,0]);
				entityMechS.adjustPartCount(Part.Gear, 		-parts_array[row_number,1]);
				entityMechS.adjustPartCount(Part.Plate,	 	-parts_array[row_number,2]);
				entityMechS.adjustPartCount(Part.Strut, 	-parts_array[row_number,3]);
				return true;
			}
		}  
		return false;
	}
	
	//Health Menu
	private void healthMenu() {
		GUI.skin = healthMenuSkin;
		//get popup menu
		popUpMenu script_popup =  GetComponent<popUpMenu>();
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 

	    //Layout start
	    GUI.BeginGroup(new Rect(window_size_x, window_size_y, window_size_width, window_size_height));
		
		//The Menu background box
	    GUI.Box(new Rect(0, 0, window_size_width, window_size_height), "");
		
		int label_x_start  = window_size_width/5;
		int button_x_start  = window_size_width/7 + window_size_width/75; 
	    int button_y_start  = window_size_height/4 + window_size_height/45; 
		int button_size_width  = window_size_width/7; 
	    int button_size_height  = window_size_height/6; 
	    int button_spacing  = button_size_height/5 + window_size_height/75; 
		
	    //Game resume button (close upgrade menu)
	    if(GUI.Button(new Rect(window_size_width - window_size_width/12, window_size_height/20, window_size_width/20, window_size_height/20), "X", default_button_style)) {
			//resume the game
		    Time.timeScale = 1;
		    inPlayMenuS script =  GetComponent<inPlayMenuS>();
    		script.enabled = true;
		    //disable pause menu
		    UpgradeMenuS script2 =  GetComponent<UpgradeMenuS>();
    		script2.enabled = false;
	    }
		
		//regenerate button 0
	    if(GUI.Button(new Rect(button_x_start, button_y_start, button_size_width, button_size_height), "" , piston_button_style)) {
			if(mech.applyUpgrade(ap_cost_health)){
				if(entityMechS.getPartCount(Part.Piston) > 0 && mech.current_hp < mech.getMaxHP()){	
					entityMechS.adjustPartCount(Part.Piston,	-1); 
					mech.healhp(2);
					mech.applyAPCost(ap_cost_health);
				}else{
					if(mech.current_hp == mech.getMaxHP())
						script_popup.text_number_choice = 3;
					else
						script_popup.text_number_choice = 4;
				
				script_popup.upgrade_popup = true;
				}
			}else{
				script_popup.text_number_choice = 0;
				script_popup.upgrade_popup = true;
			}
		}
		
		//regenerate button 1
	    if(GUI.Button(new Rect(button_x_start + (button_spacing) + (button_size_width), button_y_start, button_size_width, button_size_height), "" ,gear_button_style)) {
			if(mech.applyUpgrade(ap_cost_health)){
				if(entityMechS.getPartCount(Part.Gear) > 0 && mech.current_hp < mech.getMaxHP()){
					entityMechS.adjustPartCount(Part.Gear, 		-1);
					mech.healhp(2);
					mech.applyAPCost(ap_cost_health);
				}else{
					if(mech.current_hp == mech.getMaxHP())
						script_popup.text_number_choice = 3;
					else
						script_popup.text_number_choice = 4;
				
				script_popup.upgrade_popup = true;
				}
			}else{
				script_popup.text_number_choice = 0;
				script_popup.upgrade_popup = true;
			}
		}
		
		//regenerate button 2
	    if(GUI.Button(new Rect(button_x_start + (2 * button_spacing) + (2 * button_size_width), button_y_start, button_size_width, button_size_height), "" ,plate_button_style)) {
			if(mech.applyUpgrade(ap_cost_health)){
				if(entityMechS.getPartCount(Part.Plate) > 0 && mech.current_hp < mech.getMaxHP()){
					entityMechS.adjustPartCount(Part.Plate,	 	-1);
					mech.healhp(2);
					mech.applyAPCost(ap_cost_health);
				}else{
					if(mech.current_hp == mech.getMaxHP())
						script_popup.text_number_choice = 3;
					else
						script_popup.text_number_choice = 4;
				
				script_popup.upgrade_popup = true;
				}
			}else{
				script_popup.text_number_choice = 0;
				script_popup.upgrade_popup = true;
			}
		}
		
		//regenerate button 3
	    if(GUI.Button(new Rect(button_x_start + (3 * button_spacing) + (3 * button_size_width), button_y_start, button_size_width, button_size_height), "" ,strut_button_style)) {
			if(mech.applyUpgrade(ap_cost_health)){	
				if(entityMechS.getPartCount(Part.Strut) > 0 && mech.current_hp < mech.getMaxHP()){
					entityMechS.adjustPartCount(Part.Strut, 	-1);
					mech.healhp(2);
					mech.applyAPCost(ap_cost_health);
				}else{
					if(mech.current_hp == mech.getMaxHP())
						script_popup.text_number_choice = 3;
					else
						script_popup.text_number_choice = 4;
				
				script_popup.upgrade_popup = true;
				}
			}else{
				script_popup.text_number_choice = 0;
				script_popup.upgrade_popup = true;
			}
		}
		
		//Logo Pictures to show which upgrade has occured, 3 dots or something that will be highlighted when upgrade has ben applied
	    GUI.Label(new Rect(label_x_start- 34, button_y_start + (button_size_height), button_size_width, 40), "" + entityMechS.getPartCount(Part.Piston), enginePlayerS.gui_bold_text_static);
	    GUI.Label(new Rect(label_x_start + (button_spacing) + (button_size_width) - 34, button_y_start + (button_size_height), button_size_width, 40), "" + entityMechS.getPartCount(Part.Gear),enginePlayerS.gui_bold_text_static);
	    GUI.Label(new Rect(label_x_start + (2 * button_spacing) + (2 * button_size_width) - 34, button_y_start + (button_size_height), button_size_width, 40), "" + entityMechS.getPartCount(Part.Plate),enginePlayerS.gui_bold_text_static);
		GUI.Label(new Rect(label_x_start + (3 * button_spacing) + (3 * button_size_width) - 34, button_y_start + (button_size_height), button_size_width, 40), "" + entityMechS.getPartCount(Part.Strut),enginePlayerS.gui_bold_text_static);
		 
		//hp bar variables
		int width_denominator = 18;
		int width_numerator = 5;
		int denominator_hp = width_denominator * mech.getMaxHP();
		int multiple_hp = denominator_hp/width_denominator;
		int numerator_hp = width_numerator * multiple_hp;
		int difference_hp = mech.getMaxHP() - numerator_hp;
		
		//HP bar Button
		
		    GUI.DrawTexture(new Rect (label_x_start + (button_spacing) + (button_size_width) - label_x_start/20 - 6,
			button_y_start + (button_size_height * 3) - button_y_start/6+2,
			218,
			36 ), 
			inPlayMenuS.chris_hp_mech_bg_s, ScaleMode.StretchToFill);
			GUI.BeginGroup (new Rect (label_x_start + (button_spacing) + (button_size_width) - label_x_start/20,button_y_start + (button_size_height * 3) - button_y_start/6, (int)210*(float)entityManagerS.getMech().current_hp/(float)entityManagerS.getMech().max_hp, 24));
			GUI.DrawTexture(new Rect (0,0, 210, 24), inPlayMenuS.chris_hp_mech_s, ScaleMode.StretchToFill);
			GUI.EndGroup (); 
		 	GUI.Label(new Rect(label_x_start + (button_spacing) + (button_size_width) - label_x_start/20,button_y_start + (button_size_height * 3) - button_y_start/6, 210, 24), entityManagerS.getMech().current_hp + "/" + entityManagerS.getMech().max_hp,  enginePlayerS.gui_norm_text_static);
		
		
		 
		GUI.EndGroup();
	}
	
	//Objective Menu 
	private void mainMenu() {
		GUI.skin = objectiveMenuSkin;
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 

	    //Layout start
	    GUI.BeginGroup(new Rect(window_size_x, window_size_y, window_size_width, window_size_height));
		
		//The Menu background box
	    GUI.Box(new Rect(0, 0, window_size_width, window_size_height), "");
		
		int label_x_start  = window_size_width/3; 
	    int button_x_start  = window_size_width/3 + window_size_width/4; 
	    int button_y_start  = window_size_height/2 - window_size_width/10; 
		int button_size_width  = window_size_width/2 - window_size_width/4 - window_size_width/40; 
	    int button_size_height = window_size_height/12; 
	    int button_spacing  = button_size_height/5; 
		
	    //Game resume button (close upgrade menu)
	    if(GUI.Button(new Rect(window_size_width - window_size_width/12, window_size_height/20, window_size_width/20, window_size_height/20), "X", default_button_style)) {
			//resume the game
		    Time.timeScale = 1;
		    inPlayMenuS script =  GetComponent<inPlayMenuS>();
    		script.enabled = true;
		    //disable pause menu
		    UpgradeMenuS script2 =  GetComponent<UpgradeMenuS>();
    		script2.enabled = false;
			menu_choice = Menu.Objective; 
	    }
		
		//Load
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "Restart" )) {
			Application.LoadLevel(PlayerPrefs.GetString("CONTINUE","Level0"));
	    }
		
		//Objectives
	    if(GUI.Button(new Rect(button_x_start , button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "Objectives" )) {
		    Time.timeScale = 1;
//		    inPlayMenuS script =  GetComponent<inPlayMenuS>();
//    		script.enabled = true;
//		    UpgradeMenuS script2 =  GetComponent<UpgradeMenuS>();
//    		script2.enabled = false; 
//			
//			popUpMenu script_popup =  GetComponent<popUpMenu>();
//			script_popup.activateMenu();
			showObjectivesMenu();
	    }
		
		//Quit
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), "Quit" )) {
			Application.LoadLevel("MainMenu");
	    }
		
//		//not present at the moment...
//	    if(GUI.Button(new Rect(button_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), "" )) {
//	    }
		
		GUI.EndGroup();
	}
	
	public static inPlayMenuS inPlayMenuScript;
	public static UpgradeMenuS UpgradeMenuScript;
	public static popUpMenu popUpMenuScript;
	
	
	void Awake()
	{ 
		
		popUpMenuScript =  GetComponent<popUpMenu>();
	    inPlayMenuScript =  GetComponent<inPlayMenuS>();
		UpgradeMenuScript =  GetComponent<UpgradeMenuS>();
	}
	public static void showObjectivesMenu()
	{ 
	    Time.timeScale = 1;
		inPlayMenuScript.enabled = true;
		UpgradeMenuScript.enabled = false; 
		popUpMenuScript.activateMenu();
	}
	
	private void defaultMenu() {
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 

	    //Layout start
	    GUI.BeginGroup(new Rect(window_size_x, window_size_y, window_size_width, window_size_height));

	    //Game resume button (close upgrade menu)
	    if(GUI.Button(new Rect(0, 0, window_size_width, window_size_height), "", default_button_style)) {
			//resume the game
		    Time.timeScale = 1;
		    inPlayMenuS script =  GetComponent<inPlayMenuS>();
    		script.enabled = true;
		    //disable pause menu
		    UpgradeMenuS script2 =  GetComponent<UpgradeMenuS>();
    		script2.enabled = false;
	    }
		
		GUI.EndGroup();
	}


	void OnGUI(){
		//load GUI skin
    	GUI.skin = newSkin;
   
    	//show the mouse cursor
    	Screen.showCursor = true;
   
		//choose a menu and run script
		chooseMenu();
	}


	//menu_choice must be set by calling script otherwise the menu will be the default menu or last menu called
	private void chooseMenu(){
		switch(menu_choice){
				case Menu.BaseUpgrade1:
					//run the upgrade menu script
    				theUpgradeMenu(wall_upgrade_style,wall_upgrade_style2,wall_upgrade_style3);
					break;
			
				case Menu.BaseUpgrade2:
					//run the upgrade menu script
    				theUpgradeMenu(structure_upgrade_style,structure_upgrade_style2, structure_upgrade_style3);
					break;
			
				case Menu.BaseUpgrade3:
					//run the upgrade menu script
    				theUpgradeMenu(weapon_upgrade_style,weapon_upgrade_style2,weapon_upgrade_style3);
					break;

				case Menu.MechUpgrade1:
					//run the upgrade menu script mobile
    				theUpgradeMenu(fin_upgrade_style,moutain_upgrade_style,leg_upgrade_style);
					break;

				case Menu.MechUpgrade2:
					//run the upgrade menu script gun
    				theUpgradeMenu(gun_upgrade_style,gun_upgrade_style2,gun_upgrade_style3);
					break;
			
				case Menu.MechUpgrade3:
					//run the upgrade menu script other
    				theUpgradeMenu(goggles_upgrade_style,teleport_upgrade_style,other_upgrade_style);
					break;	
			
				case Menu.HealthUpgrade:
					//run the upgrade menu script
    				healthMenu();
			   		break;

				case Menu.Objective:
			        mainMenu();
					break;
			
				default:
					//Default menu
					defaultMenu();
					break;
		}

	}
}
