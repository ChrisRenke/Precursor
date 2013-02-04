using UnityEngine;

using System.Collections;

using System.Collections.Generic;

//
public class hexManagerS : MonoBehaviour {

	public enum Tiles {Grass, Desert, Forest, Farmland, Marsh, Snow, Mountain, Hills, Water, Settlement, Factory, Outpost, Junkyard, EditorTileA, EditorTileB};

	public enum Entities {Player, Enemy, Settlement, ResourceNode};

	public static int[,] hexes;

	public TextAsset     level_file;
	
	//EXAMPLE
	//checkTile
	//if(hex.type == HexManager.Tiles.Mountain || hex.type = HexManager.Tiles.Water)

	//return false;

	//checkTile(Tiles.Desert);

	


	void Start(){

		Load();

	}

	private void Load(){

		//	= new int[200,200]()

	}

	//Update is called once per frame
	void Update () {

	}
	
	//NEW FUNCTIONS ***
	
	//Return adjacent hexes for the given entity position ***
	public static HexData[] getAdjacent(Vector3 pos){
		//TODO: should return the six adjacent tiles relative to the players position for now it's returning a dummy array of HexData
		return new HexData[] {new HexData(-3, 1, Tiles.Water), new HexData(3, 1, Tiles.Mountain), new HexData(2, 1, Tiles.Settlement),
								new HexData(-4, 1, Tiles.Forest), new HexData(4, 1, Tiles.Factory), new HexData(5, 1, Tiles.Grass)};
	}
	
	//Update hexManager:: maybe this should be in the Update function???***
	public static void updateMap(HexData new_hex_pos, HexData old_hex_pos){
		//TODO: May or may not need this method but for now this method isn't implemented 
		//But 1) hex in actual hex manager needs to be updated as well as old hex position occupied by entity and 2) entity array needs to be updated and whatever else
		//SO: hexManager.find(new_hex_pos).occupied = true & hexManager.find(old_hex_pos).occupied = false; ...etc
	}
	

	public class HexData{

		public int	x; 
		public int	z;  
		public Tiles hex_type; 
		public GameObject occupier; //added: should be a GameObject? ***
		
		public HexData(int _x, int _z, Tiles _type){
			x = _x;
			z = _z; 
			hex_type = _type;
		}
		
		public HexData(int _x, int _z, Tiles _type, GameObject _occupier){
			x 		 = _x;
			z 		 = _z; 
			hex_type = _type;
			occupier = _occupier;
		}
		
		public bool isOccupied()
		{
			return occupier != null;
		}

	}

}



