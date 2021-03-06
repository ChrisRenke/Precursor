using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class entityEnemyS : Combatable, IMove, IPathFind {
	private List<HexData> traversable_hexes; //Hold traversable hexes
	private List<HexData> untraversable_hexes; //Hold untraversable hexes
	private List<HexData> path_to_base; //Hold traversable hex path to base
	private List<HexData> path_to_mech; //Hold traversable hex path to mech
	private List<HexData> path_to_opponent; //Hold traversable hex path to chosen opponent
	
	private bool   can_get_to_mech_location;
	private bool   can_get_to_base_location;
	private double last_path_cost; //path cost of most recent path gotten from traversable path call
	private bool   end_turn;
	
	//Put a weight on entities to decide which is more important when having to make a path choice
	public double base_weight = 10;
	public double mech_weight = 5;
	
	public bool knows_mech_location;
	public bool knows_base_location;
	
	public bool is_this_enemies_turn;
	
	
	 
	
	int t = 0; //test
	
	// Use this for initialization
	void Start () {
		path_to_base = new List<HexData>();
		path_to_mech = new List<HexData>();
		path_to_opponent = new List<HexData>();
		last_path_cost = 0;		
		end_turn = false;
		
		current_hp = 15;
		max_hp = 15;
		
		current_ap = 8;
		max_ap = 8;
	}
	
	void OnGUI()
	{
		Vector3 screen_pos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y+30, 200, 15), current_hp + "/" + max_hp + " HP", enginePlayerS.hover_text);
		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y + 45, 200, 15), current_ap + "/" + max_ap + " AP", enginePlayerS.hover_text);
	}
	
		//Extract hexes from path and put hexes into a List 
	private List<HexData> extractPath (Path path)
	{
		
		if(path != null){
			last_path_cost = path.TotalCost;
			Debug.Log ("getTraversablePath: path cost = " + last_path_cost);
		}
		
		List<HexData> list = new List<HexData>();
		if(path != null){
			foreach (HexData hex in path){
				list.Add(hex);
			}
			Debug.Log ("extractPath: size of path = " + list.Count);
			
			list.Reverse();	//path comes in backwards
			
			//if list size is 2, don't remove the enemies position from list
			if(list.Count > 2){
				list.RemoveAt(0); //first element is the current enemies position
			}
			
			list.RemoveAt(list.Count - 1); //last element is the destination hex
		}else{
			Debug.Log ("extractPath: path is empty");
		}
		
		return list;
	}
	
	// Update is called once per frame
	void Update () {	
//		print ("checking if alive enemy " + this.GetInstanceID());
		if(checkIfDead()){
		print (this.GetInstanceID() + " is DEAD!!");
			onDeath();
			
		}
		else
		{
			if(gameManagerS.current_turn == Turn.Enemy  && !lerp_move && is_this_enemies_turn)
			{
				//if( t== 0 || t == 1 || t == 2){
				Debug.Log ("enemy turn now");
				
				//get base and mech positions
				entityBaseS base_s = entityManagerS.getBase();
				entityMechS mech_s = entityManagerS.getMech();
				
				//check ap
				if(current_ap <= 0 || end_turn)
				{
					Debug.Log("Ran out of Ap or can't make a move");
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
					//if cant see mech and base and enemy then check to see if in visible range
					//then can get to mech and base is true if they are in visible range and a path can be found.
					//if they aren't in visible range or a path can't be found then can get to mech and base will be false
					
					Debug.Log ("WORKING WITH AN ENEMY");
					Debug.Log ("AP LEFT:" + current_ap);
					
					Debug.Log ("Knows mech location is " + knows_mech_location + ", Knows base location is " + knows_base_location);
					Debug.Log ("mech location is " + mech_s.x + ":" + mech_s.z + ", base location is " + base_s.x + ":" + base_s.z);
					
					
					Debug.Log ("Determine whether you can get to MECH and store path cost");
					can_get_to_mech_location = canGetToOpponent(hexManagerS.getHex(x,z),hexManagerS.getHex(mech_s.x,mech_s.z), EntityE.Player, knows_mech_location, ref path_to_mech);
					double mech_path_cost = last_path_cost;
					if(!can_get_to_mech_location)
						mech_path_cost = 0;
					//Debug.Log ("*****Mech PATH COUNT: " + path_to_mech.Count);
					Debug.Log ("Can get to mech location is " + can_get_to_mech_location + ", Mech Path Cost: " + mech_path_cost);
					Debug.Log ("-------------------------------------------------");
					
					Debug.Log ("Determine whether you can get to BASE and store path cost");
					can_get_to_base_location = canGetToOpponent(hexManagerS.getHex(x,z),hexManagerS.getHex(base_s.x,base_s.z), EntityE.Base, knows_base_location, ref path_to_base);
					double base_path_cost = last_path_cost;
					if(!can_get_to_base_location)
						base_path_cost = 0;
					//Debug.Log ("*****Bas PATH COUNT: " + path_to_base.Count);
					Debug.Log ("Can get to base location is " + can_get_to_base_location + ", Base Path Cost: " + base_path_cost);
					Debug.Log ("-------------------------------------------------");
					
					
					
					Debug.Log ("DECIDE PATH TO TAKE");
					if(knows_mech_location && knows_base_location){
						//enemy knows mech location and knows base location, check which path is shorter
						if(can_get_to_mech_location && can_get_to_base_location){
							Debug.Log ("Update 1:2: knows mech/base location, check which path is shorter");
							path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, ref path_to_mech, ref path_to_base);
						}
						else if(can_get_to_base_location){
							Debug.Log ("Update 1:3: knows mech/base location, can't get to mech so go to base");
							path_to_opponent = path_to_base;
						}
						else if(can_get_to_mech_location){
							Debug.Log ("Update 1:4: knows mech/base location, can't get to base so go to mech");
							path_to_opponent = path_to_mech;
						}else{
							Debug.Log("Update 1:3: knows mech/base location, can't find path to mech or base");
							path_to_opponent = new List<HexData>();
						}
	
					}else if(knows_base_location){
	
						if(can_get_to_mech_location && can_get_to_base_location){
							Debug.Log ("Update 2:1: knows base location, find least cost path");
							path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, ref path_to_mech, ref path_to_base);
						}
						else if(can_get_to_base_location){	
							Debug.Log ("Update 2:2: knows base location, can't get to mech, check for shorter path to base assuming we can't see mech");
							path_to_base = extractPath(hexManagerS.getTraversablePath(hexManagerS.getHex(x,z), hexManagerS.getHex(base_s.x,base_s.z), EntityE.Player, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
							base_path_cost = last_path_cost;
							if(path_to_base.Count == 0){
								Debug.Log ("Update 2:3: knows base location, shouldn't reach this code, since path to base already found");
								path_to_opponent = new List<HexData>();
							}
							else{
								Debug.Log ("Update 2:4: knows base location, going on path to base");
								path_to_opponent = path_to_base;
							}
						}else if(can_get_to_mech_location){
							Debug.Log ("Update 2:5: knows base location, can't get to base so go to mech");
							path_to_opponent = path_to_mech;
						}else{
							Debug.Log ("Update 2:6: knows base location, can't find a path to either but try to find path to base assuming mech may be blocking path");
							path_to_base =  extractPath(hexManagerS.getTraversablePath (hexManagerS.getHex(x,z), hexManagerS.getHex(base_s.x,base_s.z), EntityE.Player, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
							base_path_cost = last_path_cost;
							if(path_to_base.Count == 0){
								Debug.Log ("Update 2:7: knows base location, path not found to base or mech");
								path_to_opponent = new List<HexData>();
							}else{
								Debug.Log ("Update 2:8: knows base location, path to base found if enemy not considered");
								path_to_opponent = path_to_base;
							}
						}
					}else if(knows_mech_location){
						//enemy knows mech location, check to see if enemy can find least cost path
						if(can_get_to_mech_location && can_get_to_base_location){
							Debug.Log ("Update 3:1: knows mech location, check which path is shorter");
							path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, ref path_to_mech, ref path_to_base);
						}else if(can_get_to_mech_location){
							Debug.Log ("Update 3:2: knows mech location, can't see base so try to find shorter path to mech");
							path_to_mech = extractPath(hexManagerS.getTraversablePath(hexManagerS.getHex(x,z), hexManagerS.getHex(mech_s.x,mech_s.z), EntityE.Base, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
							mech_path_cost = last_path_cost;
							if(path_to_mech.Count == 0){
								Debug.Log ("Update 3:3: knows mech location, shouldn't reach this code since path to mech known");
								path_to_opponent = new List<HexData>();
							}else{
								Debug.Log ("Update 3:4: knows mech location, path to mech found");
								path_to_opponent = path_to_mech;
							}
						}else if(can_get_to_base_location){
							Debug.Log ("Update 3:5: knows mech location, can't get to mech but found path to base");
							path_to_opponent = path_to_base;	
						}else{
							Debug.Log ("Update 3:6: knows mech location, try to find path to mech assuming path is blocked by base");
							path_to_mech = extractPath(hexManagerS.getTraversablePath(hexManagerS.getHex(x,z), hexManagerS.getHex(mech_s.x,mech_s.z), EntityE.Base, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
							mech_path_cost = last_path_cost;
							if(path_to_mech.Count == 0){
								Debug.Log ("Update 3:7: knows mech location, can't find a path to base or mech");
								path_to_opponent = new List<HexData>();
							}else{
								Debug.Log ("Update 3:8: knows mech location, going on path to mech");
								path_to_opponent = path_to_mech;
							}
						}
					}else{
						//enemy doesn't know where anyone is, so see if anyone is around and find least cost path
						if(can_get_to_mech_location && can_get_to_base_location){
							Debug.Log ("Update 4:1: don't know base or mech location, check to see which path is shorter");
							path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, ref path_to_mech, ref path_to_base);
						}else if (can_get_to_mech_location){
							Debug.Log ("Update 4:2: don't know base or mech location, can't get to base so go to mech");
							path_to_opponent = path_to_mech;
						}else if (can_get_to_base_location){
							Debug.Log ("Update 4:3 don't know base or mech location, can't get to mech so go to base");
							path_to_opponent = path_to_base;
						}else{ 
							Debug.Log ("Update 4:5: don't know base or mech location, can't find path to either base or mech");
							path_to_opponent = new List<HexData>();
						}
					}
					
					
					
					Debug.Log ("FINALIZE path move");
					if(path_to_opponent.Count == 0){
				 		Debug.Log ("Update 5:1: no path found so try to move randomly");
						
						path_to_opponent = getAdjacentTraversableHexes();
						
						if(path_to_opponent.Count > 0){
							//try to make a random move
							int move = UnityEngine.Random.Range(0, path_to_opponent.Count);
							if(current_ap - getTraverseAPCost(path_to_opponent[move].hex_type) < 0){
								Debug.Log ("Update 5:2: can't make a move, not enough ap, END TURN");
								end_turn = true;
							}else{
								Debug.Log ("Update 5:3: Move to random position");
								makeMove(path_to_opponent[move]);
							}
						}else{
							Debug.Log ("Update 5:4: can't move, END TURN");
							end_turn = true;
						}
					}else if(path_to_opponent.Count == 1 && path_to_opponent[0].x == x && path_to_opponent[0].z == z){
						Debug.Log ("Update 5:5: enemy found attackable opponent");
						if(current_ap - attack_cost < 0){
							Debug.Log ("Update 5:6: Can't attack, not enough ap, so END TURN");
							end_turn = true;
						}else{
							int damage_done = attackTarget (entityManagerS.getCombatableAt(path_to_opponent[0].x, path_to_opponent[0].z));
							Debug.Log ("Update 5:7: ATTACK opponent, damage done = " + damage_done);
						}
						
					}else{ 
						//move enemy to next position
						if(current_ap - getTraverseAPCost(path_to_opponent[0].hex_type) < 0){
							Debug.Log ("Update 5:8: Can't make a move, not enough ap, END TURN");
							end_turn = true;
						}else{
							Debug.Log ("Update 5:9: Move enemy one position on path");
							makeMove(path_to_opponent[0]);
						}
					}			
				
				
				}
					//t++;
				//}//test
				
	
			}
			
		
			if(lerp_move)
			{	
				transform.position = Vector3.Lerp(transform.position, ending_pos,  moveTime);
		 		moveTime += Time.deltaTime/dist;
				
				
				if(Vector3.Distance(transform.position, ending_pos) <= .05)
				{ 
					lerp_move = false;
					transform.position = ending_pos;				
			 		updateFoWState();
				}	
			}
		}//not dead
	}
	
	
	public double getTraverseAPCostPathVersion (HexData hex_start, HexData hex_end)
	{	
		return (double) getTraverseAPCost(hex_end.hex_type);
	}
	
	public List<HexData> getAdjacentTraversableHexes () {
		return getAdjacentTraversableHexes (x, z);
	}
 
	//Will use this method for single movements, alternative method used for A*, see IPathFind 
	public List<HexData> getAdjacentTraversableHexes (int _x, int _z) 
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(_x, _z);
		//Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++)
			if(canTraverse(adjacent_hexes[i]))
				result_hexes.Add(adjacent_hexes[i]);
		
		//Debug.Log(result_hexes.Count + " found adjacent goods");
		return result_hexes;
	}
 
	
	public List<HexData> getAdjacentUntraversableHexes () {
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x, z);
		
		//See which of the adjacent hexes are NOT traversable
		for(int i = 0; i < adjacent_hexes.Length; i++)
			if(!canTraverse(adjacent_hexes[i]))
				result_hexes.Add(adjacent_hexes[i]); 
		
		return result_hexes;
	}

	public bool canTraverse (HexData hex)
	{
		return canTraverse(hex.x, hex.z);
	}  
	
	public bool canTraverse (int hex_x, int hex_z)
	{
		HexData hex = hexManagerS.getHex(hex_x, hex_z);
		
		if(!entityManagerS.canTraverseHex(hex.x, hex.z)){
			//Debug.Log ("enemy hex");
			return false;
		}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
			//Debug.Log ("enviro hex");
			return false;
		}else
			return true;
	} 
	
	//alternative can traverse methods where an entity can be excluded from search
	public bool canTraverse (HexData hex, EntityE entity)
	{
		return canTraverse(hex.x, hex.z, entity);
	}  
	
	public bool canTraverse (int hex_x, int hex_z, EntityE entity)
	{
		HexData hex = hexManagerS.getHex(hex_x, hex_z);
		
		if(!entityManagerS.canTraverseHex(hex.x, hex.z, entity)){
			//Debug.Log ("enemy hex");
			return false;
		}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
			//Debug.Log ("enviro hex");
			return false;
		}else
			return true;
	} 
 	
	public bool makeMove (HexData hex)
	{
		facing_direction = hex.direction_from_central_hex;
		if(!canTraverse(hex))
			throw new MissingComponentException("Can't move to this spot, invalid location!");
		
		//subtract ap cost from total
		current_ap -= getTraverseAPCost(hex.hex_type);
		
		//update enemy hex tags
		moveInWorld(hex.x, hex.z, 2F);
		x = hex.x;
		z = hex.z;
		return true;
	} 
	
	
	public List<HexData> getAdjacentTraversableHexes (HexData hex, HexData destination, EntityE entity)
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		//Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(canTraverse(adjacent_hexes[i], entity) || (adjacent_hexes[i].x == destination.x && adjacent_hexes[i].z == destination.z)){
				//add hex to traversable array
				result_hexes.Add(adjacent_hexes[i]); 
			}
		}
		
		//Debug.Log ("Number of result_hexes " + result_hexes.Count);
		return result_hexes;
	}
	
 
	public int getTraverseAPCost(Hex hex_type){
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
	
	//True if enemy can can get to specified opponent
	bool canGetToOpponent(HexData enemy, HexData opponent, EntityE entity_opponent, bool knows_opponents_location, ref List<HexData> path_to_opp){

		if(knows_opponents_location){
			//if enemy already "knows opponents location" then don't worry about visibility just get path to opponent
			Debug.Log ("canGetToOpponent: knows enemy location, get path:" + entity_opponent);
			path_to_opp = extractPath(hexManagerS.getTraversablePath(enemy, opponent, EntityE.Node, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
			
			if(path_to_opp.Count == 0){
				Debug.Log ("canGetToOpponent: knows enemy location, but can't find path");
				return false;
			}else{
				Debug.Log ("canGetToOpponent: knows enemy location, and found path");
				return true;
			}
		}
		else
		{
			Debug.Log ("canGetToOpponent: doesn't know enemy location, see if enemy is visible in sight range: " + entity_opponent);
			//TODO: TESTING****using old method*********
			if(canSeeHex(opponent)){ 
			//if(canSeeHex(opponent)){ ****
				Debug.Log ("canGetToOpponent: doesn't know enemy location, get path to " + entity_opponent);
				path_to_opp = extractPath(hexManagerS.getTraversablePath(enemy, opponent, EntityE.Node, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
				if(path_to_opp.Count == 0){
					Debug.Log ("canGetToOpponent: enemy is visible, but can't find path");
					return false;
				}else{
					Debug.Log ("canGetToOpponent: enemy is visible, and found path");
					return true;
				}
			}else{
				Debug.Log ("canGetToOpponent: doesn't know enemy location, and enemy not visible");
				return false;
			}
		}
	}
	
	
	//Return true if entity is visible(enemy_visibility_range) to enemy based on enemies current x and z
	public bool canSeeHex(HexData hex)
	{		

		Debug.Log(hex + " = hex | " + hex.x + " | " + hex.z);  
			foreach(HexData h in hexManagerS.getAdjacentHexes(hex, sight_range)){
				if(h.x == hex.x && h.z == hex.z)
				{
					//opponent is in sight range of enemy
					Debug.Log ("canSeeHex: opponent is in sight range");
					return true;
				}
				//Debug.Log ("...checking hex...");
			}  
		
		//opponent isn't in sight range of enemy
		Debug.Log ("canSeeHex: opponent is not in sight range");
		return false;
	}

	
	//Return path cost where path_cost = path cost * weight [where weight = path cost^2 * entity_weight]
	public double pathCost(double weight, double path_cost){
		return path_cost * Math.Pow(path_cost,2) * weight;
	}
	
	//return minimun cost path: i.o MIN(mech cost, base cost);
	public List<HexData> minCostPath(double weight1, double weight2, double path_cost1, double path_cost2, ref List<HexData> path1, ref List<HexData> path2){
		double cost1 = pathCost(weight1, path_cost1); 
		double cost2 = pathCost(weight2, path_cost2); 
		Debug.Log("minCostPath: comparing costs-" + cost1 +" vs " + cost2);
		if(cost1 < cost2){
			Debug.Log ("minCostPath: mech path returned, cost =" + cost1);
			return path1;
		}else if(cost2 < cost1){
			Debug.Log ("minCostPath: base path returned, cost =" + cost2);
			return path2;
		}else{
			Debug.Log ("minCostPath: paths have same cost, so return path with higher weight-" + weight1 +" vs " + weight2);
			if(weight1 > weight2){
				Debug.Log ("minCostPath: mech path had higher weight = " +weight1);
				return path1;
			}else if(weight2 > weight1){
				Debug.Log ("minCostPath: mech path had higher weight = " + weight2);
				return path2;
			}else{
				Debug.Log ("minCostPath: path are equal in weight and cost so just go to base");
				return path2;
			}
		}
	}
	
	public override int attackTarget (Combatable target)
	{
		//TODO: finish method, account for opponent defenses etc
			
		//subtract ap cost from total
		current_ap -= attack_cost;
		return target.acceptDamage(attack_damage);
	}
	
	
	public void moveInWorld(int _destination_x, int _destination_z, float _time_to_complete)
	{
		lerp_move = true;
		starting_pos =  entityManagerS.CoordsGameTo3DEntiy(x, z);
		ending_pos = entityManagerS.CoordsGameTo3DEntiy(_destination_x, _destination_z);
		time_to_complete = _time_to_complete;
		moveTime = 0.0f;
		dist = Vector3.Distance(transform.position, ending_pos) * 2;
	}
	
	float dist; 
	Vector3 starting_pos, ending_pos;
	public bool lerp_move = false; 
	float time_to_complete = 2F;
	float moveTime = 0.0f;
 
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
