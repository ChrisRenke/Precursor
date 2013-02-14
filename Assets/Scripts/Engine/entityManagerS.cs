using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityManagerS : MonoBehaviour {
	
	//Using dummy values for testing
	private static entityBaseS base_s;
	private static entityMechS mech_s;
	private static List<HexData> enemy_pos = new List<HexData>();
	private static List<HexData> resource_pos = new List<HexData>();//factory/junkyard/outpost, could be multiple arrays
	
	void Start () {
		//TODO: initialize Entity positions here
		enemy_pos.Add(hexManagerS.getHex(5, 1));
		base_s = gameObject.GetComponent<entityBaseS>();
		mech_s = gameObject.GetComponent<entityMechS>();
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static entityBaseS getBase(){
		return base_s;
	}
	
	public entityMechS getMech(){
		return mech_s;
	}
	
	public List<HexData> getEnemies(){
		return enemy_pos;
	}
	
	//check to see if a given entity type resides on hex 
	public bool isEntityPos(HexData hex, EntityE entity){
				
		if(entity == EntityE.Player){
			if(hex.x == 4 && hex.z == 4){
				return true; 
			}else{
				return false;
			}
		}else if(entity == EntityE.Base){
			if(hex.x == 1 && hex.z == 5){
				return true; 
			}else{
				return false;
			}
		}else {
			return !checkLists(hex, enemy_pos);
		}	
	}
	
	//Check to see if any entity resides on given hex
	public static bool canTraverseHex(HexData hex){
		if(hex.x == 4 && hex.z == 4){ //check mech pos
			return false;
		}else if(hex.x == 1 && hex.z == 5){ //check base pos
			return false;
		}else{ //check enemy pos and resource pos lists
			return true;
		}
	}
	
	public static bool checkLists(HexData hex, List<HexData> hexes){
		//Check chosen entity array to see if entities resides on given hex
		for(int i = 0; i < hexes.Count; i++){
			if(hex.x == hexes[i].x && hex.z == hexes[i].z){
				return false;
			}
		}
		return true;	
	}
	
}