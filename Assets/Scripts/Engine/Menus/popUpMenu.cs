using UnityEngine;
using System.Collections;

public class popUpMenu : MonoBehaviour {
	private bool activate = false; //objective menu
	public bool upgrade_popup = true;
	public bool custom_popup = false;
	public bool game_over_popup = false;
	
	public bool load_level_popup = false;
	public string level_name;
	
	//Pick a menu
	public int level_choice = 0;
	public int text_number_choice = 0;
	
	//styles
	public GUIStyle backboard;
	public GUIStyle objective_button_style;
	public GUIStyle star_button_style;
	public GUIStyle x_button_style;
	public GUIStyle title_style;
	
	//Hold the objectives for each button
	string title = "";
	string objective_text_zero = "";
	string objective_text_one = "";
	string objective_text_two = "";
	string objective_text_three = "";
    string objective_text_four = "";
	string objective_text_five = "";
	string objective_text_six = "";
	
	//custom variables
	public string custom_text = "";
	public Rect custom_rect = new Rect(0,0,0,0);
	public GUIStyle custom_backboard;
		
	//Window size
	private int screen_size_x;
	private int screen_size_y;
	private float savedTimeScale;
	
	//public Texture chris_parts;
	//Get PopUp Menu
	popUpMenu script_popup;
	
	void Start(){
		upgrade_popup = false;
		text_number_choice = 0;
		level_choice = 0;
		activate = false;
		custom_popup = false;
		script_popup = GetComponent<popUpMenu>();
	}
	
	//Update is called once per frame
	void Update () {
		screen_size_x = Screen.width;
    	screen_size_y = Screen.height;
		
	    if(Input.GetKey("o")) {
			activate = true; //turn on pop up menu if o is hit				
	    }
		
	}
	
	public void activateMenu()
	{
		activate = true;
	}
	
	void OnGUI(){
		//display objective menu if objective hit
		if (activate)
			objectiveMenu();
		
		if(upgrade_popup){
			upgradePopUpBox(text_number_choice);
		    UpgradeMenuS script2 =  GetComponent<UpgradeMenuS>();
			if(script2.enabled == false || script2.menu_choice == Menu.Objective)
				upgrade_popup = false;
		}
		
		if(custom_popup){
			//pause the game
			savedTimeScale = Time.timeScale;
		    Time.timeScale = 0;
		    //Show the pause menu
			customPopUpBox(custom_text,ref custom_rect,custom_backboard);
		}
		
		if(game_over_popup){
			gameOverPopup(custom_text,ref custom_rect,backboard);
		}
		
		if(load_level_popup)
			loadLevelPopup(custom_text,ref custom_rect,backboard);
		
		
//		GUI.DrawTexture(new Rect(screen_size_x - 245, screen_size_y - 104, 245, 104 ), chris_parts, ScaleMode.StretchToFill);
//		GUI.Label(new Rect(screen_size_x - 233, screen_size_y - 52, 43, 43 ), entityMechS.part_count[Part.Piston].ToString(), enginePlayerS.gui_norm_text_black_static);
//	 	GUI.Label(new Rect(screen_size_x - 173, screen_size_y - 52, 43, 43 ), entityMechS.part_count[Part.Gear].ToString(),   enginePlayerS.gui_norm_text_black_static);
//	 	GUI.Label(new Rect(screen_size_x - 112, screen_size_y - 52, 43, 43 ), entityMechS.part_count[Part.Plate].ToString(),  enginePlayerS.gui_norm_text_black_static);
//	 	GUI.Label(new Rect(screen_size_x - 52 , screen_size_y - 52, 43, 43 ), entityMechS.part_count[Part.Strut].ToString(),  enginePlayerS.gui_norm_text_black_static);
//			
			
		
	}
	
