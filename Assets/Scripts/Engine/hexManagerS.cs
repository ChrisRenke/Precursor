using UnityEngine; 
using System.Collections; 
using System.Collections.Generic;


public class hexManagerS : MonoBehaviour {

	
		/*
		  		north 
      northwest  ___   northeast
                /    \
      southwest \___/  southeast
				south
		*/
 
	private static HexData[,] hexes; 
	
	private static int row, col = 0; //size of hex array, used for out of bounds checking
	

	void Start(){

		Load();

	}

	private void Load(){
		//TODO: Port Load code from editorIOS
		
		//Dummy 2D array for testing ***
		hexes = new HexData[,] {{new HexData(6, 0, Hex.Perimeter), new HexData(6, 1, Hex.Perimeter), new HexData(6, 2, Hex.Perimeter), new HexData(6, 3, Hex.Perimeter), new HexData(6, 4, Hex.Perimeter), new HexData(6, 5, Hex.Perimeter), new HexData(6,6 , Hex.Perimeter)},
								{new HexData(5, 0, Hex.Perimeter), new HexData(5, 1, Hex.Mountain), new HexData(5, 2, Hex.Grass), new HexData(5, 3, Hex.Farmland), new HexData(5, 4, Hex.Grass), new HexData(5, 5, Hex.Grass), new HexData(5, 6, Hex.Perimeter)},
								{new HexData(4, 0, Hex.Perimeter), new HexData(4, 1, Hex.Grass), new HexData(4, 2, Hex.Water), new HexData(4, 3, Hex.Forest), new HexData(4, 4, Hex.Grass), new HexData(4, 5, Hex.Forest), new HexData(4, 6, Hex.Perimeter)},
								{new HexData(3, 0, Hex.Perimeter), new HexData(3, 1, Hex.Grass), new HexData(3, 2, Hex.Forest), new HexData(3, 3, Hex.Grass), new HexData(3, 4, Hex.Junkyard), new HexData(3, 5, Hex.Grass), new HexData(3, 6, Hex.Perimeter)},
								{new HexData(2, 0, Hex.Perimeter), new HexData(2, 1, Hex.Grass), new HexData(2, 2, Hex.Mountain), new HexData(2, 3, Hex.Snow), new HexData(2, 4, Hex.Grass), new HexData(2, 5, Hex.Grass), new HexData(2, 6, Hex.Perimeter)},
								{new HexData(1, 0, Hex.Perimeter), new HexData(1, 1, Hex.Grass), new HexData(1, 2, Hex.Grass), new HexData(1, 3, Hex.Water), new HexData(1, 4, Hex.Grass), new HexData(1, 5, Hex.Water), new HexData(1, 6, Hex.Perimeter)},
								{new HexData(0, 0, Hex.Perimeter), new HexData(0, 1, Hex.Perimeter), new HexData(0, 2, Hex.Perimeter), new HexData(0, 3, Hex.Perimeter), new HexData(0, 4, Hex.Perimeter), new HexData(0, 5, Hex.Perimeter), new HexData(0, 6, Hex.Perimeter)}};
		row = hexes.GetLength(0);
		col = hexes.GetLength(1);
	}
	

	//Return adjacent hexes for the given entity position
	public static HexData[] getAdjacentHexes(int x, int z){
		
		//Set values to some invalid hex
		HexData n   = new HexData(0,0,Hex.EditorTileB);
		HexData n_e = new HexData(0,0,Hex.EditorTileB);
		HexData s_e = new HexData(0,0,Hex.EditorTileB);
		HexData s   = new HexData(0,0,Hex.EditorTileB);
		HexData s_w = new HexData(0,0,Hex.EditorTileB);
		HexData n_w = new HexData(0,0,Hex.EditorTileB);
		
		if(z+1 < col && z+1 >= 0){
			//Get North
			n = hexes[x, z +1];
			if(x-1 < row && x-1 >= 0){
				//Get Northwest
				n_w = hexes[x-1, z+1];
			}
		}
		
		if(x-1 < row && x-1 >= 0){
			//Get Southwest
			s_w = hexes[x-1, z];
		}
		
		if(z-1 < col && z-1 >= 0){
			//Get South
			s = hexes[x, z-1];
			if(x+1 < row && x+1 >= 0){
				//Get Southeast
				s_e = hexes[x+1, z-1];
			}
		}
		
		if(x+1 < row && x+1 >= 0){
			//Get Northeast
			n_e = hexes[x+1, z];
		}		
		
		return new HexData[] {n, n_e, s_e,
								s, s_w,n_w};
	}
	
	//Get hex at given position in the map
	public static HexData getHex(int r, int c){
		if(r < row && r >= 0 && c < col && c >= 0){
			return hexes[r, c];
		}else{ //return dummy hex
			return new HexData(0,0,Hex.EditorTileB);
		}
	}
	 
}


 
public class HexData{ 
	
	public readonly int 	x;   	  //level x coord (NE / SW)
	public readonly int 	z;  	  //level z coord  (N / S)
	public readonly Hex 	hex_type; //enviroment type of this hex
	
	public HexData(int _x, int _z, Hex _type){
		x = _x;
		z = _z; 
		hex_type = _type;
	}
} 

