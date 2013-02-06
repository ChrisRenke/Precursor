using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
	
	public int x;
	public int y;
}

public class Node : Entity {
	
	
}

public class Damageable : Entity{
	
	public int current_hitpoints;
	public int max_hitpoints;
	public int armor;
	
	public float getHPPercent()
	{
		return (float) current_hitpoints / max_hitpoints;
	}
	
	/**
	 *	Entity recieves damage, ignoring  armor, 
	 *  this entity will always recieve at least one damage from this method
	 *  @return  remaining hitpoints, floor of 0 returned
	 */
	public int acceptDamage(int damage, bool ignore_armor)
	{
		int adjusted_damage = ignore_armor ? damage : damage - armor;
		current_hitpoints -= adjusted_damage > 0 ? adjusted_damage : 1;
		return current_hitpoints;
	}
	
	
	/**
	 *	Entity recieves damage, method accounts for armor, 
	 *  this entity will always recieve at least one damage from this method
	 *  @return  remaining hitpoints, floor of 0 returned
	 */
	public int acceptDamage(int damage)
	{
		return acceptDamage(damage, false);
	}
	
	
	/**
	 *	Entity heals hitpoints, cannot exceed max HP 
	 *  @return  remaining hitpoints, floor of 0 returned
	 */
	public int healHitpoints(int amount_to_heal)
	{
		current_hitpoints += amount_to_heal;
		current_hitpoints = current_hitpoints > max_hitpoints ? max_hitpoints : current_hitpoints;
		return current_hitpoints;
	}
}