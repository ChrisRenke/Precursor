using UnityEngine;
using System.Collections;


public class UpgradeMenuS : MonoBehaviour {
	//Menu Skin
	public GUISkin newSkin;
	public GUISkin healthMenuSkin;
	public GUISkin objectiveMenuSkin;

	//styles for text
	public GUIStyle text_style_parts_not_in_inventory;
	public GUIStyle text_style_parts_have_in_inventory;

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
	public GUIStyle other_upgrade_style1;
	public GUIStyle other_upgrade_style2;
	//base wall styles
	public GUIStyle wall_upgrade_style;
	public GUIStyle wall_upgrade_style2;
	public GUIStyle wall_upgrade_style3;
	//base structure styles
	public GUIStyle structure_upgrade_style;
	public GUIStyle structure_upgrade_style2;
	public GUIStyle structure_upgrade_style3;
	//base weapon styles
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
	//default button style, blank backboard
	public GUIStyle default_button_style;

	//Window size
	private int screen_size_x;
	private int screen_size_y;

	//Pick a menu
	public Menu menu_choice = Menu.Default;

	//Part count for upgrades: Column 0 = Piston / Column 1 = Gear / Column 2 = Plate / Column 3 = Strut
	//Mech Upgrades part array requirements
	private int[,] parts_count_for_mobile_upgrades = new int[3,4] 	{ {0,5,2,0}, {2,2,0,0}, {3,0,0,3} }; //Row 0 = water upgrade, Row 1 = mountan upgrade, Row 2 = leg upgrade
	private int[,] parts_count_for_gun_upgrades = new int[3,4] 		{ {1,1,3,0}, {1,2,4,0}, {2,2,4,0} }; //Row 0 = gun upgrade range , Row 1 = gun upgrade damage, Row 2 = gun upgrade cost
	private int[,] parts_count_for_other_upgrades = new int[3,4] 	{ {1,4,0,5}, {0,6,0,9}, {2,2,2,2} }; //Row 0 =  armour upgrade 1, Row 1 = armour upgrade 2 (teleport base upgrade?), Row 2 = armour upgrade 3 (other upgrade?)
	//Base Upgrades part array requirements
	private int[,] parts_count_for_wall_upgrades = new int[3,4] 	{ {2,0,0,1}, {4,0,0,2}, {4,0,0,3} }; //Row 0 = wall upgrade 1, Row 1 = wall upgrade 2, Row 2 = wall upgrade 3
	private int[,] parts_count_for_struc_upgrades = new int[3,4] 	{ {1,1,1,1}, {2,2,2,2}, {4,4,4,4} }; //Row 0 = structure upgrade 1, Row 1 = structure upgrade 2, Row 2 = structure upgrade 3
	private int[,] parts_count_for_weapon_upgrades = new int[3,4] 	{ {2,0,0,3}, {2,0,2,3}, {3,1,3,3} }; //Row 0 = weapon upgrade 1, Row 1 = weapon upgrade 2, Row 2 = weapon upgrade 3
	
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
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 

	    //Layout start
	    GUI.BeginGroup(new Rect(window_size_x, window_size_y, window_size_width, window_size_height));


	    //The Menu background box
	    GUI.Box(new Rect(0, 0, window_size_width, window_size_height), "");


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
		
		//continue button variables
		int continue_button_x_left = window_size_width - window_size_width/12;
		int continue_button_x_right = window_size_width/60;
		int continue_button_y = window_size_height/2;
		int continue_button_width = window_size_width/15;
		int continue_button_height = window_size_height/20;
		
		//Var for base upgrade
		bool base_upgrade_occured = false;
		
