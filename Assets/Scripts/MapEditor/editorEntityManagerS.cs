using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class editorEntityManagerS : MonoBehaviour {
	
	public static Dictionary<int, Dictionary<int, GameObject>>	  entity_db  = new Dictionary<int, Dictionary<int, GameObject>>();  
	public static Dictionary<editor_entity, GameObject>    	entity_dict  = new Dictionary<editor_entity, GameObject>(); 
	  
	public GameObject  	player_entity;
	public GameObject  	enemy_entity;
	public GameObject 	flyer_entity;
	public GameObject  	spawn_entity;
	public GameObject  	outpost_entity;
	public GameObject  	junkyard_entity; 
	public GameObject  	factory_entity; 
	public GameObject  	base_entity; 
	
	
	//enemy
	public int       enemy_current_hp = 15;
	public int 	     enemy_max_hp     = 15;
	public int       enemy_spawner_owner_id = 0;
	public bool 	 enemy_know_mech_location = false;
	public bool	 	 enemy_know_base_location = true;
	public EntityE	 enemy_type = EntityE.Enemy;
	public EntityE	 flyer_type = EntityE.Flyer;
	
	//town
	public int         town_current_hp = 100;
	public int 	       town_max_hp = 100;
	public BaseUpgradeLevel town_structure_level = BaseUpgradeLevel.Level0;
	public BaseUpgradeLevel town_wall_level      = BaseUpgradeLevel.Level0;
	public BaseUpgradeLevel town_defense_level   = BaseUpgradeLevel.Level0;
	
	//spawn
//	public int  spawner_max_enemies_from_this_spawn = 3;
	public int  spawner_id_number = 0;
	public bool spawned_enemies_know_mech_location = false;
	public bool spawned_enemies_know_base_location = true;
	public EntityE	 spawner_enemy_type = EntityE.Enemy;
	public string spawner_cadence = "1/1";
	
	//node
	public NodeLevel node_level = NodeLevel.Full; 
	
	//mech 
	public int      mech_current_hp  = 30;
	public int 	    mech_max_hp 	 = 30;
	public bool		gun_range	= false;
	public bool		gun_cost	= false;
	public bool		gun_damage	= false;
	public bool		mobi_cost	= false;
	public bool		mobi_water	= false;
	public bool		mobi_mntn	= false;
	public bool		ex_tele		= false;
	public bool		ex_armor	= false;
	public bool		ex_scav		= false; 
	
	
	
	
	// Use this for initialization
	void Start () { 
		entity_dict.Add(editor_entity.Mech, 	player_entity);
		entity_dict.Add(editor_entity.Town, 	base_entity); 
		entity_dict.Add(editor_entity.Enemy, 	enemy_entity);
		entity_dict.Add(editor_entity.Flyer, 	flyer_entity);
		entity_dict.Add(editor_entity.Spawn, 	spawn_entity);  
		entity_dict.Add(editor_entity.Factory, 	factory_entity);
		entity_dict.Add(editor_entity.Outpost, 	outpost_entity);
		entity_dict.Add(editor_entity.Junkyard, junkyard_entity);  
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void playSelectTile()
	{
		audio.Play();
	}
	
	public bool occupied(int x, int z)
	{
		if(entity_db.ContainsKey(x))
		{
			if(entity_db[x].ContainsKey(z))
			{
				return true;
			}
		}
		return false;
	}
	
  	private GameObject InstantiateEntity(Vector3 pos, editor_entity ent_type, int x, int z)
	{ 
		GameObject new_ent  = (GameObject) Instantiate(entity_dict[ent_type], pos, Quaternion.identity);
		
		switch(ent_type){
			
		case editor_entity.Enemy:{
			var script = new_ent.GetComponent<info_enemy>();
			script.x = x;
			script.z = z;
			script.name = "entity("+ x +"," + z +")";
			script.ent_type = ent_type;
			script.current_hp = enemy_current_hp;
			script.max_hp = enemy_max_hp;
			script.spawner_owner_id   = enemy_spawner_owner_id;
			script.know_mech_location = enemy_know_mech_location;
			script.know_base_location = enemy_know_base_location;
			script.enemy_type = enemy_type;
		} break;
		case editor_entity.Flyer:{
			var script = new_ent.GetComponent<info_flyer>(); //TODO
			script.x = x;
			script.z = z;
			script.name = "entity("+ x +"," + z +")";
			script.ent_type = ent_type;
			script.current_hp = enemy_current_hp;
			script.max_hp = enemy_max_hp;
			script.spawner_owner_id   = enemy_spawner_owner_id;
			script.know_mech_location = enemy_know_mech_location;
			script.know_base_location = enemy_know_base_location;
			script.enemy_type = flyer_type; 
		} break;
		case editor_entity.Spawn:{
			var script = new_ent.GetComponent<info_spawn>();
//			script.max_enemies_from_this_spawn = spawner_max_enemies_from_this_spawn;
			script.cadence = spawner_cadence;
			script.spawner_id_number = spawner_id_number;
			script.spawned_enemies_know_mech_location = spawned_enemies_know_mech_location;
			script.spawned_enemies_know_base_location = spawned_enemies_know_base_location; 
			script.x = x;
			script.z = z;
			script.name = "entity("+ x +"," + z +")";
			script.ent_type = ent_type;
			script.spawned_enemy_type = spawner_enemy_type;
		}  break;
		case editor_entity.Mech:{
			var script = new_ent.GetComponent<info_mech>();
			script.x = x;
			script.z = z;
			script.name = "entity("+ x +"," + z +")";
			script.ent_type = ent_type;
			script.current_hp =  mech_current_hp;
			script.max_hp = 	 mech_max_hp; 	
			
			
			
			
			
			
		} break;
		case editor_entity.Town:{
			var script = new_ent.GetComponent<info_town>();
			script.x = x;
			script.z = z;
			script.name = "entity("+ x +"," + z +")";
			script.ent_type = ent_type;
			script.current_hp = town_current_hp;
			script.max_hp     = town_max_hp;
			script.structure_level = town_structure_level;
			script.wall_level = town_wall_level;
			script.defense_level = town_defense_level; 
		} break;
			
		case editor_entity.Junkyard:{
			var script = new_ent.GetComponent<info_node>();
			script.x = x;
			script.z = z;
			script.name = "entity("+ x +"," + z +")";
			script.ent_type = ent_type;
			script.node_level = node_level;
		}break;
		case editor_entity.Factory:{
			var script = new_ent.GetComponent<info_node>();
			script.x = x;
			script.z = z;
			script.name = "entity("+ x +"," + z +")";
			script.ent_type = ent_type;
			script.node_level = node_level;
		}break;
		case editor_entity.Outpost:{
			var script = new_ent.GetComponent<info_node>();
			script.x = x;
			script.z = z;
			script.name = "entity("+ x +"," + z +")";
			script.ent_type = ent_type;
			script.node_level = node_level;
		}break;
		}
		
		return new_ent; 
	}
	
	
	public void deleteEntity(GameObject entity_to_delete)
	{
		
		var script = entity_to_delete.GetComponent<entity_core>();
		entity_db[script.x].Remove(script.z);
		if(entity_db[script.x].Keys.Count == 0)
		{
			print("removing sub level");
			entity_db.Remove(script.x);
		}
		
		Destroy(entity_to_delete);
	}
	
	
	public void LoadEntity(editor_entity ent_type, int x, int z)
	{
		Vector3 converted = editorUserS.CoordsGameTo3D(x, z);
		Vector3 adjusted  = new Vector3(converted.x, converted.y, converted.z + .5F);
		AddEntity(adjusted, ent_type, x, z);
	}
	
	
	public GameObject AddEntity(Vector3 pos, editor_entity ent_type, int x, int z)
	{
	
		GameObject new_ent;  
		print ("ENTITY created @ " + x + ", " + z);
		
		if(entity_db.ContainsKey(x))
		{ 
			if(entity_db[x].ContainsKey(z))
			{
				
					var script = entity_db[x][z].GetComponent<entity_core>();
					//if there is already an entity there and we want to overwrite, overwrite it 
					if(script.ent_type != editorUserS.last_created_entity_type)
					{
						new_ent   = InstantiateEntity(pos, ent_type, x, z); 
						
						//now delete what is already there
						Destroy(entity_db[x][z]);  
					}
					else
					{
					print ("--same Entity already exists there.");	
					}
			}
			else
			{
				//we've just gotta make a new z entry since nothing has ever been made in this z spot
				print ("--nothing ever in that z.");  
				
				new_ent = InstantiateEntity(pos, ent_type, x, z); 
				
				entity_db[x].Add(z,  new_ent);   
				
			}
		}
		else
		{
			//nothing has ever been made in this x row, so make the z dict and the z entry
			print ("--nothing ever in that x.");  
			new_ent = InstantiateEntity(pos, ent_type, x, z); 
			
			entity_db.Add(x, new Dictionary<int, GameObject>());
			entity_db[x].Add(z,  new_ent);  
			  
		}
		
		return entity_db[x][z];
	} 
	 
	
	void OnGUI()
	{
		if(editorUserS.entity_mode)
		{
			//draw Base config options
			if(editorUserS.last_created_entity_type == editor_entity.Town)
			{
				town_current_hp = (int)GUI.HorizontalSlider(new Rect( 30, 70, 210, 30), town_current_hp, (float) 0, (float) 200);	
				GUI.Label(new Rect(250, 65, 150, 30),  "CurrentHP: " + town_current_hp);
				
				town_max_hp = (int)GUI.HorizontalSlider(new Rect( 30, 110, 210, 30), town_max_hp, (float) 0, (float) 200);	
				GUI.Label(new Rect(250, 105, 150, 30),  "MaxHP: " +  town_max_hp  );
				
				town_structure_level = (BaseUpgradeLevel)GUI.HorizontalSlider(new Rect( 30, 150, 210, 30), (int)town_structure_level, (float) 0, (float) 3                                                                                                                                                                                                                                                                                                     );	
				GUI.Label(new Rect(250, 145, 150, 30), "Structure: " + town_structure_level);
				
				town_wall_level = (BaseUpgradeLevel)GUI.HorizontalSlider(new Rect( 30, 190, 210, 30), (int)town_wall_level, (float) 0, (float) 3);	
				GUI.Label(new Rect(250, 185, 150, 30), "Walls: " + town_wall_level);
				
				town_defense_level = (BaseUpgradeLevel)GUI.HorizontalSlider(new Rect( 30, 230, 210, 30), (int)town_defense_level, (float) 0, (float) 3);	
				GUI.Label(new Rect(250, 225, 150, 30), "Defense: " + town_defense_level);
				
			}
			else
			//draw Player config options
			if(editorUserS.last_created_entity_type == editor_entity.Mech)
			{ 
				mech_current_hp = (int)GUI.HorizontalSlider(new Rect( 30, 70, 210, 30), mech_current_hp, (float) 0, (float) 200);	
				GUI.Label(new Rect(250, 65, 150, 30),  "CurrentHP: " + mech_current_hp );
				
				mech_max_hp = (int)GUI.HorizontalSlider(new Rect( 30, 110, 210, 30), mech_max_hp, (float) 0, (float) 200);	
				GUI.Label(new Rect(250, 105, 150, 30), "MaxHP: " + mech_max_hp  );
				 
				if(GUI.Button(new Rect( 30, 150, 70, 30), "Rng:" + gun_range.ToString().Substring(0,1)))
					gun_range  = !gun_range; 
				if(GUI.Button(new Rect( 100, 150, 70, 30),"Cst:" + gun_cost.ToString().Substring(0,1)))
					gun_cost   = !gun_cost; 
				if(GUI.Button(new Rect( 170, 150, 70, 30),"Dmg:" + gun_damage.ToString().Substring(0,1)))
					gun_damage = !gun_damage; 
				
				if(GUI.Button(new Rect( 30, 185, 70, 30), "Spd:" + mobi_cost.ToString().Substring(0,1)))
					mobi_cost  = !mobi_cost; 
				if(GUI.Button(new Rect( 100, 185, 70, 30),"Wtr:" + mobi_water.ToString().Substring(0,1)))
					mobi_water = !mobi_water; 
				if(GUI.Button(new Rect( 170, 185, 70, 30),"Mtn:" + mobi_mntn.ToString().Substring(0,1)))
					mobi_mntn  = !mobi_mntn; 
				
				if(GUI.Button(new Rect( 30, 220, 70, 30), "Tele:" + ex_tele.ToString().Substring(0,1)))
					ex_tele  = !ex_tele; 
				if(GUI.Button(new Rect( 100, 220, 70, 30),"Amr:" + ex_armor.ToString().Substring(0,1)))
					ex_armor = !ex_armor; 
				if(GUI.Button(new Rect( 170, 220, 70, 30),"Scv:" + ex_scav.ToString().Substring(0,1)))
					ex_scav  = !ex_scav; 
				  
			}
			else
			//draw node config options
			if(editorUserS.last_created_entity_type == editor_entity.Factory || editorUserS.last_created_entity_type == editor_entity.Junkyard || editorUserS.last_created_entity_type == editor_entity.Outpost)
			{
				node_level = (NodeLevel)GUI.HorizontalSlider(new Rect( 30, 70, 210, 30), (int)node_level, (float) 0, (float) 2);	 
				GUI.Label(new Rect(250, 65, 100, 30),  node_level.ToString());
			}
			else
				//draw enemy config options
				if(editorUserS.last_created_entity_type == editor_entity.Enemy)
				{ 
					if(GUI.Button(new Rect( 30, 70, 210, 30), "Base: " + enemy_know_base_location))
						enemy_know_base_location  = !enemy_know_base_location; 
					
					if(GUI.Button(new Rect( 30, 110, 210, 30), "Mech: " + enemy_know_mech_location))
						enemy_know_mech_location  = !enemy_know_mech_location; 
					
	//				enemy_spawner_owner_id = (int) GUI.HorizontalSlider(new Rect( 30, 150, 210, 30), enemy_spawner_owner_id, (float) 0, (float) 20);
					val_enemy = GUI.TextField(new Rect( 30, 150, 210, 30), val_enemy, 2);
					val_enemy = Regex.Replace(val_enemy, @"[^0-9 ]", "");
					try{
						enemy_spawner_owner_id = int.Parse(val_enemy);
					}
					catch{
						enemy_spawner_owner_id = 0;
					} 
					GUI.Label(new Rect(250, 150, 100, 30),  "SpawnerID: " + enemy_spawner_owner_id);
	//				GUI.Label(new Rect(250, 150, 100, 60), "Source SpawnerID: " + enemy_spawner_owner_id );
				} 
			
//				//draw enemy config options
//				if(editorUserS.last_created_entity_type == editor_entity.Flyer)
//				{ 
//					if(GUI.Button(new Rect( 30, 70, 210, 30), "Base: " + enemy_know_base_location))
//						enemy_know_base_location  = !enemy_know_base_location; 
//					
//					if(GUI.Button(new Rect( 30, 110, 210, 30), "Mech: " + enemy_know_mech_location))
//						enemy_know_mech_location  = !enemy_know_mech_location; 
//					
//	//				enemy_spawner_owner_id = (int) GUI.HorizontalSlider(new Rect( 30, 150, 210, 30), enemy_spawner_owner_id, (float) 0, (float) 20);
//					val_enemy = GUI.TextField(new Rect( 30, 150, 210, 30), val_enemy, 2);
//					val_enemy = Regex.Replace(val_enemy, @"[^0-9 ]", "");
//					try{
//						enemy_spawner_owner_id = int.Parse(val_enemy);
//					}
//					catch{
//						enemy_spawner_owner_id = 0;
//					} 
//					GUI.Label(new Rect(250, 150, 100, 30),  "SpawnerID: " + enemy_spawner_owner_id);
//	//				GUI.Label(new Rect(250, 150, 100, 60), "Source SpawnerID: " + enemy_spawner_owner_id );
//				} 
			
		
				else
				//draw spawn config options
	if(editorUserS.last_created_entity_type == editor_entity.Spawn)
	{
	if(GUI.Button(new Rect( 30, 70, 210, 30), "Base: " + spawned_enemies_know_base_location))
	spawned_enemies_know_base_location = !spawned_enemies_know_base_location;
	
	if(GUI.Button(new Rect( 30, 110, 210, 30), "Mech: " + spawned_enemies_know_mech_location))
	spawned_enemies_know_mech_location = !spawned_enemies_know_mech_location;
	
	// spawner_id_number = (int) GUI.HorizontalSlider(new Rect( 30, 150, 210, 30), spawner_id_number, (float) 0, (float) 20);
	val_spawn = GUI.TextField(new Rect( 30, 150, 210, 30), val_spawn, 2);
	val_spawn = Regex.Replace(val_spawn, @"[^0-9 ]", "");
	try{
	spawner_id_number = int.Parse(val_spawn);
	}
	catch{
	spawner_id_number = 0;
	}
	GUI.Label(new Rect(250, 150, 100, 30), "SpawnerID: " + spawner_id_number);
	
	// spawner_max_enemies_from_this_spawn = (int) GUI.HorizontalSlider(new Rect( 30, 190, 210, 30), spawner_max_enemies_from_this_spawn, (float) 0, (float) 20);
	// GUI.Label(new Rect(250, 190, 100, 60), "Simultaneous Enemies: " + spawner_max_enemies_from_this_spawn);
	spawner_cadence = GUI.TextField(new Rect( 30, 190, 210, 30), spawner_cadence,200);
	spawner_cadence = Regex.Replace(spawner_cadence, @"[^0-9,/]", "");
	GUI.Label(new Rect(250, 190, 200, 60), "Cadence");
	GUI.Label(new Rect(30, 220, 210, 60), " MaxEnemies/RoundNum,repeat\n ie: 3/1,5/10,3/12,5/20");
	}
	}

	
}
string val_enemy = "";
string val_spawn = "";
}