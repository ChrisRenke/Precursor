using UnityEngine;
using System.Collections;

public enum Part	     { Gear, Piston, Strut, Plate };
public enum Hex  	     { Desert, Farmland, Forest, Grass, Hills, Marsh, Mountain, Water, Perimeter };
public enum EntityE      { None, Player, Base, Enemy, Flyer, Node, Spawn, NotCheckedYet };
public enum PlayerStates { Idle, Walking, Scavenging, Attacking, Upgrading };
public enum Facing       { North, NorthEast, SouthEast, South, SouthWest, NorthWest };
public enum NodeLevel    { Empty, Sparse, Full };
public enum Node         { Factory, Junkyard, Outpost}
public enum Turn         { Player, Enemy, Base };
public enum SelectLevel  { Disabled, Easy, Medium, Hard, Scavenge, Attack, Town, TownUpgrade, MechUpgrade, TownRecall, Move, AttempScavenge};
public enum Action       { Repair, UpgradeMech, UpgradeBase, Scavenge, Attack, Traverse, End };
public enum Vision       { Live, Visited, Unvisted }; 
public enum MouseState   { Idle, Over, Click } 
public enum editor_entity { Mech, Enemy, Flyer, Town, Spawn, Factory, Outpost, Junkyard };
public enum Level        {Level0,Level1,Level2} ;


public enum MechUpgradeMode        { Movement, Combat, Scavenge, Utility };
public enum BaseUpgradeMode        { Walls, Armament, Structure, Utility };  
public enum BaseUpgradeLevel      { Level0, Level1, Level2, Level3}

public enum UpgradeState           { Acquired, NotAcquired, Disabled };

public enum MechUpgrade { Move_Water, Move_Mountain, Move_Marsh, Move_Legs, Combat_Damage, Combat_Cost, Combat_Range, Combat_Armor, Combat_Dodge, 
		Scavenge_Combat, Scavenge_Greed, Scavenge_Empty, Scavenge_Cost, Util_Recall, Util_Vision, Util_AP, Util_Parts, Util_Idle, DEBUG_NONE };

 
public enum UpgradeCostFeedback { NeedMoreAP, NeedMoreParts, Disabled, Success }
public enum UpgradeMenu   { Base, Mech }


//public enum BaseUpgrade { Level0, Level1, Level2, Level3}
//public enum BaseCategories  { Structure, Walls, Defense };  
//public enum Menu  {Default , Objective, BaseUpgrade, MechUpgrade, HealthUpgrade, TransportUpgrade};  
public enum Menu  {Default , Objective, BaseUpgrade1, BaseUpgrade2, BaseUpgrade3, MechUpgrade1, MechUpgrade2, MechUpgrade3, HealthUpgrade}; 
  

//public struct Orientation{
//	public static readonly Vector3 North     = new Vector3(-12.5F, 261.25F, 306.5F);
//	public static readonly Vector3 NorthEast = new Vector3(35, 300, 315);
//	public static readonly Vector3 SouthEast = new Vector3(52, 25, 20);
//	public static readonly Vector3 South     = new Vector3(12.5F, 81.25F, 53.5F);
//	public static readonly Vector3 SouthWest = new Vector3(-35, 120, 45);
//	public static readonly Vector3 NorthWest = new Vector3(-52, 205, 340); 
////	public static readonly Quaternion North     = new Vector3(-12.5F, 261.25F, 306.5F, 1F);
////	public static readonly Quaternion NorthEast = new Vector3(35, 300, 315, 1F);
////	public static readonly Quaternion SouthEast = new Vector3(52, 25, 20, 1F);
////	public static readonly Quaternion South     = new Vector3(12.5F, 81.25F, 53.5F, 1F);
////	public static readonly Quaternion SouthWest = new Vector3(-35, 120, 45, 1F);
////	public static readonly Quaternion NorthWest = new Vector3(-52, 205, 340, 1F);
//	
//	public static Vector3 facingOrientation(Facing facing_direction)
//	{
//		switch(facing_direction)
//		{
//			case Facing.North: 		return North;
//			case Facing.NorthEast: 	return NorthEast;
//			case Facing.SouthEast: 	return SouthEast;
//			case Facing.South: 		return South;
//			case Facing.SouthWest: 	return SouthWest;
//			case Facing.NorthWest: 	return NorthWest;
//			default: throw new System.Exception("wtf, facingOrientation issue");
//		}	
//	}
//	
//}	



	
public struct HexData{ 
	public readonly int 		x;   	  //level x coord (NE / SW)
	public readonly int 		z;  	  //level z coord  (N / S)
	public readonly Hex 		hex_type; //enviroment type of this hex
	public readonly GameObject hex_object;
	public readonly engineHexS hex_script;
	public Facing   			direction_from_central_hex;
	public EntityE  			added_occupier;
	public bool					is_node_here;
	public Vision   			vision_state;
	public int      			traversal_cost;
	
	
	public HexData(int _x, int _z, Hex _type, GameObject _hex_object, engineHexS _hex_script){
		x = _x;
		z = _z; 
		hex_type = _type;
		direction_from_central_hex = Facing.South; //this is a temp value, should be replaced manually usually
		added_occupier = EntityE.NotCheckedYet;
		traversal_cost = -1;
		hex_object 	   = _hex_object;
		vision_state   = Vision.Unvisted;
		hex_script     = _hex_script;
		is_node_here = false;
	}
	
