using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityManagerS : MonoBehaviour {
	 
	//Using dummy values for testing
	public static entityBaseS base_s;
	public static entityMechS mech_s;
	public static List<entityEnemyS> enemy_list;
	public static List<entityNodeS> resource_node_list;
	public static List<entitySpawnS> spawn_list;
	public static Dictionary<int, int> spawnid_to_enemiesactive;
	
	
	//TODO replace with proper spawns
	public static List<entityNodeS> spawn_points;
	
	public static int					enemy_starting_ap = 8;
	public static int					enemy_starting_hp = 15;
	public static int					enemy_max_hp      = 15;
	
	public static soundManagerS         sm;
	
	public GameObject base_entity;
	public GameObject player_entity;
	public GameObject enemy_entity;
	public GameObject node_entity;
	public GameObject spawn_entity;
	
	public GameObject particle_gear;
	public GameObject particle_plate;
	public GameObject particle_piston;
	public GameObject particle_strut;
	
	public GameObject particle_deathenemy;
	public static GameObject particle_deathenemy_s;
	public static GameObject particle_heal_s;
	public static GameObject 	particle_tele_s;
	public   GameObject particle_heal;
	public   GameObject 	particle_tele;
	
	public static Dictionary<EntityE, GameObject> entity_dict = new Dictionary<EntityE, GameObject>();
	public static Dictionary<Part, GameObject> part_dict = new Dictionary<Part, GameObject>();
	public static Dictionary<Node, GameObject> node_dict = new Dictionary<Node, GameObject>();
	
	// Use this for initialization
	void Awake () { 
		base_s              = gameObject.GetComponent<entityBaseS>();
	    mech_s  	 	    = gameObject.GetComponent<entityMechS>();
		enemy_list 			= new List<entityEnemyS>();
		resource_node_list  = new List<entityNodeS>(); 
		spawn_list   		= new List<entitySpawnS>();
		spawnid_to_enemiesactive = new Dictionary<int, int>();

		particle_deathenemy_s = particle_deathenemy;
		particle_heal_s = particle_heal;
		particle_tele_s = particle_tele;
		
		entity_dict = new Dictionary<EntityE, GameObject>();
		entity_dict.Add(EntityE.Base, base_entity);
		entity_dict.Add(EntityE.Player, player_entity);
		entity_dict.Add(EntityE.Enemy, enemy_entity);
		entity_dict.Add(EntityE.Spawn, spawn_entity);
		
		node_dict.Add(Node.Factory, node_entity);
		node_dict.Add(Node.Outpost, node_entity);
		node_dict.Add(Node.Junkyard, node_entity);
		
		part_dict.Add(Part.Gear, particle_gear);
		part_dict.Add(Part.Plate, particle_plate);
		part_dict.Add(Part.Piston, particle_piston);
		part_dict.Add(Part.Strut, particle_strut);
		
		sm = GameObject.Find("soundManager").GetComponent<soundManagerS>();
		spawn_points 		= new List<entityNodeS>();
		
	}
	
//	public static void setSM()
//	{
//		
//	}
	
	public static void updateNodeLevel(int x, int z, NodeLevel previous_level)
	{
		foreach(entityNodeS node in resource_node_list)
			if(x == node.x && z == node.z)
			{
				if(previous_level == NodeLevel.Full)
					node.node_level = NodeLevel.Sparse;
				else
					node.node_level = NodeLevel.Empty;
			
				node.SetVisiual();		
			}
	}
	
	public static NodeData getNodeInfoAt(int x, int z)
	{
		foreach(entityNodeS node in resource_node_list)
			if(x == node.x && z == node.z)
			{
				return node.getNodeData();
			}
		throw new System.Exception("DONT getNodeInfoAt wihtout first checkin there is a not there!!");
	}
	
	public static bool isNodeAt(int x, int z)
	{
		foreach(entityNodeS node in resource_node_list)
		{
			if(x == node.x && z == node.z)
			{
				return true;
			}
		}
		return false;
	}
	
	
	public static entityBaseS getBase(){
		return base_s;
	}
	
	public static entityMechS getMech(){
		return mech_s;
	}
	
	public static List<entityEnemyS> getEnemies(){
		return enemy_list;
	}
	
	public static List<entityNodeS> getResourceNodes(){
		return resource_node_list; 
	}
	
	public static entityEnemyS getEnemyAt(int hex_x, int hex_z)
	{
		foreach(entityEnemyS enemy in enemy_list)
			if(hex_x == enemy.x && hex_z == enemy.z)
				return enemy;	
		return null;
	}
	
	public static Combatable getCombatableAt(HexData hex)
	{
		return getCombatableAt(hex.x, hex.z);
	}
	
	public static Combatable getCombatableAt(int hex_x, int hex_z)
	{
		if(isEntityPos(hex_x, hex_z, EntityE.Base))
			return base_s;
		if(!isEntityPos(hex_x, hex_z, EntityE.Player))
			return mech_s;
		if(!isEntityPos(hex_x, hex_z, EntityE.Enemy))
		{
			foreach(entityEnemyS enemy in enemy_list)
				if(hex_x == enemy.x && hex_z == enemy.z)
					return enemy;	
			throw new System.Exception("Enemy present, but now found. Idk, this is broken, check enemy locations");
		}
		
		return null;
	}
	
	public static void createPartEffect(int x, int z, Part part_type)
	{
		Instantiate(part_dict[part_type], CoordsGameTo3DEntiy(x, z) + new Vector3(0,2F, 0), Quaternion.identity);
	}
	
	public static void createDeathEffect(int x, int z)
	{
		Instantiate( particle_deathenemy_s, CoordsGameTo3DEntiy(x, z) + new Vector3(0,-2F, 0), Quaternion.identity);
	}
	
	public static void createHealEffect(int x, int z)
	{
		Instantiate( particle_heal_s, CoordsGameTo3DEntiy(x, z) + new Vector3(0,-2F, 0), Quaternion.identity);
	}
	
	public static void createTeleEffect(int x, int z)
	{
		Instantiate( particle_tele_s, CoordsGameTo3DEntiy(x, z) + new Vector3(0,-2F, 0), Quaternion.identity);
	}
	
	public static void purgeEnemy(entityEnemyS dead_enemy)
	{
		createDeathEffect(dead_enemy.x, dead_enemy.z);
		sm.playExplodeEnemy();
		enemy_list.Remove(dead_enemy);
		spawnid_to_enemiesactive[dead_enemy.spawner_owner]--;
		hexManagerS.getHex(dead_enemy.x, dead_enemy.z).hex_script.can_attack_hex = false;
		DestroyImmediate(dead_enemy.gameObject);
		
	}
	 
	public static List<HexData> getEnemyLocationsInRange(HexData center, int sight_range)
	{ 
		List<HexData> hexes_in_range = new List<HexData>();
		
		//get the hex standing on
		HexData current_hex = center;  
		
		//enter loop for surrounding hexes
		for(int ring = 1; ring <= sight_range; ring++)
		{
			 
			//draw the first "northeast" edge hex 
			current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.NorthEast);
			if(entityManagerS.getEnemyAt(current_hex.x, current_hex.z))
				hexes_in_range.Add(current_hex); 
			
			//draw the "northeast" portion
			for(int edge_hexes_drawn = 1; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{ 
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.SouthEast);// = AddHexSE(overwrite, border_mode, clicked_hex_type, brush_size, current_hex.transform.position, draw_hex_type, xcrd(current_hex), zcrd(current_hex)); 
				if(entityManagerS.getEnemyAt(current_hex.x, current_hex.z))
					hexes_in_range.Add(current_hex); 
			}
			
			//draw the "southeast" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.South);
				if(entityManagerS.getEnemyAt(current_hex.x, current_hex.z))
					hexes_in_range.Add(current_hex); 
			}
			
			//draw the "south" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.SouthWest);
				if(entityManagerS.getEnemyAt(current_hex.x, current_hex.z))
					hexes_in_range.Add(current_hex); 
			}
			
			//draw the "southwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.NorthWest);
				if(entityManagerS.getEnemyAt(current_hex.x, current_hex.z))
					hexes_in_range.Add(current_hex); 
			}
			
			//draw the "northwest" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.North);
				if(entityManagerS.getEnemyAt(current_hex.x, current_hex.z))
					hexes_in_range.Add(current_hex); 
			}
			
			//draw the "north" portion
			for(int edge_hexes_drawn = 0; edge_hexes_drawn < ring; ++edge_hexes_drawn)
			{
				current_hex = hexManagerS.getHex(current_hex.x, current_hex.z, Facing.NorthEast);
				if(entityManagerS.getEnemyAt(current_hex.x, current_hex.z))
					hexes_in_range.Add(current_hex); 
			}
		}
		
