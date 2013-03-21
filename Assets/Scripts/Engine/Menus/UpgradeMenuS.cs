using UnityEngine;
using System.Collections;

public class UpgradeMenuS : MonoBehaviour {
	//Menu Skin
	public GUISkin newSkin;
	
	//styles
	public GUIStyle default_style;
	
	//styles
	public GUIStyle fin_upgrade_style;
	public GUIStyle goggles_upgrade_style;
	public GUIStyle gun_upgrade_style;
	public GUIStyle default_button_style;
	
	//Window size
	private int screen_size_x;
	private int screen_size_y;
	
	//Pick a menu
	public Menu menu_choice = Menu.Default;
	
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
	
	private void theUpgradeMenu(GUIStyle button_one_style, GUIStyle button_two_style, GUIStyle button_three_style, GUIStyle button_four_style) {
	    int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 
	    
	    //Layout start
	    GUI.BeginGroup(new Rect(window_size_x, window_size_y, window_size_width, window_size_height));
	    
	    //The Menu background box
	    GUI.Box(new Rect(0, 0, window_size_width, window_size_height), "");
	   
	    //Game resume button (close upgrade menu)
	    if(GUI.Button(new Rect(window_size_width - window_size_width/12, window_size_height/20, window_size_width/20, window_size_height/20), "", button_four_style)) {
		    //resume the game
		    Time.timeScale = 1;
		    inPlayMenuS script =  GetComponent<inPlayMenuS>();
    		script.enabled = true;
		    //disable pause menu
		    UpgradeMenuS script2 =  GetComponent<UpgradeMenuS>();
    		script2.enabled = false;;
	    }
	    
	    //Exit button (Go to Main Menu)
		//    if(GUI.Button(new Rect(0, 0, window_size_width/20, window_size_height/20), "Exit")) {
		//    	//TODO:
		//    	//Application.LoadLevel(0); //go to Main menu, we don't have one
		//    }
	    
	    //Upgrade Menu buttons: just change these variables for changes to take place to all buttons
	    //all variables must use window size variables
	    int label_x_start  = window_size_width/2; 
	    int button_x_start  = window_size_width/9; 
	    int button_y_start  = window_size_height/4 + window_size_height/15; 
		int button_size_width  = window_size_width/6; 
	    int button_size_height  = window_size_height/8; 
	    int button_spacing  = button_size_height/2; 
	    
	    //upgrade button 1
	    if(GUI.Button(new Rect(button_x_start, button_y_start, button_size_width, button_size_height), "" ,button_one_style)) {
			canApplyUpgrade(1);
	    }
	   
	    //upgrade button 2
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "", button_two_style)) {
	    	canApplyUpgrade(2);
	    	
	    }
	   
	    //upgrade button 3
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "", button_three_style)) {
	    	canApplyUpgrade(3);
	    }
	    
