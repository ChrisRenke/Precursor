using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class entityMechS : Combatable, IMove { 
	 
	public bool upgrade_traverse_water		= false;
	public bool upgrade_traverse_mountain 	= false;
	public bool upgrade_traverse_cost 		= false; 
	public int  traverse_upgrade_cost  		= -1;
	public int  traverse_standard_cost 		=  2;
	public int  traverse_slow_cost     		=  4;
	public int  traverse_mountain_cost	 	=  5;
	public int  traverse_water_cost    		=  5;
	
	public int scavenge_base_cost           =  3;
	
	public int repair_base_cost             =  1;
	
	public int upgrade_base_cost            =  1;
		  
	public int weapon_base_damage = 3;
	public int weapon_base_range  = 1;
	public int weapon_base_cost   = 4; 
	public bool upgrade_weapon_range  = false;
	public bool upgrade_weapon_damage = false;
	public bool upgrade_weapon_cost   = false;
	public int weapon_upgrade_range  = 3;
	public int weapon_upgrade_cost   = 3;
	public int weapon_upgrade_damage = 5;
	
	public bool upgrade_armor_1 = false;
	public bool upgrade_armor_2 = false;
	public bool upgrade_armor_3 = false;
	public int armor_upgrade_1 = 2;
	public int armor_upgrade_2 = 3;
	public int armor_upgrade_3 = 4;
	
	
	
	public int starting_hp_max = 30;
	
	public int starting_ap = 18;
	public int starting_ap_max = 18;
	
	public static bool instantiated_selection_meshes_already = false;
	public static List<GameObject> selection_hexes;
	
	public static Dictionary<Part, int> part_count;
	
	public static int getPartCount(Part part_query)
	{
		return part_count[part_query];	
	}
	public static void adjustPartCount(Part part_query, int delta)
	{
		part_count[part_query] += delta;	
	}
	
	void Awake()
	{
		selection_hexes = new List<GameObject>();
		part_count = new Dictionary<Part, int>();
		part_count.Add(Part.Gear, 0);
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
	
	
	public void colorSightRange(Color clr)
	{
		foreach(HexData hex in hexManagerS.getAdjacentHexes(x, z, sight_range))
		{
			hex.hex_object.renderer.material.SetColor("_Color", clr);
		}
	}
	
	public bool destroySelectionHexes(){
		foreach(GameObject go in selection_hexes)
		{
			Destroy(go);
		}
		return true;
	}
	
	public void allowSelectionHexesDraw(){
		instantiated_selection_meshes_already = false;
	}
	
	//Update is called once per frame
	void Update () {
		
		colorSightRange(Color.red);
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
					
					hex_select.x = ath.x;
					hex_select.z = ath.z;
					
					if(ath.traversal_cost <= 2)
					{
						hex_select.select_level = SelectLevel.Easy; 
					}
					else if(ath.traversal_cost <= 4)
					{
						hex_select.select_level = SelectLevel.Medium;
					}
					else
					{
						hex_select.select_level = SelectLevel.Hard; 
					}
					
					hex_select.occupier      = ath.added_occupier;
					if( ath.added_occupier == EntityE.Node)
					{
						hex_select.node_data = entityManagerS.getNodeInfoAt(ath.x, ath.z);
					}
					hex_select.hex_type      = ath.hex_type;
					hex_select.action_cost = ath.traversal_cost;
					hex_select.genTextString();
					
				}
				
			
				
				//if we're on a resource node and we have enough ap to scavenge, then build a seclection hex for right here
				if(entityManagerS.isEntityPos(x, z, EntityE.Node) && current_ap >= scavenge_base_cost)
				{
					NodeData nd = entityManagerS.getNodeInfoAt(x, z);
					
					//if the node isn't already empty
					if(nd.node_level != NodeLevel.Empty)
					{
						selection_hexes.Add(InstantiateSelectionHex(x, z));
						selectionHexS hex_select = selection_hexes[selection_hexes.Count-1].GetComponent<selectionHexS>();
						
						hex_select.x = x;
						hex_select.z = z;
						hex_select.direction_from_center = Facing.NorthWest;
						hex_select.occupier      = EntityE.Node;
						hex_select.hex_type      = hexManagerS.getHex(x, z).hex_type;
						hex_select.action_cost   = scavenge_base_cost;
						hex_select.select_level  = SelectLevel.Scavenge;
						hex_select.node_data     = nd;
						hex_select.genTextString();
					}
					
				}
				instantiated_selection_meshes_already = true;
				Debug.Log("END INSTANTIATING SELECTION MESHES");
			}
		}
		
		if(lerp_move)
		{	
			transform.position = Vector3.Lerp(transform.position, ending_pos,  moveTime);
     		moveTime += Time.deltaTime/dist;
			
			if( Vector3.Distance(transform.position, ending_pos) <= .05)
			{
				allowSelectionHexesDraw();
				lerp_move = false;
				transform.position = ending_pos;
			}
				
		}
	}
	
	public bool scavengeParts(Node node_type, NodeLevel resource_level, int x, int z)
	{
		
		if(scavenge_base_cost > current_ap)
			throw new System.Exception("Don't have enough AP to scavenge, why is this being offered????");
		
		if(resource_level == NodeLevel.Empty)
			return false;
		
		int num_of_each_type = (int) resource_level;
		
		if(node_type == Node.Factory)
		{
			part_count[Part.Piston] += num_of_each_type;
			entityManagerS.createPartEffect(x,z,Part.Piston);
			part_count[Part.Gear] += num_of_each_type;
			entityManagerS.createPartEffect(x,z,Part.Gear);
			
		}
		else if(node_type == Node.Outpost)
		{
			part_count[Part.Strut] += num_of_each_type;
			entityManagerS.createPartEffect(x,z,Part.Strut);
			
			part_count[Part.Plate] += num_of_each_type;
			entityManagerS.createPartEffect(x,z,Part.Plate);
		}
		else
		{
			//increase random part count by two, twice
			for(int i =0; i < 2; i++)
			{
				int ind = UnityEngine.Random.Range(0, 3);
				part_count[(Part) ind] += num_of_each_type;
				entityManagerS.createPartEffect(x,z,(Part) ind);
			}
		}
		
		current_ap -= scavenge_base_cost;
		
		allowSelectionHexesDraw();
		
		entityManagerS.updateNodeLevel(x, z, resource_level);
		return true;
	}
	
	
	public void moveToHex(int x, int z, int movement_fee, EntityE _standing_on_top_of_entity)
	{
		current_ap -= movement_fee; 
		setLocation(x, z);	
		moveInWorld(x, z, 6F);
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
			{
				adjacent_hexes[i].traversal_cost = getTraverseAPCost(adjacent_hexes[i].hex_type);
				adjacent_hexes[i] = entityManagerS.fillEntityData(adjacent_hexes[i]);
				if(adjacent_hexes[i].traversal_cost <= current_ap)
					result_hexes.Add(adjacent_hexes[i]);
			}
		
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
		
	
	public void moveInWorld(int _destination_x, int _destination_z, float _time_to_complete)
	{
		lerp_move = true;
		starting_pos =  entityManagerS.CoordsGameTo3DEntiy(x, z);
		ending_pos = entityManagerS.CoordsGameTo3DEntiy(_destination_x, _destination_z);
		time_to_complete = _time_to_complete;
		moveTime = 0.0f;
		dist = Vector3.Distance(transform.position, ending_pos) * 2;
	}
	
	float dist; 
	Vector3 starting_pos, ending_pos;
	bool lerp_move = false; 
	float time_to_complete = 2F;
	float moveTime = 0.0f;
 
	
	
}