		//Continue Buttons and part count labels
		switch(menu_choice){
			case Menu.BaseUpgrade1:
				//Next button
				if(GUI.Button(new Rect(continue_button_x_left, continue_button_y, continue_button_width, continue_button_height), "Next" , default_button_style)) {
					menu_choice = Menu.BaseUpgrade2;
	    		}
				
				//draw part labels
				part_labels(window_size_width, window_size_height, ref parts_count_for_wall_upgrades);	
			
				break;
	
			case Menu.BaseUpgrade2:
			    //Next button
				if(GUI.Button(new Rect(continue_button_x_left, continue_button_y, continue_button_width, continue_button_height), "Next" ,default_button_style)) {
					menu_choice = Menu.BaseUpgrade3;
	    		}
				//Back button
				if(GUI.Button(new Rect(continue_button_x_right, continue_button_y, continue_button_width, continue_button_height), "Back" ,default_button_style)) {
					menu_choice = Menu.BaseUpgrade1;
	    		}
			
				//draw part labels
				part_labels(window_size_width, window_size_height,  ref parts_count_for_struc_upgrades);	
			
				break;
			
			case Menu.BaseUpgrade3:
			    //Back button
				if(GUI.Button(new Rect(continue_button_x_right, continue_button_y, continue_button_width, continue_button_height), "Back" ,default_button_style)) {
					menu_choice = Menu.BaseUpgrade2;
	    		}
				
				//draw part labels
				part_labels(window_size_width, window_size_height, ref parts_count_for_weapon_upgrades);	
			
				break;
			
			case Menu.MechUpgrade1:
				//Next button
				if(GUI.Button(new Rect(continue_button_x_left, continue_button_y, continue_button_width, continue_button_height), "Next" ,default_button_style)) {
					menu_choice = Menu.MechUpgrade2;
	    		}
			
				//draw part labels
				part_labels(window_size_width, window_size_height,  ref parts_count_for_mobile_upgrades);	
			
				break;
			
			case Menu.MechUpgrade2:
				//Next button
				if(GUI.Button(new Rect(continue_button_x_left, continue_button_y, continue_button_width, continue_button_height), "Next" ,default_button_style)) {
					menu_choice = Menu.MechUpgrade3;
	    		}
				//Back button
				if(GUI.Button(new Rect(continue_button_x_right, continue_button_y, continue_button_width, continue_button_height), "Back" ,default_button_style)) {
					menu_choice = Menu.MechUpgrade1;
	    		}
				
				//draw part labels
				part_labels(window_size_width, window_size_height, ref parts_count_for_gun_upgrades);	
			
			
				break;
			
			case Menu.MechUpgrade3:
				//Back button
				if(GUI.Button(new Rect(continue_button_x_right, continue_button_y, continue_button_width, continue_button_height), "Back" ,default_button_style)) {
					menu_choice = Menu.MechUpgrade2;
	    		}
			
				//draw part labels
				part_labels(window_size_width, window_size_height, ref parts_count_for_other_upgrades);	
			
			
				break;
			
			default:
				//not on the correct menu
				break;
		}


	    //Exit button (Go to Main Menu)
		//    if(GUI.Button(new Rect(0, 0, window_size_width/20, window_size_height/20), "Exit")) {
		//    	//TODO:
		//    	//Application.LoadLevel(0); //go to Main menu, we don't have one
		//    }


	    //Upgrade Menu buttons
	    int label_x_start  = window_size_width/2; 
	    int button_x_start  = window_size_width/9; 
	    int button_y_start  = window_size_height/4 + window_size_height/15; 
		int button_size_width  = window_size_width/6; 
	    int button_size_height  = window_size_height/8; 
	    int button_spacing  = button_size_height/2; 
		
	    //upgrade button 0
	    if(GUI.Button(new Rect(button_x_start, button_y_start, button_size_width, button_size_height), "" ,button_one_style)) {
			canApplyUpgrade(0);
	    }


