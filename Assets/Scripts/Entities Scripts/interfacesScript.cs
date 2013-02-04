using UnityEngine;
using System.Collections;

//General Interfaces for Entities
//*******************************

//For entites that move
public interface IMove {
	
	//Get traversable hexes around entity
	hexManagerScript.HexData[] getTraversableHexes();
	
	//Get untraversable hexes around entity
	hexManagerScript.HexData[] getUntraversableHexes();
	
	//Check whether a hex is traversable
	bool isTraversable(hexManagerScript.HexData hex);
	
	//Move entity and update hex manager
	void makeMove(hexManagerScript.HexData hex);
	
}

//For entities who can heal damage
public interface IHeal {
	
	void healDamage(int health_regained);
		
}

//For entities that damage dealt and recieve damage in battle 
public interface IAttack {
	
	void dealDamage(Transform opponent_position, int damage, int splash_range); //maybe change it to opponents hex instead of transform
	void recieveDamage(int damage_recieved);
	hexManagerScript.HexData[] getAttackableHexes();
}

//For damage dealt and recieved in battle 
public interface ICollect {
	
	void collectResources(int item, Transform resource_position); //may change it to resource hex instead of transform
	hexManagerScript.HexData[] getCollectableHexes();
	bool canCollectResources();
	int getResourceCount();
	void consumeResource(); //used for removing items during healing and upgrades?
}


//General Interfaces Other
//*******************************