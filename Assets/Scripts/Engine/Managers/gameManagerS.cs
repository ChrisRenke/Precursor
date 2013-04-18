using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gameManagerS : MonoBehaviour {
	
	public Turn current_turn;
	public int  current_round;
	public Level current_level;
	
	
	public AudioClip _level_complete_screen_sound;
	public AudioClip _fanfare_success;
	public AudioClip _fanfare_failure;
	
	
	public static AudioClip level_complete_screen_sound;
	public static AudioClip fanfare_success;
	public static AudioClip fanfare_failure;
	
	public bool mouse_over_gui = false;
	
	
	public List<Enemy >.Enumerator	enemy_enumerator;
	public bool 	  					enemy_currently_acting = false;
	
	public  bool rebuilt_enemy_lcoations = false;
	public  bool spawned_enemies_this_round = false;
	
	public enginePlayerS ep;
	public entityManagerS em; 
	public hexManagerS hm; 
	
	void Awake(){ 
		ep = GameObject.Find("enginePlayer").GetComponent<enginePlayerS>();
		hm = GameObject.Find("engineHexManager").GetComponent<hexManagerS>();
	
		current_turn  = Turn.Player;
		current_round = 1;
		
		level_complete_screen_sound = _level_complete_screen_sound;
		fanfare_success = _fanfare_success;
		fanfare_failure = _fanfare_failure;
	}
	
	
	
	// Use this for initialization
	void Start () {
		hm.setNodePresenseOnHexes();
		em = GameObject.Find("engineEntityManager").GetComponent<entityManagerS>();
	}
	 
	public bool posted_this_round = false;
	public float time_after_shot_start = 0F;
	public bool waiting_after_shot = false;
	
	
	
	 
	
	void checkVictoryConditions()
	{ 
		switch(current_level)
		{
			case Level.Level0: 
				if(em.getBase().wall_level >= BaseUpgradeLevel.Level1)
				{
					endGame(true);
				}
			break;
			case Level.Level1: 
				if(em.killed_enemy_count >= 25)
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
		
		checkVictoryConditions();
	
		//if the player still has actions 
		if(current_turn == Turn.Player) 
		{  
			if(!rebuilt_enemy_lcoations)
			{
				entityMechS ems = em.getMech();
				ems.updateAttackableEnemies();
				rebuilt_enemy_lcoations = true;
			} 
		}
		else if(current_turn == Turn.Base)
		{
			//handled in base's Update() method
		}			
		else if(current_turn == Turn.Enemy)
		{ 
			if(!spawned_enemies_this_round)
			{
				em.instantiateEnemiesFromSpawns();
				spawned_enemies_this_round = true;
				enemy_enumerator = em.getEnemies().GetEnumerator();
			}
			else{
				 				
				if(!enemy_currently_acting)
				{
					if(!waiting_after_shot)
					{
					
						if(enemy_enumerator.MoveNext())
				 		{
							//entityEnemyS current_enemy = enemy_enumerator.Current;
							Enemy current_enemy = enemy_enumerator.Current;
							current_enemy.is_this_enemies_turn = true;
							enemy_currently_acting = true;
						}
						else //if there are no more enemies to move, then its the players turn again
						{
//							List<entityEnemyS> result = new List<entityEnemyS>();
//							foreach(entityEnemyS current_enemy in em.enemy_list){
//								current_enemy.current_ap = current_enemy.max_ap;
//								if(!current_enemy.checkIfDead())
//									result.Add(current_enemy);		
//							}
							
							endAllEnemeyTurn();
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
	
	public void forcePlayerTurn(){
		current_turn = Turn.Player;
		em.getMech().current_ap = em.getMech().max_ap;
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
			ShadowAndOutline.DrawOutline(new Rect(0, 54 ,776,19), level_complete_top_stats,ep.gui_norm_text, new Color(0,0,0,.6F),Color.white, 3F);
			ShadowAndOutline.DrawOutline(new Rect(0, 80 ,776,19), level_complete_bottom_stats,ep.gui_norm_text, new Color(0,0,0,.6F),Color.white, 3F);
		
			if(GUI.Button(new Rect(223,  232, 330, 40),"",ep.HUD_button))
			{
				Application.LoadLevel("Level2");	
			} 
			ShadowAndOutline.DrawOutline(new Rect(223,  232, 330, 38), won_the_game ? "Next Level" : "Restart",ep.gui_norm_text, new Color(0,0,0,.6F),Color.white, 3F);
				
		GUI.EndGroup();
	}
		
    private bool game_is_over;
	private string level_complete_top_stats;
	private string level_complete_bottom_stats;
	
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
			em.getMech().adjustPartCount(Part.Gear, 2000);
			em.getMech().adjustPartCount(Part.Piston, 2000);
			em.getMech().adjustPartCount(Part.Plate, 2000);
			em.getMech().adjustPartCount(Part.Strut, 2000);
		}
	}
	
	public void endPlayerTurn()
	{ 
		if(em.getMech().upgrade_util_idle)
			em.getMech().idleHeal();
		em.getMech().current_ap =  0;   
		rebuilt_enemy_lcoations = false;
		current_turn = Turn.Enemy;
	}
	
	public void endAllEnemeyTurn()
	{  
		List<Enemy> result = new List<Enemy>();
		foreach(Enemy current_enemy in em.enemy_list){
			current_enemy.current_ap = current_enemy.max_ap;
			if(!current_enemy.checkIfDead())
				result.Add(current_enemy);		
		}
		em.enemy_list = result; //update enemy list to account for enemies that exploded
		 
		spawned_enemies_this_round = false; 
		 
		
		em.getBase().current_attacks_this_round = 0;
		current_turn = Turn.Base;
	}
	
	public void endBaseTurn()
	{
		//TODO: not sure if this is finished
		em.getMech().current_ap = em.getMech().max_ap; 
		current_round++;
		em.getMech().updateAttackableEnemies();
		current_turn = Turn.Player; 
		
	}
}
