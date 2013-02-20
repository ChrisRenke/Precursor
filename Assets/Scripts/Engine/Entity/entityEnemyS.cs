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
	
	private bool can_get_to_mech_location;
	private bool can_get_to_base_location;
	private double last_path_cost; //path cost of most recent path gotten from traversable path call
	
	//Put a weight on entities to decide which is more important when having to make a path choice
	public double base_weight;
	public double mech_weight;
	
	public bool knows_mech_location;
	public bool knows_base_location;
	
	public bool is_this_enemies_turn;
	
	//test var
	public int enemy_sight_range; //visible distance (in hexes) where opponent can be seen by enemy, if 3 then checks 3 hexes out in 6 directions 	
	
	int t = 0;
	
	// Use this for initialization
	void Start () {
		path_to_base = new List<HexData>();
		path_to_mech = new List<HexData>();
		path_to_opponent = new List<HexData>();
		last_path_cost = 0;		 
	}
	
	// Update is called once per frame
	void Update () {
		//TODO: UPDATE() not tested, contains some sudo code
		
		//TESTING VARIABLES, must be set for algorithm to work*****
		knows_mech_location = true;
		knows_base_location = false;
		base_weight = 10;
		mech_weight = 5;
		enemy_sight_range = 3; //Don't go higher than 3 for my method, very slow, will rework later, but it does work
		
		if(gameManagerS.current_turn == Turn.Enemy  && !lerp_move && is_this_enemies_turn)
		{
			//if( t== 0 || t == 1 || t == 2){
			print ("enemy turn now");
			
			//get base and mech positions
			entityBaseS base_s = entityManagerS.getBase();
			entityMechS mech_s = entityManagerS.getMech();
			
			//check ap
			if(current_ap <= 0)
			{
				is_this_enemies_turn = false;
				gameManagerS.enemy_currently_acting = false;
				//t =0;	//test
			}else{
				
				//******AP COST SEEMS TO BE MESSING WITH THE PATH, SO HAVE TO HAVE SOME MECHANISIM FOR FOLLING
				//THE SAME PATH WITHOUT RE WORKING DURING TURN, B/C MAY KEEP GOING BACK AND FORTH AND USING
				//UP AP WIHTOUT GETING ANYWHERE***********
				//ALSO MAKE ATTACK METHOD TAKE DOWN AP, THIS MIGHT HELP THE ISSUE ABOVE
			
				//MAIN IDEA
				//if can see mech and base and enemy is able to get to both mech and base
				//then can get to mech and base is true.
				//if cant see mech and base and enemy then check to see if in visible range
				//then can get to mech and base is true if they are in visible range and a path can be found.
				//if they aren't in visible range or a path can't be found then can get to mech and base will be false
				
				print ("WORKING WITH AN ENEMY");
				print ("AP LEFT:" + current_ap);
				

				print ("Knows mech location is " + knows_mech_location + ", Knows base location is " + knows_base_location);
				print ("mech location is " + mech_s.x + ":" + mech_s.z + ", base location is " + base_s.x + ":" + base_s.z);
				//Determine whether you can get to mech and base and store there path costs
				can_get_to_mech_location = canGetToOpponent(hexManagerS.getHex(x,z),hexManagerS.getHex(mech_s.x,mech_s.z), EntityE.Player, knows_mech_location, path_to_mech);
				double mech_path_cost = last_path_cost;
				
				if(!can_get_to_mech_location)
					mech_path_cost = 0;
				print ("Can get to mech location is " + can_get_to_mech_location + ", Mech Path Cost: " + mech_path_cost);
				print ("-------------------------------------------------");
				
				can_get_to_base_location = canGetToOpponent(hexManagerS.getHex(x,z),hexManagerS.getHex(base_s.x,base_s.z), EntityE.Base, knows_base_location, path_to_base);
				double base_path_cost = last_path_cost;
				if(!can_get_to_base_location)
					base_path_cost = 0;
				print ("Can get to base location is " + can_get_to_base_location + ", Base Path Cost: " + base_path_cost);
				print ("-------------------------------------------------");
				
				
				//Decide path to take
				print ("Decide PATH to take");
				if(knows_mech_location && knows_base_location){
					//enemy knows mech location and knows base location, check which path is shorter
					if(can_get_to_mech_location && can_get_to_base_location){

						//find path with lower cost
						print ("Update 1:2: knows mech/base location, check which path is shorter");
						path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, path_to_mech, path_to_base);
					}
					else if(can_get_to_base_location){
						print ("Update 1:3: knows mech/base location, can't get to mech so go to base");
						path_to_opponent = path_to_base;
					}
					else if(can_get_to_mech_location){
						print ("Update 1:4: knows mech/base location, can't get to base so go to mech");
						path_to_opponent = path_to_mech;
					}
					else{
						//no new path found
						print("Update 1:3: knows mech/base location, can't find path to mech or base");
						path_to_opponent = new List<HexData>();
					}

				}else if(knows_base_location){

					if(can_get_to_mech_location && can_get_to_base_location){
						//enemy knows base location, find least cost path 
						print ("Update 2:1: knows base location, check which path is shorter");
						path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, path_to_mech, path_to_base);
					}
					else if(can_get_to_base_location){	
						print ("Update 2:2: knows base location, can't get to mech, check for shorter path to base assuming we can't see mech");
						//check to see if we can find shorter path to base assuming we can't see mech
						path_to_base = getTraversablePath (hexManagerS.getHex(x,z), hexManagerS.getHex(base_s.x,base_s.z), EntityE.Player);
						base_path_cost = last_path_cost;
						if(path_to_base.Count == 0){
							//no new path found
							print ("Update 2:3: knows base location, shouldn't reach this code, since path to base already found");
							path_to_opponent = new List<HexData>();
						}
						else{
							print ("Update 2:4: knows base location, going on path to base");
							path_to_opponent = path_to_base;
						}
					}else if(can_get_to_mech_location){
						print ("Update 2:5: knows base location, can't get to base so go to mech");
						path_to_opponent = path_to_mech;
					}else{
						//no new path found, check to see if we can find shorter path to base assuming we can't see mech
						print ("Update 2:6: knows base location, can't find a path to either but try to find path to base assuming mech may be blocking path");
						path_to_base = getTraversablePath (hexManagerS.getHex(x,z), hexManagerS.getHex(base_s.x,base_s.z), EntityE.Player);
						base_path_cost = last_path_cost;
						if(path_to_base.Count == 0){
							//no new path found
							print ("Update 2:7: knows base location, path not found to base or mech");
							path_to_opponent = new List<HexData>();
						}else{
							print ("Update 2:8: knows base location, path to base found if enemy not considered");
							path_to_opponent = path_to_base;
						}
					}
				}else if(knows_mech_location){
					//enemy knows mech location, check to see if enemy can find least cost path
					if(can_get_to_mech_location && can_get_to_base_location){
						//find path with lower cost
						print ("Update 3:1: knows mech location, check which path is shorter");
						path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, path_to_mech, path_to_base);
					}else if(can_get_to_mech_location){
						//check to see if we can find shorter path to mech assuming we can't see base
						print ("Update 3:2: knows mech location, can't see base so try to find shorter path to mech");
						path_to_mech = getTraversablePath (hexManagerS.getHex(x,z), hexManagerS.getHex(mech_s.x,mech_s.z), EntityE.Base);
						mech_path_cost = last_path_cost;
						if(path_to_mech.Count == 0){
							//no path found
							print ("Update 3:3: knows mech location, shouldn't reach this code since path to mech known");
							path_to_opponent = new List<HexData>();
						}else{
							print ("Update 3:4: knows mech location, path to mech found");
							path_to_opponent = path_to_mech;
						}
					}else if(can_get_to_base_location){
						print ("Update 3:5: knows mech location, can't get to mech but found path to base");
						path_to_opponent = path_to_base;	
					}else{
						//haven't found new path, check to see if we can find shorter path to mech assuming we can't see base
						print ("Update 3:6: knows mech location, try to find path to mech assuming path is blocked by base");
						path_to_mech = getTraversablePath (hexManagerS.getHex(x,z), hexManagerS.getHex(mech_s.x,mech_s.z), EntityE.Base);
						mech_path_cost = last_path_cost;
						if(path_to_mech.Count == 0){
							print ("Update 3:7: knows mech location, can't find a path to base or mech");
							path_to_opponent = new List<HexData>();
						}else{
							print ("Update 3:8: knows mech location, going on path to mech");
							path_to_opponent = path_to_mech;
						}
					}
				}else{
					//enemy doesn't know where anyone is, so see if anyone is around and find least cost path
					if(can_get_to_mech_location && can_get_to_base_location){
						//find path with lower cost
						print ("Update 4:1: don't know base or mech location, check to see which path is shorter");
						path_to_opponent = minCostPath(mech_weight, base_weight, mech_path_cost, base_path_cost, path_to_mech, path_to_base);
					}else if (can_get_to_mech_location){
						print ("Update 4:2: don't know base or mech location, can't get to base so go to mech");
						path_to_opponent = path_to_mech;
					}else if (can_get_to_base_location){
						print ("Update 4:3 don't know base or mech location, can't get to mech so go to base");
						path_to_opponent = path_to_base;
					}else{ 
						//can't find a path
						print ("Update 4:1: don't know base or mech location, can't find path to either base or mech");
						path_to_opponent = new List<HexData>();
					}
				}
				
				
				//Finalize path move
				if(path_to_opponent.Count == 0){
			 		//no path found so have enemy move randomly
			 		print ("Update 5:1: no path found so move randomly");
					path_to_opponent = getAdjacentTraversableHexes();
					
					if(path_to_opponent.Count > 0){
						makeMove(path_to_opponent[UnityEngine.Random.Range(0, path_to_opponent.Count)]);
					}else{
						//can't make a move, do nothing
						print ("Update 5:2: can't move so let next enemy go");
					}
				}else if(path_to_opponent.Count == 1 && path_to_opponent[0].x == x && path_to_opponent[0].z == z){
					//TODO: enemy found path, so attack
					print ("Update 5:3: enemy found opponent, so att");
				}else{ 
					//move enemy to next position
					print ("Move enemy one position on path");
					makeMove(path_to_opponent[0]);
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
			}	
		}
		
		
		
		//TODO: END TURN IF AP OUT
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
 
	
	public List<HexData> getAdjacentUntraversableHexes () 
	{
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
			//print ("enemy hex");
			return false;
		}else 
		if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
			//print ("enviro hex");
			return false;
		}
		else if(getTraverseAPCost(hex.hex_type) > current_ap)
		{
			return false;
		}
		else
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
			//print ("enemy hex");
			return false;
		}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
			//print ("enviro hex");
			return false;
		}
		else if(getTraverseAPCost(hex.hex_type) > current_ap)
		{
			print ("canTraverse: ap over load: " + hex.x + " " + hex.z + " current ap:" + current_ap);
			return false;
		}
		else
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
	
	//Get path to base
	public List<HexData> getTraversablePath (HexData start, HexData destination, EntityE entity)
	{
		//Send hex of base and hex of enemy to aStar
		var path = aStar.FindPath(start, destination, entity, calcDistance, calcEstimate, getAdjacentTraversableHexes);
		if(path != null){
			//get path cost
			last_path_cost = path.TotalCost;
			print ("getTraversablePath: path cost = " + last_path_cost);
		}
		return extractPath(path);
	}
	
	//Extract hexes from path and put hexes into a List 
	private List<HexData> extractPath (IEnumerable<HexData> path)
	{
		List<HexData> list = new List<HexData>();
		if(path != null){
			foreach (HexData hex in path){
				list.Add(hex);
			}
			print ("extractPath: size of path = " + list.Count);
			
			list.Reverse();	//path comes in backwards
			
			//if list size is 2, don't remove the enemies position from list
			if(list.Count > 2){
				list.RemoveAt(0); //first element is the current enemies position
			}
			
			list.RemoveAt(list.Count - 1); //last element is the destination hex
		}else{
			print ("extractPath: path is empty ");
		}
		
		return list;
	}
	
	
	#region IPathFind implementation
	public double calcDistance (HexData hex_start, HexData hex_end)
	{	
		return (double) getTraverseAPCost(hex_end.hex_type);
	}

	public double calcEstimate (HexData hex_start, HexData hex_end)
	{
		//TODO: may need to be adjusted later
        return Math.Abs(hex_start.x - hex_end.x) + Math.Abs(hex_start.z - hex_end.z);
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
		
		//print ("Number of result_hexes " + result_hexes.Count);
		//Debug.Log(result_hexes.Count + " are adjacent goods");
		return result_hexes;
	}
	#endregion
	
 
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
	bool canGetToOpponent(HexData enemy, HexData opponent, EntityE entity_opponent, bool knows_opponents_location, List<HexData> path_to_opp){

		if(knows_opponents_location){
			//if enemy already "knows opponents location" then don't worry about visibility just get path to opponent
			print ("canGetToOpponent: knows enemy location, get path:" + entity_opponent);
			path_to_opp = getTraversablePath(enemy, opponent, EntityE.Node);	
			if(path_to_opp.Count == 0){
				//no path found 
				print ("canGetToOpponent: knows enemy location, but can't find path");
				return false;
			}else{
				print ("canGetToOpponent: knows enemy location, and found path");
				return true;
			}
		}
		else
		{
			//if enemy doesn't "know opponents location" then check to see if opponent is visibile
			print ("canGetToOpponent: doesn't know enemy location, see if enemy in sight range: " + entity_opponent);
			//TODO: TESTING****using old method*********
			if(canSeeHex(opponent)){ 
			//if(canSeeHex(opponent)){ ****
				print ("canGetToOpponent: doesn't know enemy location, get path to " + entity_opponent);
				//find path to opponent
				path_to_opp = getTraversablePath(enemy, opponent, entity_opponent);
				if(path_to_opp.Count == 0){
					print ("canGetToOpponent: enemy is visible, but can't find path");
					//no path found 
					return false;
				}else{
					print ("canGetToOpponent: enemy is visible, and found path");
					return true;
				}
			}else{
				print ("canGetToOpponent: doesn't know enemy location, and enemy not visible");
				//opponent isn't visible
				return false;
			}
		}
	}
	
