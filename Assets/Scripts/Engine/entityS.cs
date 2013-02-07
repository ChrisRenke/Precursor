using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
	
	public int x;
	public int y;
}

public abstract class  Combatable : Entity{
	
	public int current_hitpoints;
	public int max_hitpoints;
	public int armor;
	
	
	/**
	 *	Deal damage to the target
	 *  @param   the Combatable entity to damage
	 *  @return  damage delt
	 */
	public abstract int attackTarget(Combatable target);
	
	/**
	 *	Deal damage to whatever occupies the hex at cooridnates x, z
	 *  @param   x - x coord
	 *  @param   z - z coord
	 *  @return  damage delt
	 */
	public abstract int attackHex(int x, int z);
	
	/**
	 *	Get the percentage of hitpoints remaining
	 *  @return  remaining ratio of hitpoints current to total
	 */
	public float getHitpointsPercent()
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