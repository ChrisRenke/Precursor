using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//General Interfaces for Entities
//*******************************


//For entites that move
public interface IMove {
	
	//Get traversable hexes around entity
	List<HexData> getAdjacentTraversableHexes(HexData hex);
	
	//Get untraversable hexes around entity
	List<HexData> getAdjacentUntraversableHexes(HexData hex);
	
	//Check whether a hex can be traversed
	bool canTraverse(HexData hex);
	
	//Move entity to a give hex
	void makeMove(HexData hex);
	
	//
	/*int getCost(method cost){
		if blank... ConnectionTester, Status is two
			mountian = 2222
				forest mrsh....333
				water...9999
				
	}*/
	
}


//For damage dealt and recieved in battle 
public interface ICollect {
	
	/**
	 * add a part to the players invetory
	 * @return  success of operation, false if player has no room left for parts
	 */
	bool 				collectPart(Part collected_part);   
	
	/**
	 * get a count of each type of part the player has
	 * @return  success of operation, false if part does not exist and can't be consumed
	 */
	Dictionary<Part, int>	getResourceCount();
	
	/**
	 * remove a part from player inventory
	 * @return  success of operation, false if part does not exist and can't be consumed
	 */
	bool consumePart(Part part_consumed); //used for removing items during healing and upgrades?
}


//General Interfaces Other
//*******************************


//methods needed for A*
public interface IPathFind{
	//Return distance between two adjacent nodes
	double calcDistance(HexData hex_start, HexData hex_end);
	

    //Return estimated distance between any node and destination node
    double calcEstimate(HexData hex_start, HexData hex_end);
	
	//Secondary version of traversable hexes(in IMove), accounts for a destination hex
	List<HexData> getAdjacentTraversableHexes (HexData hex, HexData destination);
}