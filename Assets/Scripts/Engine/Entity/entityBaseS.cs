using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityBaseS : Combatable {
	
	
	public static List<HexData> adjacent_visible_hexes;
	// Use this for initialization
	void Start () {
		adjacent_visible_hexes = hexManagerS.getAdjacentHexes(x, z, sight_range);
		
		//turn hexes within range now to Live state
		foreach(HexData hex in adjacent_visible_hexes)
		{ 
			hexManagerS.updateHexVisionState(hex, Vision.Live);
			hex.hex_script.updateFoWState();
		} 
	}
	// Update is called once per frame
	void Update () {
	}

	
	#region implemented abstract members of Combatable
	public override int attackTarget (Combatable target)
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
