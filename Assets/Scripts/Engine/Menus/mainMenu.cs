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
		 
		//Continue
	    if(GUI.Button(new Rect(490, 375, 300, 50), "Continue")) {
			Application.LoadLevel(PlayerPrefs.GetString("CONTINUE"));
	    }
		
		//New Game
	    if(GUI.Button(new Rect(490, 435, 300, 50), "New Game")) {
			PlayerPrefs.SetString("CONTINUE","Level0");
			Application.LoadLevel("Level0");
	    } 
		
		//Quit
	    if(GUI.Button(new Rect(490, 495, 300, 50), "Quit" )) {
			Application.Quit();
	    }
		 
		
	}
	
	
}
