using UnityEngine;
using System.Collections;

public class popUpMenu : MonoBehaviour {
	private bool activate = false;
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
	
	//Pick a menu
	public int level_choice = 0;
		
	//Window size
	private int screen_size_x;
	private int screen_size_y;
	
	//Update is called once per frame
	void Update () {
		screen_size_x = Screen.width;
    	screen_size_y = Screen.height;
		
	    if(Input.GetKey("o")) {
			activate = true; //turn on pop up menu if o is hit				
	    }
	}
	
	void OnGUI(){
		
		if (activate) { //display objective menu if objective hit.
	        //GUI.Box(/*Rect, "Pop up box", style*/);
			objectiveMenu();
		}
	}
	
	private void objectiveMenu() {
		int window_size_x  = screen_size_x /4 - screen_size_x /16; 
	    int window_size_y  = screen_size_y /14; 
		int window_size_width  = screen_size_x - (screen_size_x /3 + screen_size_x /20); 
	    int window_size_height  = screen_size_y - screen_size_y /8; 

	    //The Menu background box
		GUI.Box(new Rect(screen_size_x /4 - screen_size_x /16, screen_size_y /14, window_size_width, window_size_height), "", backboard);
	
		//Game resume button (close upgrade menu)
	    if(GUI.Button(new Rect(window_size_x + window_size_width - window_size_width/12, window_size_y + window_size_height/20, window_size_width/20, window_size_height/20), "X", x_button_style)) {;
		    //disable pop up menu
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
	
	//Get descriptions for buttons based on level choice
	private void getDescriptions(){
		switch(level_choice){
			case 0:
				title = "OBJECTIVES";
				objective_text_zero = "Each objective can be two lines in length \n" +
					"total of seven";
				objective_text_one = "";
				objective_text_two = "";
				objective_text_three = "";
				objective_text_four = "";
				objective_text_five = "";
				objective_text_six = "";
	
				break;
	
			case 1:
			    title = "OBJECTIVES";
				objective_text_zero = "...";
				objective_text_one = "....";
				objective_text_two = "";
				objective_text_three = "";
				objective_text_four = "";
				objective_text_five = "";
				objective_text_six = "";
				break;
			
			case 2:
			    title = "OBJECTIVES";
				objective_text_zero = "...";
				objective_text_one = "....";
				objective_text_two = "....";
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
