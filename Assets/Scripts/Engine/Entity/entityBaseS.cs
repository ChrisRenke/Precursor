using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class entityBaseS : Combatable {
	
	//variables 
	public BaseUpgrade structure_level = BaseUpgrade.Level0;
	public BaseUpgrade defense_level = BaseUpgrade.Level0;
	public BaseUpgrade wall_level = BaseUpgrade.Level0;
	private int heal_amount = 5;
	private int heal_cost = 10;
	private bool  can_not_heal;
	private bool  can_not_attack;
	
	
	public Renderer walls;
	public Renderer def;
	public Renderer struc;
	
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
		attack_range  = 1;
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
				}else{
					//Check to see if base can heal
					int temp = current_hp;
					if(heal()){ 
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
			
		}
		
	}
	
	
	void OnGUI()
	{
		Vector3 screen_pos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y+30, 200, 15), current_hp + "/" + max_hp + " HP", enginePlayerS.hover_text);
		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y + 45, 200, 15), current_ap + "/" + max_ap + " AP", enginePlayerS.hover_text);
	}
	
	#region implemented abstract members of Combatable
	public override int attackTarget (Combatable target)
	{
		//subtract ap cost from total
		//Debug.LogWarning("ABOUT TO ATTACK ENTITY ON - " + target.x + "," + target.z);
		current_ap -= attack_cost;
		
		//Debug.LogWarning("ABOUT TO ATTACK ENTITY " + target.GetInstanceID());
		if(target != null)
			return target.acceptDamage(attack_damage);
		
		//Debug.Log ("ERROR: didn't pick a combatable target");
		return 0; //nothing to damage if we get here
	}
	
	#endregion	
	
	public override bool onDeath()
	{
		Application.Quit();
		return true;
	}
	
	//Player calls this method to change bases upgrade level
	//Assumption: entityMech will take care of whether upgrade is allowed	
	public bool upgradeBase(BaseCategories category, BaseUpgrade upgrade){
		//increase health, walls(armour), attack/range cost, and reduce ap
		//Upgrades can only be applied once
		switch(category){

		case BaseCategories.Structure:
		{
			switch(upgrade)
			{
				case BaseUpgrade.Level1:
					if(structure_level != upgrade && structure_level < upgrade){
						structure_level = BaseUpgrade.Level1;
						max_hp += 15;
						return true;
					}else{
						//Debug.Log ("Base Already has this structure upgrade, can't downgrade");
						return false;
					}
			
				case BaseUpgrade.Level2:
					if(structure_level != upgrade && structure_level < upgrade){
						structure_level = BaseUpgrade.Level2;
						max_hp += 20;
						return true;
					}else{
						//Debug.Log ("Base Already has this structure upgrade, can't downgrade");
						return false;
					}
					
				case BaseUpgrade.Level3:
					if(structure_level != upgrade && structure_level < upgrade){
						structure_level = BaseUpgrade.Level3;
						max_hp += 25;
						return true;
					}else{
						//Debug.Log ("Base Already has this structure upgrade, can't downgrade");
						return false;
					}
			}
				break;
		}
		case BaseCategories.Defense:
		{
			switch(upgrade)
			{
				case BaseUpgrade.Level1:
					if(defense_level != upgrade && defense_level < upgrade){
						defense_level = BaseUpgrade.Level1;
						attack_range  += 1;
						attack_damage += 1;
						return true;
					}else{
						//Debug.Log ("Base Already has this defense upgrade, can't downgrade");
						return false;
					}
	
				case BaseUpgrade.Level2:
					if(defense_level != upgrade && defense_level < upgrade){
						defense_level = BaseUpgrade.Level2;
						attack_range  += 1;
						attack_damage += 2;
						return true;
					}else{
						//Debug.Log ("Base Already has this defense upgrade, can't downgrade");
						return false;
					}
					
				case BaseUpgrade.Level3:
					if(defense_level != upgrade && defense_level < upgrade){
						defense_level = BaseUpgrade.Level3;
						attack_range  += 0;
						attack_damage += 3;
					return true;
					}else{
						//Debug.Log ("Base Already has this defense upgrade, can't downgrade");
						return false;
					}
				
			}
				break;
		}
		case BaseCategories.Walls:
		{
			switch(upgrade)
			{
			case BaseUpgrade.Level1:
					if(wall_level != upgrade && wall_level < upgrade){
						wall_level = BaseUpgrade.Level1;
						base_armor += 1;
						max_ap   += 5;
						return true;
					}else{
						//Debug.Log ("Base Already has this AP cost upgrade, can't downgrade");
						return false;
					}
	
				case BaseUpgrade.Level2:
					if(wall_level != upgrade && wall_level < upgrade){
						wall_level = BaseUpgrade.Level2;
						base_armor += 1;
						max_ap   += 5;
						return true;
					}else{
						//Debug.Log ("Base Already has this AP cost upgrade, can't downgrade");
						return false;
					}
					
				case BaseUpgrade.Level3:
					if(wall_level != upgrade && wall_level < upgrade){
						wall_level = BaseUpgrade.Level3;
						max_ap   += 5;
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
	
	//heal self when it's base turn only if no enemy next to base
	bool heal(){
			//Get adjacent hexes
			HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x, z);
			
			//See if any of the adjacent_hexes are an enemy
			for(int i = 0; i < adjacent_hexes.Length; i++)
				if(entityManagerS.getEnemyAt(adjacent_hexes[i].x, adjacent_hexes[i].z) != null)
					return false; 
			
			return true;
	}
	
	//return script of weakest enemy, this method seems slow, may adjust later
	entityEnemyS getWeakestEnemy(){
		int low_health = 9999;
		entityEnemyS final_weak_enemy = null;
		//get all hexes in attack range
		foreach(HexData h in hexManagerS.getAdjacentHexes(x,z, attack_range)){
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

}
