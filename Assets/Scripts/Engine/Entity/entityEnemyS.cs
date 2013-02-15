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
	
	public bool knows_mech_location;
	public bool knows_base_location;
	
	public bool is_this_enemies_turn;
	
	//test var
	public int enemy_visibility_range; //visible distance (in hexes) where opponent can be seen by enemy	
	
	// Use this for initialization
	void Start () {
		path_to_base = new List<HexData>();
		path_to_mech = new List<HexData>();
		path_to_opponent = new List<HexData>();
		can_get_to_mech_location = false;
	    can_get_to_base_location = false;	
		
		//test var
		enemy_visibility_range = 3; 
	}
	
	// Update is called once per frame
	void Update () {
		//TODO: UPDATE() not tested, contains some sudo code
		
		if(gameManagerS.current_turn == Turn.Enemy  && !lerp_move && is_this_enemies_turn)
		{
			print ("enemy turn now");
			
			//get base and mech positions
			entityBaseS base_s = entityManagerS.getBase();
			entityBaseS mech_s = entityManagerS.getBase();
			
			//check ap
			if(current_ap <= 0)
			{
				is_this_enemies_turn = false;
				gameManagerS.enemy_currently_acting = false;
			}
			
			//scan through list of enemies
			foreach(entityEnemyS enemy in entityManagerS.getEnemies())
			{
				
				print ("working with an enemy");
				
				
				//TODO: Make a method: bool canGetToOpponent(HexData enemy, HexData opponent)
				//TODO:Determine whether you can get to opponent
				//if enemy already "knows opponents location" then don't worry about visibility
					//just get path to opponent: path_to_opponent = getTraversablePath (hexManagerS.getHex(enemy.x,enemy.z), opponent);	
					//if no path found 
						//then "can_get_to_opponents_location" = false
					//else 
						//can_"get_to_opponents_location" = true;
				//else enemy doesn't "know opponents location" then check to see if opponent is visibile
					//if opponent is visible
						//find path to opponent: path_to_opponent = getTraversablePath (hexManagerS.getHex(enemy.x,enemy.z), opponent);
						//if no path found 
							//then "can_get_to_opponents_location" = false
						//else 
							//can_"get_to_opponents_location" = true;
					//else opponent isn't visible
						//can_"get_to_opponents_location" = false
				
				
				//Determine whether you can get to mech and base
				//can_get_to_mech_location = canGetToOpponent(hexManagerS.getHex(enemy.x,enemy.z),hexManagerS.getHex(mech_s.x,mech_s.z));
				//can_get_to_base_location = canGetToOpponent(hexManagerS.getHex(enemy.x,enemy.z),hexManagerS.getHex(base_s.x,base_s.z));
				
				
				//TODO: Decide path to take
				//if enemy knows mech location and knows base location check to see which path is shorter
					//if "can_get_to_mechs_location" & "can_get_to_base_location"
						//find mech cost = mech path cost * weight [where weight = mech path cost^2 * mech_weight]
						//find base cost = base path cost * weight [where weight = base path cost^2 * base_weight]
						//path_to_opponent = get path with lowest cost i.o MIN(mech cost, base cost);
					//else if "can get to base location"
						//path_to_opponent = path to base
					//else if "can get to mech location"
						//path_to_opponent = path to mech
					//else we can't get find a path
						//path_to_opponent = new List<HexData>();
				//else if enemy knows base location check to see if enemy can find a shorter path
					//if "can_get_to_mechs_location" & "can_get_to_base_location"
						//find mech cost = mech path cost * weight [where weight = mech path cost^2 * mech_weight]
						//find base cost = base path cost * weight [where weight = base path cost^2 * base_weight]
						//path_to_opponent = get path with lowest cost i.o MIN(mech cost, base cost);
					//else if "can get to base location"
						//check to see if we can find shorter path to base assuming we can't see mech, use pathFind where mech ignored in canTraverse
						//if(path to base.Count == 0)
							//path_to_opponent = new List<HexData>();
						//else
							//path_to_opponent = path to base;
					//else if "can get to mech location"
						//path_to_opponent = path to mech
					//else we can't get find a path
						//check to see if we can find shorter path to base assuming we can't see mech, use pathFind where mech ignored in canTraverse
						//if(path to base.Count == 0)
							//path_to_opponent = new List<HexData>();
						//else
							//path_to_opponent = path to base;
				//else if enemy knows mech location check to see if enemy can find a shorter path
					//if "can_get_to_mechs_location" & "can_get_to_base_location"
						//find mech cost = mech path cost * weight [where weight = mech path cost^2 * mech_weight]
						//find base cost = base path cost * weight [where weight = base path cost^2 * base_weight]
						//path_to_opponent = get path with lowest cost i.o MIN(mech cost, base cost);
					//else if "can get to mech location"
						//check to see if we can find shorter path to mech assuming we can't see base, use pathFind where base ignored in canTraverse
						//if(path to mech.Count == 0)
							//path_to_opponent = new List<HexData>();
						//else
							//path_to_opponent = path to mech;
					//else if "can get to base location"
						//path_to_opponent = path to base
					//else we can't find a path
						//check to see if we can find shorter path to mech assuming we can't see base, use pathFind where base ignored in canTraverse
						//if(path to mech.Count == 0)
							//path_to_opponent = new List<HexData>();
						//else
							//path_to_opponent = path to mech;
				//else, enemy doesn't know where anyone is
					//if "can_get_to_mechs_location" & "can_get_to_base_location"
						//find mech cost = mech path cost * weight [where weight = mech path cost^2 * mech_weight]
						//find base cost = base path cost * weight [where weight = base path cost^2 * base_weight]
						//path_to_opponent = get path with lowest cost i.o MIN(mech cost, base cost);
					//else if "can get to mech location"
						//path_to_opponent = path to mech;
					//else if "can get to base location"
						//path_to_opponent = path to base
					//else we can't find a path
						//path_to_opponent = new List<HexData>();
					
				
				
				//TODO: Finalize path move
				if(path_to_opponent.Count == 0){
			 		//TODO: no path found
					//have enemy move randomly 
					print ("no path found,  move randomly");
				}else if(path_to_opponent.Count == 1 && path_to_opponent[0].x == enemy.x && path_to_opponent[0].z == enemy.z){
					//TODO: enemy found path, so attack
					print ("enemy found opponent, so att");
				}else{ 
					//move enemy to next position
					print ("move mech to next position");
					makeMove(path_to_opponent[0]);
				}			
				
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
	
		}
	}
 
	//Will use this method for single movements, alternative method used for A*, see IPathFind 
	public List<HexData> getAdjacentTraversableHexes () 
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x, z);
		Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++)
			if(canTraverse(adjacent_hexes[i]))
				result_hexes.Add(adjacent_hexes[i]);
		
		Debug.Log(result_hexes.Count + " found adjacent goods");
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
//		
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
	public List<HexData> getTraversablePath (HexData start, HexData destination)
	{
		//Send hex of base and hex of enemy to aStar 
		var path = aStar.FindPath(start, destination, calcDistance, calcEstimate, getAdjacentTraversableHexes);
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
			
			list.Reverse();	//path comes in backwards
			
			//if list size is 2, don't remove the enemies position from list
			if(list.Count > 2){
				list.RemoveAt(0); //first element is the current enemies position
			}
			
			list.RemoveAt(list.Count - 1); //last element is the destination hex
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
	
	public List<HexData> getAdjacentTraversableHexes (HexData hex, HexData destination)
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(canTraverse(adjacent_hexes[i]) || (adjacent_hexes[i].x == destination.x && adjacent_hexes[i].z == destination.z)){
				//add hex to traversable array
				result_hexes.Add(adjacent_hexes[i]); 
			}
		}
		Debug.Log(result_hexes.Count + " are adjacent goods");
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
				return 99999999;
			
			case Hex.Water:
				return 99999999;
				
			case Hex.Perimeter: 
			default:
				return 999999;
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