//VERSION 1 SIGHT RANGE METHODS
	/*
	
	//Return true if entity is visible(enemy_visibility_range) to enemy based on enemies current x and z
	public bool canSeeHex(HexData hex)
	{		
		Debug.Log("canSeeHex: " + hex + " = hex | " + hex.x + " | " + hex.z);  
			foreach(HexData h in getSightRange()){
				print ("canSeeHex: hex: " + h.x + ":" + h.z);
				if(h.x == hex.x && h.z == hex.z)
				{
					//opponent is in sight range of enemy
					print ("canSeeHex: opponent is in sight range");
					return true;
				}
				//Debug.Log ("...checking hex...");
			}  
		
		//opponent isn't in sight range of enemy
		print ("canSeeHex: opponent is not in sight range");
		return false;
	}
	
	

	public List<HexData> getSightRange()
	{
		HexData current_hex;    
		List<HexData> hexes_in_range = new List<HexData>();
		
		//draw center hex, the one clicked on
		current_hex = hexManagerS.getHex(x, z); 
		
		hexes_in_range.Add(current_hex);
		
		//enter loop for surrounding hexes
		for(int ring = 1; ring < enemy_sight_range; ring++)
		{
			 
			//draw the first "northeast" edge hex 
			current_hex = hexManagerS.getHex(x, z, Facing.NorthEast);
			hexes_in_range.Add(current_hex); 
			
			//draw the "northeast" portion
			for(int edge_hexes_drawn = 1; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{ 
				current_hex = hexManagerS.getHex(x, z, Facing.SouthEast);// = AddHexSE(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex)); 
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "southeast" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(x, z, Facing.South);
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "south" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(x, z, Facing.SouthWest);
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "southwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(x, z, Facing.NorthWest);
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "northwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(x, z, Facing.North);
				hexes_in_range.Add(current_hex); 
			}
			
			//draw the "north" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(x, z, Facing.NorthEast);
				hexes_in_range.Add(current_hex); 
			}
		}

		Debug.LogWarning("hexes_in_range size = " + hexes_in_range.Count);
		return hexes_in_range;
	}
		
//
*/
	
