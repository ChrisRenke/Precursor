using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexManager : MonoBehaviour {
	
	
	public enum Tiles {Grass, Desert, Forest, Farmland, Marsh, Snow, Mountain, Hills, Water, Settlement, Factory, Outpost, Junkyard, EditorTileA, EditorTileB};
	public enum Entities {Player, Enemy, Settlement, ResourceNode};
	
	public static int[,] hexes;
	public TextAsset     level_file;
	
	//if(hex.type == HexManager.Tiles.Mountain || hex.type = HexManager.Tiles.Water)
		//return false;
	//checkTile(Tiles.Desert);
	 
	void Start()
	{
		Load();
	}
	
	private void Load()
	{
		
//		= new int[200,200]()
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private class HexData{
		public int		x;
		public int		y;
		public Tiles	tile_type;
		
		public HexData(int _x, int _y, Tiles _type)
		{
			x = _x;
			y = _y;
			tile_type = _type;
		}
	}
}
