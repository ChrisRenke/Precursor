using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gameManagerS : MonoBehaviour {
	
	public static Turn current_turn;
	public static int  current_round;
	public static Level current_level;
	
	public static bool mouse_over_gui = false;
	
	public GameObject  selection_hex_input;
	public static GameObject selection_hex;
	
	public static List<entityEnemyS >.Enumerator	enemy_enumerator;
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
	 
	public bool posted_this_round = false;
	public static float time_after_shot_start = 0F;
	public static bool waiting_after_shot = false;
	void postMessageLevel2()
	{
//		switch(current_round){
//		case 1:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "Oh donkies in heaven! There are a lot" +
//											"\nof unruly wild mech's about. Help keep" +
//											 "\nour town alive for 40 rounds " +
//											"\nand I'll make it worth your while!"; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		case 5:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "If you get low on HP, check out" +
//											"\nthe repair menu.  You can convert\n"
//											+"one part into 2 HP. Not a bad deal."; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		case 6:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "As for reparing the town," +
//			"\nwe'll handle that when we're not" +
//			"\nin combat; you just have to keep" +
//			"\nyourself in good repair."; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		case 7:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "Generally, you can upgrade our town to keep" +
//											"\nus strong enough to defend ourselves while\n"
//											+"you go around and collect more resources for" +
//												"\neither your or us!  Good luck!"; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		}
		posted_this_round = true;
	}
	
	void postMessageLevel1()
	{
//		switch(current_round){
//		case 1:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "Alrighty then, on to the basics of combat." +
//				"\nJust go exploring a bit, " +
//				"\nI'm sure you'll find some trouble." ; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		case 4:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "When you see an enemy, you can attack him from" +
//				"\nup to two hexes away, so " +
//				"\nno need to get toooo close." ; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		case 6:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "I heard there were three of these punks" +
//				"\nroaming about. Take 'em all down" +
//				 "\nand I'll give ya your next map." ; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		case 8:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "So apparently the 3rd baddie is " +
//				"\n in a little bit of a mountain valley." +
//				"\nYou're gonna need to get Climbin"
//				+"\nClaws to cross into it." ; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		case 9:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "Just scavenge up 3 pistons," +
//											"\n4 gears, and a strut then" +
//											"\nclick on the Mech Upgrade button" +
//											"\nin the bottom left." ; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		}
		posted_this_round = true;
	}
	
	void postMessageLevel0()
	{
//		switch(current_round){
//		case 1:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = "Oh Goodness! Thank the donkey you made it!!" +
//				"\nErm, thats slang around here..." +
//				"\nAnyways, try moving around the world a bit. " +
//				"\nMoving costs you AP, so watch your meter!" +
//				"\nBut don't worry, ya get all the turns ya need."; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;   
//			break;
//		case 2:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = 
//				"Use WASD to move the camera around,"+
//				"\npress - + to zoom in and out,"+
//				"\nand click on stuff to do actions!"+
//					
//				"\n\nWhy don't you head for some old scrap structures" +
//				 "\nin the area. That cataclsym that shook" +
//				 "\nloose your there mech also uncovered a" +
//				 "\nwhole lota these rusted ruins."; //not enough ap	//not enough ap	
//			inPlayMenuS.popup.custom_popup = true;
//			break; 
//		case 3:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = 
//				"Yep, that's right, just click on a ruin" +
//				 "\nto scavenge some parts!" +
//				 "\nYou can use them parts for fancy" +
//				 "\nnew upgrades for our town..." +
//				  "\n(and your mech too I s'pose)"; //not enough ap	
//			inPlayMenuS.popup.custom_popup = true;
//			break;
//		case 7:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = 
//				"How's about you build us a nice new wall?" +
//				 "\nI'll give ya a map to the next level..." +
//				 "\n...erm, 'island' that is...";
//			inPlayMenuS.popup.custom_popup = true;
//			break;
//		case 8:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = 
//				"Now's probably a good time to mention:" +
//				 "\nFactories give ya gears and pistons" +
//				 "\nOutposts give ya struts and plates" +
//				 "\nand Junkyards give ya whatever they want!";
//			inPlayMenuS.popup.custom_popup = true;
//			break;
//		case 10:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = 
//				"You'll need 4 plates, 3 gears, and 3 struts" +
//				 "\nto upgrade our little stone wall!" +
//				 "\nKeep on scavenging!" +
//				 "\nWhen you're ready, just come over to the base.";
//			inPlayMenuS.popup.custom_popup = true;
//			break;
//		case 11:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = 
//				"All's ya gots to do is just click the" +
//				"\n'Upgrade Town' button in the bottom left" +
//				"\nwhen you're standing next to the town!" +
//				"\nEasy as pie.";
//			inPlayMenuS.popup.custom_popup = true;
//			break;
//		case 20:
//			inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
//			inPlayMenuS.popup.custom_text = 
//				"We ain't got all day... " +
//				"\nNo wall equals no map to the next area!";
//			inPlayMenuS.popup.custom_popup = true;
//			break;
//		default:
//			break;
//		}
//		posted_this_round = true;
	}
	
	
	void checkVictoryConditions()
	{ 
		switch(current_level)
		{
			case Level.Level0: 
				if(entityManagerS.getBase().wall_level >= BaseUpgrade.Level1)
				{
					beatLevel0();
				}
			break;
			case Level.Level1: 
				if(entityManagerS.getEnemies().Count == 0)
				{
					beatLevel1();
				}
			break;
			case Level.Level2: 
				if(current_round >= 41)
				{
					beatLevel2();
				}
			break; 
		} 
	}
	
	
	// Update is called once per frame
	void Update () {
		
		if(!posted_this_round && current_level == Level.Level0)
			postMessageLevel0();
		else
		if(!posted_this_round && current_level == Level.Level1)
			postMessageLevel1();
		else
		if(!posted_this_round && current_level == Level.Level2)
			postMessageLevel2();
		
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
	
	
	
	
	public static void gameOver()
	{
		inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216,   210, 432,  200);
		inPlayMenuS.popup.custom_text = "Game Over! \n Better luck next time!"; //not enough ap	
		inPlayMenuS.popup.game_over_popup = true;
	}
	
	public static void beatLevel0()
	{
		inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216,   210, 432,  200);
		inPlayMenuS.popup.custom_text = "You've got the hang of the base!\nNow onto the fightin' enemies."; //not enough ap	 
		inPlayMenuS.popup.level_name = "Level1"; 
		PlayerPrefs.SetString("CONTINUE","Level1");
		inPlayMenuS.popup.load_level_popup = true;
	}
	 
	public static void beatLevel1()
	{
		inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216,   210, 432,  200);
		inPlayMenuS.popup.custom_text = "Alrighty, now you can kill baddies and upgrade\nyour mech & our town. Onto a real challenge!"; //not enough ap	
		inPlayMenuS.popup.level_name = "levelto2"; 
		PlayerPrefs.SetString("CONTINUE","Level2");
		inPlayMenuS.popup.load_level_popup = true; 
		
	}
	 
	public static void beatLevel2()
	{
		inPlayMenuS.popup.custom_rect = new Rect(Screen.width/2 - 216, 210, 432,  200);
		inPlayMenuS.popup.custom_text = "Awesome! You've defended the lands!\nNow to an early retirement for donkey farming!"; //not enough ap	
		PlayerPrefs.SetString("CONTINUE","Level2");
		inPlayMenuS.popup.game_over_popup = true; 
	}
	
	void OnGUI()
	{  
	
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
