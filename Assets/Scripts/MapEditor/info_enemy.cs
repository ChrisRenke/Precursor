using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text; 

public class info_enemy : entity_core {
	
//public enum BaseUpgrade { Level0, Level1, Level2, Level3}
//public enum BaseCategories  { Structure, Walls, Defense }; 
	   
	public int       current_hp = 15;
	public int 	     max_hp     = 15; 
	
	public int       spawner_owner_id = 0;
	
	public bool know_mech_location = false;
	public bool know_base_location = true;
	
	public GUIStyle appearnce;
	
	public override string getOutput()
	{
		StringBuilder sb = makeOutputHeaderSB();
		sb.AppendLine("\t\t\tcurrent_hp = " + current_hp);
		sb.AppendLine("\t\t\tmax_hp     = " + max_hp);
		sb.AppendLine("\t\t\tmech_loc   = " + know_mech_location);
		sb.AppendLine("\t\t\tbase_loc   = " + know_base_location);
        sb.AppendLine("\t\t}");
		return sb.ToString();
	}
	
	
	void OnGUI(){ 
		Vector3 screen_pos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y, 200, 35), "Spawner: " + spawner_owner_id, appearnce);
	}
}
