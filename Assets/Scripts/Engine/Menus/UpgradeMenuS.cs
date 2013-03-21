using UnityEngine;
using System.Collections;

public class UpgradeMenuS : MonoBehaviour {
	//Menu Skin
	public GUISkin newSkin;
	
	//window size
	private int screen_size_x;
	private int screen_size_y;
	
	//Update is called once per frame
	void Update () {
		screen_size_x = Screen.width;
    	screen_size_y = Screen.height;
		//Disable other UpdateScript so buttons don't show up
		inPlayMenuS script =  GetComponent<inPlayMenuS>();
    	script.enabled = false;
	}
	
	private void theUpgradeMenu() {
	    int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 
	    
	    //Layout start
	    GUI.BeginGroup(new Rect(window_size_x, window_size_y, window_size_width, window_size_height));
	    
	    //The Menu background box
	    GUI.Box(new Rect(0, 0, window_size_width, window_size_height), "");
	   
	    //Game resume button (close upgrade menu)
	    if(GUI.Button(new Rect(window_size_width - window_size_width/12, window_size_height/20, window_size_width/20, window_size_height/20), "X")) {
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
	    int label_x_start  = window_size_width/2 +  window_size_width/8; 
	    int button_x_start  = window_size_width/8; 
	    int button_y_start  = window_size_height/6; 
		int button_size_width  = window_size_width/4; 
	    int button_size_height  = window_size_height/9; 
	    int button_spacing  = button_size_height/2; 
	    
	    //upgrade button 1
	    if(GUI.Button(new Rect(button_x_start, button_y_start, button_size_width, button_size_height), "Upgrade 1")) {
			//TODO
	    }
	   
	    //upgrade button 2
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "Upgrade 2")) {
	    	//TODO: upgrade
	    	
	    }
	   
	    //upgrade button 3
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "Upgrade 3")) {
	    	//TODO; upgrade
	    }
	    
	    //upgrade button 4
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), "Updgrade 4")) {
	    	//TODO: upgrade
	    }
	    
	    //Logo Pictures to show which upgrade has occured, 3 dots or something that will be highlighted when upgrade has ben applied
	    GUI.Label(new Rect(label_x_start, button_y_start, button_size_width, button_size_height), "Image 1");
	    GUI.Label(new Rect(label_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "Image 2");
	    GUI.Label(new Rect(label_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "Image 3");
	    GUI.Label(new Rect(label_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), "Image 4");
	    
	    //layout end
	    GUI.EndGroup();
	}

	
	void OnGUI(){
		//load GUI skin
    	GUI.skin = newSkin;
   
    	//show the mouse cursor
    	Screen.showCursor = true;
   
    	//run the upgrade menu script
    	theUpgradeMenu();
	}
}
