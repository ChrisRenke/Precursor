using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gameManagerS : MonoBehaviour {
	
	public static Turn current_turn;
	public static int  current_round;
	
	public GameObject  selection_hex_input;
	public static GameObject selection_hex;
	
	public static List<entityEnemyS>.Enumerator	enemy_enumerator;
	public static bool 	  					enemy_currently_acting = false;
	
	public static bool rebuilt_enemy_lcoations = false;
	public static bool spawned_enemies_this_round = false;
	
	void Awake()
	{
		selection_hex = selection_hex_input;
		current_turn  = Turn.Player;
		current_round = 1;
	}
	
	// Use this for initialization
	void Start () {
		hexManagerS.setNodePresenseOnHexes();
	}
	 
	
	// Update is called once per frame
	void Update () {
	
		//if the player still has actions 
		if(current_turn == Turn.Player || current_turn == Turn.Base) 
		{  
			if(!rebuilt_enemy_lcoations)
			{
				entityManagerS.getMech().updateAttackableEnemies();
				rebuilt_enemy_lcoations = true;
			} 
		}
		else if(current_turn == Turn.Enemy)
		{ 
			if(!spawned_enemies_this_round)
			{
				entityManagerS.instantiateEnemiesFromSpawns();
				spawned_enemies_this_round = true;
				enemy_enumerator = entityManagerS.getEnemies().GetEnumerator();
			}
			else{
				 				
				if(!enemy_currently_acting)
				{
					
					if(enemy_enumerator.MoveNext())
			 		{
						entityEnemyS current_enemy = enemy_enumerator.Current;
						current_enemy.is_this_enemies_turn = true;
						enemy_currently_acting = true;
					}
					else //if there are no more enemies to move, then its the players turn again
					{
						foreach(entityEnemyS current_enemy in entityManagerS.enemy_list)
							current_enemy.current_ap = current_enemy.max_ap;
						current_turn = Turn.Player;
						spawned_enemies_this_round = false;
						entityManagerS.getMech().current_ap = entityManagerS.getMech().max_ap;
						entityManagerS.getMech().updateAttackableEnemies();
						current_round++;
					}
				}
			}
		}
	}
	
	
	void OnGUI()
	{ 
		if(entityBaseS.show_health_bar){
	    	GUI.Label(new Rect(Screen.width/2 - Screen.width/7 + Screen.width/30, 10, Screen.width/15, 30), current_turn.ToString() + " turn",  enginePlayerS.gui_norm_text_static);
    		GUI.Label(new Rect(Screen.width/2 - Screen.width/10 + Screen.width/6 + Screen.width/42, 10, Screen.width/15, 30), "Round " + current_round.ToString() , enginePlayerS.gui_norm_text_static); //+ current_round.ToString() + ""
		}
		 
//		if(GUI.Button(new Rect(Screen.width - 30 - 180, 70, 180, 30), "Force Player Turn"))
//		{
//			current_turn = Turn.Player;
//			entityManagerS.getMech().current_ap =  entityManagerS.getMech().max_ap;
//			
//					entityManagerS.getMech().destroySelectionHexes();
//					entityManagerS.getMech().allowSelectionHexesDraw();
//		}
		
//		if(GUI.Button(new Rect(Screen.width - 30 - 180, 70, 180, 30), "End Turn"))
//		{
//			endPlayerTurn();
//		}
		
//		if(GUI.Button(new Rect(gui_spacing, gui_spacing + 40  + gui_spacing , 180, 40), "End Turn"))
//		{
//			mech.current_ap = 0;
//			mech.destroySelectionHexes();
//			mech.allowSelectionHexesDraw();
//		}
		
	
	}
	
	public static void endPlayerTurn()
	{ 
		current_turn = Turn.Base;
		entityManagerS.getMech().current_ap =  0;  
	}
	
	public static void endBaseTurn()
	{
		//TODO: not sure if this is finished
		current_turn = Turn.Enemy;
		entityManagerS.getBase().current_ap =  entityManagerS.getBase().max_ap;
		entityManagerS.getMech().current_ap =  0;
		
	}
}
