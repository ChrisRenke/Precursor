using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {
		
	public gameManagerS  gm;
	public enginePlayerS ep;
	public entityManagerS em; 
	public hexManagerS hm; 
	
	void Awake(){ 
		gm = GameObject.Find("engineGameManager").GetComponent<gameManagerS>();
		ep = GameObject.Find("enginePlayer").GetComponent<enginePlayerS>();
		em = GameObject.Find("engineEntityManager").GetComponent<entityManagerS>();
		hm = GameObject.Find("engineHexManager").GetComponent<hexManagerS>();
	}
	public int x;
	public int z;
	
	public void setLocation(int _x, int _z){
		x = _x;
		z = _z;
	}
	
	public void updateFoWState()
	{
		HexData occupying_hex = hm.getHex(x, z);
		switch(occupying_hex.vision_state)
		{
		case Vision.Live:
			gameObject.renderer.enabled = true;
			renderer.material.SetColor("_Color", Color.white);
			break;
		case Vision.Visited:
			gameObject.renderer.enabled = true;
			renderer.material.SetColor("_Color", Color.gray);
			break;
		case Vision.Unvisted:
			gameObject.renderer.enabled = false;
			break;
		default:
			throw new System.Exception("update FoW Combatable error!!");
		}
	}
	
}


public abstract class  Combatable : Entity{
	
	public Facing facing_direction;
	public int current_hp;
	public int max_hp;
	
	public int base_armor = 0;
	
	public int current_ap;
	public int max_ap;
	
	public int attack_cost   = 5;
	public int attack_range  = 2;
	public int attack_damage = 5;
	
	public int sight_range = 3;
	
	public int getCurrentHP(){
		return current_hp;
	}
	
	public int getCurrentAP(){
		return current_ap;
	}
	
	public int getMaxHP(){
		return max_hp;
	}
	
	public int getMaxAP(){
		return max_ap;
	} 
	
	
	public Facing getFacingDirection(){
		return facing_direction;
	}
	public void setFacingDirection(Facing in_direction){
		facing_direction = in_direction;
	}
	
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
//	 */
//	public int attackHex(int att_x, int att_z)
//	{
//		Debug.LogWarning("ABOUT TO ATTCK ENTITY ON - "+ att_x + "," + att_z);
//		Combatable target = em.getCombatableAt(att_x, att_z);
//		
//		Debug.LogWarning("ABOUT TO ATTCK ENTITY "+ target.GetInstanceID());
//		if(target != null)
//			return target.acceptDamage(attack_damage);
//		
//		return 0; //nothing to damage if we get here			
//	}
//	
	/**
	 *	Get the percentage of hp remaining
	 *  @return  remaining ratio of hp current to total
	 */
	public float gethpPercent()
	{
		return (float) current_hp / max_hp;
	}
	
	/**
	 *	Entity recieves damage, ignoring  armor, 
	 *  this entity will always recieve at least one damage from this method
	 *  @return  remaining hp, floor of 0 returned
	 */
	public int acceptDamage(int damage, bool ignore_armor)
	{
		int adjusted_damage = ignore_armor ? damage : damage - base_armor;
		adjusted_damage = adjusted_damage > 0 ? adjusted_damage : 1;
		current_hp -= adjusted_damage;
		
		int hp_remaning = current_hp; 
		if(checkIfDead()){
            onDeath();
        }else{
            em.createShotEffect(x,z);
        }
			
		return hp_remaning;
	}
	
	/**
	 *	Entity recieves damage, method accounts for armor, 
	 *  this entity will always recieve at least one damage from this method
	 *  @return  remaining hp, floor of 0 returned
	 */
	public int acceptDamage(int damage)
	{
		return acceptDamage(damage, false);
	}
	
	/**
	 *	Entity heals hp, cannot exceed max HP 
	 *  @return  remaining hp, floor of 0 returned
	 */
	public int healhp(int amount_to_heal)
	{ 
		em.createHealEffect(x,z);
		current_hp += amount_to_heal;
		current_hp = current_hp > max_hp ? max_hp : current_hp;
		return current_hp;
	}
	
	public int setCurrentHPviaPercentage(float hp_percentage)
	{
		current_hp = (int) (max_hp * (hp_percentage / 100F));
		return current_hp;
	}
	
	public bool checkIfDead()
	{
		return current_hp <= 0;
	}
	
	
	//retrurn true if game should end as a result
	public abstract bool onDeath();

	
}