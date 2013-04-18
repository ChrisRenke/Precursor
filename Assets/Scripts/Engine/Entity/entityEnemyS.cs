using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class entityEnemyS : Enemy {
	private bool   end_turn;
	private HexData last_move; //Can't Move Backwards unless can't move anywhere else; 
	
	public AudioClip sound_shoot;
	
	public override int attackTarget(Combatable t)
	{
		return 0;
	}
	
	int t = 0; //test
	
	//Use this for initialization
	void Start () {
 		child_fire = gameObject.transform.GetChild(0);//.GetComponentsInChildren<ParticleSystem>();
//		Debug.Log(child_fire.name); //gets the fire child 
		turnOffFire();
		path_to_base = new List<HexData>();
		path_to_mech = new List<HexData>();
		path_to_opponent = new List<HexData>();
		last_path_cost = 0;		
		end_turn = false;
		
		current_hp = 15;
		max_hp = 15;
		
		enemy_type = EntityE.Enemy;
		
		current_ap = 8;
		max_ap = 8;
		attack_cost = 4;
		attack_range = 2;
		attack_damage = 4;
		last_move = hm.getHex(x,z); //last move = current position  

	}
	
	public Transform child_fire;  
	
	public void turnOnFire(){
		
		child_fire.transform.GetChild(0).particleEmitter.emit = true; 
		child_fire.transform.GetChild(1).particleEmitter.emit = true; 
		child_fire.transform.GetChild(2).particleEmitter.emit = true; 
	}
	
	public void turnOffFire(){ 
		child_fire.transform.GetChild(0).particleEmitter.emit = false; 
		child_fire.transform.GetChild(1).particleEmitter.emit = false; 
		child_fire.transform.GetChild(2).particleEmitter.emit = false; 
	}
	
	public bool onFire = false;
	
	void OnGUI()
	{ 
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int i = 0;
		Vector3 max = vertices[1] + transform.position;
		Vector3 min = vertices[3]+ transform.position;
		center_top = new Vector3((max.x + min.x)/2, max.y+5, max.z);
		
		//script variable used to check whether menu screen is on
		if(hm.getHex(x,z).vision_state == Vision.Live){
			Vector3 center_top_ss = Camera.main.WorldToScreenPoint (center_top);
//			Vector3 center_top_ss = Camera.main.WorldToScreenPoint (transform.position + new Vector3(0,0,1.3F));
			
			GUI.DrawTexture(new Rect(center_top_ss.x - 43, Screen.height - center_top_ss.y, 83, 18), ep.chris_hp_bg_in); 
			GUI.BeginGroup (new Rect (center_top_ss.x - 38, Screen.height - center_top_ss.y+4, (int)(73*((float)current_hp/(float)max_hp)), 10));
				GUI.DrawTexture(new Rect (0, 0, 73, 10), ep.chris_hp_in, ScaleMode.StretchToFill);
			GUI.EndGroup (); 
		 	
			GUI.Label(new Rect(center_top_ss.x - 39, Screen.height - center_top_ss.y+4, 75, 10),  current_hp + "/" +  max_hp,  ep.hp_bar); 
		}
	}
	
	// Update is called once per frame
	void Update () {	 
		//print("checking if alive enemy " + this.GetInstanceID());
		if(t==0){
			//Debug.Log ("enemies on board: " + em.enemy_list.Count);
			t=1;
		}
		
		if(!onFire && (float)current_hp/(float)max_hp < .5F &&  isVisibleInWorld())
		{
			turnOnFire();
			onFire = true;
		}
		 
		if(checkIfDead()){
			//Debug.Log(this.GetInstanceID() + " is DEAD!!");
			onDeath();
		}
		else
		{
			if(gm.current_turn == Turn.Enemy  && !lerp_move && is_this_enemies_turn)
			{
				//if( t== 0 || t == 1 || t == 2){
				//Debug.Log ("ENEMY TURN NOW " + x + ":" + z);
//				print ("ENEMY hp = " + current_hp);
//				print ("ENEMY Ap = " + current_ap);
				
				//get base and mech positions
				entityBaseS base_s = em.getBase();
				entityMechS mech_s = em.getMech();
				chosen_path_is_mech = false;
				chosen_path_is_base = false;
				
				//check ap
				if(current_ap <= 0 || end_turn)
				{
					//Debug.Log("Ran out of Ap or can't make a move");
					is_this_enemies_turn = false;
					gm.enemy_currently_acting = false;
					end_turn = false;
					last_path_cost = 0;
					path_to_base = new List<HexData>();
					path_to_mech = new List<HexData>();
					path_to_opponent = new List<HexData>();
					//t =0;	//test
				}
				else if(!gm.waiting_after_shot){
				
					//MAIN IDEA
					//if can see mech and base and enemy is able to get to both mech and base
					//then can get to mech and base is true.
					//if cant see mech and base and enemy then checks to see if in visible range
					//then can get to mech and base is true if they are in visible range and a path can be found.
					//if they aren't in visible range or a path can't be found then can get to mech and base will be false
					
					//Debug.Log ("WORKING WITH AN ENEMY");
					//Debug.Log ("AP LEFT:" + current_ap);
					
					//Debug.Log ("Knows mech location is " + knows_mech_location + ", Knows base location is " + knows_base_location);
					//Debug.Log ("mech location is " + mech_s.x + ":" + mech_s.z + ", base location is " + base_s.x + ":" + base_s.z);
					
					
					//Debug.Log ("Determine whether you can get to MECH and store path cost");
					can_get_to_mech_location = canGetToOpponent(hm.getHex(x,z),hm.getHex(mech_s.x,mech_s.z), EntityE.Player, knows_mech_location, ref path_to_mech);
					double mech_path_cost = last_path_cost;
					if(!can_get_to_mech_location)
						mech_path_cost = 0;
					////Debug.Log ("*****Mech PATH COUNT: " + path_to_mech.Count);
					//Debug.Log ("Can get to mech location is " + can_get_to_mech_location + ", Mech Path Cost: " + mech_path_cost);
					//Debug.Log ("-------------------------------------------------");
					
					//Debug.Log ("Determine whether you can get to BASE and store path cost");
					can_get_to_base_location = canGetToOpponent(hm.getHex(x,z),hm.getHex(base_s.x,base_s.z), EntityE.Base, knows_base_location, ref path_to_base);
					double base_path_cost = last_path_cost;
					if(!can_get_to_base_location)
						base_path_cost = 0;
					////Debug.Log ("*****Bas PATH COUNT: " + path_to_base.Count);
					//Debug.Log ("Can get to base location is " + can_get_to_base_location + ", Base Path Cost: " + base_path_cost);
					//Debug.Log ("-------------------------------------------------");
					
					
					
					//Debug.Log ("DECIDE PATH TO TAKE");
					if(knows_mech_location && knows_base_location){
						//enemy knows mech location and knows base location, check which path is shorter
						if(can_get_to_mech_location && can_get_to_base_location){
							//Debug.Log ("Update 1:2: knows mech/base location, check which path is shorter");
							path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, ref path_to_mech, ref path_to_base);
						}
						else if(can_get_to_base_location){
							//Debug.Log ("Update 1:3: knows mech/base location, can't get to mech so go to base");
							path_to_opponent = path_to_base;
							chosen_path_is_base = true;
						}
						else if(can_get_to_mech_location){
							//Debug.Log ("Update 1:4: knows mech/base location, can't get to base so go to mech");
							path_to_opponent = path_to_mech;
							chosen_path_is_mech = true;
						}else{
							//Debug.Log("Update 1:3: knows mech/base location, can't find path to mech or base");
							path_to_opponent = new List<HexData>();
						}
	
					}else if(knows_base_location){
	
						if(can_get_to_mech_location && can_get_to_base_location){
							//Debug.Log ("Update 2:1: knows base location, find least cost path");
							path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, ref path_to_mech, ref path_to_base);
						}
						else if(can_get_to_base_location){	
							//Debug.Log ("Update 2:2: knows base location, can't get to mech, check for shorter path to base assuming we can't see mech");
							path_to_base = extractPath(hm.getTraversablePath(hm.getHex(x,z), hm.getHex(base_s.x,base_s.z), EntityE.Player, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
							base_path_cost = last_path_cost;
							if(path_to_base.Count == 0){
								//Debug.Log ("Update 2:3: knows base location, shouldn't reach this code, since path to base already found");
								path_to_opponent = new List<HexData>();
							}
							else{
								//Debug.Log ("Update 2:4: knows base location, going on path to base");
								path_to_opponent = path_to_base;
								chosen_path_is_base = true;
							}
						}else if(can_get_to_mech_location){
							//Debug.Log ("Update 2:5: knows base location, can't get to base so go to mech");
							path_to_opponent = path_to_mech;
						}else{
							//Debug.Log ("Update 2:6: knows base location, can't find a path to either but try to find path to base assuming mech may be blocking path");
							path_to_base =  extractPath(hm.getTraversablePath (hm.getHex(x,z), hm.getHex(base_s.x,base_s.z), EntityE.Player, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
							base_path_cost = last_path_cost;
							if(path_to_base.Count == 0){
								//Debug.Log ("Update 2:7: knows base location, path not found to base or mech");
								path_to_opponent = new List<HexData>();
							}else{
								//Debug.Log ("Update 2:8: knows base location, path to base found if enemy not considered");
								path_to_opponent = path_to_base;
								chosen_path_is_base = true;
							}
						}
					}else if(knows_mech_location){
						//enemy knows mech location, check to see if enemy can find least cost path
						if(can_get_to_mech_location && can_get_to_base_location){
							//Debug.Log ("Update 3:1: knows mech location, check which path is shorter");
							path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, ref path_to_mech, ref path_to_base);
						}else if(can_get_to_mech_location){
							//Debug.Log ("Update 3:2: knows mech location, can't see base so try to find shorter path to mech");
							path_to_mech = extractPath(hm.getTraversablePath(hm.getHex(x,z), hm.getHex(mech_s.x,mech_s.z), EntityE.Base, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
							mech_path_cost = last_path_cost;
							if(path_to_mech.Count == 0){
								//Debug.Log ("Update 3:3: knows mech location, shouldn't reach this code since path to mech known");
								path_to_opponent = new List<HexData>();
							}else{
								//Debug.Log ("Update 3:4: knows mech location, path to mech found");
								path_to_opponent = path_to_mech;
								chosen_path_is_mech = true;
							}
						}else if(can_get_to_base_location){
							//Debug.Log ("Update 3:5: knows mech location, can't get to mech but found path to base");
							path_to_opponent = path_to_base;
							chosen_path_is_base = true;
						}else{
							//Debug.Log ("Update 3:6: knows mech location, try to find path to mech assuming path is blocked by base");
							path_to_mech = extractPath(hm.getTraversablePath(hm.getHex(x,z), hm.getHex(mech_s.x,mech_s.z), EntityE.Base, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
							mech_path_cost = last_path_cost;
							if(path_to_mech.Count == 0){
								//Debug.Log ("Update 3:7: knows mech location, can't find a path to base or mech");
								path_to_opponent = new List<HexData>();
							}else{
								//Debug.Log ("Update 3:8: knows mech location, going on path to mech");
								path_to_opponent = path_to_mech;
								chosen_path_is_mech = true;
							}
						}
					}else{
						//enemy doesn't know where anyone is, so see if anyone is around and find least cost path
						if(can_get_to_mech_location && can_get_to_base_location){
							//Debug.Log ("Update 4:1: don't know base or mech location, check to see which path is shorter");
							path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, ref path_to_mech, ref path_to_base);
						}else if (can_get_to_mech_location){
							//Debug.Log ("Update 4:2: don't know base or mech location, can't get to base so go to mech");
							path_to_opponent = path_to_mech;
							chosen_path_is_mech = true;
						}else if (can_get_to_base_location){
							//Debug.Log ("Update 4:3 don't know base or mech location, can't get to mech so go to base");
							path_to_opponent = path_to_base;
							chosen_path_is_base = true;
						}else{ 
							//Debug.Log ("Update 4:5: don't know base or mech location, can't find path to either base or mech");
							path_to_opponent = new List<HexData>();
						}
					}
					
					
					//Debug.Log ("FINALIZE path move");
					if(path_to_opponent.Count == 0){
				 		//Debug.Log ("Update 5:1: no path found so try to move randomly or attack");
						
						//check to see if enemy is in attackable range
						Combatable target = targetInAttackRange(base_s, mech_s);						
						if(target != null){
							if(current_ap - attack_cost < 0){
								//Debug.Log ("Update 5:6: Can't attack, not enough ap, so END TURN");
								end_turn = true;
							}else{
								int damage_done = -1;
								audio.PlayOneShot(sound_shoot);
								damage_done = shootStuff (target);
							}
						}else{
							//nothing in attackable range so try to move randomly
							path_to_opponent = getAdjacentTraversableHexes();
							
							if(path_to_opponent.Count > 0){
								//try to make a random move
								int move = UnityEngine.Random.Range(0, path_to_opponent.Count);
								bool not_end = true;
								
								//don't enter loop if path_to_opponent.Count is 1 since only one choice to make
								//Debug.Log ("LastMove:" + last_move.x + ":" + last_move.z);
								while(not_end && path_to_opponent.Count != 1){
									if(path_to_opponent[move].x == last_move.x && path_to_opponent[move].z == last_move.z){
										//Debug.Log("Enemy trying to move backwards, try another move");
										move = UnityEngine.Random.Range(0, path_to_opponent.Count);
									}else{
								 		//Debug.Log("Found a good move");
										not_end = false;
									}
								}
								//Debug.Log ("NewMove:" + path_to_opponent[move].x + ":" + path_to_opponent[move].z);
								if(current_ap - getTraverseAPCost(path_to_opponent[move].hex_type) < 0){
									//Debug.Log ("Update 5:2: can't make a move, not enough ap, END TURN");
									end_turn = true;
								}else{
									//Debug.Log ("Update 5:3: Move to random position");
									last_move = hm.getHex(x,z);
									makeMove(path_to_opponent[move]);
								}
								
								
							}else{
								//Debug.Log ("Update 5:4: can't move, END TURN");
								end_turn = true;
							}
						}
					}else if(path_to_opponent.Count == 1 && path_to_opponent[0].x == x && path_to_opponent[0].z == z){
						//Debug.Log ("Update 5:5: enemy found attackable opponent");
						if(current_ap - attack_cost < 0){
							//Debug.Log ("Update 5:6: Can't attack, not enough ap, so END TURN");
							end_turn = true;
						}else{
							int damage_done = -1;
							if(chosen_path_is_mech){
								audio.PlayOneShot(sound_shoot);
								damage_done = shootStuff (mech_s); 
								//Debug.Log ("Target is Mech , damage done = " + damage_done);
							}else if (chosen_path_is_base){
								audio.PlayOneShot(sound_shoot);
								damage_done = shootStuff (base_s);
								//Debug.Log ("Target is Base , damage done = " + damage_done);
							}else{
								//Debug.Log ("ERROR: ATTACK opponent, wasn't an attackable opponent, damage done = " + damage_done);
								end_turn = true;
							}
						}
						
					}else{ 
						//check to see if enemy is in attackable range
						Combatable target = targetInAttackRange(base_s, mech_s);						
						if(target != null){
							if(current_ap - attack_cost < 0){
								//Debug.Log ("Update 5:6: Can't attack, not enough ap, so END TURN");
								end_turn = true;
							}else{
								int damage_done = -1;
								audio.PlayOneShot(sound_shoot);
								damage_done = shootStuff (target);
							}
						}else{
							//move enemy to next position
							if(current_ap - getTraverseAPCost(path_to_opponent[0].hex_type) < 0){
								//Debug.Log ("Update 5:8: Can't make a move, not enough ap, END TURN");
								end_turn = true;
							}else{
								//Debug.Log ("Update 5:9: Move enemy one position on path");
								last_move = hm.getHex(x,z);
								makeMove(path_to_opponent[0]);
							}
						}
					}			
				
				
				}
					//t++;
				//}//test
	
			}
			
		
			if(lerp_move)
			{	
				if(hm.getHex(x,z).vision_state != Vision.Live)
				{
					
					lerp_move = false;
					transform.position = ending_pos;				
			 		updateFoWState();
					
				}
				else{
						
					transform.position = Vector3.Lerp(transform.position, ending_pos,  moveTime);
			 		moveTime += Time.deltaTime/dist;
					
					
					if(Vector3.Distance(transform.position, ending_pos) <= .05)
					{ 
						lerp_move = false;
						transform.position = ending_pos;				
				 		updateFoWState();
					}	
				}
			}
		}//not dead
	}
	
	public override bool canTraverseHex (int hex_x, int hex_z, EntityE entity)
	{
		HexData hex = hm.getHex(hex_x, hex_z);
		
		if(!em.canTraverseHex(hex.x, hex.z, entity)){
			//Debug.Log ("enemy hex");
			return false;
		}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
			//Debug.Log ("enviro hex");
			return false;
		}else
			return true;
	}
	
	public override bool canTraverseHex (int hex_x, int hex_z)
	{
		HexData hex = hm.getHex(hex_x, hex_z);
		
		if(!em.canTraverseHex(hex.x, hex.z)){
			//Debug.Log ("enemy hex");
			return false;
		}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
			//Debug.Log ("enviro hex");
			return false;
		}else
			return true;
	} 
	
//	public int shootStuff(Combatable target)
//	{
//		getDirectionToFaceToAttack(target);
//		
//		
//		int huzzah = target.acceptDamage(attack_damage);
//		current_ap -= attack_cost;
//		audio.PlayOneShot(sound_shoot);
//		gm.time_after_shot_start = Time.time;
//		return huzzah;
//	}

	public override int getTraverseAPCost(Hex hex_type){
		switch(hex_type)
		{
			case Hex.Desert:
			case Hex.Farmland:
			case Hex.Grass:
				return 2;
			
			case Hex.Marsh:
			case Hex.Hills:
			case Hex.Forest:
				return 4;
				
			case Hex.Mountain:
				return 9999;
			
			case Hex.Water:
				return 9999;
				
			case Hex.Perimeter: 
			default:
				return 9999;
		} 
	}
	
 
	public bool isVisibleInWorld(){
		HexData occupying_hex = hm.getHex(x, z);
		return occupying_hex.vision_state == Vision.Live;
	}
	
	
	
	public void updateFoWState()
	{
		HexData occupying_hex = hm.getHex(x, z);
		switch(occupying_hex.vision_state)
		{
		case Vision.Live: 
			gameObject.renderer.enabled = true;
			if(current_hp <= max_hp/2)
			{
			    onFire = true;
				turnOnFire();
			}
			renderer.material.SetColor("_Color", Color.white);
			break;
		case Vision.Visited:
			gameObject.renderer.enabled = true;
			if(onFire)
				turnOffFire();
			renderer.material.SetColor("_Color", Color.gray);
			break;
		case Vision.Unvisted: 
			if(onFire)
				turnOffFire();
			gameObject.renderer.enabled = false;
			break;
		default:
			throw new System.Exception("update FoW Combatable error!!");
		}
	}

	
	public override bool onDeath()
	{
		em.purgeEnemy(this);
		return false;
	}
}