	public HexData(int _x, int _z, Hex _type, GameObject _hex_object, engineHexS _hex_script, Vision _vision){
		x 							= _x;
		z 							= _z; 
		hex_type 					= _type;
		hex_object 	   				= _hex_object;
		vision_state   				= _vision;
		direction_from_central_hex  = Facing.South; //this is a temp value, should be replaced manually usually
		added_occupier 				= EntityE.NotCheckedYet;
		traversal_cost 				= -1;
		hex_script     = _hex_script;
		is_node_here = false;
	}
	
	public HexData(int _x, int _z, bool THIS_METHOD_IS_FOR_DUMMY_PERIMETER_HEXES_ONLY){
		x = _x;
		z = _z; 
		hex_type = Hex.Perimeter;
		direction_from_central_hex = Facing.South; 
		added_occupier = EntityE.NotCheckedYet;
		traversal_cost = -1;
		hex_object 	   = null;
		vision_state   = Vision.Unvisted;
		hex_script     = null;
		is_node_here = false;
	}
	
	public bool Equals(object obj) 
	{
		if (!(obj is HexData))
			return false;
		
		HexData other_hex = (HexData) obj;
		return this.x == other_hex.x && this.z == other_hex.z; 
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

public struct UpgradeEntry{ 
	public readonly string 	title;
	public readonly string 	description;  
	public readonly	int		gear_cost;
	public readonly int		piston_cost;
	public readonly int		plate_cost;
	public readonly int		strut_cost;
	public readonly int		ap_cost;
	public readonly Texture thumbnail; 
	public readonly MechUpgrade upgrade_type;   
	
	public BaseUpgradeLevel base_level; 
	
	public UpgradeEntry(string title, string description, int gear_cost, int piston_cost, int plate_cost, int strut_cost, int ap_cost, Texture thumbnail, MechUpgrade upgrade_type)
	{
		this.title = title;
		this.description = description; 
		this.gear_cost = gear_cost;
		this.piston_cost = piston_cost;
		this.plate_cost = plate_cost;
		this.strut_cost = strut_cost;
		this.ap_cost = ap_cost;
		this.thumbnail = thumbnail; 
		this.upgrade_type = upgrade_type;
		base_level = BaseUpgradeLevel.Level0;
	}
	
	public UpgradeEntry(string title, string description, int gear_cost, int piston_cost, int plate_cost, int strut_cost, int ap_cost, Texture thumbnail)
	{
		this.title = title;
		this.description = description; 
		this.gear_cost = gear_cost;
		this.piston_cost = piston_cost;
		this.plate_cost = plate_cost;
		this.strut_cost = strut_cost;
		this.ap_cost = ap_cost;
		this.thumbnail = thumbnail; 
		this.upgrade_type = MechUpgrade.DEBUG_NONE;
		base_level = BaseUpgradeLevel.Level0; 
	}
}