//	    //upgrade button 4
//	    if(GUI.Button(new Rect(button_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), "Updgrade 4")) {
//	    	canApplyUpgrade(4);
//	    }
	    
	    //Logo Pictures to show which upgrade has occured, 3 dots or something that will be highlighted when upgrade has ben applied
	    GUI.Label(new Rect(label_x_start, button_y_start, button_size_width, button_size_height), "Image 1");
	    GUI.Label(new Rect(label_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "Image 2");
	    GUI.Label(new Rect(label_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "Image 3");
//	    GUI.Label(new Rect(label_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), "Image 4");
	    
	    //layout end
	    GUI.EndGroup();
		

		
		//ADD THESE BUTTONS and put conditions in method: canApplyUpgrade(int button_style_number)
//		
//		
//		if(GUI.Button(new Rect(gui_spacing + 180 + gui_spacing , gui_spacing, 180, 40), "Climbing Hooks 5gr | 2pst"))
//		{  
//			
//				if(
//					(entityMechS.getPartCount(Part.Piston) >= 2) && 
//					(entityMechS.getPartCount(Part.Gear) >= 5))
//				{
//					entityMechS.adjustPartCount(Part.Piston, -2);
//					entityMechS.adjustPartCount(Part.Gear, -5);
//					mech.upgrade_traverse_mountain = true;
//					mech.destroySelectionHexes();
//					mech.allowSelectionHexesDraw();
//				}  
//			else
//					print ("no good!");
//		}
//		
//		if(GUI.Button(new Rect(gui_spacing + 180 + gui_spacing + 180 + gui_spacing, gui_spacing, 180, 40), "Leg Speed 3pst | 3str"))
//		{   
//				if(
//					(entityMechS.getPartCount(Part.Piston) >= 3) && 
//					(entityMechS.getPartCount(Part.Strut) >= 3))
//				{
//					entityMechS.adjustPartCount(Part.Piston, -3);
//					entityMechS.adjustPartCount(Part.Strut, -3);
//					mech.upgrade_traverse_cost = true;
//					mech.destroySelectionHexes();
//					mech.allowSelectionHexesDraw();
//				}  
//			else
//					print ("no good!");
//		}
		
	}
	
		//see if an upgrade can be applied
	private void canApplyUpgrade(int button_style_number){
		//find the button style number and then menu that was chosen and see if upgrade can be applied
		switch(button_style_number){
			case 1:
				switch(menu_choice){
						case Menu.BaseUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
					
						case Menu.MechUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
							
						case Menu.HealthUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
							 
						case Menu.TransportUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
											
						default:
							//see if we can apply upgrade
		    				print ("wrong menu");
							break;
				}
			break;
			
			case 2:
				switch(menu_choice){
						case Menu.BaseUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
					
						case Menu.MechUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
							
						case Menu.HealthUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
							 
						case Menu.TransportUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
											
						default:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
				}
			break;
			
			case 3:
				switch(menu_choice){
						case Menu.BaseUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
					
						case Menu.MechUpgrade:
							//see if we can apply upgrade
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
							break;
							
						case Menu.HealthUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
								print ("no good!");
				
							break;
							 
						case Menu.TransportUpgrade:
							//see if we can apply upgrade
		    				print ("TODO");
							if(
								false){
								print ("upgrade?");
							}  
							else
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
	
	private void defaultMenu() {
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 
	    
	    //Layout start
	    GUI.BeginGroup(new Rect(window_size_x, window_size_y, window_size_width, window_size_height));
	   
	    //Game resume button (close upgrade menu)
	    if(GUI.Button(new Rect(0, 0, window_size_width, window_size_height), "", default_style)) {
		    //resume the game
		    Time.timeScale = 1;
		    inPlayMenuS script =  GetComponent<inPlayMenuS>();
    		script.enabled = true;
		    //disable pause menu
		    UpgradeMenuS script2 =  GetComponent<UpgradeMenuS>();
    		script2.enabled = false;;
	    }
		
	}

	
	void OnGUI(){
		//load GUI skin
    	GUI.skin = newSkin;
   
    	//show the mouse cursor
    	Screen.showCursor = true;
   
		//choose a menu and run script
		chooseMenu();
	}
	
	//menu_choice must be set by calling script
	private void chooseMenu(){
		switch(menu_choice){
				case Menu.BaseUpgrade:
					//run the upgrade menu script
    				theUpgradeMenu(goggles_upgrade_style,gun_upgrade_style,fin_upgrade_style,default_button_style);
					break;
			
				case Menu.MechUpgrade:
					//run the upgrade menu script
    				theUpgradeMenu(goggles_upgrade_style,gun_upgrade_style,fin_upgrade_style,default_button_style);
					break;
					
				case Menu.HealthUpgrade:
					//run the upgrade menu script
    				theUpgradeMenu(goggles_upgrade_style,gun_upgrade_style,fin_upgrade_style,default_button_style);
			   		break;
					 
				case Menu.TransportUpgrade:
					//run the upgrade menu script
    				theUpgradeMenu(goggles_upgrade_style,gun_upgrade_style,fin_upgrade_style,default_button_style);
					break;
	
				case Menu.Objective:
					//TODO:
			        defaultMenu();
					break;
									
				default:
					//Default menu
					defaultMenu();
					break;
		}
	
	}
}
