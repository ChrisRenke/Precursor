using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class entitySpawnS : Entity {
	
	public readonly EntityE   entity_type = EntityE.Spawn;
	
	public int spawner_id_number; 
	public bool spawned_enemies_know_mech_location;
	public bool spawned_enemies_know_base_location;
	public Dictionary<int, int> round_to_enemy_count;
	public string cadence_DISPLAY;
	public string cadence_input_DISPLAY;
	private List<int> round_entries;
//	entityManagerS.instantiateSpawn(x, z, spawner_id_number, spawner_cadence, spawned_enemies_know_mech_location, spawned_enemies_know_base_location);
					 
	
	
	public void createCadence(string spawner_cadence){
		cadence_input_DISPLAY = spawner_cadence;
		round_entries = new List<int>();
		round_to_enemy_count = new Dictionary<int, int>();
		Debug.Log("Creating cadence intervals for spawner " + spawner_id_number);
		string[] cadence_intervals = spawner_cadence.Split(new string[] {","},StringSplitOptions.None);
		
		foreach(string interval in cadence_intervals)
		{
			string[] cadence = interval.Split(new string[] {"/"},StringSplitOptions.None);
			int round = int.Parse(cadence[1]);
			int enemies = int.Parse(cadence[0]);
			round_to_enemy_count.Add(round, enemies);
			round_entries.Add(round);
			Debug.Log("Round: " + round + " + Enemies: " + enemies);
			cadence_DISPLAY += "Round: " + round + " + Enemies: " + enemies + " | ";
		}
	}
	
	public int getMaxEnemyCountForThisRound(int current_round)
	{
		int enemy_count = 0;
		for(int i = 0; i < round_entries.Count; i++)
		{
			if(current_round >= round_entries[i])
				enemy_count = round_to_enemy_count[round_entries[i]];
			else
				break;
		}
		//current_round 6
		//1 - 1
		//3 - 5
		//2 - 10
		return enemy_count;
	}
	
	
	
	
	 
	//SetSpriteAnimation
	public void SetVisiual(){
		   
		//vars for the whole sheet
		int colCount    = 2;
		int rowCount    = 1;
		 
		//vars for animation
		int rowNumber   = 0; //Zero Indexed
		int colNumber   = 0; //Zero Indexed
		int totalCells  = 2;
		int frame_index = 0;
		
		frame_index = UnityEngine.Random.Range(0,1);
			
		// Size of every cell
	    float sizeX = 1.0f / colCount;
	    float sizeY = 1.0f / rowCount;
	    Vector2 size =  new Vector2(sizeX,sizeY);
	 
	    // split into horizontal and vertical index
	    var uIndex = frame_index % colCount;
	    var vIndex = frame_index / colCount;
	 
	    // build offset
	    // v coordinate is the bottom of the image in opengl so we need to invert.
	    float offsetX = (uIndex+colNumber) * size.x;
	    float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
	    Vector2 offset = new Vector2(offsetX,offsetY);
	  
	    renderer.material.SetTextureOffset ("_MainTex", offset); 
	    renderer.material.SetTextureScale  ("_MainTex", size); 
	}
}