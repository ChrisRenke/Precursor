using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class entityMechS : Combatable, IMove { 
	 
	public bool upgrade_traverse_water = false;
	public bool upgrade_traverse_mountain = true;
	public bool upgrade_traverse_cost = false; 
	public int  traverse_upgrade_cost  = -1;
	public int  traverse_standard_cost =  2;
	public int  traverse_slow_cost     =  4;
	public int  traverse_mountain_cost =  5;
	public int  traverse_water_cost    =  5;
	
	public bool upgrade_weapon_range  = false;
	public bool upgrade_weapon_damage = false;
	public bool upgrade_weapon_cost   = false;
	
	public bool upgrade_armor_1 = false;
	public bool upgrade_armor_2 = false;
	public bool upgrade_armor_3 = false;
		  
	public int weapon_base_damage = 3;
	public int weapon_base_range  = 1;
	public int weapon_base_cost   = 4; 
	
	public int weapon_upgrade_range  = 3;
	public int weapon_upgrade_cost   = 3;
	public int weapon_upgrade_damage = 5;
	
	public int armor_upgrade_1 = 2;
	public int armor_upgrade_2 = 3;
	public int armor_upgrade_3 = 4;
	
	public int starting_hp_max = 30;
	
	public int starting_ap = 18;
	public int starting_ap_max = 18;
	
	public static bool instantiated_selection_meshes_already = false;
	public static List<GameObject> selection_hexes;
	
	public static Dictionary<Part, int> part_count;
	
	
	
	void Awake()
	{
		selection_hexes = new List<GameObject>();
		part_count = new Dictionary<Part, int>();
		part_count.Add(Part.Cog, 0);
		part_count.Add(Part.Plate, 0);
		part_count.Add(Part.Piston, 0);
		part_count.Add(Part.Strut, 0);
		
		max_hp     = starting_hp_max;
		
		current_ap = starting_ap;
		max_ap = starting_ap_max;
	}
	
	//Use this for initialization
	void Start () { 
		
	}
	
	//Update is called once per frame
	void Update () {
		if(gameManagerS.current_turn == Turn.Player)
		{
			
			if(!instantiated_selection_meshes_already)
			{
				Debug.Log("BEGIN INSTANTIATING SELECTION MESHES");
				foreach(HexData ath in getAdjacentTraversableHexes())
				{
					
					Debug.Log("making a mesh for selection");
					selection_hexes.Add(InstantiateSelectionHex(ath.x, ath.z));
					selectionHexS hex_select = selection_hexes[selection_hexes.Count-1].GetComponent<selectionHexS>();
					hex_select.direction_from_center = ath.direction_from_central_hex;
					
					if(ath.hex_type == Hex.Forest || ath.hex_type == Hex.Hills || ath.hex_type == Hex.Marsh)
					{
						hex_select.select_level = SelectLevel.Medium;
					}
					else if(ath.hex_type == Hex.Mountain || ath.hex_type == Hex.Water)
					{
						hex_select.select_level = SelectLevel.Hard; 
					}
					else
					{
						hex_select.select_level = SelectLevel.Easy; 
					}
					
					hex_select.hex_type     = ath.hex_type;
					hex_select.movement_cost = getTraverseAPCost(ath.hex_type);
					
				}
				instantiated_selection_meshes_already = true;
				Debug.Log("END INSTANTIATING SELECTION MESHES");
			}
		}
	}

	public List<HexData> getAdjacentTraversableHexes ()
	{ 
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x, z);
		Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++)
			if(canTraverse(adjacent_hexes[i]))
				result_hexes.Add(adjacent_hexes[i]);
			
		
		Debug.Log(result_hexes.Count + " found adjacent goods");
		return result_hexes;
	}
 
	public List<HexData> getAdjacentUntraversableHexes() 
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(x, z);
		
		//See which of the adjacent hexes are NOT traversable
		for(int i = 0; i < adjacent_hexes.Length; i++)
			if(!canTraverse(adjacent_hexes[i]))
				result_hexes.Add(adjacent_hexes[i]); 
		
		return result_hexes;
	}
	
	
	public bool canTraverse (HexData hex){
		return canTraverse(hex.x, hex.z);
	}	
	public bool canTraverse (int hex_x, int hex_z)
	{
		HexData hex = hexManagerS.getHex(hex_x, hex_z);
		
		//if its a perimeter tile
		if(hex.hex_type == Hex.Perimeter)
			return false;
		
		//if it has a player, base, or enemy on it
		if(!entityManagerS.canTraverseHex(hex_x, hex_z))
			return false;
			
		//account for upgrades here
		if(hex.hex_type == Hex.Water && !upgrade_traverse_water)
				return false;
		
		if(hex.hex_type == Hex.Mountain && !upgrade_traverse_mountain)
				return false;
			
		return true;
	} 

	public bool makeMove (HexData hex)
	{
		if(!canTraverse(hex))
			throw new MissingComponentException("Can't move to this spot, invalid location!");
		
		
		//TODO add visual engine.move hooks
		
		//update players hex tags
		x = hex.x;
		z = hex.z;
		return true;
	}
	
	public int getAttackAPCost()
	{
		return upgrade_weapon_cost ? weapon_upgrade_cost : weapon_base_cost;
	}
	
	public int getTraverseAPCost(Hex hex_type)
	{
		
		switch(hex_type)
		{
		case Hex.Desert:
		case Hex.Farmland:
		case Hex.Grass:
			return traverse_standard_cost + (upgrade_traverse_cost ? traverse_upgrade_cost : 0);
		
		case Hex.Marsh:
		case Hex.Hills:
		case Hex.Forest:
			return traverse_slow_cost + (upgrade_traverse_cost ? traverse_upgrade_cost : 0);
			
		case Hex.Mountain:
			return traverse_mountain_cost + (upgrade_traverse_cost ? traverse_upgrade_cost : 0);
		
		case Hex.Water:
			return traverse_water_cost + (upgrade_traverse_cost ? traverse_upgrade_cost : 0);
			
		case Hex.Perimeter: 
		default:
			return 999999;
		}  
	}
	
	/**
	 *	Deal damage to the target
	 *  @param   the Combatable entity to damage
	 *  @return  damage delt
	 */
	public override int attackTarget(Combatable target){
//		int range  = upgrade_weapon_range ? weapon_upgrade_range : weapon_base_range;
		int damage = upgrade_weapon_damage ? weapon_upgrade_damage : weapon_base_damage;
		int cost   = upgrade_weapon_cost ? weapon_upgrade_cost : weapon_base_cost;
		
		current_ap -= cost;
		return target.acceptDamage(damage);
	}
	
		
	private GameObject InstantiateSelectionHex(int x, int z)
	{ 
		GameObject new_hex = (GameObject) Instantiate(gameManagerS.selection_hex, hexManagerS.CoordsGameTo3D(x, z) + new Vector3(0, 4, 0), Quaternion.identity); 
		return new_hex;
	}
			
}