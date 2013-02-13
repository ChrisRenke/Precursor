using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityEnemyS : Combatable, IMove {
	 
	private List<HexData> traversable_hexes; //Hold traversable hexes
	private List<HexData> untraversable_hexes; //Hold untraversable hexes
	private List<HexData> path_hexes; //Hold untraversable hexes 

	
	public bool knows_mech_location;
	public bool knows_base_location;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
 
	 
	public List<HexData> getAdjacentTraversableHexes () 
	{
//		int index = 0;
//		HexData[] result_hexes = new HexData[size]; //hold resulting hexes
//		
//		//Get adjacent tiles around player mech
//		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
//		
//		//See which of the adjacent hexes are traversable
//		for(int i = 0; i < adjacent_hexes.Length; i++){
//			if(canTraverse(adjacent_hexes[i]) && index < size){
//				//add hex to traversable array
//				result_hexes[index] = adjacent_hexes[i]; 
//				index++;
//			}
//		}
		
		return null;
	}
 
	public List<HexData> getAdjacentUntraversableHexes () 
	{
//		int index = 0;
//		HexData[] result_hexes = new HexData[size]; //hold resulting hexes
//		
//		//Get adjacent tiles around player mech
//		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
//		
//		//See which of the adjacent hexes are untraversable
//		for(int i = 0; i < adjacent_hexes.Length; i++){
//			if(!canTraverse(adjacent_hexes[i]) && index < size){
//				//add hex to untraversable array
//				result_hexes[index] = adjacent_hexes[i]; 
//				index++;
//			}
//		}
		
		return null;
	}

	public bool canTraverse (HexData hex)
	{
		//TODO: 1. Expand on hex options once fully known
		//		2. Will need to add the upgrade options into the traversable descision
//			if(entityManagerS.canTraverseHex(hex)){
//				return false;
//			}else if(hex.hex_type == Hex.Water || hex.hex_type == Hex.Mountain || hex.hex_type == Hex.Perimeter){
//				return false;
//			}else{
//				return true;
//			}
//		
		return true;
	}  
	
	public int getTraverseAPCost(Hex hex_type){
		return 2;
	}
 	
	public bool makeMove (HexData hex)
	{
		//move enemy to next hex, update hex pos
		x = hex.x;
		z = hex.z;
		//TODO: Some code dealing with the level editor will probably go here ***
		return true;
	
	} 
	
 
	public override int attackTarget (Combatable target)
	{
		throw new System.NotImplementedException ();
	}
 
	
 
}
