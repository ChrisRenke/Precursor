using UnityEngine;
using System.Collections;

public class gameManagerS : MonoBehaviour {
	
	public static Turn current_turn;
	public static int  current_round;
	
	public GameObject  selection_hex_input;
	public static GameObject selection_hex;
	
	void Awake()
	{
		selection_hex = selection_hex_input;
		current_turn  = Turn.Player;
		current_round = 1;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		//if the player still has actions
		if(entityManagerS.getMech().current_ap > 0)
		{ 
			current_turn  = Turn.Player;
		}
		else
		{ 
			current_turn  = Turn.Enemy;
		}
	}
	
	void OnGUI()
	{
	    GUI.Label(new Rect(Screen.width - 30 - 180, 30, 180, 30), current_turn == Turn.Player ? "Player Turn" : "Enemy Turn",  enginePlayerS.gui_norm_text_static);
    
		if(GUI.Button(new Rect(Screen.width - 30 - 180, 70, 180, 30), "Force Player Turn"))
		{
			current_turn = Turn.Player;
			entityManagerS.getMech().current_ap =  entityManagerS.getMech().max_ap;
			
					entityManagerS.getMech().destroySelectionHexes();
					entityManagerS.getMech().allowSelectionHexesDraw();
		}
	
	}
}
