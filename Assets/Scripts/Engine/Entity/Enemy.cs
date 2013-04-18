using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : Combatable, IMove, IPathFind {

	public List<HexData> traversable_hexes; //Hold traversable hexes
	public List<HexData> untraversable_hexes; //Hold untraversable hexes
	public List<HexData> path_to_base; //Hold traversable hex path to base
	public List<HexData> path_to_mech; //Hold traversable hex path to mech
	public List<HexData> path_to_opponent; //Hold traversable hex path to chosen opponent
	
	public int spawner_owner;
	public bool   can_get_to_mech_location;
	public bool   can_get_to_base_location;
	public double last_path_cost; //path cost of most recent path gotten from traversable path call
	public bool   chosen_path_is_mech = false;
	public bool   chosen_path_is_base = false;
	public HexData last_move; //Can't move backwards unless can't move anywhere else; 
	
	//Put a weight on entities to decide which is more important when having to make a path choice
	public double base_weight = 10;
	public double mech_weight = 5;
	
	public bool knows_mech_location;
	public bool knows_base_location;
	public EntityE enemy_type;
	
	public bool is_this_enemies_turn;
	
	public Vector3 center_top;
	
	//Extract hexes from path and put hexes into a List 
	public List<HexData> extractPath (Path path)
	{
		
		if(path != null){
			last_path_cost = path.TotalCost;
			//Debug.Log ("getTraversablePath: path cost = " + last_path_cost);
		}
		
		List<HexData> list = new List<HexData>();
		if(path != null){
			foreach (HexData hex in path){
				list.Add(hex);
			}
			//Debug.Log ("extractPath: size of path = " + list.Count);
			
			list.Reverse();	//path comes in backwards
			
			//if list size is 2, don't remove the enemies position from list
			if(list.Count > 2){
				list.RemoveAt(0); //first element is the current enemies position
			}
			
			list.RemoveAt(list.Count - 1); //last element is the destination hex
		}else{
			//Debug.Log ("extractPath: path is empty");
		}
		
		return list;
	}
	

	//return attackable target in range of enemy
	public Combatable targetInAttackRange(Combatable base_s, Combatable mech_s){
		Combatable final_target = null;
		//If attack enemy based on weight choice
		//bool found_base = false;
		//bool found_mech = false;
		
		//get all hexes in attack range
		foreach(HexData h in hexManagerS.getAdjacentHexes(x,z,attack_range)){
			//check to see if base or mech is at one the hexes
			if(h.x == base_s.x && h.z == base_s.z){
				final_target = base_s;
				break;
			}
			
			if(h.x == mech_s.x && h.z == mech_s.z){
				final_target = mech_s;
				break;
			}
			//if(found_base & found_mech)
			//see which enemy has a higher weight
			//attack enemy with higher wieght
			//otherwise choose base
			//break;
		}
		
		return final_target;
		
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
		////Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++)
			if(canTraverse(adjacent_hexes[i]))
				result_hexes.Add(adjacent_hexes[i]);
		
		////Debug.Log(result_hexes.Count + " found adjacent goods");
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
		return canTraverseHex (hex_x, hex_z);
	} 
	
	public abstract bool canTraverseHex (int hex_x, int hex_z);
	
	//alternative can traverse methods where an entity can be excluded from search
	public bool canTraverse (HexData hex, EntityE entity)
	{
		return canTraverse(hex.x, hex.z, entity);
	}  
	
	public bool canTraverse (int hex_x, int hex_z, EntityE entity){
		return canTraverseHex (hex_x, hex_z, entity);
	}
 
	public abstract bool canTraverseHex (int hex_x, int hex_z, EntityE entity);
		
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
	
	//Get path to base
	public List<HexData> getTraversablePath (HexData start, HexData destination, EntityE entity)
	{
		//Send hex of base and hex of enemy to aStar
		var path = aStar.FindPath(start, destination, entity, calcCostToTravelAdjacentHex, calcCostToTravelToDistantHex, getAdjacentTraversableHexes);
		if(path != null){
			last_path_cost = path.TotalCost;
			//Debug.Log ("getTraversablePath: path cost = " + last_path_cost);
		}
		return extractPath(path);
	}
	
	//Extract hexes from path and put hexes into a List 
	public List<HexData> extractPath (IEnumerable<HexData> path)
	{
		List<HexData> list = new List<HexData>();
		if(path != null){
			foreach (HexData hex in path){
				list.Add(hex);
			}
			//Debug.Log ("extractPath: size of path = " + list.Count);
			
			list.Reverse();	//path comes in backwards
			
			//if list size is 2, don't remove the enemies position from list
			if(list.Count > 2){
				list.RemoveAt(0); //first element is the current enemies position
			}
			
			list.RemoveAt(list.Count - 1); //last element is the destination hex
		}else{
			//Debug.Log ("extractPath: path is empty");
		}
		
		return list;
	}
	
	 
	public double calcCostToTravelAdjacentHex (HexData hex_start, HexData hex_end)
	{	
		return (double) getTraverseAPCost(hex_end.hex_type);
	}

	public double calcCostToTravelToDistantHex (HexData hex_start, HexData hex_end)
	{
		//TODO: may need to be adjusted later
        return Math.Sqrt(Math.Pow(Math.Abs(hex_start.x - hex_end.x),2) + Math.Pow(Math.Abs(hex_start.z - hex_end.z),2))*2;
	} 
	
	public List<HexData> getAdjacentTraversableHexes (HexData hex, HexData destination, EntityE entity)
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		////Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(canTraverse(adjacent_hexes[i], entity) || (adjacent_hexes[i].x == destination.x && adjacent_hexes[i].z == destination.z)){
				//add hex to traversable array
				result_hexes.Add(adjacent_hexes[i]); 
			}
		}
		
		////Debug.Log ("Number of result_hexes " + result_hexes.Count);
		return result_hexes;
	}
	
 
	public abstract int getTraverseAPCost(Hex hex_type);
	
	//True if enemy can can get to specified opponent
	public bool canGetToOpponent(HexData enemy, HexData opponent, EntityE entity_opponent, bool knows_opponents_location, ref List<HexData> path_to_opp){

		if(knows_opponents_location){
			//if enemy already "knows opponents location" then don't worry about visibility just get path to opponent
			//Debug.Log ("canGetToOpponent: knows enemy location, get path:" + entity_opponent);
			path_to_opp = extractPath(hexManagerS.getTraversablePath(enemy, opponent, EntityE.Node, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
			
			if(path_to_opp.Count == 0){
				//Debug.Log ("canGetToOpponent: knows enemy location, but can't find path");
				return false;
			}else{
				//Debug.Log ("canGetToOpponent: knows enemy location, and found path");
				return true;
			}
		}
		else
		{
			//Debug.Log ("canGetToOpponent: doesn't know enemy location, see if enemy is visible in sight range: " + entity_opponent);
			//TODO: TESTING****using old method*********
			if(canSeeHex(opponent)){ 
			//if(canSeeHex(opponent)){ ****
				//Debug.Log ("canGetToOpponent: doesn't know enemy location, get path to " + entity_opponent);
				path_to_opp = extractPath(hexManagerS.getTraversablePath(enemy, opponent, EntityE.Node, getTraverseAPCostPathVersion, getAdjacentTraversableHexes));
				if(path_to_opp.Count == 0){
					//Debug.Log ("canGetToOpponent: enemy is visible, but can't find path");
					return false;
				}else{
					//Debug.Log ("canGetToOpponent: enemy is visible, and found path");
					return true;
				}
			}else{
				//Debug.Log ("canGetToOpponent: doesn't know enemy location, and enemy not visible");
				return false;
			}
		}
	}
	
	
	//Return true if entity is visible(enemy_visibility_range) to enemy based on enemies current x and z
	public bool canSeeHex(HexData hex)
	{		

		//Debug.Log(hex + " = hex | " + hex.x + " | " + hex.z);  
			foreach(HexData h in hexManagerS.getAdjacentHexes(x,z, sight_range)){
				if(h.x == hex.x && h.z == hex.z)
				{
					//opponent is in sight range of enemy
					//Debug.Log ("canSeeHex: opponent in sight, sight_range = " + sight_range);
					return true;
				}
				////Debug.Log ("...checking hex...");
			}  
		
		//opponent isn't in sight range of enemy
		//Debug.Log ("canSeeHex: opponent is not in sight range");
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
		//Debug.Log("minCostPath: comparing costs-" + cost1 +" vs " + cost2);
		if(cost1 < cost2){
			//Debug.Log ("minCostPath: mech path returned, cost =" + cost1);
			chosen_path_is_mech = true;
			return path1;
		}else if(cost2 < cost1){
			//Debug.Log ("minCostPath: base path returned, cost =" + cost2);
			chosen_path_is_base = true;
			return path2;
		}else{
			//Debug.Log ("minCostPath: paths have same cost, so return path with higher weight-" + weight1 +" vs " + weight2);
			if(weight1 > weight2){
				//Debug.Log ("minCostPath: mech path had higher weight = " +weight1);
				chosen_path_is_mech = true;
				return path1;
			}else if(weight2 > weight1){
				//Debug.Log ("minCostPath: base path had higher weight = " + weight2);
				chosen_path_is_base = true;
				return path2;
			}else{
				//Debug.Log ("minCostPath: path are equal in weight and cost so just go to base");
				chosen_path_is_base = true;
				return path2;
			}
		}
	}
	
	public override int attackTarget (Combatable target)
	{
		//subtract ap cost from total
		//Debug.LogWarning("ABOUT TO ATTACK ENTITY ON - " + target.x + "," + target.z);
		Facing new_facing = getDirectionFacing(target.x, target.z);
		if(new_facing != facing_direction)
			facing_direction = new_facing;
		
		print ("FACING" + facing_direction);
		
		current_ap -= attack_cost;
		entityManagerS.sm.playGunNormal();
		
		//Debug.LogWarning("ABOUT TO ATTACK ENTITY " + target.GetInstanceID());
		if(target != null)
			 target.acceptDamage(attack_damage);
		 gameManagerS.waiting_after_shot = true;
		gameManagerS.time_after_shot_start = Time.time;
		//Debug.Log ("ERROR: didn't pick a combatable target");
		
//		StartCoroutine(DelayStuff(.8f));
		return 0; //nothing to damage if we get here
	}	
	
	public Facing getDirectionFacing(int target_x, int target_z){
				
		return hexManagerS.getFacingDirection(x,z,target_x,target_z,attack_range);
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
	
	public float dist; 
	public Vector3 starting_pos, ending_pos;
	public bool lerp_move = false; 
	public float time_to_complete = 2F;
	public float moveTime = 0.0f;



}
