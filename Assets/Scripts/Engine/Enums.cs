using UnityEngine;
using System.Collections;

public enum Part	     { Gear, Piston, Strut, Plate };
public enum Hex  	     { Desert, Farmland, Forest, Grass, Hills, Marsh, Mountain, Water, Perimeter };
public enum EntityE      { None, Player, Base, Enemy, Node, NotCheckedYet };
public enum PlayerStates { Idle, Walking, Scavenging, Attacking, Upgrading };
public enum Facing       { North, NorthEast, SouthEast, South, SouthWest, NorthWest };
public enum NodeLevel    { Empty, Sparse, Full  };
public enum Node         { Factory, Junkyard, Outpost}
public enum Turn         { Player, Enemy, Base };
public enum SelectLevel  { Disabled, Easy, Medium, Hard, Scavenge, Attack, Upgrade };
public enum Action       { Repair, UpgradeMech, UpgradeBase, Scavenge, Attack, Traverse, End };
public enum Vision       { Live, Visited, Unvisted };


public struct HexData{ 
	
	public readonly int 	x;   	  //level x coord (NE / SW)
	public readonly int 	z;  	  //level z coord  (N / S)
	public readonly Hex 	hex_type; //enviroment type of this hex
	public Facing   		direction_from_central_hex;
	public EntityE  		added_occupier;
	public Vision   		vision_state;
	public int      		traversal_cost;
	
	public HexData(int _x, int _z, Hex _type){
		x = _x;
		z = _z; 
		hex_type = _type;
		direction_from_central_hex = Facing.South; //this is a temp value, should be replaced manually usually
		added_occupier = EntityE.NotCheckedYet;
		traversal_cost = -1;
		vision_state   				= Vision.Live;
	}
	
	public HexData(int _x, int _z, Hex _type, Vision _vision){
		x 							= _x;
		z 							= _z; 
		hex_type 					= _type;
		direction_from_central_hex  = Facing.South; //this is a temp value, should be replaced manually usually
		added_occupier 				= EntityE.NotCheckedYet;
		traversal_cost 				= -1;
		vision_state   				= _vision;
	}
} 

public struct NodeData{
	
	public readonly int 	x;   	  //level x coord (NE / SW)
	public readonly int 	z;  	  //level z coord  (N / S) 
	public readonly Node     node_type;
	public readonly NodeLevel node_level;
	
	public NodeData(int _x, int _z, Node _type, NodeLevel _lvl){
		x = _x;
		z = _z; 
		node_type = _type;
		node_level = _lvl;
	}
}
//
//public struct EntityData{
//	public readonly int 	x;   	  //level x coord (NE / SW)
//	public readonly int 	z;  	  //level z coord  (N / S)
//	public readonly EntityE	entity_type; //entity type of this hex
//	
//	
//}