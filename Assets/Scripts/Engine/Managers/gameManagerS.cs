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
		requiresHUDIndicatorAtStart();
		
		
		if(current_level != Level.Level0)
		{
			ep.displayObjectivesMenu();
		}
	}
	 
	public bool posted_this_round = false;
	public float time_after_shot_start = 0F;
	public bool waiting_after_shot = false;
	
	
	
	 bool fired_win_already = false;
	
	void checkVictoryConditions()
	{ 
		switch(current_level)
		{
			case Level.Level0: 
				if(em.killed_enemy_count >= 3)
				{
					if(!fired_win_already)
					{
						fired_win_already = true; 
						endGame(true);
					}
				}
			break;
			case Level.Level1: 
				if(em.killed_enemy_count >= 20)
				{
					
					if(!fired_win_already)
					{
						fired_win_already = true;
						endGame(true);
					}
				}
			break;
			case Level.Level2: 
				if(em.getBase().isThereALevel3Tier() && em.getMech().getUpgradeCount() >= 3)
				{
					if(!fired_win_already)
					{
						fired_win_already = true;
						endGame(true);
					}
				}
			break; 
			case Level.Level3: 
				if(current_round >= 35)
				{
					if(!fired_win_already)
					{
						fired_win_already = true;
						endGame(true);
					}
				}
			break; 
			case Level.Level4: 
				if(current_round >= 41)
				{
					if(!fired_win_already)
					{
						fired_win_already = true;
						endGame(true);
					}
				}
			break; 
		} 
	}
	
	bool flipped_once = false;
	bool flipped_once2 = false;
	bool flipped_once3 = false;
	
	// Update is called once per frame
	void Update () {
		
		if(current_level == Level.Level0)
		{
			if(em.getMech().x == 22 && em.getMech().z == 13)
			{
				first_move_popup_display = false;
				if(!flipped_once)
				{ 
					show_fst = true;
					flipped_once = true;
				}
			}
			
			if(em.getMech().player_kills > 0 )
			{
				if(!flipped_once2)
				{ 
					show_fst = true;
					flipped_once2 = true;
				}
			}
//			
			if(em.getBase().getUpgradeCount() > 0 )
			{
				if(!flipped_once3)
				{ 
					show_fst = true;
					flipped_once3 = true;
					em.getMech().player_kills = 0;
					display_hud_obj_area = true;
					
				}
			}
//			o
			
		}
		
		
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
				else if(time_after_shot_start + .8F < Time.time)
				{
					waiting_after_shot = false;
				}
			}
		}
	}
	
	public void quitToMenu(){
		Application.LoadLevel("MainMenu");
	}
	
	public string getHUDIndicatorText()
	{
		switch(current_level)
		{
			case Level.Level0:
				return "Enemy Kills: " + em.killed_enemy_count + "/3";
			
			case Level.Level1:
				return "Enemy Kills: " + em.killed_enemy_count + "/20";
				
			case Level.Level2:
				return "";// "Maxed Tier: " + (em.getBase().isThereALevel3Tier() ? "Y" : "N") + " | Mech Ups: " + em.getMech().getUpgradeCount() + "/3";
				
			case Level.Level3:
				return "Rounds: " + current_round + "/35";
				
			case Level.Level4:
				return " ";
				 
		}
		return null;
	}
	
	
	public void advanceFST()
	{
		if(current_fst == 2)
		{
			
			current_fst++;
			show_fst = false;
			first_move_popup_display = true;
			return;
		}
		
		if(current_fst == 7)
		{ 
			
			current_fst++;
			show_fst = false;
			return;
		}
		
		if(current_fst == 10)
		{
			
			current_fst++;
			show_fst = false;
			return;
		}
		
		if(current_fst == 12)
		{
			
			current_fst++;
			show_fst = false;
			return;
		}
		
		if(show_fst)
			current_fst++;
	}
	
	public int current_fst = 0;
	public bool show_fst = true;