//		Debug.LogWarning("hexes_in_range size = " + hexes_in_range.Count);
		return hexes_in_range;
	}
	
	public static void updateEntityFoWStates()
	{
//		for(entityNodeS node in resource_node_list)
//		{
//			if(previously_visible.Contains())
//		}
		foreach(entityNodeS node in resource_node_list)
			node.updateFoWState();
		foreach(entityEnemyS enemy in enemy_list)
			enemy.updateFoWState(); 
		foreach(entitySpawnS spawn in spawn_list)
			spawn.updateFoWState();
		
	}
	
	public static HexData fillEntityData(HexData hex){
		
		hex.added_occupier = EntityE.None;
		
		if(isEntityPos(hex, EntityE.Player))
			hex.added_occupier = EntityE.Player;
		
		else if(isEntityPos(hex, EntityE.Base))
			hex.added_occupier = EntityE.Base;
		
		else if(isEntityPos(hex, EntityE.Enemy))
			hex.added_occupier = EntityE.Enemy;
		
		else 
			foreach(entityNodeS node in resource_node_list)
				if(hex.x == node.x && hex.z == node.z)
				{
					hex.added_occupier = EntityE.Node;
				}
		return hex;
	}
	  
	public static bool isEntityPos(HexData hex, EntityE entity){
		return isEntityPos(hex.x, hex.z, entity);
	}
	public static bool isEntityPos(int hex_x, int hex_z, EntityE entity){
		
		switch(entity)
		{
			case EntityE.Player:

				if(hex_x == mech_s.x && hex_z == mech_s.z)
					return true;
				return false;
				
			case EntityE.Base:

				if(hex_x == base_s.x && hex_z == base_s.z)
					return true;
				return false;
				 
			case EntityE.Node:
				if(resource_node_list != null){
					foreach(entityNodeS node in resource_node_list)
						if(hex_x == node.x && hex_z == node.z)
							return true;
				}
				return false;
				
			case EntityE.Enemy:
				if(enemy_list != null){
					foreach(entityEnemyS enemy in enemy_list)
						if(hex_x == enemy.x && hex_z == enemy.z)
							return true;
				}
				return false;
			
			default:
				return false;
		}
	}
		
	//Check to see if the indicated hex can be moved onto, ie, its not occupied by an enemy, player, or base
	public static bool canTraverseHex(HexData hex){
		return canTraverseHex(hex.x, hex.z);
	}
	public static bool canTraverseHex(int hex_x, int hex_z){
		if(!isEntityPos(hex_x, hex_z, EntityE.Enemy)  &&
		   !isEntityPos(hex_x, hex_z, EntityE.Base)   && 
		   !isEntityPos(hex_x, hex_z, EntityE.Player))
			return true;
		return false;
	}
	
	//Same has other canTraversHex except check to see if the indicated hex can be moved onto without considering excluded entity into calculations 
	//assumption is that if you send in "null" you will get to default case otherwise send it NODE
	public static bool canTraverseHex(HexData hex, EntityE exclude){
		return canTraverseHex(hex.x, hex.z, exclude);
	}
	
	public static bool canTraverseHex(int hex_x, int hex_z, EntityE exclude){
		switch(exclude)
		{
			case EntityE.Player:
				if(!isEntityPos(hex_x, hex_z, EntityE.Enemy)  &&
				   !isEntityPos(hex_x, hex_z, EntityE.Base))
					return true;
				return false;
			
			case EntityE.Base:
				if(!isEntityPos(hex_x, hex_z, EntityE.Enemy)  && 
				   !isEntityPos(hex_x, hex_z, EntityE.Player))
					return true;
				return false;
			
			case EntityE.Enemy:
				if(!isEntityPos(hex_x, hex_z, EntityE.Base)   && 
				   !isEntityPos(hex_x, hex_z, EntityE.Player))
					return true;
				return false;
			
			default:
				if(!isEntityPos(hex_x, hex_z, EntityE.Enemy)  &&
				   !isEntityPos(hex_x, hex_z, EntityE.Base)   && 
				   !isEntityPos(hex_x, hex_z, EntityE.Player))
					return true;
				return false;
		}
	}
	
	//instantiate an object into the gamespace
	private static GameObject instantiateEntity(int x, int z, EntityE entity_type)
	{
		if(entity_type == EntityE.Spawn)
			return (GameObject) Instantiate(entity_dict[entity_type], CoordsGameTo3DSpawnEntiy(x, z), Quaternion.identity);
		if(entity_type == EntityE.Base)
			return (GameObject) Instantiate(entity_dict[entity_type], CoordsGameTo3DSpawnEntiy(x, z) - new Vector3(0, -2F, 0), Quaternion.identity);
		else
		if(entity_type == EntityE.Player)
			return (GameObject) Instantiate(entity_dict[entity_type], CoordsGameTo3DSpawnEntiy(x, z) + new Vector3(0, 1F, 0), Quaternion.identity);
		else
			return (GameObject) Instantiate(entity_dict[entity_type], CoordsGameTo3DEntiy(x, z), Quaternion.identity);
	}
	
	//instantiate an object into the gamespace
	private static GameObject instantiateEntityNode(int x, int z, Node node_type)
	{
		return (GameObject) Instantiate(node_dict[node_type], CoordsGameTo3DEntiy(x, z) - new Vector3(0, .5F, 0), Quaternion.identity);
	}
	
	
	//create a base for the level
	public static bool instantiateBase(int x, int z, int town_current_hp, int town_max_hp, BaseUpgrade town_wall_level,
					BaseUpgrade town_defense_level, BaseUpgrade town_structure_level)
	{
		sm = GameObject.Find("soundManager").GetComponent<soundManagerS>();
		bool is_first_base = base_s == null;
		print (is_first_base + " base make");
		
		GameObject new_entity = instantiateEntity(x, z, EntityE.Base);
		base_s = (entityBaseS) new_entity.AddComponent("entityBaseS"); 
		
		if(base_s == null)
			throw new System.Exception("Base Entity not created properly D: | " + (base_s == null));
			
		base_s.x = x;
		base_s.z = z;
		
		base_s.current_hp = town_current_hp;
		base_s.max_hp = town_max_hp;
		base_s.wall_level = town_wall_level;
		base_s.structure_level = town_structure_level;
		base_s.defense_level = town_defense_level;
		
		hexManagerS.getHex(x, z).hex_script.setAsTownHex();
		enginePlayerS.setBase();
		return is_first_base;
	}
 
	//create a player for the level
	public static bool instantiatePlayer(int x, int z, int mech_current_hp, int mech_max_hp, 
					bool gun_range,
					bool gun_cost	,
					bool gun_damage	,
					bool mobi_cost	,
					bool mobi_water	,
					bool mobi_mntn	,
					bool ex_tele	,
					bool ex_armor	,
					bool ex_scav)
	{
		bool is_first_mech = mech_s == null;
		GameObject new_entity = instantiateEntity(x, z, EntityE.Player);
		mech_s = (entityMechS) new_entity.GetComponent("entityMechS");
		
		if(mech_s == null)
			throw new System.Exception("Mech Entity not created properly D:");
		
		mech_s.x = x;
		mech_s.z = z;
		mech_s.current_hp = mech_current_hp;
		mech_s.max_hp = mech_max_hp;
		
		mech_s.upgrade_combat_armor = ex_armor;
		mech_s.upgrade_util_recall = ex_tele;
		mech_s.upgrade_scavenge_cost = ex_scav;
		
		mech_s.upgrade_move_cost = mobi_cost;
		mech_s.upgrade_move_mountain = mobi_mntn;
		mech_s.upgrade_move_water = mobi_water;
		
		mech_s.upgrade_combat_cost = gun_cost;
		mech_s.upgrade_combat_damage = gun_damage;
		mech_s.upgrade_combat_range = gun_range;
		
		hexManagerS.getHex(x,z).hex_script.mech_is_here = true;
		
		return is_first_mech;
	}
	
	
	public static bool instantiateSpawn(int x, int z, int spawner_id_number, string spawner_cadence, 
		bool spawned_enemies_know_mech_location, bool spawned_enemies_know_base_location){
		 
		GameObject new_entity = instantiateEntity(x, z, EntityE.Spawn); 
		entitySpawnS spawn_s = (entitySpawnS) new_entity.AddComponent("entitySpawnS");
		
		spawn_s.x = x;
		spawn_s.z = z;
		spawn_s.spawner_id_number = spawner_id_number; 
		spawn_s.spawned_enemies_know_mech_location = spawned_enemies_know_mech_location;
		spawn_s.spawned_enemies_know_base_location = spawned_enemies_know_base_location;
		spawn_s.createCadence(spawner_cadence); 
		spawn_s.SetVisiual();
		spawn_list.Add(spawn_s);
		return true;
	}
					
	//create a base for the level
	public static bool instantiateEnemy(int x, int z, int enemy_current_hp, int enemy_max_hp, 
		int enemy_spawner_id_number, bool enemy_knows_base_loc, bool enemy_knows_mech_loc)
	{
		GameObject new_entity = instantiateEntity(x, z, EntityE.Enemy); 
		entityEnemyS new_enemy_s = (entityEnemyS) new_entity.AddComponent("entityEnemyS");
//		enemyDisplayS new_display = (enemyDisplayS) new_entity.AddComponent("enemyDisplayS");
		
		if(new_enemy_s == null)
			throw new System.Exception("Enemy Entity not created properly D:");
		
		new_enemy_s.x = x;
		new_enemy_s.z = z;
		new_enemy_s.current_ap = enemy_starting_ap;
		new_enemy_s.max_hp = enemy_max_hp;
		new_enemy_s.current_hp = enemy_current_hp;
		new_enemy_s.spawner_owner = enemy_spawner_id_number;		
		new_enemy_s.knows_base_location = enemy_knows_base_loc;
		new_enemy_s.knows_mech_location = enemy_knows_mech_loc;
		enemy_list.Add(new_enemy_s);
	
		//past the first turn of the game, spawned enemies manage their own presence in the spawner number data shit
		if(gameManagerS.current_round > 1)
		{
			spawnid_to_enemiesactive[enemy_spawner_id_number]++;
		}
		return true;
	}
	
	//create a base for the level
	public static bool instantiateResourceNode(int x, int z, Node node_type, NodeLevel node_level)
	{
		if(node_type == Node.Factory || node_type == Node.Junkyard || node_type == Node.Outpost)
		{
			GameObject new_entity = instantiateEntityNode(x, z, node_type);
			entityNodeS new_node = (entityNodeS) new_entity.AddComponent("entityNodeS");
			
			if(new_node == null)
				throw new System.Exception("Node Entity not created properly D:");
				
			new_node.x = x;
			new_node.z = z;
			new_node.node_type  = node_type;
			new_node.node_level = node_level;
			new_node.SetVisiual();
			resource_node_list.Add(new_node);
			
			
			return true;
		}
		else
		{
			throw new System.Exception("Trying to create a resource node, but passing in an invalid EntityE type! D:");
		}
	}
	
	
	//converts engine coordinates into 3D space cordinates
	public static Vector3 CoordsGameTo3DEntiy(int x, int z)
	{  
		return new Vector3(x * 2.30024F + z * -0.841947F, 5, z * 1.81415F + x * 1.3280592F + .5F);
	}
	
	public static Vector3 CoordsGameTo3DSpawnEntiy(int x, int z)
	{  
		return new Vector3(x * 2.30024F + z * -0.841947F, 3, z * 1.81415F + x * 1.3280592F + .5F);
	}
	
		
