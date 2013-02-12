using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityManagerS : MonoBehaviour {
	
	//Using dummy values for testing
	private static entityBaseS base_s;
	private static entityMechS mech_s;
	private static List<entityEnemyS> enemy_list;
	private static List<entityNodeS> resource_node_list;
	
	// Use this for initialization
	void Start () { 
		base_s              = gameObject.GetComponent<entityBaseS>();
	    mech_s  	 	    = gameObject.GetComponent<entityMechS>();
		enemy_list 			= new List<entityEnemyS>();
		resource_node_list  = new List<entityNodeS>(); 
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
	
	
	//check to see if given entity resides on hex 
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
				
			case EntityE.Factory:
			case EntityE.Junkyard:
			case EntityE.Outpost:
				foreach(entityNodeS node in resource_node_list)
					if(hex_x == node.x && hex_z == node.z)
						return true;	
				return false;
				
			case EntityE.Enemy:
				foreach(entityEnemyS enemy in enemy_list)
					if(hex_x == enemy.x && hex_z == enemy.z)
						return true;	
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
}