	private void objectiveMenu() {
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 

	    //The Menu background box
		if(GUI.Button(new Rect(screen_size_x /4 - screen_size_x /16, screen_size_y /14, window_size_width, window_size_height), "", backboard)){
			activate = false;
		}
		
		int label_x_start  = window_size_x + window_size_width/11;
		int label_width = window_size_width/15;
	    int button_x_start  = window_size_x + window_size_width/11 + label_width + label_width/10; 
	    int button_y_start  = window_size_y + window_size_height/10 + window_size_height/11; 
		int title_width  = (window_size_width - window_size_width/6 - window_size_width/40)/2;
		int button_size_width = window_size_width - window_size_width/6 - label_width - label_width/10;
	    int button_size_height  = window_size_height/15; 
	    int button_spacing  = button_size_height/2 + button_size_height/10; 
		int label_height = button_size_height - button_size_height/8;
		
		getDescriptions();
		
		//Title 0
		if(GUI.Button(new Rect(window_size_width/2 + window_size_width/10, window_size_y + window_size_height/10, title_width, window_size_height/15), title, title_style)){
		}

		//objective 0
		if(GUI.Button(new Rect(button_x_start, button_y_start, button_size_width, button_size_height), objective_text_zero, objective_button_style)){}
		if(!objective_text_zero.Equals(""))
			GUI.Button(new Rect(label_x_start, button_y_start, label_width, label_height), "", star_button_style);
		
		//objective 1
	    if(GUI.Button(new Rect(button_x_start , button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), objective_text_one, objective_button_style)){}
		if(!objective_text_one.Equals(""))
			GUI.Button(new Rect(label_x_start, button_y_start + (button_spacing) + (button_size_height), label_width, label_height), "", star_button_style);
		
		//objective 2
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), objective_text_two, objective_button_style)){}
		if(!objective_text_two.Equals(""))
			GUI.Button(new Rect(label_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), label_width, label_height), "", star_button_style);
		
		//objective 3
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), objective_text_three, objective_button_style)){}
		if(!objective_text_three.Equals(""))
			GUI.Button(new Rect(label_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), label_width, label_height), "", star_button_style);
		
		//objective 4
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (4 * button_spacing) + (4 * button_size_height), button_size_width, button_size_height), objective_text_four, objective_button_style)){}
		if(!objective_text_four.Equals(""))
			GUI.Button(new Rect(label_x_start, button_y_start + (4 * button_spacing) + (4 * button_size_height), label_width, label_height), "", star_button_style);
		
		//objective 5
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (5 * button_spacing) + (5 * button_size_height), button_size_width, button_size_height), objective_text_five, objective_button_style)){}
		if(!objective_text_five.Equals(""))
			GUI.Button(new Rect(label_x_start, button_y_start + (5 * button_spacing) + (5 * button_size_height), label_width, label_height), "", star_button_style);
		
		//objective 6
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (6 * button_spacing) + (6 * button_size_height), button_size_width, button_size_height), objective_text_six, objective_button_style)){}
			if(!objective_text_six.Equals(""))
			GUI.Button(new Rect(label_x_start, button_y_start + (6 * button_spacing) + (6 * button_size_height), label_width, label_height), "", star_button_style);
	}
	
	public void upgradePopUpBox(int text_choice){
		switch(text_choice){
			case 0:
				custom_text = "You don't have enough ap";
	
				break;
	
			case 1:
				custom_text = "Your missing parts \n for this upgrade";
			
				break;
			
			case 2:
				custom_text = "You already have this upgrade";
			
				break;
			
			case 3:
				custom_text = "Your health is full";
			
				break;
			
			case 4:
				custom_text = "You don't have enough of this \n resource to regenerate your health";
			
				break;
			
			default:
				//use custom text			
				break;
		}
		
		int window_size_x  = screen_size_x /2 - screen_size_x /9 - screen_size_x /25; 
	    int window_size_y  = screen_size_y/2 - screen_size_y/10; 
		int window_size_width  = screen_size_x - (screen_size_x /2 + screen_size_x /5); 
	    int window_size_height  = screen_size_y/4; 

	    //The Pop Up box
		if(GUI.Button(new Rect(window_size_x, window_size_y, window_size_width, window_size_height), custom_text, backboard)){
			upgrade_popup = false;
		}
		
	}
	
	public void customPopUpBox(string text, ref Rect rec, GUIStyle style){
		//The Pop Up box
		
		if(GUI.Button(rec, text, style)){
			Time.timeScale = savedTimeScale; 
			custom_popup = false;
		}
	}
	
	public void gameOverPopup(string text, ref Rect rec, GUIStyle style){
		//The Pop Up box
		
		Debug.Log("Showing popup");
		if(GUI.Button(rec, text, style)){
			Application.LoadLevel("leveltoMenu"); 
		}
	}
		
	
	public void loadLevelPopup(string text, ref Rect rec, GUIStyle style){
		//The Pop Up box
		
		Debug.Log("Showing popup");
		if(GUI.Button(rec, text, style)){
			Application.LoadLevel(level_name); 
		}
	}
		
	
	//Get descriptions for buttons based on level choice
	private void getDescriptions(){
		switch((int)gameManagerS.current_level){
			case 0:
				title = "OBJECTIVES";
				objective_text_zero = "Learn the basics of movement.";
				objective_text_one = "Learn to scavenge parts.";
				objective_text_two = "Construct a wall upgrade for the town";
				objective_text_three = "";
				objective_text_four = "";
				objective_text_five = "";
				objective_text_six = "";
	
				break;
	
			case 1:
			    title = "OBJECTIVES";
				objective_text_zero = "Kill all three enemies on the map.";
				objective_text_one = "Construct a Climbing Hooks to reach the last enemy.";
				objective_text_two = "";
				objective_text_three = "";
				objective_text_four = "";
				objective_text_five = "";
				objective_text_six = "";
				break;
			
			case 2:
			    title = "OBJECTIVES";
				objective_text_zero = "You and your base survive 40 rounds!";
				objective_text_one = "";
				objective_text_two = "";
				objective_text_three = "";
				objective_text_four = "";
				objective_text_five = "";
				objective_text_six = "";
				break;
			
			case 3:
			    title = "OBJECTIVES";
				objective_text_zero = "...";
				objective_text_one = "....";
				objective_text_two = "....";
				objective_text_three = "...";
				objective_text_four = "";
				objective_text_five = "";
				objective_text_six = "";
				break;
			
			case 4:
			    title = "OBJECTIVES";
				objective_text_zero = "...";
				objective_text_one = "....";
				objective_text_two = "....";
				objective_text_three = "...";
				objective_text_four = "...";
				objective_text_five = "";
				objective_text_six = "";
				break;
			
			case 5:
			    title = "OBJECTIVES";
				objective_text_zero = "...";
				objective_text_one = "....";
				objective_text_two = "....";
				objective_text_three = "...";
				objective_text_four = "...";
				objective_text_five = "...";
				objective_text_six = "...";
				break;
			
			default:
				title = "OBJECTIVES";
				objective_text_zero = "";
				objective_text_one = "";
				objective_text_two = "";
				objective_text_three = "";
				objective_text_four = "";
				objective_text_five = "";
				objective_text_six = "";
				break;
		}
	}
}
