using UnityEngine;
using System.Collections;

public enum Part	     { Cog, Piston, Strut, Plate };
public enum Hex  	     { Grass, Desert, Forest, Farmland, Marsh, Snow, Mountain, Hills, Water, Perimeter };
public enum EntityE      { Player, Base, Enemy, Junkyard, Outpost, Factory };
public enum PlayerStates { Idle, Walking, Scavenging, Attacking, Upgrading };
public enum Facing       { North, NorthEast, SouthEast, South, SouthWest, NorthWest };
public enum NodeLevel    { Full, Sparse, Empty };
public enum Turn         { Player, Enemy, Base};
public enum SelectLevel  { Disabled, Easy, Medium, Hard }


public struct HexData{ 
	
	public readonly int 	x;   	  //level x coord (NE / SW)
	public readonly int 	z;  	  //level z coord  (N / S)
	public readonly Hex 	hex_type; //enviroment type of this hex
	public Facing   direction_from_central_hex;
	
	public HexData(int _x, int _z, Hex _type){
		x = _x;
		z = _z; 
		hex_type = _type;
		direction_from_central_hex = Facing.South; //this is a temp value, should be replaced manually usually
	}
} 
