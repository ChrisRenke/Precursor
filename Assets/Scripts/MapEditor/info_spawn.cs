using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text; 

public class info_spawn : entity_core {
	
//public enum BaseUpgrade { Level0, Level1, Level2, Level3}
//public enum BaseCategories  { Structure, Walls, Defense }; 
	
	public int  max_enemies_from_this_spawn = 3;
	public int  spawner_id_number = 0;
	public bool spawned_enemies_know_mech_location = false;
	public bool spawned_enemies_know_base_location = true;
	
	public GUIStyle appear;
	
	public override string getOutput()
	{
		StringBuilder sb = makeOutputHeaderSB();
		sb.AppendLine("\t\t\tspawn_id   = " + spawner_id_number); 
		sb.AppendLine("\t\t\tmax_enemy  = " + max_enemies_from_this_spawn); 
		sb.AppendLine("\t\t\tmech_loc   = " + spawned_enemies_know_mech_location); 
		sb.AppendLine("\t\t\tbase_loc   = " + spawned_enemies_know_base_location); 
        sb.AppendLine("\t\t}");
		return sb.ToString();
	}
	
	void OnGUI(){ 
		Vector3 screen_pos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y, 200, 35), "ID: " + spawner_id_number  + "\nEnemies: " + max_enemies_from_this_spawn, appear);
	}
}
