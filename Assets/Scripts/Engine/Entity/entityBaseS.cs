using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class entityBaseS : Combatable {
	 
	//variables 
	public BaseUpgradeLevel structure_level = BaseUpgradeLevel.Level0;
	public BaseUpgradeLevel defense_level = BaseUpgradeLevel.Level0;
	public BaseUpgradeLevel wall_level = BaseUpgradeLevel.Level0; 
	private int heal_amount = 2;
	private int heal_cost = 3;
	private bool  can_not_heal;
	private bool  can_not_attack; 
	
	public Renderer walls;
	public Renderer def;
	public Renderer struc;
	 
	public static bool show_health_bar = true; 
	
	public static List<HexData> adjacent_visible_hexes;
	//Use this for initialization
	void Start () {
 		child_fire = gameObject.transform.GetChild(3);//.GetComponentsInChildren<ParticleSystem>();
		Debug.Log("BASE CHILD SELECTED = " + child_fire.name);
		turnOffFire();
		def = transform.FindChild("entityBaseDefense").renderer;
		walls = transform.FindChild("entityBaseWalls").renderer;
		struc = transform.FindChild("entityBaseStructure").renderer;
		
		max_hp = 100;
		current_hp = 100;
		
		max_ap = 10;
		current_ap = 10;
		
		base_armor = 0;
		attack_cost   = 5;
		attack_range  = 2;
		attack_damage = 5;
		
		can_not_heal = false;
		can_not_attack = false;
		
		adjacent_visible_hexes = hexManagerS.getAdjacentHexes(x, z, sight_range);
		
		//turn hexes within range now to Live state
		foreach(HexData hex in adjacent_visible_hexes)
		{ 
			hexManagerS.updateHexVisionState(hex, Vision.Live);
			hex.hex_script.updateFoWState();
		} 
		entityManagerS.updateEntityFoWStates();
	}
	
	public Texture armaments_1;
	public Texture armaments_2;
	public Texture armaments_3;
	public Texture walls_1;
	public Texture walls_2;
	public Texture walls_3;
	public Texture structure_1;
	public Texture structure_2;
	public Texture structure_3;
	
	private Dictionary<BaseUpgradeMode, List<UpgradeEntry>> baseupgrademode_entrieslists;
	
		private void createUpgradeMenuEntries()
	{
		baseupgrademode_entrieslists = new Dictionary<BaseUpgradeMode, List<UpgradeEntry>>(); 
//		baseupgrade_to_entries = new Dictionary<BaseUpgradeLevel, UpgradeEntry>();
		
		List<UpgradeEntry> walls_upgrades = new List<UpgradeEntry>();
		List<UpgradeEntry> armament_upgrades = new List<UpgradeEntry>();
		List<UpgradeEntry> structure_upgrades = new List<UpgradeEntry>(); 
//		 																					 			 		gear pis plt stru
		armament_upgrades.Add (new UpgradeEntry("Iron Cannons", 		"Allows Water hex traversal (5 AP)",		1,	5,	1,	1,	2, armaments_1));
		armament_upgrades.Add (new UpgradeEntry(" Rounds",	"Allows Mountain hex traversal (5 AP)",					2,	4,	0,	2,	2, armaments_2));
		armament_upgrades.Add (new UpgradeEntry("Marsh Stabilizers",	"Reduces Marsh hex traversal by 1 AP",		0,	1,	4,	4,	2, armaments_3));
		 	 
		walls_upgrades.Add (new UpgradeEntry("Mortar Cannons",			"Increases attack damage by 2",				1,	2,	4,	2,	2, walls_1));
		walls_upgrades.Add (new UpgradeEntry("Heavy Ordinance", 		"Reduces attack cost by 2 AP",				2,	2,	0,	2,	2, walls_2));
		walls_upgrades.Add (new UpgradeEntry("Annihilation Payload", 	"Increases attack range by 1",				4,	1,	1,	2,	2, walls_3));
		 			 
		structure_upgrades.Add (new UpgradeEntry("Combat Salvage",		"Gain one random part from kills",			2,	3,	0,	2,	2, structure_1));
		structure_upgrades.Add (new UpgradeEntry("Greedy Gather", 		"Gain one extra part per scavenge",			0,	2,	1,	4,	2, structure_2));
		structure_upgrades.Add (new UpgradeEntry("Probing Sensors", 		"15% change to scavenge empty nodes",	4,	2,	0,	0,	2, structure_3 ));
		 
		baseupgrademode_entrieslists.Add(BaseUpgradeMode.Armament, armament_upgrades);
		baseupgrademode_entrieslists.Add(BaseUpgradeMode.Walls, walls_upgrades);
		baseupgrademode_entrieslists.Add(BaseUpgradeMode.Structure, structure_upgrades); 
		 
		
	} 
	
			//vars for the whole sheet
	public int colCount    = 4;
	public int rowCount    = 2;
	 
	//vars for animation
	public int rowNumber   = 3; //Zero Indexed
	public int colNumber   = 4; //Zero Indexed
	public int totalCells  = 12;
	
	public static Transform child_fire;  
	
	public static void turnOnFire(){
		
		child_fire.transform.GetChild(0).particleEmitter.emit = true; 
		child_fire.transform.GetChild(1).particleEmitter.emit = true; 
		child_fire.transform.GetChild(2).particleEmitter.emit = true; 
	}
	
	public static void turnOffFire(){
		
		child_fire.transform.GetChild(0).particleEmitter.emit = false; 
		child_fire.transform.GetChild(1).particleEmitter.emit = false; 
		child_fire.transform.GetChild(2).particleEmitter.emit = false; 
	}
	
	void visualDisplay()
	{
		int def_index = (int)defense_level;
		int struct_index = (int)structure_level ;
		int wall_index = (int) wall_level;
		  
	    Vector2 struct_offset = new Vector2((float) structure_level/4, 0);
	    Vector2 wall_offset = new Vector2((float) wall_level/4, .3333333F);
	    Vector2 def_offset = new Vector2((float) defense_level/4, .666666F);
	 
	    def.material.SetTextureOffset ("_MainTex", def_offset);
	    walls.material.SetTextureOffset ("_MainTex", wall_offset);
	    struc.material.SetTextureOffset ("_MainTex", struct_offset);
//	    def.material.SetTextureOffset ("_MainTex", def_offset);
//	    renderer.material.SetTextureScale  ("_MainTex", size);
//	
//		def.renderer.material.SetColor("_Color",Color.green);
//		struc.renderer.material.SetColor("_Color",Color.red);
//		walls.renderer.material.SetColor("_Color",Color.blue);
		
	
	
	}
	
	public bool onFire = false;
	// Update is called once per frame
	void Update () {
		visualDisplay();
		
		
		if((float)current_hp/(float)max_hp < .5F )
		{
			if(!onFire)
				turnOnFire(); 
			onFire = true;
		}
		else{
			if(onFire)
				turnOffFire();
			onFire = false;
		}
		 
		if(checkIfDead()){
			print (this.GetInstanceID() + " is DEAD!!");
			onDeath();
		}else{
		
			if(gameManagerS.current_turn == Turn.Base)
			{
				if(!waiting_after_shot)
				{
					
	//				//Debug.Log("BASE TURN NOW");
	//				print ("BASE hp = " + current_hp);
	//				print ("BASE Ap = " + current_ap);
					
					//check ap
					if(current_ap <= 0 || (can_not_heal && can_not_attack))
					{
						//Debug.Log("Ran out of Ap or can't make a move");
						gameManagerS.endBaseTurn();
						can_not_heal = false;
						can_not_attack = false;
					}
					else
					{
						//Check to see if base can heal
						int temp = current_hp;
						if(current_hp < max_hp && canHeal()){ 
							int heal_points = healhp(heal_amount);
							if(heal_points == temp){
								//Debug.Log ("no healing occurred");
								can_not_heal = true;
							}else{
								if(current_ap - heal_cost < 0){
									//Debug.Log ("not enough ap to heal");
									can_not_heal = true;
								}else{
									//Debug.Log ("healing occured");
									current_ap -= heal_cost;
								}
							}
						}else{
							//Debug.Log ("can't heal, under attack");
							can_not_heal = true;
						}
						
						//Check to see if base can attack
						entityEnemyS enemy_s = getWeakestEnemy();
						if(enemy_s != null){	
							if(current_ap - attack_cost < 0){
								//Debug.Log ("Can't attack, not enough ap, so END TURN");
								can_not_attack = true;
							}else{
								//current_ap -= attack_cost;
								int damage_done = attackTarget (enemy_s);
								//Debug.Log ("ATTACK opponent, damage done = " + damage_done);
							}
							
						}else{
							//Debasug.Log ("no enemies in range, can't attack");
							can_not_attack = true;
						}
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
	
	float time_after_shot_start;
	bool waiting_after_shot = false;
	
//	void OnGUI()
//	{
//		//hp bar variables
//		int width_denominator = 18;
//		int width_numerator = 5;
//		int denominator_hp = width_denominator * max_hp;
//		int multiple_hp = denominator_hp/width_denominator;
//		int numerator_hp = width_numerator * multiple_hp;
//		int difference_hp = max_hp - numerator_hp;	
//			
////		Vector3 screen_pos = Camera.main.WorldToScreenPoint (transform.position);
////		if(show_health_bar){
////			GUI.Button(new Rect(((Screen.width/2)),(((Screen.height/6)*5) + (Screen.height/8)), (((numerator_hp - (max_hp - current_hp - difference_hp)) * (width_numerator * Screen.height))/denominator_hp) , (Screen.width/40)), current_hp + "/" + max_hp + " HP", enginePlayerS.hp_bar_for_base);
////		}
//	}
	
	#region implemented abstract members of Combatable
	public override int attackTarget (Combatable target)
	{
		//subtract ap cost from total
		//Debug.LogWarning("ABOUT TO ATTACK ENTITY ON - " + target.x + "," + target.z);
		entityManagerS.sm.playGunBigl();
		current_ap -= attack_cost;
		
		//Debug.LogWarning("ABOUT TO ATTACK ENTITY " + target.GetInstanceID());
		if(target != null)
			 target.acceptDamage(attack_damage);
		
		
		 gameManagerS.waiting_after_shot = true;
		gameManagerS.time_after_shot_start = Time.time;
		
		//Debug.Log ("ERROR: didn't pick a combatable target");
		return 0; //nothing to damage if we get here
	}
	
	#endregion	
	
	public override bool onDeath()
	{
		gameManagerS.gameOver();
		return true;
	}
	
	//Player calls this method to change bases upgrade level
	//Assumption: entityMech will take care of whether upgrade is allowed	
	public bool upgradeBase(BaseUpgradeMode category, BaseUpgradeLevel upgrade){ 
		//increase health, walls(armour), attack/range cost, and reduce ap
		//Upgrades can only be applied once
		switch(category){ 

		case BaseUpgradeMode.Structure:
		{
			switch(upgrade)
			{
				case BaseUpgradeLevel.Level1:
					if(structure_level != upgrade && structure_level < upgrade){
						structure_level = BaseUpgradeLevel.Level1;
						max_hp += 10;
						return true;
					}else{
						//Debug.Log ("Base Already has this structure upgrade, can't downgrade");
						return false;
					}
			
				case BaseUpgradeLevel.Level2:
					if(structure_level != upgrade && structure_level < upgrade){
						structure_level = BaseUpgradeLevel.Level2;
						max_hp += 15;
						return true;
					}else{
						//Debug.Log ("Base Already has this structure upgrade, can't downgrade");
						return false;
					}
					
				case BaseUpgradeLevel.Level3:
					if(structure_level != upgrade && structure_level < upgrade){
						structure_level = BaseUpgradeLevel.Level3;
						max_hp += 20;
						return true;
					}else{
						//Debug.Log ("Base Already has this structure upgrade, can't downgrade");
						return false;
					}
			}
				break;
		}
		case BaseUpgradeMode.Armament:
		{
			switch(upgrade)
			{
				case BaseUpgradeLevel.Level1:
					if(defense_level != upgrade && defense_level < upgrade){
						defense_level = BaseUpgradeLevel.Level1;
						attack_range  += 1; 
						return true;
					}else{
						//Debug.Log ("Base Already has this defense upgrade, can't downgrade");
						return false;
					}
	
				case BaseUpgradeLevel.Level2:
					if(defense_level != upgrade && defense_level < upgrade){
						defense_level = BaseUpgradeLevel.Level2; 
						attack_damage += 2;
						return true;
					}else{
						//Debug.Log ("Base Already has this defense upgrade, can't downgrade");
						return false;
					}
					
				case BaseUpgradeLevel.Level3:
					if(defense_level != upgrade && defense_level < upgrade){
						defense_level = BaseUpgradeLevel.Level3;
						attack_range  += 1;
						attack_damage += 1;
					return true;
					}else{
						//Debug.Log ("Base Already has this defense upgrade, can't downgrade");
						return false;
					}
				
			}
				break;
		}
		case BaseUpgradeMode.Walls:
		{
			switch(upgrade)
			{
			case BaseUpgradeLevel.Level1:
					if(wall_level != upgrade && wall_level < upgrade){
						wall_level = BaseUpgradeLevel.Level1;
						base_armor += 1;
						max_ap   += 2;
						return true;
					}else{
						//Debug.Log ("Base Already has this AP cost upgrade, can't downgrade");
						return false;
					}
	
				case BaseUpgradeLevel.Level2:
					if(wall_level != upgrade && wall_level < upgrade){
						wall_level = BaseUpgradeLevel.Level2;
						base_armor += 1;
						max_ap   += 3;
						return true;
					}else{
						//Debug.Log ("Base Already has this AP cost upgrade, can't downgrade");
						return false;
					}
					
				case BaseUpgradeLevel.Level3:
					if(wall_level != upgrade && wall_level < upgrade){
						wall_level = BaseUpgradeLevel.Level3;
						max_ap   += 3;
						base_armor += 1;
						return true;
					}else{
						//Debug.Log ("Base Already has this AP cost upgrade, can't downgrade");
						return false;
					}
			}
				break;
		}
				
				default:
					//Nothing get's updated
					return false;
		}
		return false;
	}
	
	private int health_last_turn = 100; 
	
	//heal self when it's base turn only if no enemy next to base
	bool canHeal(){
			//Get adjacent hexes
		if(health_last_turn > current_hp)
		{
			health_last_turn = current_hp;
			return false;
		}
	
			health_last_turn = current_hp;
		return true;
	}
//		
//		
//			HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x, z);
//			
//			//See if any of the adjacent_hexes are an enemy
//			for(int i = 0; i < adjacent_hexes.Length; i++)
//				if(entityManagerS.getEnemyAt(adjacent_hexes[i].x, adjacent_hexes[i].z) != null)
//					return false; 
//			
//			return true;
	
	//return script of weakest enemy, this method seems slow, may adjust later
	entityEnemyS getWeakestEnemy(){
		int low_health = 9999;
		entityEnemyS final_weak_enemy = null;
		//get all hexes in attack range
		foreach(HexData h in hexManagerS.getAdjacentHexes(x,z,attack_range)){
				//check to see if enemy is at hex
				entityEnemyS weak_enemy = entityManagerS.getEnemyAt(h.x, h.z);
				if(weak_enemy != null){
					if( weak_enemy.current_hp < low_health){
						//Debug.Log("Weakest enemy is: " + weak_enemy.x + ":" + weak_enemy.z);
						low_health = weak_enemy.current_hp;
						final_weak_enemy = weak_enemy;
					}
				}
		}
		
		if(low_health == 9999){ 
			//Debug.Log ("no enemy's in attack range");
			return null;
		}
		
		return final_weak_enemy;
		
	}
	
	//return script of weakest enemy, this method seems slow, may adjust later
	public bool mechNextToBase(int m_x, int m_z){
		//get all hexes in attack range
		foreach(HexData h in hexManagerS.getAdjacentHexes(x,z)){
			//check to see if enemy is at hex
			if(m_x == h.x && m_z == h.z){
				return true;
			}
		}
		return false;
		
	}
	
	public bool transportMechToBase(){
		//get all hexes in attack range
		foreach(HexData h in hexManagerS.getAdjacentHexes(x,z)){
			//check to see if enemy is at hex
			if(!entityManagerS.isEntityPos(h,EntityE.Enemy)){
				entityMechS script_mech = entityManagerS.getMech();
				script_mech.transportToHex(h,false);
				return true;
			}
		}
		return false;
		
	}
	
	

}