//	public static List<entityNodeS> getSpawnPoints(){
//		return spawn_points; 
//	}
//	
//	
//	//Make spawn points, ASSUMPTION: assumes atleast 5 resouce nodes in existence that aren't 
//	//close to base or mech starting positions
////	public static bool initEnemySpawnPoints(){
////		//After resource nodes, mech and base are made grab resource node positions and choose 5 spawn spots
////		int distance_from_other_entities = 5;
////		int number_of_resource_nodes_needed_on_map = 6;
////		spawn_points = new List<entityNodeS>(); //always reset list if this method is called
////		
////		if(resource_node_list.Count < number_of_resource_nodes_needed_on_map){
////			Debug.Log ("Not enough resource nodes on map");
////			return false;
////		}
////		
////		foreach(entityNodeS r_node in resource_node_list){
////			//only choose spawn spots that are atleast "distance_from_other_entities" hexes away from base and mech start position
////			//then make spawn spot on the resource node, plus check that an enemy isn't already on the spawn spot 
////			if(!entity_in_sight(r_node.x, r_node.z, distance_from_other_entities) && getEnemyAt(r_node.x, r_node.z) == null){
////				spawn_points.Add(r_node);
////			}
////		}
////		
////		if(spawn_points.Count == 0){
////			Debug.Log ("Couldn't make any spawn points, either all hexes occupied or all nodes are close to mech and base");
////			return false;
////		}
////			
////		return true;
////	}
////	
//	
//	private static bool entity_in_sight(int x_r, int z_r, int distance_from_entities){
//		Debug.Log("Resource node " + x_r + " | " + z_r);  
//			foreach(HexData h in hexManagerS.getAdjacentHexes(x_r, z_r, distance_from_entities)){
//				if((h.x == mech_s.x && h.z == mech_s.z) || (h.x == base_s.x && h.z == base_s.z))
//				{
//					//entity mech or base is too close to resource node
//					Debug.Log ("entity mech or base is too close to resource node, sight_range = " + distance_from_entities);
//					return true;
//				}
//			}  
//		Debug.Log ("no entities close to resouce node");
//		return false;
//	}
	 
	
	public static void buildEnemySpawnCounts(){
		foreach(entityEnemyS enemy in enemy_list)
		{
			if(spawnid_to_enemiesactive.ContainsKey(enemy.spawner_owner))
				spawnid_to_enemiesactive[enemy.spawner_owner]++;
			else
				spawnid_to_enemiesactive.Add(enemy.spawner_owner, 1);
		}
		
		
		foreach(entitySpawnS spawn in spawn_list)
		{
			if(!spawnid_to_enemiesactive.ContainsKey(spawn.spawner_id_number)) 
				spawnid_to_enemiesactive.Add(spawn.spawner_id_number, 0);
		}
		
		
	}
	
	public static bool instantiateEnemiesFromSpawns()
	{ 
		foreach(entitySpawnS spawn in spawn_list)
		{
//			if(spawnid_to_enemiesactive.ContainsKey()spawn.spawner_id_number])
//			Debug.Log("Current Max: " + spawn.getMaxEnemyCountForThisRound(gameManagerS.current_round) + " | deployed: " + spawnid_to_enemiesactive[spawn.spawner_id_number]);
			if(spawn.getMaxEnemyCountForThisRound(gameManagerS.current_round) > spawnid_to_enemiesactive[spawn.spawner_id_number])
			{
				//if there are no enemies ontop of the spawn and hte player isn't on top of the spawn
				if(entityManagerS.getEnemyAt(spawn.x, spawn.z) == null && 
					!(entityManagerS.getMech().x == spawn.x && entityManagerS.getMech().z == spawn.z))
				{
					instantiateEnemy(spawn.x, spawn.z, enemy_starting_hp, enemy_max_hp, spawn.spawner_id_number,
						spawn.spawned_enemies_know_base_location, spawn.spawned_enemies_know_mech_location);
					
//					Debug.LogWarning("SPAWNED AN ENEMY FROM SPAWNERID " + spawn.spawner_id_number);
				}
//				Debug.LogWarning("FAILED TO SPAWN @ SPAWNERID " + spawn.spawner_id_number +", SOMETHING IN WAY");
				
			}
//			Debug.LogWarning("FAILED TO SPAWN @ SPAWNERID " + spawn.spawner_id_number +", MAX OF " + spawn.getMaxEnemyCountForThisRound(gameManagerS.current_round) + " ALREADY REACHED");
				
		}
		return true;
	}
	
