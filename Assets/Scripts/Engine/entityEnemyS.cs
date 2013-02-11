using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityEnemyS : Combatable, IMove, IPathFind {
	
	private int size = 6; //size of traversable_hexes
	private HexData[] traversable_hexes; //Hold traversable hexes
	private HexData[] untraversable_hexes; //Hold untraversable hexes
	private List<HexData> path_to_base; //Hold traversable hex path  
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
	public HexData[] getAdjacentTraversableHexes (int x_pos, int z_pos)
	{
		int index = 0;
		HexData[] result_hexes = new HexData[size]; //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x_pos, z_pos);
		
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

	public HexData[] getAdjacentUntraversableHexes (int x_pos, int z_pos)
	{
		int index = 0;
		HexData[] result_hexes = new HexData[size]; //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x_pos, z_pos);
		
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
	public List<HexData> getTraversablePath ()
	{
		//Send hex of base and hex of enemy to aStar 
		var path = aStar.FindPath(start, destination, calcDistance, calcEstimate, getAdjacentTraversableHexes);
		return extractPath(path);
	}
	
	//Extract hexes from path and put hexes into a List 
	private List<HexData> extractPath (IEnumerable<HexData> path){
		List<HexData> list = new List<HexData>();
		
		foreach (HexData hex in path)
        {
			list.Add(hex);
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
		//TODO: this is dummy code, can be adjusted later
        return 1;
	}

	public double calcEstimate (HexData hex_start, HexData hex_end)
	{
		//TODO: this is dummy code, can be adjusted later
		
        HexData destTile = destTileTB.hex;
        float deltaX = Mathf.Abs(destTile.x - hex.x);
        float deltaZ = Mathf.Abs(destTile.z - hex.z);
        
        return Mathf.Max(deltaX, deltaZ);
	}
	#endregion
}
