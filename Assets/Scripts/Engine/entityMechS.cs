using UnityEngine;
using System.Collections;

public class entityMechS : Combatable, IMove {
	
	private int size = 16; //size of traversable_hexes
	private HexData[] traversable_hexes; //Hold traversable hexes
	private HexData[] untraversable_hexes; //Hold untraversable hexes
		
	//Test vars ***
	bool players_turn = true;
	int test = 0;
	private GameObject 	hex_choice;
	public Transform player_location; //hold player location, for testing
	
	
	
	//Use this for initialization
	void Start () {
		traversable_hexes = new HexData[size];
		untraversable_hexes = new HexData[size];
		x = 5;
		z = 5;
	}
	
	//Update is called once per frame
	void Update () {
		
		//ADDED: ROUGH OUTLINE for Update method below ***
		//		Uncomment the block comment to test
		
		if(players_turn){
			//getTraversible hexes
			traversable_hexes = getAdjacentTraversableHexes(hexManagerS.getHex(x,z));
			
			//TODO: some mechanism for highlighting valid hexes?
			
			//Check to see if player clicked a hex (left click)
			if(Input.GetMouseButtonDown(0)){
					//TODO: check to see if player clicked on a traversible hex from the traversable_hexes array
					//		if traversible hex clicked then call makeMove, otherwise do nothing (or print why invalid hex)
					
					//TEST: For testing purposes: the player moves randomly for each mouse click at the moment
					if(test == 0){
						makeMove(new HexData(-3, 1, Hex.Grass));
						test = 1;
					}else if (test == 1){
						makeMove(new HexData(-2, 1, Hex.Grass));
						test = 2;
					}else{
						makeMove(new HexData(-1, 1, Hex.Grass));	
						test = 0;
					}
				
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
	public HexData[] getAdjacentTraversableHexes (HexData hex)
	{
		int index = 0;
		HexData[] result_hexes = new HexData[size]; //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(canTraverse(adjacent_hexes[i]) && index < size){
				//add hex to traversable array
				result_hexes[index] = adjacent_hexes[i]; 
				index++;
			}
		}
		
		return result_hexes;
	}

	public HexData[] getAdjacentUntraversableHexes (HexData hex)
	{
		int index = 0;
		HexData[] result_hexes = new HexData[size]; //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		
		//See which of the adjacent hexes are untraversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(!canTraverse(adjacent_hexes[i]) && index < size){
				//add hex to untraversable array
				result_hexes[index] = adjacent_hexes[i]; 
				index++;
			}
		}
		
		return result_hexes;
	}

	public bool canTraverse (HexData hex)
	{
		//TODO: 1. Expand on hex options once fully known
		//		2. Will need to add the upgrade options into the traversable descision
			if(occupied(hex)){
				return false;
			}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
				return false;
			}else{
				return true;
			}
	}
	
	public bool occupied (HexData hex)
	{
		return entityManagerS.isEntityPos(hex, EntityE.Enemy) || entityManagerS.isEntityPos(hex, EntityE.Base) || entityManagerS.isEntityPos(hex, EntityE.Factory);
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

