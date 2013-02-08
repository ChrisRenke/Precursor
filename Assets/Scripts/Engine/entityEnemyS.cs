using UnityEngine;
using System.Collections;

public class entityEnemyS : Combatable, IMove {
	
	private HexData[] traversable_hexes; //Hold traversable hexes
	private HexData[] untraversable_hexes; //Hold untraversable hexes

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	#region IMove implementation
	public HexData[] getAdjacentTraversableHexes ()
	{
		throw new System.NotImplementedException ();
	}

	public HexData[] getAdjacentUntraversableHexes ()
	{
		throw new System.NotImplementedException ();
	}

	public bool canTraverse (HexData hex)
	{
		throw new System.NotImplementedException ();
	}

	public void makeMove (HexData hex)
	{
		throw new System.NotImplementedException ();
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