	    //upgrade button 1
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "", button_two_style)) {
	    	canApplyUpgrade(1);
	    }


	    //upgrade button 2
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "", button_three_style)) {
	    	canApplyUpgrade(2);
	    }


	    //Logo Pictures to show which upgrade has occured, 3 dots or something that will be highlighted when upgrade has ben applied
	    GUI.Label(new Rect(label_x_start, button_y_start, button_size_width, button_size_height), "Description 1");
	    GUI.Label(new Rect(label_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "Description 2");
	    GUI.Label(new Rect(label_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "Description 3");


	    //layout end
	    GUI.EndGroup();

	}
	
	private void part_labels (int window_size_width, int window_size_height, ref int[,] parts_array){
		//part count variables
		int part_x_start  = window_size_width/2 + (window_size_width/29) * 5; 
	    int part_y_start  = window_size_height/6 + window_size_height/12; 
		int part_size_width  = window_size_width/6; 
	    int part_size_height  = window_size_height/8; 
	    int part_spacing  = part_size_height/4; 
		int part_shift  = part_y_start + window_size_height/15 + window_size_height/7; 
		int part_shift_one  = part_shift + part_shift/3 + part_shift/8; 
		int row1 = 0;
		int row2 = 1;
		int row3 = 2;
		
		if(entityMechS.getPartCount(Part.Piston) 	>= parts_array[row1,0]){
			GUI.Label(new Rect(part_x_start, part_y_start, part_size_width, part_size_height), "" + parts_array[row1,0], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start, part_y_start, part_size_width, part_size_height), "" + parts_array[row1,0], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Gear) 	>= parts_array[row1,1]){
			GUI.Label(new Rect(part_x_start, part_y_start + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row1,1], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start, part_y_start + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row1,1], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Plate) 	>= parts_array[row1,2]){
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_y_start, part_size_width, part_size_height), "" + parts_array[row1,2], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_y_start, part_size_width, part_size_height), "" + parts_array[row1,2], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Strut) 	>= parts_array[row1,3]){
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_y_start + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row1,3], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_y_start + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row1,3], text_style_parts_not_in_inventory);
		}
		
		if(entityMechS.getPartCount(Part.Piston) 	>= parts_array[row2,0]){
			GUI.Label(new Rect(part_x_start, part_shift, part_size_width, part_size_height), "" + parts_array[row2,0], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start, part_shift, part_size_width, part_size_height), "" + parts_array[row2,0], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Gear) 	>= parts_array[row2,1]){
			GUI.Label(new Rect(part_x_start, part_shift + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row2,1], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start, part_shift + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row2,1], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Plate) 	>= parts_array[row2,2]){
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_shift, part_size_width, part_size_height), "" + parts_array[row2,2], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_shift, part_size_width, part_size_height), "" + parts_array[row2,2], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Strut) 	>= parts_array[row2,3]){
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_shift + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row2,3], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_shift + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row2,3], text_style_parts_not_in_inventory);
		}
		
		if(entityMechS.getPartCount(Part.Piston) 	>= parts_array[row3,0]){
			GUI.Label(new Rect(part_x_start, part_shift_one, part_size_width, part_size_height), "" + parts_array[row3,0], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start, part_shift_one, part_size_width, part_size_height), "" + parts_array[row3,0], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Gear) 	>= parts_array[row3,1]){
			GUI.Label(new Rect(part_x_start, part_shift_one + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row3,1], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start, part_shift_one + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row3,1], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Plate) 	>= parts_array[row3,2]){
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_shift_one, part_size_width, part_size_height), "" + parts_array[row3,2], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_shift_one, part_size_width, part_size_height), "" + parts_array[row3,2], text_style_parts_not_in_inventory);
		}
		if(entityMechS.getPartCount(Part.Strut) 	>= parts_array[row3,3]){
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_shift_one + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row3,3], text_style_parts_have_in_inventory);
		}else{
			GUI.Label(new Rect(part_x_start + (window_size_width/29) * 3, part_shift_one + part_spacing * 3, part_size_width, part_size_height), "" + parts_array[row3,3], text_style_parts_not_in_inventory);
		}
		
	}


		//see if an upgrade can be applied
	private void canApplyUpgrade(int button_style_number){
		//find the button style number and then menu that was chosen and see if upgrade can be applied
		switch(button_style_number){
			case 0:
				switch(menu_choice){
						case Menu.BaseUpgrade1:
							//see if we can apply upgrade
		    				if(part_check_for_base(button_style_number, ref parts_count_for_wall_upgrades, BaseUpgrade.AP1)){
								//upgrade was successful, TODO: change look of base
							}else
								print ("no good!");
							break;
				
						case Menu.BaseUpgrade2:
							//see if we can apply upgrade
		    				if(part_check_for_base(button_style_number, ref parts_count_for_struc_upgrades, BaseUpgrade.Structure1)){
								//upgrade was successful, TODO: change look of base
							}else
								print ("no good!");
							break;
				
						case Menu.BaseUpgrade3:
							//see if we can apply upgrade
		    				if(part_check_for_base(button_style_number, ref parts_count_for_weapon_upgrades, BaseUpgrade.Defenses1)){
								//upgrade was successful, TODO: change look of base
							}else
								print ("no good!");
							break;


						case Menu.MechUpgrade1:
							//see if we can apply upgrade water
		    				if(part_check(button_style_number, ref parts_count_for_mobile_upgrades)){
								mech.upgrade_traverse_water = true;
								mech.destroySelectionHexes();
								mech.allowSelectionHexesDraw();
							}else
								print ("no good!");  
							break;
				
						case Menu.MechUpgrade2:
							//see if we can apply upgrade legs
		    				if(part_check(button_style_number, ref parts_count_for_gun_upgrades)){
								mech.upgrade_weapon_range = true;
								mech.destroySelectionHexes();
								mech.allowSelectionHexesDraw();
							}else
								print ("no good!");  
							break;
				
						case Menu.MechUpgrade3:
							//see if we can apply upgrade legs
		    				if(part_check(button_style_number, ref parts_count_for_other_upgrades)){
								mech.upgrade_armor_1 = true;
								mech.destroySelectionHexes();
								mech.allowSelectionHexesDraw();
							}else
								print ("no good!");  
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
		    				if(part_check_for_base(button_style_number, ref parts_count_for_wall_upgrades, BaseUpgrade.AP2)){
								//upgrade was successful, TODO: change look of base
							}else
								print ("no good!");
							break;
				
						case Menu.BaseUpgrade2:
							//see if we can apply upgrade
		    				if(part_check_for_base(button_style_number, ref parts_count_for_struc_upgrades, BaseUpgrade.Structure2)){
								//upgrade was successful, TODO: change look of base
							}else
								print ("no good!");
							break;
				
						case Menu.BaseUpgrade3:
							//see if we can apply upgrade
		    				if(part_check_for_base(button_style_number, ref parts_count_for_weapon_upgrades, BaseUpgrade.Defenses2)){
								//upgrade was successful, TODO: change look of base
							}else
								print ("no good!");
							break;


						case Menu.MechUpgrade1:
							//see if we can apply upgrade mountain
		    				if(part_check(button_style_number, ref parts_count_for_mobile_upgrades)){
								mech.upgrade_traverse_mountain = true;
								mech.destroySelectionHexes();
								mech.allowSelectionHexesDraw();
							}else
								print ("no good!");  
							break;
				
						case Menu.MechUpgrade2:
							//see if we can apply upgrade legs
		    				if(part_check(button_style_number, ref parts_count_for_gun_upgrades)){
								mech.upgrade_weapon_damage = true;
								mech.destroySelectionHexes();
								mech.allowSelectionHexesDraw();
							}else
								print ("no good!");  
							break;
				
						case Menu.MechUpgrade3:
							//see if we can apply upgrade legs
		    				if(part_check(button_style_number, ref parts_count_for_other_upgrades)){
								mech.upgrade_armor_2 = true;
								mech.destroySelectionHexes();
								mech.allowSelectionHexesDraw();
							}else
								print ("no good!");
							break;


						default:
							//see if we can apply upgrade
								print ("no good!");
							break;
				}
			break;


			case 2:
				switch(menu_choice){
						case Menu.BaseUpgrade1:
							//see if we can apply upgrade
		    				if(part_check_for_base(button_style_number, ref parts_count_for_wall_upgrades, BaseUpgrade.AP3)){
								//upgrade was successful, TODO: change look of base
							}else
								print ("no good!");
							break;
				
						case Menu.BaseUpgrade2:
							//see if we can apply upgrade
		    				if(part_check_for_base(button_style_number, ref parts_count_for_struc_upgrades, BaseUpgrade.Structure3)){
								//upgrade was successful, TODO: change look of base
							}else
								print ("no good!");
							break;
				
						case Menu.BaseUpgrade3:
							//see if we can apply upgrade
		    				if(part_check_for_base(button_style_number, ref parts_count_for_weapon_upgrades, BaseUpgrade.Defenses3)){
								//upgrade was successful, TODO: change look of base
							}else
								print ("no good!");
							break;


						case Menu.MechUpgrade1:
							//see if we can apply upgrade legs
		    				if(part_check(button_style_number, ref parts_count_for_mobile_upgrades)){
								mech.upgrade_traverse_cost = true;
								mech.destroySelectionHexes();
								mech.allowSelectionHexesDraw();
							}else
								print ("no good!");  
							break;
				
						case Menu.MechUpgrade2:
							//see if we can apply upgrade legs
		    				if(part_check(button_style_number, ref parts_count_for_gun_upgrades)){
								mech.upgrade_weapon_cost = true;
								mech.destroySelectionHexes();
								mech.allowSelectionHexesDraw();
							}else
								print ("no good!");  


							break;
				
						case Menu.MechUpgrade3:
							//see if we can apply upgrade legs
		    				if(part_check(button_style_number, ref parts_count_for_other_upgrades)){
								mech.upgrade_armor_3 = true;
								mech.destroySelectionHexes();
								mech.allowSelectionHexesDraw();
							}else
								print ("no good!");
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
	private bool part_check_for_base(int row_number, ref int [,] parts_array, BaseUpgrade upgrade_type){
		if(
			(entityMechS.getPartCount(Part.Piston) 	>= parts_array[row_number,0]) &&
			(entityMechS.getPartCount(Part.Gear)	>= parts_array[row_number,1]) &&
			(entityMechS.getPartCount(Part.Plate) 	>= parts_array[row_number,2]) &&
			(entityMechS.getPartCount(Part.Strut) 	>= parts_array[row_number,3]))
		{
			//get base and check to see if this upgrade can be applied or already has been applied
			entityBaseS base_s = entityManagerS.getBase();
			if(base_s.upgradeBase(upgrade_type)){
				entityMechS.adjustPartCount(Part.Piston,	-parts_array[row_number,0]);
				entityMechS.adjustPartCount(Part.Gear, 		-parts_array[row_number,1]);
				entityMechS.adjustPartCount(Part.Plate,	 	-parts_array[row_number,2]);
				entityMechS.adjustPartCount(Part.Strut, 	-parts_array[row_number,3]);
				return true;
			}
		}  
		return false;
	}
	
	private void healthMenu() {
		GUI.skin = healthMenuSkin;
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 

	    //Layout start
	    GUI.BeginGroup(new Rect(window_size_x, window_size_y, window_size_width, window_size_height));
		
		//The Menu background box
	    GUI.Box(new Rect(0, 0, window_size_width, window_size_height), "");
		
		int label_x_start  = window_size_width/6; 
	    int button_x_start  = window_size_width/11; 
	    int button_y_start  = window_size_height/4 + window_size_height/15; 
		int button_size_width  = window_size_width/6; 
	    int button_size_height  = window_size_height/8; 
	    int button_spacing  = button_size_height/2; 
		
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
			if(entityMechS.getPartCount(Part.Piston) > 0 && mech.current_hp < mech.getMaxHP()){	
				entityMechS.adjustPartCount(Part.Piston,	-1);
				mech.current_hp ++;
			}
	    }
		
		//regenerate button 1
	    if(GUI.Button(new Rect(button_x_start + (button_spacing) + (button_size_width), button_y_start, button_size_width, button_size_height), "" ,gear_button_style)) {
			if(entityMechS.getPartCount(Part.Gear) > 0 && mech.current_hp < mech.getMaxHP()){
				entityMechS.adjustPartCount(Part.Gear, 		-1);
				mech.current_hp ++;
			}
	    }
		
		//regenerate button 2
	    if(GUI.Button(new Rect(button_x_start + (2 * button_spacing) + (2 * button_size_width), button_y_start, button_size_width, button_size_height), "" ,plate_button_style)) {
			if(entityMechS.getPartCount(Part.Plate) > 0 && mech.current_hp < mech.getMaxHP()){
				entityMechS.adjustPartCount(Part.Plate,	 	-1);
				mech.current_hp ++;
			}
	    }
		
		//regenerate button 3
	    if(GUI.Button(new Rect(button_x_start + (3 * button_spacing) + (3 * button_size_width), button_y_start, button_size_width, button_size_height), "" ,strut_button_style)) {
			if(entityMechS.getPartCount(Part.Strut) > 0 && mech.current_hp < mech.getMaxHP()){
				entityMechS.adjustPartCount(Part.Strut, 	-1);
				mech.current_hp ++;
			}
	    }
		
		//Logo Pictures to show which upgrade has occured, 3 dots or something that will be highlighted when upgrade has ben applied
	    GUI.Label(new Rect(label_x_start, button_y_start + (button_size_height), button_size_width, button_size_height), "" + entityMechS.getPartCount(Part.Piston));
	    GUI.Label(new Rect(label_x_start + (button_spacing) + (button_size_width), button_y_start + (button_size_height), button_size_width, button_size_height), "" + entityMechS.getPartCount(Part.Gear));
	    GUI.Label(new Rect(label_x_start + (2 * button_spacing) + (2 * button_size_width), button_y_start + (button_size_height), button_size_width, button_size_height), "" + entityMechS.getPartCount(Part.Plate));
		GUI.Label(new Rect(label_x_start + (3 * button_spacing) + (3 * button_size_width), button_y_start + (button_size_height), button_size_width, button_size_height), "" + entityMechS.getPartCount(Part.Strut));
		
		GUI.Label(new Rect(label_x_start + (2 * button_spacing) + (button_size_width), button_y_start + (button_size_height * 2), button_size_width, button_size_height), "Current HP = " + mech.current_hp);
		
		GUI.EndGroup();
	}
	
	private void objectiveMenu() {
		GUI.skin = objectiveMenuSkin;
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 

	    //Layout start
	    GUI.BeginGroup(new Rect(window_size_x, window_size_y, window_size_width, window_size_height));
		
		//The Menu background box
	    GUI.Box(new Rect(0, 0, window_size_width, window_size_height), "");
		
		int label_x_start  = window_size_width/12; 
	    int button_x_start  = window_size_width/11; 
	    int button_y_start  = window_size_height/10 + window_size_height/15; 
		int button_size_width  = window_size_width - window_size_width/6 - window_size_width/40; 
	    int button_size_height  = window_size_height/8; 
	    int button_spacing  = button_size_height/2; 
		
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
		
		//objective button 0
	    if(GUI.Button(new Rect(button_x_start, button_y_start, button_size_width, button_size_height), "objective" )) {
	    }
		
		//objective button 1
	    if(GUI.Button(new Rect(button_x_start , button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "objective" )) {
	    }
		
		//objective button 2
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "objective" )) {
	    }
		
		//objective button 3
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), "objective" )) {
	    }
		
		GUI.EndGroup();
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
    				theUpgradeMenu(goggles_upgrade_style,other_upgrade_style1,other_upgrade_style2);
					break;	
			
				case Menu.HealthUpgrade:
					//run the upgrade menu script
    				healthMenu();
			   		break;

				case Menu.Objective:
					//TODO:
			        objectiveMenu();
					break;
			
				default:
					//Default menu
					defaultMenu();
					break;
		}


	}
}
