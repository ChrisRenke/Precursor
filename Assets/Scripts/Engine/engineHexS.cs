using UnityEngine;
using System.Collections;

public class engineHexS : MonoBehaviour {
	 
	public  int x, z;
	public  Hex hex_type;
	
	public void buildHexData(int _x, int _z, Hex _type)
	{
		hex_type = _type;
		x = _x;
		z = _z; 
	}
	 
}