//	public static bool spawnNewEnemy(){ 
//		int enemies_on_board = entityManagerS.getEnemies().Count;
//		int enemy_quota = 12; //if number of enemies on board is lower than this number, more enemies need to be made
//		bool sp_made = initEnemySpawnPoints(); //get valid spawn points
//		
//		if(!sp_made){
//			Debug.Log ("Get Spawn Points failed");
//			return sp_made;
//		}
//		
//		//Check the quota of enemies to see if we can make more enemies
//		if (enemies_on_board < enemy_quota){
//			//Have a chance algorithm for what enemy should spawn
//			int r = UnityEngine.Random.Range(0,20);
//			int r1 = UnityEngine.Random.Range(0,spawn_points.Count);
//			if(r < 4){
//				//Initialize new enemy at one point in List of Spawn Points(r1)
//				bool check = entityManagerS.instantiateEnemy(spawn_points[r1].x, spawn_points[r1].z, true, true);	
//				Debug.Log("make_new_enemy: " + check + " , x= " + spawn_points[r1].x + ", z=" + spawn_points[r1].z);
//				return check;
//			}else if(r < 8){
//				//Initialize new enemy at one point in List of Spawn Points(r1)
//				bool check = entityManagerS.instantiateEnemy(spawn_points[r1].x, spawn_points[r1].z, true, false);	
//				Debug.Log("make_new_enemy: " + check + " , x= " + spawn_points[r1].x + ", z=" + spawn_points[r1].z);
//				return check;
//			}else if(r < 12){
//				//Initialize new enemy at one point in List of Spawn Points(r1)
//				bool check = entityManagerS.instantiateEnemy(spawn_points[r1].x, spawn_points[r1].z, false, true);	
//				Debug.Log("make_new_enemy: " + check + " , x= " + spawn_points[r1].x + ", z=" + spawn_points[r1].z);
//				return check;
//			}else if(r < 16){
//				//Initialize new enemy at one point in List of Spawn Points(r1)
//				bool check = entityManagerS.instantiateEnemy(spawn_points[r1].x, spawn_points[r1].z, false, false);	
//				Debug.Log("make_new_enemy: " + check + " , x= " + spawn_points[r1].x + ", z=" + spawn_points[r1].z);
//				return check;
//			}else{
//				Debug.Log("enemy didn't spawn this time around, 20% chance this will happen");
//				return false;
//			}
//		
//		}else{
//			Debug.Log("There are enough enemies on board, won't spawn more");
//			return false;
//		}
//		return false;
//	}
	

}
	

