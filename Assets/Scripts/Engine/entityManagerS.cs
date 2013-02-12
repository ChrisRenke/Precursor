using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityManagerS : MonoBehaviour {
	
	/*
	 //OUTLINE FROM WIKI
	bool[] getUntraversableHexes(HexData[] adjacent_hexes) //loop canTraverHex and store into boolarray, return that array

	bool canTraverseHex(HexData in_hex) //***added

	Entity entityAt(int x, int z) {

	}

	Entity entityAt(HexData hex_in) { return Entity(hex_in.x, hex_in.z); }

	baseScriptS getBase(); //**added

	mechScriptS getMech(); //**added

	List getEnemies(); //**added

	bool makeEntity(EntityE type, int x, int y) //and more to determine stuff about enemy starting state
	*/

	
	//Using dummy values for testing
	private static entityBaseS base_s;
	private static entityMechS mech_s;
	private static List<HexData> enemy_pos = new List<HexData>();
	private static List<HexData> resource_pos = new List<HexData>();//factory/junkyard/outpost, could be multiple arrays
	
	// Use this for initialization
	void Start () {
		//TODO: initialize Entity positions here
		base_s = gameObject.GetComponent<entityBaseS>();
		//base_s = gameObject
	    mech_s = gameObject.GetComponent<entityMechS>();
		enemy_pos.Add(new HexData(1, 2, Hex.Grass));
		resource_pos.Add(new HexData(2, 5, Hex.Grass));
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public entityBaseS getBase(){
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
			if(hex.x == mech_s.x && hex.z == mech_s.z){
				return true; 
			}else{
				return false;
			}
		}else if(entity == EntityE.Base){
			if(hex.x == base_s.x && hex.z == base_s.z){
				return true; 
			}else{
				return false;
			}
		}else if(entity == EntityE.Enemy){ //get enemy hexes
			return checkLists(hex, enemy_pos); 
		}else{
			return checkLists(hex, resource_pos); //get resource hexes
		}	
	}
	
	//Check to see if any entity resides on given hex
	public static bool canTraverseHex(HexData hex){
		if(hex.x == mech_s.x && hex.z == mech_s.z){ //check mech pos
			return false;
		}else if(hex.x == base_s.x && hex.z == base_s.z){ //check base pos
			return false;
		}else{ //check enemy pos and resource pos lists
			return checkLists(hex, enemy_pos) && checkLists(hex, resource_pos);
		}
	}
	
	public static bool checkLists(HexData hex, List<HexData> hexes){
		//Check chosen entity array to see if entities resides on given hex
		for(int i = 0; i < hexes.Count; i++){
			if(hex.x == hexes[i].x && hex.z == hexes[i].z){
				return true;
			}
		}
		return false;	
	}
	
}