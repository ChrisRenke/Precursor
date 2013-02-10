using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityEnemyS : Combatable, IMove {
	
	private int size = 6; //size of traversable_hexes
	private HexData[] traversable_hexes; //Hold traversable hexes
	private HexData[] untraversable_hexes; //Hold untraversable hexes
	private HexData[] path_hexes; //Hold traversable hex path  
	//Script for mech and base
	entityBaseS base_s;
	entityMechS mech_s;

	// Use this for initialization
	void Start () {
		base_s = gameObject.GetComponent<entityBaseS>();
	    mech_s = gameObject.GetComponent<entityMechS>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	#region IMove implementation
	public HexData[] getAdjacentTraversableHexes ()
	{
		int index = 0;
		HexData[] result_hexes = new HexData[size]; //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x, z);
		
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

	public HexData[] getAdjacentUntraversableHexes ()
	{
		int index = 0;
		HexData[] result_hexes = new HexData[size]; //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x, z);
		
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
		return entityManagerS.isEntityPos(hex, EntityE.Player) || entityManagerS.isEntityPos(hex, EntityE.Base) || entityManagerS.isEntityPos(hex, EntityE.Factory);
	}

	public void makeMove (HexData hex)
	{
		//move enemy to next hex, update hex pos
		x = hex.x;
		z = hex.z;
		
		//TODO: Some code dealing with the level editor will probably go here ***
	
	}
	#endregion
	
	//Get path to base
	public HexData[] getTraversablePath ()
	{
		//Send hex of base and hex of enemy to aStar
		LinkedList<HexData> path_temp = aStar.search(hexManagerS.getHex(x, z), hexManagerS.getHex(base_s.x, base_s.z));
		throw new System.NotImplementedException ();
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
	
}