//VERSION 2 sight range methods
	
	//Return true if entity is visible(enemy_visibility_range) to enemy based on enemies current x and z
	public bool canSeeHex(HexData hex)
	{		
		Debug.Log("canSeeHex: enemy hex =" + hex + " = hex | " + hex.x + " | " + hex.z);  
			foreach(HexData h in getSightRange()){
				//print ("canSeeHex: hex: " + h.x + ":" + h.z);
				if(h.x == hex.x && h.z == hex.z)
				{
					//opponent is in sight range of enemy
					print ("canSeeHex: opponent is in sight range");
					return true;
				}
				//Debug.Log ("...checking hex...");
			}  
		
		//opponent isn't in sight range of enemy
		print ("canSeeHex: opponent is not in sight range");
		return false;
	}
	
	//find all hexes in sight range of enemy, sight range set by enemy_sight_range variable 
	public List<HexData> getSightRange(){
		List<HexData> hexes_start = getAdjacentTraversableHexes(); //get adj hexes
		if(hexes_start != null){
			int last_added = hexes_start.Count; //the number of hexes add to resulting array
			int left = enemy_sight_range - 1; //number of hexes left to span
			int index = 0; //index we are currently on in result array
			return sightRangeHelper(left, last_added, hexes_start, index);
		}
		return new List<HexData>();
	}
	
	
	public List<HexData> sightRangeHelper (int left, int last_added, List<HexData> result, int index)
	{
		if(left == 0 || last_added == 0 ){
			return result;
		}else{
			int count = 0;
			HexData[] temp;
			for(int i = 0; i < last_added; i++){
				temp = hexManagerS.getAdjacentHexes(result[index].x, result[index].z);
				//add adj hexes to result
				for(int j =0; j < temp.Length; j++){
					if(temp[j].hex_type != Hex.Perimeter){
						result.Add(temp[j]);
						count++;
					}
				}
				index++;
			}
			last_added = count;
			left = left - 1;
			return sightRangeHelper (left, last_added, result, index);
		}
	}
	
	//Return path cost where path_cost = path cost * weight [where weight = path cost^2 * entity_weight]
	public double pathCost(double weight, double path_cost){
		return path_cost * Math.Pow(path_cost,2) * weight;
	}
	
	//return minimun cost path: i.o MIN(mech cost, base cost);
	public List<HexData> minCostPath(double weight1, double weight2, double path_cost1, double path_cost2, List<HexData> path1, List<HexData> path2){
		double cost1 = pathCost(weight1, path_cost1); 
		double cost2 = pathCost(weight2, path_cost2); 
		print("minCostPath: comparing costs-" + cost1 +" vs " + cost2);
		if(cost1 < cost2){
			print ("minCostPath: mech path returned, cost =" + cost1);
			return path1;
		}else if(cost2 < cost1){
			print ("minCostPath: base path returned, cost =" + cost2);
			return path2;
		}else{
			//if paths are equal then return path with higher weight
			print ("minCostPath: paths have same cost, so return path with higher weight-" + weight1 +" vs " + weight2);
			if(weight1 > weight2){
				print ("minCostPath: mech path had higher weight = " +weight1);
				return path1;
			}else if(weight2 > weight1){
				print ("minCostPath: mech path had higher weight = " + weight2);
				return path2;
			}else{
				print ("minCostPath: path are equal in weight and cost so just go to base");
				return path2;
			}
		}
	}
	
	public override int attackTarget (Combatable target)
	{
		//TODO:
		throw new System.NotImplementedException ();
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
	bool lerp_move = false; 
	float time_to_complete = 2F;
	float moveTime = 0.0f;
 
}
