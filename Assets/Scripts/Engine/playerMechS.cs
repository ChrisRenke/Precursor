using UnityEngine;
using System.Collections;

public class playerMechS : MonoBehaviour, IMove {
	public Transform player_location; //hold player location
	
	private int size = 16; //size of traversable_hexes
	private hexManagerS.HexData[] traversable_hexes; //Hold traversable hexes
	private hexManagerS.HexData[] untraversable_hexes; //Hold untraversable hexes
	//array of hex data that will contain the hexes the player can move to***
	//If base.enemy then array is treated as a path with index 0 being first hex on path to base
	//path
	int test = 0;
	
	//Use this for initialization
	void Start () {
		traversable_hexes = new hexManagerS.HexData[size];
		untraversable_hexes = new hexManagerS.HexData[size];
	}
	
	//Update is called once per frame
	void Update () {
		
		//TODO: Need to base players moves off of turn-base and mouse click etc
		
		/*TESTS TRAVERSABLE
		traversable_hexes = getTraversableHexes();
		for(int i = 0; i < traversable_hexes.Length; i++){
			if(traversable_hexes[i] != null){
				print(traversable_hexes[i].hex_type);
			}
		}
		*/
		
		/*TEST UNTRAVERSABLE
		untraversable_hexes = getUntraversableHexes();
		for(int i = 0; i < untraversable_hexes.Length; i++){
			if(untraversable_hexes[i] != null){
				print(untraversable_hexes[i].hex_type);
			}
		}
		*/
		
		/*TESTS MOVE
		if(test == 0){
			makeMove(new hexManagerS.HexData(-3, 1, hexManagerS.Tiles.Water, false));
			test = 1;
		}else if (test == 1){
			makeMove(new hexManagerS.HexData(-2, 1, hexManagerS.Tiles.Water, false));
			test = 2;
		}else{
			makeMove(new hexManagerS.HexData(-1, 1, hexManagerS.Tiles.Water, false));	
			test = 0;
		}
		*/
	}


	#region IMove implementation
	public hexManagerS.HexData[] getTraversableHexes ()
	{
		int index = 0;
		
		//Get adjacent tiles around player mech
		hexManagerS.HexData[] adjacent_hexes = hexManagerS.getAdjacent(player_location.position);
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(isTraversable(adjacent_hexes[i]) && index < size){
				//add hex to traversable array
				traversable_hexes[index] = adjacent_hexes[i]; 
				index++;
			}
		}
		
		return traversable_hexes;
	}

	public hexManagerS.HexData[] getUntraversableHexes ()
	{
		int index = 0;
		
		//Get adjacent tiles around player mech
		hexManagerS.HexData[] adjacent_hexes = hexManagerS.getAdjacent(player_location.position);
		
		//See which of the adjacent hexes are untraversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(!isTraversable(adjacent_hexes[i]) && index < size){
				//add hex to untraversable array
				untraversable_hexes[index] = adjacent_hexes[i]; 
				index++;
			}
		}
		
		return untraversable_hexes;
	}

	public bool isTraversable (hexManagerS.HexData hex)
	{
		//TODO: 1. Expand on hex options once fully known
		//		2. Will need to add the upgrade options into the traversable descision
		if(hex.occupied){
			return false;
		}else if(hex.hex_type == hexManagerS.Tiles.Water || hex.hex_type == hexManagerS.Tiles.Mountain || hex.hex_type == hexManagerS.Tiles.EditorTileB || hex.hex_type == hexManagerS.Tiles.EditorTileA){
			return false;
		}else{
			return true;
		}
	}

	public void makeMove (hexManagerS.HexData desination)
	{  
		//TODO: 1. I think the z_axis needs to be updated, and so will change later
		//		2. I'm not sure how the hex manager will work, so may or may not need to commit to it
		//Vector3 temp = new Vector3(hex.x,hex.y,0);
//		player_location.position = temp;
		//commit move to hex manager
//		hexManagerS.updateMap(hex, hex); //dummy value
	}
	
	public void makeMove (int x, int z)
	{  
		//TODO: 1. I think the z_axis needs to be updated, and so will change later
		//		2. I'm not sure how the hex manager will work, so may or may not need to commit to it
		//Vector3 temp = new Vector3(hex.x,hex.y,0);
//		player_location.position = temp;
		//commit move to hex manager
//		hexManagerS.updateMap(hex, hex); //dummy value
	}
	#endregion
}
