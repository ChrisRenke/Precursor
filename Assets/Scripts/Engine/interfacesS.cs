using UnityEngine;
using System.Collections;

//General Interfaces for Entities
//*******************************


//For entites that move
public interface IMove {
	
	//Get traversable hexes around entity
	hexManagerS.HexData[] getTraversableHexes();
	
	//Get untraversable hexes around entity
	hexManagerS.HexData[] getUntraversableHexes();
	
	//Check whether a hex is traversable
	bool isTraversable(hexManagerS.HexData hex);
	
	//Move entity and update hex manager
	void makeMove(hexManagerS.HexData hex);
	
}

//For entities who can heal damage
public interface IHeal {
	
	void healDamage(int health_regained);
		
}

//For entities that damage dealt and recieve damage in battle 
public interface IAttack {
	
	void dealDamage(Transform opponent_position, int damage, int splash_range); //maybe change it to opponents hex instead of transform
	void recieveDamage(int damage_recieved);
	hexManagerS.HexData[] getAttackableHexes();
}

//For damage dealt and recieved in battle 
public interface ICollect {
	
	void collectResources(int item, Transform resource_position); //may change it to resource hex instead of transform
	hexManagerS.HexData[] getCollectableHexes();
	bool canCollectResources();
	int getResourceCount();
	void consumeResource(); //used for removing items during healing and upgrades?
}


//General Interfaces Other
//*******************************