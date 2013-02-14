using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class entityEnemyS : Combatable, IMove, IPathFind {
	private List<HexData> traversable_hexes; //Hold traversable hexes
	private List<HexData> untraversable_hexes; //Hold untraversable hexes
	private List<HexData> path_to_base; //Hold traversable hex path
	
	public bool knows_mech_location;
	public bool knows_base_location;
	
	// Use this for initialization
	void Start () {
		path_to_base = new List<HexData>();
		x = 5;
		z = 1;
	}
	
	// Update is called once per frame
	void Update () {
		//TODO: UPDATE() not tested, contains some sudo code
		
		if(gameManagerS.current_turn == Turn.Enemy)
		{
			//get base position
			entityBaseS base_s = entityManagerS.getBase();
			
			//scan through list of enemies
			foreach(entityEnemyS enemy in entityManagerS.getEnemies())
			{
				//find path from enemy to base
				path_to_base = getTraversablePath (hexManagerS.getHex(enemy.x,enemy.z), hexManagerS.getHex(base.x,base.z));
			
				if(path_to_base.Count == 0){
			 		//TODO: no path found to base, have player move randomly 
				}else if(path_to_base.Count == 1 && path_to_base[0].x == enemy.x && path_to_base[0].z == enemy.z){
					//TODO: player is at base, so attack
				}else{ 
					//move mech to next position
					makeMove(path_to_base[0]);
				}
			}
				
		}
	
	}
 
	//will use this method for single movements, alternative method used for A*, see IPathFind 
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
		
		if(!entityManagerS.canTraverseHex(hex.x, hex.z)){
			//print ("enemy hex");
			return false;
		}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
			//print ("enviro hex");
			return false;
		}else{
			return true;
		}
	} 
 	
	public bool makeMove (HexData hex)
	{
		if(!canTraverse(hex))
			throw new MissingComponentException("Can't move to this spot, invalid location!");
		
		//TODO add visual engine.move hooks
		
		//update enemy hex tags
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
	private List<HexData> extractPath (IEnumerable<HexData> path){
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
		//TODO: will need to be adjusted later
        return Math.Abs(hex_start.x - hex_end.x) + Math.Abs(hex_start.z - hex_end.z);
	}

	public double calcEstimate (HexData hex_start, HexData hex_end)
	{
		//TODO: will need to be adjusted later
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
	
 //TODO
	public int getTraverseAPCost(Hex hex_type){
		return 2;
	}
	
	public override int attackTarget (Combatable target)
	{
		throw new System.NotImplementedException ();
	}
 
}
