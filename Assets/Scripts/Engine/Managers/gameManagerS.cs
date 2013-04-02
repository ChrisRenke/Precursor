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
	
	
	void Awake()
	{
		selection_hex = selection_hex_input;
		current_turn  = Turn.Player;
		current_round = 1;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	private int spawn_tries = 0;
	
	// Update is called once per frame
	void Update () {
	
		//if the player still has actions
//		if(entityManagerS.getMech().current_ap > 0)
		if(current_turn == Turn.Player || current_turn == Turn.Base)
		{ 
			spawn_tries = 0;
//			current_turn  = Turn.Player;
			//TODO nothing hahah
		}
		else if(current_turn == Turn.Enemy)
		{ 
			if(spawn_tries < 5 )
			{
				entityManagerS.spawnNewEnemy();
				spawn_tries++;
			}
			
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
					entityManagerS.getMech().current_ap = entityManagerS.getMech().max_ap;
				}
			}
		}
	}
	
	
	void OnGUI()
	{
	    GUI.Label(new Rect(Screen.width - 30 - 180, 30, 180, 30), current_turn == Turn.Player ? "Player Turn" : "Enemy Turn",  enginePlayerS.gui_norm_text_static);
    
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
//<<<<<<< HEAD
//		current_turn = Turn.Enemy;
////		entityManagerS.getMech().current_ap =  entityManagerS.getMech().max_ap;
//=======
////		current_turn = Turn.Enemy;
//		entityManagerS.getMech().current_ap =  entityManagerS.getMech().max_ap;
//		entityManagerS.getMech().destroySelectionHexes();
//		entityManagerS.getMech().allowSelectionHexesDraw();
//		enemy_enumerator = entityManagerS.getEnemies().GetEnumerator();
		
		
		current_turn = Turn.Base;
		entityManagerS.getMech().current_ap =  0;
		entityManagerS.getMech().destroySelectionHexes();
		entityManagerS.getMech().allowSelectionHexesDraw();
		
	}
	
	public static void endBaseTurn()
	{
		//TODO: not sure if this is finished
		current_turn = Turn.Enemy;
		entityManagerS.getBase().current_ap =  entityManagerS.getBase().max_ap;
		enemy_enumerator = entityManagerS.getEnemies().GetEnumerator();
		entityManagerS.getMech().current_ap =  0;
		
	}
}