//	
//	public Texture getCurrentFullscreenTip()
//	{
//		if(current_level == Level.Level0)
//		{
//			if(!show_fst)
//				return null;
//			if(show_fst && current_level == Level.Level0)
//				switch(current_fst){ 
//					case 0: return camera_controls;
//					case 1: return ap_info;
//					case 2: return movement_information;
//				
//					case 3: return hp_parts;
//					case 4: return enemies_spawns;
//					case 5: return repair_town;
//					case 6: return rounds_objectives;
//					case 7: return town_defend;
//				
//					case 8: return upgrade_base_objective;
//					case 9: return upgrade_base;
//					case 10: return nodes;
//				
//					case 11: return kill_5; 
//					case 12: return upgrade_mech;
//				}
//		return null;
//		}
//		else
//			if(current_level == Level.Level3){
//				if(current_fst == 0)  
//					return flyer_warning;
//				else
//					return null;
//			
//		}
//		return null;
//	}
	
	
//	public Texture ap_info; 
//	public Texture camera_controls;
//	public Texture enemies_spawns;
//	public Texture hp_parts;
//	public Texture kill_5;
//	public Texture nodes;
//	public Texture repair_town;
//	public Texture rounds_objectives;
//	public Texture upgrade_mech;
//	public Texture upgrade_base;
//	public Texture upgrade_base_objective;
//	public Texture movement_information;
//	public Texture town_defend;
//	public Texture repair_alert_corner;
//	
	public bool first_move_popup_display = false;
