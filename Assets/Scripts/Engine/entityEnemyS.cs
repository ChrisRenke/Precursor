using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityEnemyS : Combatable, IMove {
	
	private List<HexData> traversable_hexes; //Hold traversable hexes
	private List<HexData> untraversable_hexes; //Hold untraversable hexes
	private List<HexData> path_hexes; //Hold untraversable hexes

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
 
	
	public List<HexData> getAdjacentTraversableHexes ()
	{
		throw new System.NotImplementedException ();
	}

	public List<HexData> getAdjacentUntraversableHexes ()
	{
		throw new System.NotImplementedException ();
	}

	public bool canTraverse (HexData hex)
	{
		throw new System.NotImplementedException ();
	}

	public bool makeMove (HexData hex)
	{
		throw new System.NotImplementedException ();
	} 
	
	public int getTraverseAPCost(Hex hex_type)
	{
		throw new System.NotImplementedException ();
	}
 
	public override int attackTarget (Combatable target)
	{
		throw new System.NotImplementedException ();
	}
 
	
}
