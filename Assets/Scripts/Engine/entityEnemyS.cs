using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class entityEnemyS : Combatable, IMove, IPathFind {
	private List<HexData> traversable_hexes; //Hold traversable hexes
	private List<HexData> untraversable_hexes; //Hold untraversable hexes
	private List<HexData> path_to_base; //Hold traversable hex path 
	
	//Test vars ***
	bool enemy_turn = true;
	int test = 0;
	private GameObject 	hex_choice;
	public Transform enemy_location; //hold enemy location
	
	// Use this for initialization
	void Start () {
		traversable_hexes = new List<HexData>();
		untraversable_hexes = new List<HexData>();
		path_to_base = new List<HexData>();
		x = 5;
		z = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
				
		//ADDED: ROUGH OUTLINE for Update method below ***
		//		Uncomment the block comment to test
		
		if(enemy_turn){
			//getTraversible hexes
			//traversable_hexes = getAdjacentTraversableHexes(hexManagerS.getHex(x,z));
			
			if(test == 0){
				path_to_base = getTraversablePath (hexManagerS.getHex(x,z), hexManagerS.getHex(1,5));
				print ("Size" + path_to_base.Count); //if size is 0 then no path found,if size is 1 then enemy is at base
				test = 1;
			}
			
			//TODO: some mechanism for highlighting valid hexes?
			
			//Check to see if player clicked a hex (left click)
			if(Input.GetMouseButtonDown(0)){
				//TODO: check to see if player clicked on a traversible hex from the traversable_hexes array
				//		if traversible hex clicked then call makeMove, otherwise do nothing (or print why invalid hex)
					
				if(test < path_to_base.Count + 1){
					//TEST: print pos
					print ("Pos: " + path_to_base[test - 1].x + " " + path_to_base[test - 1].z + " " +path_to_base[test - 1].hex_type);
					makeMove(path_to_base[test - 1]);
					test++;
				}				
			}
		}
	}

	
	#region IMove implementation
	public List<HexData> getAdjacentTraversableHexes (HexData hex)
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		//print ("length hexes " + adjacent_hexes.Length);
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			//print("Adj list hex type:" + adjacent_hexes[i].hex_type);
			if(canTraverse(adjacent_hexes[i])){
				//add hex to traversable array
				result_hexes.Add(adjacent_hexes[i]); 
			}
		}
		//print ("length result " + result_hexes.Count);
		
		return result_hexes;
	}

	public List<HexData> getAdjacentUntraversableHexes (HexData hex)
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		
		//See which of the adjacent hexes are untraversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(!canTraverse(adjacent_hexes[i])){
				//add hex to untraversable array
				result_hexes.Add(adjacent_hexes[i]); 
			}
		}
		
		return result_hexes;
	}

	public bool canTraverse (HexData hex)
	{
		//TODO: 1. Expand on hex options once fully known
		//		2. Will need to add the upgrade options into the traversable descision
			if(entityManagerS.canTraverseHex(hex) == false){
				//print ("enemy hex");
				return false;
			}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
				//print ("enviro hex");
				return false;
			}else{
				return true;
			}
	}
	
	public void makeMove (HexData hex)
	{
		//move player to new hex position
		Vector3 temp = new Vector3(hex.x,enemy_location.position.y,hex.z);
		enemy_location.position = temp;
		
		//move enemy to next hex, update hex pos
		x = hex.x;
		z = hex.z;
		//TODO: Some code dealing with the level editor will probably go here ***
	
	}
	#endregion
	
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
			foreach (HexData hex in path)
	        {
				list.Add(hex);
			}
			
			list.Reverse();	//path comes in backwards
			
			if(list.Count > 2){
				list.RemoveAt(0);//first element is the current enemies position
			}
			list.RemoveAt(list.Count - 1); //last element is the destination hex
		}
		return list;
	}

	#region implemented abstract members of Combatable
	public override int attackTarget (Combatable target)
	{
		throw new System.NotImplementedException ();
	}

	public override int attackHex (int x, int z)
	{
		throw new System.NotImplementedException ();
	}
	#endregion
	
	
	#region IPathFind implementation
	public double calcDistance (HexData hex_start, HexData hex_end)
	{
		//TODO: this is dummy code, will be adjusted later

        return Math.Abs(hex_start.x - hex_end.x) + Math.Abs(hex_start.z - hex_end.z);
	}

	public double calcEstimate (HexData hex_start, HexData hex_end)
	{
		//TODO: this is dummy code, will be adjusted later
        return Math.Abs(hex_start.x - hex_end.x) + Math.Abs(hex_start.z - hex_end.z);
	}
	
	public List<HexData> getAdjacentTraversableHexes (HexData hex, HexData destination)
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		print ("length hexes " + adjacent_hexes.Length);
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			print("Adj list hex type:" + adjacent_hexes[i].hex_type);
			if(canTraverse(adjacent_hexes[i]) || (adjacent_hexes[i].x == destination.x && adjacent_hexes[i].z == destination.z)){
				//add hex to traversable array
				result_hexes.Add(adjacent_hexes[i]); 
			}
		}
		print ("length result " + result_hexes.Count);
		//print ("length " + result_hexes[0].hex_type + " " + result_hexes[1].hex_type + + " " + result_hexes[2].hex_type + " " + result_hexes[3].hex_type + " " + result_hexes[4].hex_type + " " + result_hexes[5].hex_type + " " + result_hexes[6].hex_type);
		return result_hexes;
	}
	#endregion
}

