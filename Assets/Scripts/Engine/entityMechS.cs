using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class entityMechS : Combatable, IMove {
	
	private int size = 16; //size of traversable_hexes
	private List<HexData> traversable_hexes; //Hold traversable hexes
	private List<HexData> untraversable_hexes; //Hold untraversable hexes
		
	//Test vars ***
	bool players_turn = true;
	int test = 0;
	private GameObject 	hex_choice;
	public Transform player_location; //hold player location, for testing
	
	
	
	//Use this for initialization
	void Start () {
		traversable_hexes = new List<HexData>();
		untraversable_hexes = new List<HexData>();
		x = 4;
		z = 4;
	}
	
	//Update is called once per frame
	void Update () {
		
		//ADDED: ROUGH OUTLINE for Update method below ***
		//		Uncomment the block comment to test
		
		if(players_turn){
			//getTraversible hexes
			//traversable_hexes = getAdjacentTraversableHexes(hexManagerS.getHex(x,z));
			
			//TODO: some mechanism for highlighting valid hexes?
			
			//Check to see if player clicked a hex (left click)
			if(Input.GetMouseButtonDown(0)){
					//TODO: check to see if player clicked on a traversible hex from the traversable_hexes array
					//		if traversible hex clicked then call makeMove, otherwise do nothing (or print why invalid hex)
					
					//TEST: For testing purposes: the player moves randomly for each mouse click at the moment
					/*if(test == 0){
						makeMove(new HexData(-3, 1, Hex.Grass));
						test = 1;
					}else if (test == 1){
						makeMove(new HexData(-2, 1, Hex.Grass));
						test = 2;
					}else{
						makeMove(new HexData(-1, 1, Hex.Grass));	
						test = 0;
					}*/
				
			}
		}
		
		
		//TESTS ***
		/*//TESTS TRAVERSABLE
		traversable_hexes = getAdjacentTraversableHexes(hexManagerS.getHex(x,z));
		for(int i = 0; i < traversable_hexes.Length; i++){
			if(traversable_hexes[i] != null){
				print(traversable_hexes[i].hex_type);
			}
		}
		*/
		
		/*//TEST UNTRAVERSABLE
		untraversable_hexes = getAdjacentUntraversableHexes(hexManagerS.getHex(x,z));
		for(int i = 0; i < untraversable_hexes.Length; i++){
			if(untraversable_hexes[i] != null){
				print(untraversable_hexes[i].hex_type);
			}
		}
		*/
		
		/*//TESTS MOVE
		if(test == 0){
			makeMove(new HexData(-3, 1, Hex.Water));
			test = 1;
		}else if (test == 1){
			makeMove(new HexData(-2, 1, Hex.Water));
			test = 2;
		}else{
			makeMove(new HexData(-1, 1, Hex.Water));	
			test = 0;
		}
		*/
	}

	#region IMove implementation
	public List<HexData> getAdjacentTraversableHexes (HexData hex)
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(canTraverse(adjacent_hexes[i])){
				//add hex to traversable array
				result_hexes.Add(adjacent_hexes[i]); 
			}
		}
		//print ("length" + result_hexes.Length);
		//print ("length " + result_hexes[0].hex_type + " " + result_hexes[1].hex_type + + " " + result_hexes[2].hex_type + " " + result_hexes[3].hex_type + " " + result_hexes[4].hex_type + " " + result_hexes[5].hex_type + " " + result_hexes[6].hex_type);
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
			if(entityManagerS.canTraverseHex(hex)){
				return false;
			}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
				return false;
			}else{
				return true;
			}
	}
	

	public void makeMove (HexData hex)
	{
		//move player to new hex position
		Vector3 temp = new Vector3(hex.x,player_location.position.y,hex.z);
		player_location.position = temp;
		
		//update players hex tags
		x = hex.x;
		z = hex.z;
		
		//TODO: Some code dealing with the level editor will probably go here ***
	}
	#endregion

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
}

