using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gameManagerS : MonoBehaviour {
	
	public static Turn current_turn;
	public static int  current_round;
	public static Level current_level;
	
	
	public AudioClip _level_complete_screen_sound;
	public AudioClip _fanfare_success;
	public AudioClip _fanfare_failure;
	
	
	public static AudioClip level_complete_screen_sound;
	public static AudioClip fanfare_success;
	public static AudioClip fanfare_failure;
	
	public static bool mouse_over_gui = false;
	
	public GameObject  selection_hex_input;
	public static GameObject selection_hex;
	
	public static List<entityEnemyS>.Enumerator	enemy_enumerator;
	public static bool 	enemy_currently_acting = false;
	
	public static bool rebuilt_enemy_lcoations = false;
	public static bool spawned_enemies_this_round = false;
	
	void Awake()
	{
		selection_hex = selection_hex_input;
		current_turn  = Turn.Player;
		current_round = 1;
		
		level_complete_screen_sound = _level_complete_screen_sound;
		fanfare_success = _fanfare_success;
		fanfare_failure = _fanfare_failure;
	}
	
	// Use this for initialization
	void Start () {
		hexManagerS.setNodePresenseOnHexes();
	}
	 
	public bool posted_this_round = false;
	public static float time_after_shot_start = 0F;
	public static bool waiting_after_shot = false;
	
	
	public void tutorialMessages(){
		   
	}
	
	
	
	 
	
	
	void checkVictoryConditions()
	{ 
		switch(current_level)
		{
			case Level.Level0: 
				if(entityManagerS.getBase().wall_level >= BaseUpgradeLevel.Level1)
				{
					endGame(true);
				}
			break;
			case Level.Level1: 
				if(entityManagerS.getEnemies().Count == 0)
				{
					endGame(true);
				}
			break;
			case Level.Level2: 
				if(current_round >= 41)
				{
					endGame(true);
				}
			break; 
		} 
	}
	
	
	// Update is called once per frame
	void Update () {
		
//		if(!posted_this_round && current_level == Level.Level0)
//			postMessageLevel0();
//		else
//		if(!posted_this_round && current_level == Level.Level1)
//			postMessageLevel1();
//		else
//		if(!posted_this_round && current_level == Level.Level2)
//			postMessageLevel2();
		
		checkVictoryConditions();
	
		//if the player still has actions 
		if(current_turn == Turn.Player) 
		{  
			if(!rebuilt_enemy_lcoations)
			{
				entityManagerS.getMech().updateAttackableEnemies();
				rebuilt_enemy_lcoations = true;
			} 
		}
		else if(current_turn == Turn.Base)
		{
		
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
					if(!waiting_after_shot)
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
							posted_this_round = false;
							entityManagerS.getMech().current_ap = entityManagerS.getMech().max_ap;
							entityManagerS.getMech().updateAttackableEnemies();
							current_round++;
						}	
						
					}
					else
					{
						if(time_after_shot_start + .8F < Time.time)
						{
							waiting_after_shot = false;
						}
					}
				}
			}
		}
	}
	
	public static void forcePlayerTurn(){
		current_turn = Turn.Player;
		entityManagerS.getMech().current_ap = entityManagerS.getMech().max_ap;
	}
	
	
	 
	
	
	public Texture completescreen_left;
	public Texture completescreen_right;
	public Texture completescreen_center;
	public Texture completescreen_center_victory;
	public Texture completescreen_center_defeat;
	
	public bool won_the_game = false;
	
	private float completescreen_start_time = 0; 
	private float completescreen_animation_duration = .55F;
	private float completescreen_animation_duration_center = .45F; 
	
	private float complete_left_origin_lerp  = -335;
	private float complete_right_origin_lerp = Screen.width;
	
	private float complete_left_destin_lerp  = 0;
	private float complete_right_destin_lerp = Screen.width - 335;
	
	private float complete_center_origin_lerp = -366;
	private float complete_center_destin_lerp  = 190;
	
	private void lerpInCompletedSides()
	{ 
		float current_t_param = (Time.time - completescreen_start_time)/completescreen_animation_duration; 
		if(current_t_param < 1)
		{
			float result = Mathf.SmoothStep(complete_left_origin_lerp,complete_left_destin_lerp, current_t_param);
			left_complete_rect = new Rect(result, 247, 335,247);  
			
			result = Mathf.SmoothStep(complete_right_origin_lerp,complete_right_destin_lerp, current_t_param);
			right_complete_rect = new Rect(result, 247, 335,247);  
		} 
	}
	
	private void lerpInCompletedCenter()
	{ 
		float current_t_param = (Time.time - completescreen_start_time)/completescreen_animation_duration_center; 
		if(current_t_param < 1)
		{
			float result = Mathf.SmoothStep(complete_center_origin_lerp,complete_center_destin_lerp, current_t_param);
			center_complete_rect = new Rect(252, result, 776,366);   
		} 
		else
			center_complete_rect = new Rect(252, 190, 776,366);   
	}
	
	private Rect left_complete_rect;
	private Rect right_complete_rect;
	private Rect center_complete_rect = new Rect(252, -366, 776,366);   
	 
	
	private void drawLevelOverScreen(){ 
		
		lerpInCompletedSides();
		
		GUI.DrawTexture(left_complete_rect, completescreen_left);	 
		GUI.DrawTexture(right_complete_rect, completescreen_right);
		
		lerpInCompletedCenter();
		GUI.BeginGroup(center_complete_rect);
			GUI.DrawTexture(new Rect(0,0,776,366), completescreen_center);	
			GUI.DrawTexture(new Rect(0,119, 776, 92), won_the_game ? completescreen_center_victory : completescreen_center_defeat);	
		
			//Game Info
			ShadowAndOutline.DrawOutline(new Rect(0, 54 ,776,19), level_complete_top_stats, enginePlayerS.gui_norm_text_static, new Color(0,0,0,.6F),Color.white, 3F);
			ShadowAndOutline.DrawOutline(new Rect(0, 80 ,776,19), level_complete_bottom_stats, enginePlayerS.gui_norm_text_static, new Color(0,0,0,.6F),Color.white, 3F);
		
			if(GUI.Button(new Rect(223,  250, 330, 40),"",enginePlayerS.HUD_button_static))
				endGame(Time.time % 2 == 1);
			ShadowAndOutline.DrawOutline(new Rect(223,  250, 330, 40), won_the_game ? "Next Level" : "Restart", enginePlayerS.gui_norm_text_static, new Color(0,0,0,.6F),Color.white, 3F);
				
		GUI.EndGroup();
	}
		
    private static bool game_is_over;
	private static string level_complete_top_stats;
	private static string level_complete_bottom_stats;
	
	public void endGame(bool victory)
	{
		won_the_game = victory;
		level_complete_top_stats =  "Parts Collected: 13   |   " +
									"Upgrades Built: 12   |   " +
									"Enemies Killed: 61";
		level_complete_bottom_stats = "Time Elapsed: " + (int)Time.time/60 + ":" + (int)Time.time%60;
 		completescreen_start_time = Time.time;  
		game_is_over = true; 
		audio.PlayOneShot(level_complete_screen_sound);
	}
	
	void OnGUI()
	{  
		if(game_is_over)
			drawLevelOverScreen();
		
		if(Input.GetKeyDown(KeyCode.F10))
		{
			endGame((int)Time.time % 2 == 1);
		}
		
		if(Input.GetKeyDown(KeyCode.F9))
		{
			entityMechS.adjustPartCount(Part.Gear, 2000);
			entityMechS.adjustPartCount(Part.Piston, 2000);
			entityMechS.adjustPartCount(Part.Plate, 2000);
			entityMechS.adjustPartCount(Part.Strut, 2000);
		}
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
		entityManagerS.getMech().current_ap =  0;
		
	}
}
