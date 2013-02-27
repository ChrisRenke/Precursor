using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class entityBaseS : Combatable {
	
	//variables
	BaseUpgrade health_level;
	BaseUpgrade structure_level;
	BaseUpgrade defense_level;
	BaseUpgrade ap_cost_level;
	private int heal_amount = 3;
	private int heal_cost = 3;
	private bool  can_not_heal;
	private bool  can_not_attack;
	
	
	public static List<HexData> adjacent_visible_hexes;
	//Use this for initialization
	void Start () {
		max_hp = 100;
		current_hp = 100;
		
		max_ap = 18;
		current_ap = 18;
		
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
	}
	
	// Update is called once per frame
	void Update () {
			
		if(checkIfDead()){
			onDeath();
		}else{
		
			if(gameManagerS.current_turn == Turn.Base)
			{
				
				Debug.Log("BASE TURN NOW");
				print ("BASE hp = " + current_hp);
				print ("BASE Ap = " + current_ap);
				
				//check ap
				if(current_ap <= 0 || (can_not_heal && can_not_attack))
				{
					Debug.Log("Ran out of Ap or can't make a move");
					gameManagerS.endBaseTurn();
					can_not_heal = false;
					can_not_attack = false;
				}else{
					//Check to see if base can heal
					int temp = current_hp;
					if(heal()){ 
						int heal_points = healhp(heal_amount);
						if(heal_points == temp){
							Debug.Log ("no healing occurred");
							can_not_heal = true;
						}else{
							if(current_ap - heal_cost < 0){
								Debug.Log ("not enough ap to heal");
								can_not_heal = true;
							}else{
								Debug.Log ("healing occured");
								current_ap -= heal_cost;
							}
						}
					}else{
						Debug.Log ("can't heal, under attack");
						can_not_heal = true;
					}
					
					//Check to see if base can attack
					entityEnemyS enemy_s = getWeakestEnemy();
					if(enemy_s != null){	
						if(current_ap - attack_cost < 0){
							Debug.Log ("Can't attack, not enough ap, so END TURN");
							can_not_attack = true;
						}else{
							int damage_done = attackTarget (entityManagerS.getEnemyAt(enemy_s.x, enemy_s.z));
							Debug.Log ("ATTACK opponent, damage done = " + damage_done);
						}
						
					}else{
						Debug.Log ("no enemies in range, can't attack");
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
		Debug.Log ("Target pos " + target.x + ":" + target.z);
		Debug.Log ("Base pos " + x + ":" + z);
		current_ap -= attack_cost;
		return target.acceptDamage(attack_damage);
	}
	#endregion
	
	public override bool onDeath()
	{
		Application.Quit();
		return true;
	}
	
	//Player calls this method to change bases upgrade level
	//Assumption: entityMech will take care of whether upgrade is allowed	
	public bool upgradeBase(BaseUpgrade upgrade){
		//increase health, walls(armour), attack/range cost, and reduce ap
		switch(upgrade){
				case BaseUpgrade.Health1:
					health_level = BaseUpgrade.Health1;
					max_hp += 10;
					current_hp = max_hp; //when upgrade happens base health gets refilled
					return true;		
			
				case BaseUpgrade.Health2:
					health_level = BaseUpgrade.Health2;
					max_hp += 15;
					current_hp = max_hp; //when upgrade happens base health gets refilled
					return true;		
			
				case BaseUpgrade.Health3:
					health_level = BaseUpgrade.Health3;
					max_hp += 20;
					current_hp = max_hp; //when upgrade happens base health gets refilled
					return true;		
			
				case BaseUpgrade.Structure1:
					structure_level = BaseUpgrade.Structure1;
					base_armor += 5;
					return true;
			
				case BaseUpgrade.Structure2:
					structure_level = BaseUpgrade.Structure2;
					base_armor += 5;
					return true;
					
				case BaseUpgrade.Structure3:
					structure_level = BaseUpgrade.Structure3;
					base_armor += 10;
					return true;
					 
				case BaseUpgrade.Defenses1:
					defense_level = BaseUpgrade.Defenses1;
					attack_range  += 1;
					attack_damage += 2;
					return true;
	
				case BaseUpgrade.Defenses2:
					defense_level = BaseUpgrade.Defenses2;
					attack_range  += 1;
					attack_damage += 3;
					return true;
					
				case BaseUpgrade.Defenses3:
					defense_level = BaseUpgrade.Defenses3;
					attack_range  += 2;
					attack_damage += 5;
					return true;
	
				case BaseUpgrade.AP1:
					ap_cost_level = BaseUpgrade.AP1;
					attack_cost   -= 1;
					return true;
	
				case BaseUpgrade.AP2:
					ap_cost_level = BaseUpgrade.AP2;
					attack_cost   -= 2;
					return true;
					
				case BaseUpgrade.AP3:
					ap_cost_level = BaseUpgrade.AP3;
					attack_cost   -= 1;
					return true;
				
				default:
					//Nothing get's updated
					return false;
		}
	
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
		foreach(HexData h in hexManagerS.getAdjacentHexes(hexManagerS.getHex(x,z), attack_range)){
				//check to see if enemy is at hex
				entityEnemyS weak_enemy = entityManagerS.getEnemyAt(h.x, h.z);
				if(weak_enemy != null){
					if( weak_enemy.current_hp < low_health){
						Debug.Log("Weakest enemy is: " + weak_enemy.x + ":" + weak_enemy.z);
						low_health = weak_enemy.current_hp;
						final_weak_enemy = weak_enemy;
					}
				}
		}
		
		if(low_health == 9999){ 
			Debug.Log ("no enemy's in attack range");
			return null;
		}
		
		return final_weak_enemy;
		
	}

}