using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityBaseS : Combatable {
	
	
	public static List<HexData> adjacent_visible_hexes;
	// Use this for initialization
	void Start () {
		adjacent_visible_hexes = hexManagerS.getAdjacentHexes(x, z, sight_range);
		
		current_hp = 100;
		max_hp = 100;
		
		//turn hexes within range now to Live state
		foreach(HexData hex in adjacent_visible_hexes)
		{ 
			hexManagerS.updateHexVisionState(hex, Vision.Live);
			hex.hex_script.updateFoWState();
		} 
		entityManagerS.updateEntityFoWStates();
	}
	// Update is called once per frame
	void Update () {
			
		if(checkIfDead())
			onDeath();
	}
	
	
	void OnGUI()
	{
		Vector3 screen_pos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y+30, 200, 15), current_hp + "/" + max_hp + " HP", enginePlayerS.hover_text);
		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y + 45, 200, 15), current_ap + "/" + max_ap + " AP", enginePlayerS.hover_text);
	}
	
	#region implemented abstract members of Combatable
	public override int attackTarget (Combatable target)
	{
		throw new System.NotImplementedException ();
	}
	#endregion
	
	public override bool onDeath()
	{
		Application.Quit();
		return true;
	}
}
