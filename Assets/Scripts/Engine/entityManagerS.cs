using UnityEngine;
using System.Collections;

public class entityManagerS : MonoBehaviour {
	
	//Using dummy values for testing
	public static HexData player_pos = new HexData(4, 1, Hex.Grass);
	
	public static HexData[] settlement_pos = new HexData[]{new HexData(5, 5, Hex.Grass)};
	
	public static HexData[] enemy_pos = new HexData[]{new HexData(1, 2, Hex.Grass)};
	
	public static HexData[] resource_pos = new HexData[]{new HexData(2, 5, Hex.Grass)};
	
	// Use this for initialization
	void Start () {
		//TODO: initialize Entity positions here
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//check to see if given entity resides on hex 
	public static bool isEntityPos(HexData hex, Entities entity){
				
		if(entity == Entities.Player){
			if(hex.x == player_pos.x && hex.z == player_pos.z){
				return true; //player mech resides on this hex
			}else{
				return false;
			}
		}else if(entity == Entities.Enemy){ //get enemy hexes
			return check(hex, enemy_pos); 
		}else if(entity == Entities.Base){ //get settlement hexes
			return check(hex, settlement_pos); 
		}else{
			return check(hex, resource_pos); //get resource hexes
		}	
	}
	
	private static bool check(HexData hex, HexData[] hexes){
		//Check chosen entity array to see if entities resides on given hex
		for(int i = 0; i < hexes.Length; i++){
			if(hex.x == hexes[i].x && hex.z == hexes[i].z){
				return true;
			}
		}
		
		return false;	
	}
	
	public static void updatePos(){
		//TODO
	}
}