//	
	public Vector3 getScreenSpotInt(int x, int z)
	{
		return Camera.main.WorldToScreenPoint (hm.CoordsGameTo3D(x, z)); 
	}		
	
	public Vector3 getScreenSpot(float xx, float yy)
	{
		Vector2 v2 = new Vector2(xx,yy);
		return Camera.main.WorldToScreenPoint (new Vector3(v2.x,0,v2.y)); 
	}		 
	public List<PopupInfo> getPopupInfoForLevel(){
		return new List<PopupInfo>();
		switch(current_level)
		{
			case Level.Level0:
				List<PopupInfo> values = new List<PopupInfo>();
				PopupInfo temp  = new PopupInfo("Click here to move.", getScreenSpot(28.54951F, 57.88783F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 152;
				values.Add(temp);
				
				temp  = new PopupInfo("Next, move here!", getScreenSpot(27.5F, 66.5F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 152;
				values.Add(temp);
			
				temp  = new PopupInfo("Move on to this tile,\nthen click it to <b>Scavenge</b>\nfor <b>Parts</b> when here.", getScreenSpot(33F,67.3F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 106;
				values.Add(temp);
			
				temp  = new PopupInfo("This is an <b>Outpost</b>.\nIt gives you <b>Plates</b> & <b>Struts</b>.\nClick here to <b>Scavenge</b>."
				, getScreenSpot(37.5F,69.9F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 106;
				values.Add(temp);
				temp  = new PopupInfo("This is a <b>Factory</b>.\nIt gives you <b>Pistons</b> & <b>Gears</b>.\nClick here to<b> Scavenge</b>.", getScreenSpot(35F,75.4F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 106;
				values.Add(temp);
				temp  = new PopupInfo("This is a <b>Junkyard</b>.\nIt gives you random parts.\nClick here to <b>Scavenge</b>.", getScreenSpot(43.6F,75.8F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 106;
				values.Add(temp);
			 
				temp  = new PopupInfo("When you have upgraded <b>Mountaineering Claws,</b>\ncross to here!", getScreenSpot(53.6F,79.2F), Trigger.Visible, 1);
				temp.widthless = 100; temp.heightless = 106;
				values.Add(temp);
			
			 
			 
			 

			 
//				temp  = new PopupInfo("This is an OUTPOST.\nScavenge here to get\nPlates and Struts!",getScreenSpot(23,31), Trigger.AlwaysOn, 3);
//				temp.widthless = 100; temp.heightless = 120;
//				values.Add(temp);
//				
//				temp  = new PopupInfo("This is a JUNKYARD.\nScavenge here to get\nrandom parts!",getScreenSpot(12,29), Trigger.AlwaysOn, 4);
//				temp.widthless = 100; temp.heightless = 120;
//				values.Add(temp);
//				
//				temp  = new PopupInfo("Click on this tile to move here!\nNotice your mech will automatically\nnavigate the shortest parth!",getScreenSpot(22,13), Trigger.Proximity, 5);
//				temp.widthless = 50; temp.heightless = 90;
//				values.Add(temp);
 				return values;
			
			case Level.Level1:
			  values = new List<PopupInfo>(); 
 				return values;											   
																			 
			case Level.Level2:											   
		 values = new List<PopupInfo>();									  
 				return values;					 	   
												 		 
			case Level.Level3:					 	   
		  values = new List<PopupInfo>();		 	     
 				return values;	 				   
								 					 
			case Level.Level4:	 				   
			  values = new List<PopupInfo>();	     
 				return values;	 				 
								 				 
			case Level.Level5:	 				 
			  values = new List<PopupInfo>();	  
 				return values;
		}
		return null;
	}
	public bool display_hud_obj_area = false;
	
	
	
	
	
	
	
	public bool step_1_popup = false;
	public bool step_2_popup = false;
	public bool step_5_popup = false; 
	public bool step_9_popup = false; 
	
//	Step 1 
//Click here to move.
//x = 28.54951
//z = 57.88783
//
//Step 2 
//Now move here!
//x = 27.48197
//z = 66.47248
//
//Step 5 
//Move on to this tile, and then click here to Scavenge for parts.
//x = 32.92439
//z = 67.31445
//
//Step 6
//This is an outpost. It can give you [metal plates] and [struts]. Click here to Scavenge.
//x = 37.52487
//z = 69.97057
//
//This is a factory. It can give you [pistons] and [gears]. Click here to Scavenge.
//x = 34.99903
//z = 75.41302
//
//This is a junkyard. It randomly gives you any kind of part. Click here to Scavenge.
//x = 43.58364
//z = 75.7689
//
//Step 9 
//Move here
//x = 53.62655
//z = 79.26698
//
//Step 9.5 Hovering over walker enemy
//Enemies Mechs get a turn after you take your turn. This enemy walks across the land. They also have an attack range of 2 tiles.
//
//Step 10 Hovering over the town
//This is your Town. It takes its turn after the Enemy Mechs’ turn ends.
//x = 52.1078
//z = 101.3927
//
//Step 12 Town location
//Thanks for the walls! Please protect us from the enemies that have been terrorizing our town.
//x = 52.1078
//z = 101.3927
//
//
//Step 13 Hover over flyer enemy and spawn point
//This enemy can fly across the water and mountains. They can move twice and explode on impact inflicting 10 damage.
//
//This is an enemy spawn point. During the Enemy’s Turn, an Enemy may appear from here and come to attack your Town.
//
//Flyer enemy spawn point
//x = 40.381
//z = 101.5229
//
//Walker enemy spawn point 1
//x = 56.48268
//z = 110.8193
//
//Walker enemy spawn point 2
//x = 57.32463
//z = 109.0051
//
//Step 15 Town location
//Good job! An earthquake has released rogue mechs into our peaceful world. Please protect us!
//x = 52.1078
//z = 101.3927
//
//Extra Nodes: (Hover) near Town
//Outpost
//56.70828
//104.0488
//
//Factory
//57.55023
//102.2346
//
//Junkyard
//56.09194
//99.09242

	
	public void requiresHUDIndicatorAtStart()
	{
		switch(current_level)
		{
			case Level.Level0:
				display_hud_obj_area = false;
			break;
			case Level.Level1:
				display_hud_obj_area = true;
			break;
				
			case Level.Level2:
				display_hud_obj_area = false; 
			break;
			case Level.Level3:
				display_hud_obj_area = true;
			break;
				
			case Level.Level4:
				display_hud_obj_area = false;
			break; 
		} 
	}
	
	
	public void loadNextLevel(){
		switch(current_level)
		{
			case Level.Level0: 
				PlayerPrefs.SetString("CONTINUE","Level1");
				Application.LoadLevel("Level1");
				break;
			
			case Level.Level1:
				PlayerPrefs.SetString("CONTINUE","Level2");
				Application.LoadLevel("Level2");
				break;
				
			case Level.Level2:
				PlayerPrefs.SetString("CONTINUE","Level3");
				Application.LoadLevel("Level3");
				break;
				
			case Level.Level3:
				PlayerPrefs.SetString("CONTINUE","Level4");
				Application.LoadLevel("Level4");
				break;
				
			case Level.Level4:
				PlayerPrefs.SetString("CONTINUE","Level4");
				Application.LoadLevel("MainMenu");
				break;
				 
		}
	}
	
	public void restartLevel(){
		switch(current_level)
		{
			case Level.Level0:
				Application.LoadLevel("Level0");
				break;
			
			case Level.Level1:
				Application.LoadLevel("Level1");
				break;
				
			case Level.Level2:
				Application.LoadLevel("Level2");
				break;
				
			case Level.Level3:
				Application.LoadLevel("Level3");
				break;
				
			case Level.Level4:
				Application.LoadLevel("Level4");
				break;
				 
		}
	}
	
	
	public string getObjectiveText(){
		  
		switch(current_level)
		{
		case Level.Level0:
			return "1.) Learn the basics of the game.\n2.) Upgrade your mech with Mountaineering Claws.\n3.) Kill an enemy.\n4.) Upgrade base with Ironplate Retrofit.\n5.) Kill 2 additional enemies." ;
		case Level.Level1:
			return "1.) Kill 20 enemies.\n2.) Protect yourself and your base.";	
		case Level.Level2:
			return "1.) Upgrade one base category to level 3.\n2.) Acquire any three mech upgrades.";
		case Level.Level3:
			return "1.) Survive for 35 rounds."; 
		case Level.Level4:
			return "1.) Travel Southeast and reach the sacred valley farm in the mountain crater.\n2.) Beware of the kamikaze mechs.";
	
			 
		}
		
		return "something went wrong D:";
	}
	
	public void forcePlayerTurn(){
		current_turn = Turn.Player;
		em.getMech().current_ap = em.getMech().max_ap;
	}
	
	public Texture flyer_warning;
	 
	
	
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
				if( won_the_game)
					loadNextLevel();
				else
					restartLevel();
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
		level_complete_top_stats =  "Parts Collected: " + em.getMech().parts_collected +  "   |   " +
									"Upgrades Built: " + (em.getMech().getUpgradeCount() + em.getBase().upgrades_built) +"   |   " +
									"Enemies Killed: " + em.killed_enemy_count;
		level_complete_bottom_stats = "Time Elapsed: " + (int)Time.time/60 + " minutes, " + (int)Time.time%60 + " seconds";
 		completescreen_start_time = Time.time;  
		game_is_over = true; 
		audio.PlayOneShot(level_complete_screen_sound);
	}
	
	void OnGUI()
	{  
		if(game_is_over)
			drawLevelOverScreen();
		
		
//		if(current_level == Level.Level0 && em.getMech().current_hp <= em.getMech().getMaxHP()/2)
//		{
//			GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), repair_alert_corner);
//		}
//		
	 
		
		if(Input.GetKeyDown(KeyCode.F4))
		{
			em.getMech().adjustPartCount(Part.Gear, 4);
			em.getMech().adjustPartCount(Part.Piston, 4);
			em.getMech().adjustPartCount(Part.Plate, 4);
			em.getMech().adjustPartCount(Part.Strut, 4);
		}
		if(Input.GetKeyDown(KeyCode.F5))
		{
			Application.LoadLevel("Level0");
		}
		if(Input.GetKeyDown(KeyCode.F6))
		{
			Application.LoadLevel("Level1");
		}
		if(Input.GetKeyDown(KeyCode.F6))
		{
			Application.LoadLevel("Level2");
		}
		if(Input.GetKeyDown(KeyCode.F7))
		{
			Application.LoadLevel("Level3");
		}
		if(Input.GetKeyDown(KeyCode.F8))
		{
			Application.LoadLevel("Level4");
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
