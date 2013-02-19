using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityManagerS : MonoBehaviour {
	 
	//Using dummy values for testing
	public static entityBaseS base_s;
	public static entityMechS mech_s;
	public static List<entityEnemyS> enemy_list;
	public static List<entityNodeS> resource_node_list;
	
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
				if(hex_x == mech_s.x && mech_s.z == hex_z)
					return true;
				return false;
				
			case EntityE.Base:
				if(hex_x == base_s.x && base_s.z == hex_z)
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
		mech_s = (entityMechS) new_entity.AddComponent("entityMechS");
		
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
		new_enemy_s.current_ap = 6;
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
	
	//converts engine coordinates into 3D space cordinates
	public static Vector3 CoordsGameTo3DEntiy(int x, int z)
	{  
		return new Vector3(x * 2.30024F + z * -0.841947F, 5, z * 1.81415F + x * 1.3280592F + .5F);
	}
	

}
	