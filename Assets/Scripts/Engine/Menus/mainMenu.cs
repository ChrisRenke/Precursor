using UnityEngine;
using System.Collections;

public class mainMenu : MonoBehaviour {
	public GUISkin objectiveMenuSkin;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//Objective Menu 
	private void OnGUI(){
		GUI.skin = objectiveMenuSkin;
		int window_size_x  = 0; 
	    int window_size_y  = 0; 
		int window_size_width  = Screen.width; 
	    int window_size_height  = Screen.height; 
		
		//The Menu background box
	    GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
		
		int label_x_start  = window_size_width/3; 
	    int button_x_start  = window_size_width/3 + window_size_width/4; 
	    int button_y_start  = window_size_height/2 - window_size_width/10; 
		int button_size_width  = window_size_width/2 - window_size_width/4 - window_size_width/40; 
	    int button_size_height = window_size_height/12; 
	    int button_spacing  = button_size_height/5; 
		
		//Continue
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (button_spacing) + (button_size_height), button_size_width, button_size_height), "Continue")) {
			Application.LoadLevel(1);
	    }
		
		//New Game
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (2 * button_spacing) + (2 * button_size_height), button_size_width, button_size_height), "New Game")) {
			Application.LoadLevel(1);
	    } 
		
		//Quit
	    if(GUI.Button(new Rect(button_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), "Quit" )) {
			Application.Quit();
	    }
		
//		//not present at the moment...
//	    if(GUI.Button(new Rect(button_x_start, button_y_start + (3 * button_spacing) + (3 * button_size_height), button_size_width, button_size_height), "" )) {
//	    }
		
	}
	
	
}
