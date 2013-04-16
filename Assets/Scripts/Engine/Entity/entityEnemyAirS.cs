using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class entityEnemyAirS : Enemy {
	private bool   end_turn;
	private HexData last_move; //Can't Move Backwards unless can't move anywhere else; 
	
	public static bool show_health_bar = true;
	
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
		
		current_ap = 5;
		max_ap = 5;
		attack_cost = 4;
		attack_range = 2;
		attack_damage = 6;
		last_move = hexManagerS.getHex(x,z); //last move = current position  
		
//        while (i < vertices.Length) {
//			print (vertices[i].z);
////            vertices[i] += Vector3.up * Time.deltaTime;
//            i++;
//        } 
	}
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
		if(hexManagerS.getHex(x,z).vision_state == Vision.Live && show_health_bar){
			Vector3 center_top_ss = Camera.main.WorldToScreenPoint (center_top);
//			Vector3 center_top_ss = Camera.main.WorldToScreenPoint (transform.position + new Vector3(0,0,1.3F));
			
			GUI.DrawTexture(new Rect(center_top_ss.x - 43, Screen.height - center_top_ss.y, 83, 18), enginePlayerS.chris_hp_bg); 
			GUI.BeginGroup (new Rect (center_top_ss.x - 38, Screen.height - center_top_ss.y+4, (int)(73*((float)current_hp/(float)max_hp)), 10));
				GUI.DrawTexture(new Rect (0, 0, 73, 10), enginePlayerS.chris_hp, ScaleMode.StretchToFill);
			GUI.EndGroup (); 
		 	
			GUI.Label(new Rect(center_top_ss.x - 39, Screen.height - center_top_ss.y+4, 75, 10),  current_hp + "/" +  max_hp,  enginePlayerS.hp_bar_for_enemy); 
		}
	}
	
	// Update is called once per frame
	void Update () {	 
		//print("checking if alive enemy " + this.GetInstanceID());
		if(t==0){
			//Debug.Log ("enemies on board: " + entityManagerS.enemy_list.Count);
			t=1;
		}
		
		if(!onFire && (float)current_hp/(float)max_hp < .5F )
		{
			turnOnFire();
			onFire = true;
		}
		 
		if(checkIfDead()){
			//Debug.Log(this.GetInstanceID() + " is DEAD!!");
			onDeath();
			is_this_enemies_turn = false;
			gameManagerS.enemy_currently_acting = false;
			end_turn = false;
			last_path_cost = 0;
			path_to_base = new List<HexData>();
			path_to_mech = new List<HexData>();
			path_to_opponent = new List<HexData>();
		}
		else
		{
			if(gameManagerS.current_turn == Turn.Enemy  && !lerp_move && is_this_enemies_turn)
			{
				//if( t== 0 || t == 1 || t == 2){
				//Debug.Log ("ENEMY TURN NOW " + x + ":" + z);
//				print ("ENEMY hp = " + current_hp);
//				print ("ENEMY Ap = " + current_ap);
				
				//get base and mech positions
				entityBaseS base_s = entityManagerS.getBase();
				entityMechS mech_s = entityManagerS.getMech();
				chosen_path_is_mech = false;
				chosen_path_is_base = false;
				
				//check ap
				if(current_ap <= 0 || end_turn)
				{
					//Debug.Log("Ran out of Ap or can't make a move");
					is_this_enemies_turn = false;
					gameManagerS.enemy_currently_acting = false;
					end_turn = false;
					last_path_cost = 0;
					path_to_base = new List<HexData>();
					path_to_mech = new List<HexData>();
					path_to_opponent = new List<HexData>();
					//t =0;	//test
				}else{
				
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
					can_get_to_mech_location = canGetToOpponent(hexManagerS.getHex(x,z),hexManagerS.getHex(mech_s.x,mech_s.z), EntityE.Player, knows_mech_location, ref path_to_mech);
					double mech_path_cost = last_path_cost;
					if(!can_get_to_mech_location)
						mech_path_cost = 0;
					////Debug.Log ("*****Mech PATH COUNT: " + path_to_mech.Count);
					//Debug.Log ("Can get to mech location is " + can_get_to_mech_location + ", Mech Path Cost: " + mech_path_cost);
					//Debug.Log ("-------------------------------------------------");
					
					//Debug.Log ("Determine whether you can get to BASE and store path cost");
					can_get_to_base_location = canGetToOpponent(hexManagerS.getHex(x,z),hexManagerS.getHex(base_s.x,base_s.z), EntityE.Base, knows_base_location, ref path_to_base);
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
							path_to_base = extractPath(hexManagerS.getTraversablePath(hexManagerS.getHex(x,z), hexManagerS.getHex(base_s.x,base_s.z), EntityE.Player, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
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
							path_to_base =  extractPath(hexManagerS.getTraversablePath (hexManagerS.getHex(x,z), hexManagerS.getHex(base_s.x,base_s.z), EntityE.Player, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
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
							path_to_mech = extractPath(hexManagerS.getTraversablePath(hexManagerS.getHex(x,z), hexManagerS.getHex(mech_s.x,mech_s.z), EntityE.Base, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
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
							path_to_mech = extractPath(hexManagerS.getTraversablePath(hexManagerS.getHex(x,z), hexManagerS.getHex(mech_s.x,mech_s.z), EntityE.Base, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
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
								damage_done = attackTarget (target);
								current_hp = 0;
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
									last_move = hexManagerS.getHex(x,z);
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
								damage_done = attackTarget (mech_s);
								current_hp = 0;
								//Debug.Log ("Target is Mech , damage done = " + damage_done);
							}else if (chosen_path_is_base){
								damage_done = attackTarget (base_s);
								current_hp = 0;
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
								damage_done = attackTarget (target);
								current_hp = 0;
							}
						}else{
							//move enemy to next position
							if(current_ap - getTraverseAPCost(path_to_opponent[0].hex_type) < 0){
								//Debug.Log ("Update 5:8: Can't make a move, not enough ap, END TURN");
								end_turn = true;
							}else{
								//Debug.Log ("Update 5:9: Move enemy one position on path");
								last_move = hexManagerS.getHex(x,z);
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
				if(hexManagerS.getHex(x,z).vision_state != Vision.Live)
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
		HexData hex = hexManagerS.getHex(hex_x, hex_z);
		
		if(!entityManagerS.canTraverseHex(hex.x, hex.z, entity)){
			//Debug.Log ("enemy hex");
			return false;
		}else if(hex.hex_type == Hex.Perimeter){
			//Debug.Log ("enviro hex");
			return false;
		}else
			return true;
	} 
	
	public override bool canTraverseHex (int hex_x, int hex_z)
	{
		HexData hex = hexManagerS.getHex(hex_x, hex_z);
		
		if(!entityManagerS.canTraverseHex(hex.x, hex.z)){
			//Debug.Log ("enemy hex");
			return false;
		}else if(hex.hex_type == Hex.Perimeter){
			//Debug.Log ("enviro hex");
			return false;
		}else
			return true;
	} 
	
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
				return 2;
				
			case Hex.Mountain:
				return 2;
			
			case Hex.Water:
				return 2;
				
			case Hex.Perimeter: 
			default:
				return 9999;
		} 
	}
 	
	
	public void updateFoWState()
	{
		HexData occupying_hex = hexManagerS.getHex(x, z);
		switch(occupying_hex.vision_state)
		{
		case Vision.Live:
			gameObject.renderer.enabled = true;
			renderer.material.SetColor("_Color", Color.white);
			break;
		case Vision.Visited:
			gameObject.renderer.enabled = true;
			renderer.material.SetColor("_Color", Color.gray);
			break;
		case Vision.Unvisted:
			gameObject.renderer.enabled = false;
			break;
		default:
			throw new System.Exception("update FoW Combatable error!!");
		}
	}

	
	public override bool onDeath()
	{
		entityManagerS.purgeEnemy(this);
		return false;
	}
}
