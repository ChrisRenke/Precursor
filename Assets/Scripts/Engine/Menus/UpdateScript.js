//Display Play Menu Screens

//text style/color, set in inspector	
var gui_normal_text: GUIStyle;

//hp bars
var ap_bar : Texture2D;
var hp_bar : Texture2D;

//menu backboards
var hp_backboard : Texture2D;
var parts_backboard : Texture2D;
var top_menu_backboard : Texture2D;
var upgrade_backboard : Texture2D;	

//buttons
var base_button : Texture2D;
var end_turn_button : Texture2D;
var health_button : Texture2D;
var mech_button : Texture2D;
var star_button : Texture2D;
var transport_button : Texture2D;

//window size
private var screen_size_x : int;
private var screen_size_y : int;

function Update(){
	screen_size_x = Screen.width;
    screen_size_y = Screen.height;
 
}

function OnGUI(){
	 //All Backboard variables must be based on "screen_size_x" variable and "screen_size_y" variable
	 var button_x_start : int = screen_size_x/8; 
     var button_y_start : int = screen_size_y/6; 
	 var button_size_width : int = screen_size_x/4; 
     var button_size_height : int = screen_size_y/9; 
     var button_spacing : int = button_size_height/2; 
     
	 //Base Button
	 if(GUI.Button(Rect(0, 0, 500, 100), base_button)) {
	      //pause the game
	    Time.timeScale = 0;
	    //show the pause menu
	    var script = GetComponent("UpgradeMenuS");
	    script.enabled = true;
	 }
	 
	 //Mech Button  
	 if(GUI.Button(Rect(0, 0, 0, 0), mech_button)) {
	      //pause the game
	    Time.timeScale = 0;
	    //show the pause menu
	    var script1 = GetComponent("UpgradeMenuS");
	    script1.enabled = true;
	 }
	 
	 //Health button
	 if(GUI.Button(Rect(0, 0, 0, 0), health_button)) {
	      //pause the game
	    Time.timeScale = 0;
	    //show the pause menu
	    var script2 = GetComponent("UpgradeMenuS");
	    script2.enabled = true;
	 }
	 
	 //Transport button
	 if(GUI.Button(Rect(0, 0, 0, 0), transport_button)) {
	      //pause the game
	    Time.timeScale = 0;
	    //show the pause menu
	    var script3 = GetComponent("UpgradeMenuS");
	    script3.enabled = true;
	 }
	 
	 //End Turn Button
	 if(GUI.Button(Rect(0, 0, 100, 100), end_turn_button)) {
	      //pause the game
	    Time.timeScale = 0;
	    //show the pause menu
	    var script4 = GetComponent("UpgradeMenuS");
	    script4.enabled = true;
	 }
	 
	 //add variables here for adjusting these labels just base everything off of screen size
	GUI.DrawTexture(Rect(400, 0, 450, 105), top_menu_backboard, ScaleMode.ScaleToFit, true);
    GUI.DrawTexture(Rect(-150, 10 * 55, 540, 120), upgrade_backboard, ScaleMode.ScaleToFit, true);
	GUI.DrawTexture(Rect(830, 10 * 53, 630, 140), parts_backboard, ScaleMode.ScaleToFit, true);
	GUI.DrawTexture(Rect(325, 10 * 53, 630, 140), hp_backboard, ScaleMode.ScaleToFit, true);
	GUI.DrawTexture(Rect(553, 10 * 59, 180, 20), hp_bar, ScaleMode.ScaleToFit, true);
	GUI.DrawTexture(Rect(553, 10 * 62, 180, 20), ap_bar, ScaleMode.ScaleToFit, true);

   
    GUI.Label(Rect(Screen.width - (1 * (10 + 80)), Screen.height - (10 + 80), 80, 80), "AP",  gui_normal_text);
	GUI.Label(Rect(Screen.width - (2 * (10 + 80)), Screen.height - (10 + 80), 80, 80), "HP",  gui_normal_text);
		

}
