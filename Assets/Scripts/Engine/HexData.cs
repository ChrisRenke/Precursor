using UnityEngine;
using System.Collections;

public class HexData{ 
	
	public readonly int 	x;   	  //level x coord (NE / SW)
	public readonly int 	z;  	  //level z coord  (N / S)
	public readonly Hex 	hex_type; //enviroment type of this hex
	
	public HexData(int _x, int _z, Hex _type){
		x = _x;
		z = _z; 
		hex_type = _type;
	}
	
	public HexData(HexData hex){
		x = hex.x;
		z = hex.z; 
		hex_type = hex.hex_type;
	}
} 
