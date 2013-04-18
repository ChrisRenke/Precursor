using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class entityBaseS : Combatable {
	 
	//variables 
	public BaseUpgradeLevel structure_level = BaseUpgradeLevel.Level0;
	public BaseUpgradeLevel defense_level = BaseUpgradeLevel.Level0;
	public BaseUpgradeLevel wall_level = BaseUpgradeLevel.Level0; 
	private int heal_amount = 5;
//	private int heal_cost = 3;
	private bool  can_not_heal;
	private bool  can_not_attack; 
	
	public int upgrades_built = 0;
	
	public Renderer walls_f;
	public Renderer walls_b;
	public Renderer def;
	public Renderer struc;
	
	public AudioClip sound_repair;
	public AudioClip sound_burning;
	public AudioClip sound_attack_core;
	public AudioClip sound_attack_upgrade;
	public AudioClip sound_attack_gatling;
	public AudioClip sound_upgrade;
	  
	
	public int current_attacks_this_round = 0;
	public int max_attacks_this_round = 2;
	
	
	public List<HexData> adjacent_visible_hexes;
	
	public int getUpgradeCount(){
		return upgrades_built;
	}
	//Use this for initialization
	void Start () {
 		child_fire = gameObject.transform.GetChild(4);//.GetComponentsInChildren<ParticleSystem>();
		Debug.Log("BASE CHILD SELECTED = " + child_fire.name);
		turnOffFire();
		def 	= transform.FindChild("entityBaseDefense").renderer;
		struc 	= transform.FindChild("entityBaseStructure").renderer;
		walls_f = transform.FindChild("entityBaseWallsBack").renderer;
		walls_b	= transform.FindChild("entityBaseWallsFore").renderer;
		
		max_hp = 70;
		current_hp = 70;
		
		max_ap = 10;
		current_ap = 10;
		
		base_armor = 0;
		attack_cost   = 5;
		attack_range  = 2;
		attack_damage = 4;
		
		can_not_heal = false;
		can_not_attack = false;
		
		sight_range = 5;
		
		adjacent_visible_hexes = hm.getAdjacentHexes(x, z, sight_range);
		
		//turn hexes within range now to Live state
		foreach(HexData hex in adjacent_visible_hexes)
		{ 
			hm.updateHexVisionState(hex, Vision.Live);
			hex.hex_script.updateFoWState();
		} 
		em.updateEntityFoWStates();
		createUpgradeMenuEntries();
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
		
		List<UpgradeEntry> walls_upgrades = new List<UpgradeEntry>();
		List<UpgradeEntry> armament_upgrades = new List<UpgradeEntry>();
		List<UpgradeEntry> structure_upgrades = new List<UpgradeEntry>(); 
//		 																					 			 		gear pis plt stru
		 
		UpgradeEntry temp;
		temp = (new UpgradeEntry("Heavy Ordnance", 			"Increase base attack range by 1",		2,	3,	1,	1,	2, armaments_1));
		temp.base_level = BaseUpgradeLevel.Level1;
		armament_upgrades.Add(temp);
		temp = new UpgradeEntry("Mortar Cannons",			"Increase base attack damage by 3",		1,	5,	0,	2,	3, armaments_2);
		temp.base_level = BaseUpgradeLevel.Level2;
		armament_upgrades.Add(temp);
		temp = new UpgradeEntry("Gatling Repeaters",		"Grants base etxra an extra attack",	1,	6,	2,	3,	4, armaments_3);
		temp.base_level = BaseUpgradeLevel.Level3;
		armament_upgrades.Add(temp); 
		 	 
		temp = new UpgradeEntry("Iron Plate Retrofit",			"Reduces incoming attack damage by 1",	0,	0,	4,	3,	2, walls_1);
		temp.base_level = BaseUpgradeLevel.Level1;
		walls_upgrades.Add(temp); 
		temp = new UpgradeEntry("Copper Battlements", 			"Reduces incoming attack damage by 1",	1,	1,	5,	4,	3, walls_2);
		temp.base_level = BaseUpgradeLevel.Level2;
		walls_upgrades.Add(temp); 
		temp = new UpgradeEntry("Golden Fortress", 			"Reduces incoming attack damage by 1",	2,	2,	6,	5,	4, walls_3);
		temp.base_level = BaseUpgradeLevel.Level3;
		walls_upgrades.Add(temp);  
		 			 
		temp  = new UpgradeEntry("Castle Expansion",		"Increase base HP by 10",				4,	2,	1,	1,	2, structure_1);
		temp.base_level = BaseUpgradeLevel.Level1;
		structure_upgrades.Add(temp);  
		temp  = new UpgradeEntry("Hybrid Observatory", 		"Increase base HP by 15",				5,	3,	2,	4,	3, structure_2);
		temp.base_level = BaseUpgradeLevel.Level2;
		structure_upgrades.Add(temp);  
		temp  = new UpgradeEntry("Industrial Revolution",	"Increase base HP by 25",				6,	4,	3,	3,	4, structure_3);
		temp.base_level = BaseUpgradeLevel.Level3;
		structure_upgrades.Add(temp);   
		 
		baseupgrademode_entrieslists.Add(BaseUpgradeMode.Armament, armament_upgrades);
		baseupgrademode_entrieslists.Add(BaseUpgradeMode.Walls, walls_upgrades);
		baseupgrademode_entrieslists.Add(BaseUpgradeMode.Structure, structure_upgrades); 
		 
		
	} 
	
	
	public List<UpgradeEntry> getUpgradeEntries(BaseUpgradeMode upgrade_type){
		if(baseupgrademode_entrieslists[upgrade_type] == null)
			throw new System.Exception("HOLY FUCK CANT GET BASE SHIT!");
		return baseupgrademode_entrieslists[upgrade_type];
	}
	
			//vars for the whole sheet
	public int colCount    = 4;
	public int rowCount    = 2;
	 
	//vars for animation
	public int rowNumber   = 3; //Zero Indexed
	public int colNumber   = 4; //Zero Indexed
	public int totalCells  = 12;
	
	public  Transform child_fire;  
	
	public  void turnOnFire(){
		
		child_fire.transform.GetChild(0).particleEmitter.emit = true; 
		child_fire.transform.GetChild(1).particleEmitter.emit = true; 
		child_fire.transform.GetChild(2).particleEmitter.emit = true; 
	}
	
	public  void turnOffFire(){
		
		child_fire.transform.GetChild(0).particleEmitter.emit = false; 
		child_fire.transform.GetChild(1).particleEmitter.emit = false; 
		child_fire.transform.GetChild(2).particleEmitter.emit = false; 
	}
	
	void visualDisplay()
	{
		int def_index    = (int) defense_level;
		int wall_b_index = (int) wall_level;
		int wall_f_index = (int) wall_level;
		int struct_index = (int) structure_level;
		  
	    Vector2 def_offset = new Vector2((float) defense_level/4, -.25F);
	    Vector2 wall_offset_b = new Vector2((float) wall_level/4, -.5F);
	    Vector2 wall_offset_f = new Vector2((float) wall_level/4, -.75F);
	    Vector2 struct_offset = new Vector2((float) structure_level/4, -0);
	 
	    def.material.SetTextureOffset	  ("_MainTex", def_offset);
	    walls_f.material.SetTextureOffset ("_MainTex", wall_offset_b); 
	    walls_b.material.SetTextureOffset ("_MainTex", wall_offset_f);
	    struc.material.SetTextureOffset   ("_MainTex", struct_offset); 
		
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
			Debug.LogError("SHIT BE DEAD YO!");
			print (this.GetInstanceID() + " is DEAD!!");
			onDeath();
		}
		if(gm.current_turn == Turn.Base)
		{
 
			if(!waiting_after_shot)
			{
				Enemy targeted_enemy = getWeakestEnemy();
				
//				Debug.Log("BASEINFOLOG - " + current_attacks_this_round);
				//if there's something to shoot
				if(targeted_enemy != null && current_attacks_this_round < max_attacks_this_round)
				{ 
					attackTarget(targeted_enemy);
				}
				else
				if(current_attacks_this_round == 0)
				{
					
					if(current_hp < max_hp)
					{
						em.createHealEffect(x,z);
						audio.PlayOneShot(sound_repair);
						current_hp += heal_amount;		
						if(current_hp > max_hp)
							current_hp = max_hp;
					}
						
					gm.endBaseTurn();
				}
				else
				{
					gm.endBaseTurn();
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
		if(defense_level == BaseUpgradeLevel.Level3)
			audio.PlayOneShot(sound_attack_gatling);
		else if(defense_level == BaseUpgradeLevel.Level0)
			audio.PlayOneShot(sound_attack_core);
		else
			audio.PlayOneShot(sound_attack_upgrade);
		//subtract ap cost from total
		//Debug.LogWarning("ABOUT TO ATTACK ENTITY ON - " + target.x + "," + target.z); 
//		current_ap -= attack_cost;
		
		current_attacks_this_round++;
		
		
		//Debug.LogWarning("ABOUT TO ATTACK ENTITY " + target.GetInstanceID());
		if(target != null)
			 target.acceptDamage(attack_damage);
		else
			throw new System.Exception("HOLY SHIT YOU CANT ATTACK NOTHING BRO!");
		
		
		waiting_after_shot = true;
		time_after_shot_start = Time.time;
		
		return 0; //nothing to damage if we get here
	}
	
	#endregion	
	
	public override bool onDeath()
	{
		gm.endGame(false);
		return true;
	}
	
	public bool isThereALevel3Tier(){
		
		return (wall_level == BaseUpgradeLevel.Level3 || structure_level == BaseUpgradeLevel.Level3 || defense_level == BaseUpgradeLevel.Level3);
	}
	
	
	//Player calls this method to change bases upgrade level
	//Assumption: entityMech will take care of whether upgrade is allowed	
	public bool upgradeBase(BaseUpgradeMode category, BaseUpgradeLevel upgrade){ 
		//increase health, walls(armour), attack/range cost, and reduce ap
		//Upgrades can only be applied once
		upgrades_built++;
		em.getMech().subtractPartCosts(baseupgrademode_entrieslists[category][(int)upgrade -1 ]);
		audio.PlayOneShot(sound_upgrade);
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
						attack_damage += 3;
						return true;
					}else{
						//Debug.Log ("Base Already has this defense upgrade, can't downgrade");
						return false;
					}
					
				case BaseUpgradeLevel.Level3:
					if(defense_level != upgrade && defense_level < upgrade){
						defense_level = BaseUpgradeLevel.Level3;
						max_attacks_this_round++;
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
						return true;
					}else{
						//Debug.Log ("Base Already has this AP cost upgrade, can't downgrade");
						return false;
					}
	
				case BaseUpgradeLevel.Level2:
					if(wall_level != upgrade && wall_level < upgrade){
						wall_level = BaseUpgradeLevel.Level2;
						base_armor += 1; 
						return true;
					}else{
						//Debug.Log ("Base Already has this AP cost upgrade, can't downgrade");
						return false;
					}
					
				case BaseUpgradeLevel.Level3:
					if(wall_level != upgrade && wall_level < upgrade){
						wall_level = BaseUpgradeLevel.Level3;
						max_ap   += 1;
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
	
	//return script of weakest enemy, this method seems slow, may adjust later
	private Enemy getWeakestEnemy(){
		int low_health = 9999;
		Enemy final_weak_enemy = null;
		//get all hexes in attack range
		foreach(HexData h in hm.getAdjacentHexes(x,z,attack_range)){
				//check to see if enemy is at hex
			
				Enemy current_enemy = em.getEnemyAt(h.x, h.z);
			
				//is an enemy there?
				if(current_enemy != null){
					if( current_enemy.current_hp < low_health){ 
						low_health = current_enemy.current_hp;
						final_weak_enemy = current_enemy;
					}
				}
		} 
		
		return final_weak_enemy;
		
	}
	
	//return script of weakest enemy, this method seems slow, may adjust later
	public bool mechNextToBase(int m_x, int m_z){
		//get all hexes in attack range
		foreach(HexData h in hm.getAdjacentHexes(x,z)){
			//check to see if enemy is at hex
			if(m_x == h.x && m_z == h.z){
				return true;
			}
		}
		return false;
		
	}
	
	public bool transportMechToBase(){
		//get all hexes in attack range
		foreach(HexData h in hm.getAdjacentHexes(x,z)){
			//check to see if enemy is at hex
			if(!em.isEntityPos(h,EntityE.Enemy)){
				entityMechS script_mech = em.getMech();
				script_mech.transportToHex(h,false);
				return true;
			}
		}
		return false;
		
	}
	
	
	public BaseUpgradeLevel getHighestLevelUpgrade(BaseUpgradeMode bum)
	{
		
		if(bum == BaseUpgradeMode.Armament) 
			return defense_level;
		if(bum == BaseUpgradeMode.Walls)
			return wall_level;
		if(bum == BaseUpgradeMode.Structure)
			return structure_level;
		
		throw new System.Exception("well shit, broke stuff on base get highest level");
	}
	

}