/*

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityManagerS : MonoBehaviour {
	 
	//Using dummy values for testing
	public static entityBaseS base_s;
	public static entityMechS mech_s;
	public static List<entityEnemyS> enemy_list;
	public static List<entityNodeS> resource_node_list;
	public static List<entityNodeS> spawn_points;
	
	
	public static int					enemy_starting_ap = 8;
	
	public GameObject base_entity;
	public GameObject player_entity;
	public GameObject enemy_entity;
	public GameObject junkyard_entity;
	public GameObject outpost_entity;
	public GameObject factory_entiy;
	
	public GameObject particle_gear;
	public GameObject particle_plate;
	public GameObject particle_piston;
	public GameObject particle_strut;
	
	public static Dictionary<EntityE, GameObject> entity_dict = new Dictionary<EntityE, GameObject>();
	public static Dictionary<Part, GameObject> part_dict = new Dictionary<Part, GameObject>();
	public static Dictionary<Node, GameObject> node_dict = new Dictionary<Node, GameObject>();
	
	// Use this for initialization
	void Awake () { 
		base_s              = gameObject.GetComponent<entityBaseS>();
	    mech_s  	 	    = gameObject.GetComponent<entityMechS>();
		enemy_list 			= new List<entityEnemyS>();
		resource_node_list  = new List<entityNodeS>(); 
		spawn_points 		= new List<entityNodeS>();
		
		entity_dict = new Dictionary<EntityE, GameObject>();
		entity_dict.Add(EntityE.Base, base_entity);
		entity_dict.Add(EntityE.Player, player_entity);
		entity_dict.Add(EntityE.Enemy, enemy_entity);
		
		node_dict.Add(Node.Factory, factory_entiy);
		node_dict.Add(Node.Outpost, outpost_entity);
		node_dict.Add(Node.Junkyard, junkyard_entity);
		
		part_dict.Add(Part.Gear, particle_gear);
		part_dict.Add(Part.Plate, particle_plate);
		part_dict.Add(Part.Piston, particle_piston);
		part_dict.Add(Part.Strut, particle_strut);
		
	}
	
	public static void updateNodeLevel(int x, int z, NodeLevel previous_level)
	{
		foreach(entityNodeS node in resource_node_list)
			if(x == node.x && z == node.z)
			{
				if(previous_level == NodeLevel.Full)
					node.node_level = NodeLevel.Sparse;
				else
					node.node_level = NodeLevel.Empty;
					
			}
	}
	
	public static NodeData getNodeInfoAt(int x, int z)
	{
		foreach(entityNodeS node in resource_node_list)
			if(x == node.x && z == node.z)
			{
				return node.getNodeData();
			}
		throw new System.Exception("DONT getNodeInfoAt wihtout first checkin there is a not there!!");
	}
	
	public static bool isNodeAt(int x, int z)
	{
		foreach(entityNodeS node in resource_node_list)
			if(x == node.x && z == node.z)
			{
				return true;
			}
		return false;
		
	}
	
	
	public static entityBaseS getBase(){
		return base_s;
	}
	
	public static entityMechS getMech(){
		return mech_s;
	}
	
	public static List<entityEnemyS> getEnemies(){
		return enemy_list;
	}
	
	public static List<entityNodeS> getResourceNodes(){
		return resource_node_list; 
	}
	
	public static entityEnemyS getEnemyAt(int hex_x, int hex_z)
	{
		foreach(entityEnemyS enemy in enemy_list)
			if(hex_x == enemy.x && hex_z == enemy.z)
				return enemy;	
		return null;
	}

	public static Combatable getCombatableAt(HexData hex)
	{
		return getCombatableAt(hex.x, hex.z);
	}
	
	public static Combatable getCombatableAt(int hex_x, int hex_z)
	{
		if(isEntityPos(hex_x, hex_z, EntityE.Base))
			return base_s;
		if(!isEntityPos(hex_x, hex_z, EntityE.Player))
			return mech_s;
		if(!isEntityPos(hex_x, hex_z, EntityE.Enemy))
		{
			foreach(entityEnemyS enemy in enemy_list)
				if(hex_x == enemy.x && hex_z == enemy.z)
					return enemy;	
			throw new System.Exception("Enemy present, but now found. Idk, this is broken, check enemy locations");
		}
		
		return null;
	}
	
	public static void createPartEffect(int x, int z, Part part_type)
	{
		Instantiate(part_dict[part_type], CoordsGameTo3DEntiy(x, z) + new Vector3(0,2F, 0), Quaternion.identity);
	}
	
	public static void purgeEnemy(entityEnemyS dead_enemy){
		enemy_list.Remove(dead_enemy);
		DestroyImmediate(dead_enemy.gameObject);
		
	}
	
	public static void updateEntityFoWStates()
	{
//		for(entityNodeS node in resource_node_list)
//		{
//			if(previously_visible.Contains())
//		}
		foreach(entityNodeS node in resource_node_list)
			node.updateFoWState();
		foreach(entityEnemyS enemy in enemy_list)
			enemy.updateFoWState();
	}
	
	public static HexData fillEntityData(HexData hex){
		
		hex.added_occupier = EntityE.None;
		
		if(isEntityPos(hex, EntityE.Player))
			hex.added_occupier = EntityE.Player;
		
		else if(isEntityPos(hex, EntityE.Base))
			hex.added_occupier = EntityE.Base;
		
		else if(isEntityPos(hex, EntityE.Enemy))
			hex.added_occupier = EntityE.Enemy;
		
		else 
			foreach(entityNodeS node in resource_node_list)
				if(hex.x == node.x && hex.z == node.z)
				{
					hex.added_occupier = EntityE.Node;
				}
		return hex;
	}
	  
	public static bool isEntityPos(HexData hex, EntityE entity){
		return isEntityPos(hex.x, hex.z, entity);
	}
	public static bool isEntityPos(int hex_x, int hex_z, EntityE entity){
		
		switch(entity)
		{
			case EntityE.Player:

				if(hex_x == mech_s.x && hex_z == mech_s.z)
					return true;
				return false;
				
			case EntityE.Base:

				if(hex_x == base_s.x && hex_z == base_s.z)
					return true;
				return false;
				 
			case EntityE.Node:
				if(resource_node_list != null){
					foreach(entityNodeS node in resource_node_list)
						if(hex_x == node.x && hex_z == node.z)
							return true;
				}
				return false;
				
			case EntityE.Enemy:
				if(enemy_list != null){
					foreach(entityEnemyS enemy in enemy_list)
						if(hex_x == enemy.x && hex_z == enemy.z)
							return true;
				}
				return false;
			
			default:
				return false;
		}
	}
		
	//Check to see if the indicated hex can be moved onto, ie, its not occupied by an enemy, player, or base
	public static bool canTraverseHex(HexData hex){
		return canTraverseHex(hex.x, hex.z);
	}
	public static bool canTraverseHex(int hex_x, int hex_z){
		if(!isEntityPos(hex_x, hex_z, EntityE.Enemy)  &&
		   !isEntityPos(hex_x, hex_z, EntityE.Base)   && 
		   !isEntityPos(hex_x, hex_z, EntityE.Player))
			return true;
		return false;
	}
	
	//Same has other canTraversHex except check to see if the indicated hex can be moved onto without considering excluded entity into calculations 
	//assumption is that if you send in "null" you will get to default case otherwise send it NODE
	public static bool canTraverseHex(HexData hex, EntityE exclude){
		return canTraverseHex(hex.x, hex.z, exclude);
	}
	
	public static bool canTraverseHex(int hex_x, int hex_z, EntityE exclude){
		switch(exclude)
		{
			case EntityE.Player:
				if(!isEntityPos(hex_x, hex_z, EntityE.Enemy)  &&
				   !isEntityPos(hex_x, hex_z, EntityE.Base))
					return true;
				return false;
			
			case EntityE.Base:
				if(!isEntityPos(hex_x, hex_z, EntityE.Enemy)  && 
				   !isEntityPos(hex_x, hex_z, EntityE.Player))
					return true;
				return false;
			
			case EntityE.Enemy:
				if(!isEntityPos(hex_x, hex_z, EntityE.Base)   && 
				   !isEntityPos(hex_x, hex_z, EntityE.Player))
					return true;
				return false;
			
			default:
				if(!isEntityPos(hex_x, hex_z, EntityE.Enemy)  &&
				   !isEntityPos(hex_x, hex_z, EntityE.Base)   && 
				   !isEntityPos(hex_x, hex_z, EntityE.Player))
					return true;
				return false;
		}
	}
	
	//instantiate an object into the gamespace
	private static GameObject instantiateEntity(int x, int z, EntityE entity_type)
	{
		return (GameObject) Instantiate(entity_dict[entity_type], CoordsGameTo3DEntiy(x, z), Quaternion.identity);
	}
	
	//instantiate an object into the gamespace
	private static GameObject instantiateEntityNode(int x, int z, Node node_type)
	{
		return (GameObject) Instantiate(node_dict[node_type], CoordsGameTo3DEntiy(x, z) - new Vector3(0, .5F, 0), Quaternion.identity);
	}
	
	
	//create a base for the level
	public static bool instantiateBase(int x, int z, float starting_hp_percentage)
	{
		bool is_first_base = base_s == null;
		print (is_first_base + "base make");
		GameObject new_entity = instantiateEntity(x, z, EntityE.Base);
		base_s = (entityBaseS) new_entity.AddComponent("entityBaseS"); 
		
		if(base_s == null)
			throw new System.Exception("Base Entity not created properly D: | " + (base_s == null));
			
		base_s.x = x;
		base_s.z = z;
		base_s.setCurrentHPviaPercentage(starting_hp_percentage);
		return is_first_base;
	}
 
	//create a player for the level
	public static bool instantiatePlayer(int x, int z, float starting_hp_percentage)
	{
		bool is_first_mech = mech_s == null;
		GameObject new_entity = instantiateEntity(x, z, EntityE.Player);
		mech_s = (entityMechS) new_entity.GetComponent("entityMechS");
		
		if(mech_s == null)
			throw new System.Exception("Mech Entity not created properly D:");
		
		mech_s.x = x;
		mech_s.z = z;
		mech_s.setCurrentHPviaPercentage(starting_hp_percentage);
		return is_first_mech;
	}
	
	//create a base for the level
	public static bool instantiateEnemy(int x, int z, bool knows_base_location, bool knows_mech_location)
	{
		GameObject new_entity = instantiateEntity(x, z, EntityE.Enemy); 
		entityEnemyS new_enemy_s = (entityEnemyS) new_entity.AddComponent("entityEnemyS");
//		enemyDisplayS new_display = (enemyDisplayS) new_entity.AddComponent("enemyDisplayS");
		
		if(new_enemy_s == null)
			throw new System.Exception("Enemy Entity not created properly D:");
		
		new_enemy_s.x = x;
		new_enemy_s.z = z;
		new_enemy_s.current_ap = enemy_starting_ap;
		new_enemy_s.knows_base_location = knows_base_location;
		new_enemy_s.knows_mech_location = knows_mech_location;
		enemy_list.Add(new_enemy_s);
		return true;
	}
	
	//create a base for the level
	public static bool instantiateResourceNode(int x, int z, Node node_type, NodeLevel node_level)
	{
		if(node_type == Node.Factory || node_type == Node.Junkyard || node_type == Node.Outpost)
		{
			GameObject new_entity = instantiateEntityNode(x, z, node_type);
			entityNodeS new_node = (entityNodeS) new_entity.AddComponent("entityNodeS");
			
			if(new_node == null)
				throw new System.Exception("Node Entity not created properly D:");
				
			new_node.x = x;
			new_node.z = z;
			new_node.node_type  = node_type;
			new_node.node_level = node_level;
			resource_node_list.Add(new_node);
			return true;
		}
		else
		{
			throw new System.Exception("Trying to create a resource node, but passing in an invalid EntityE type! D:");
		}
	}
	
	//Make spawn points, ASSUMPTION: assumes atleast 5 resouce nodes in existence that aren't 
	//close to base or mech starting positions
	public static bool initEnemySpawnPoints(){
		//After resource nodes, mech and base are made grab resource node positions and choose 5 spawn spots
		int distance_from_other_entities = 5;
		int number_of_resource_nodes_needed_on_map = 6;
		spawn_points = new List<entityNodeS>(); //always reset list if this method is called
		
		if(resource_node_list.Count < number_of_resource_nodes_needed_on_map){
			Debug.Log ("Not enough resource nodes on map");
			return false;
		}
		
		foreach(entityNodeS r_node in resource_node_list){
			//only choose spawn spots that are atleast "distance_from_other_entities" hexes away from base and mech start position
			//then make spawn spot on the resource node, plus check that an enemy isn't already on the spawn spot 
			if(!entity_in_sight(r_node.x, r_node.z, distance_from_other_entities) && getEnemyAt(r_node.x, r_node.z) == null){
				spawn_points.Add(r_node);
			}
		}
		
		if(spawn_points.Count == 0){
			Debug.Log ("Couldn't make any spawn points, either all hexes occupied or all nodes are close to mech and base");
			return false;
		}
			
		return true;
	}
	
	private static bool entity_in_sight(int x_r, int z_r, int distance_from_entities){
		Debug.Log("Resource node " + x_r + " | " + z_r);  
			foreach(HexData h in hexManagerS.getAdjacentHexes(x_r, z_r, distance_from_entities)){
				if((h.x == mech_s.x && h.z == mech_s.z) || (h.x == base_s.x && h.z == base_s.z))
				{
					//entity mech or base is too close to resource node
					Debug.Log ("entity mech or base is too close to resource node, sight_range = " + distance_from_entities);
					return true;
				}
			}  
		Debug.Log ("no entities close to resouce node");
		return false;
	}
	
	
	public static bool Make_A_New_Enemy_At_Spawn_Point(){ 
		int enemies_on_board = entityManagerS.getEnemies().Count;
		int enemy_quota = 5; //if number of enemies on board is lower than this number, more enemies need to be made
		bool sp_made = initEnemySpawnPoints(); //get valid spawn points
		
		if(!sp_made){
			Debug.Log ("Get Spawn Points failed");
			return sp_made;
		}
		
		//Check the quota of enemies to see if we can make more enemies
		if (enemies_on_board <= enemy_quota){
			//Have a chance algorithm for what enemy should spawn
			int r = UnityEngine.Random.Range(0,20);
			int r1 = UnityEngine.Random.Range(0,spawn_points.Count);
			if(r < 4){
				//Initialize new enemy at one point in List of Spawn Points(r1)
				bool check = entityManagerS.instantiateEnemy(spawn_points[r1].x, spawn_points[r1].z, true, true);	
				Debug.Log("make_new_enemy: " + check + " , x= " + spawn_points[r1].x + ", z=" + spawn_points[r1].z);
				return check;
			}else if(r < 8){
				//Initialize new enemy at one point in List of Spawn Points(r1)
				bool check = entityManagerS.instantiateEnemy(spawn_points[r1].x, spawn_points[r1].z, true, false);	
				Debug.Log("make_new_enemy: " + check + " , x= " + spawn_points[r1].x + ", z=" + spawn_points[r1].z);
				return check;
			}else if(r < 12){
				//Initialize new enemy at one point in List of Spawn Points(r1)
				bool check = entityManagerS.instantiateEnemy(spawn_points[r1].x, spawn_points[r1].z, false, true);	
				Debug.Log("make_new_enemy: " + check + " , x= " + spawn_points[r1].x + ", z=" + spawn_points[r1].z);
				return check;
			}else if(r < 16){
				//Initialize new enemy at one point in List of Spawn Points(r1)
				bool check = entityManagerS.instantiateEnemy(spawn_points[r1].x, spawn_points[r1].z, false, false);	
				Debug.Log("make_new_enemy: " + check + " , x= " + spawn_points[r1].x + ", z=" + spawn_points[r1].z);
				return check;
			}else{
				Debug.Log("enemy didn't spawn this time around, 20% chance this will happen");
				return false;
			}
		
		}else{
			Debug.Log("There are enough enemies on board, won't spawn more");
			return false;
		}
	}
	
	//converts engine coordinates into 3D space cordinates
	public static Vector3 CoordsGameTo3DEntiy(int x, int z)
	{  
		return new Vector3(x * 2.30024F + z * -0.841947F, 5, z * 1.81415F + x * 1.3280592F + .5F);
	}
	

}
	
	
	*/