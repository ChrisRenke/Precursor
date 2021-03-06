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
	
	public List<HexData> adjacent_visible_hexes;
	public List<HexData> previous_adjacent_visible_hexes;
	
	public static bool moving_on_path = false;
	public static List<HexData> travel_path;
	public static IEnumerator<HexData> travel_path_en;
	public static bool is_standing_on_node;
	
	
	
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
		
		updateFoWStates();
	}
	
//	
//	void OnGUI()
//	{
//		Vector3 screen_pos = Camera.main.WorldToScreenPoint (transform.position);
//		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y+30, 200, 15), current_hp + "/" + max_hp + " HP", enginePlayerS.hover_text);
//		GUI.Label(new Rect(screen_pos.x - 100, Screen.height - screen_pos.y + 45, 200, 15), current_ap + "/" + max_ap + " AP", enginePlayerS.hover_text);
//	}
	
	
	public static void moveToHexViaPath(Path _travel_path)
	{
		travel_path = _travel_path.getTraverseOrderList();
		travel_path_en = travel_path.GetEnumerator();
		travel_path_en.MoveNext();
		moving_on_path = true;
		
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
		
			
		if(checkIfDead())
			onDeath();
		
		if(gameManagerS.current_turn == Turn.Player && current_ap <= 0)
		{
			moving_on_path = false;
			travel_path_en = null;
			travel_path    = null;
			gameManagerS.endPlayerTurn();
		}
		
		if(gameManagerS.current_turn == Turn.Player)
		{
			
		
				
				
			if(moving_on_path && !lerp_move)
			{
				
				if(travel_path_en.MoveNext())
				{
					moveToHex(travel_path_en.Current, 
						entityManagerS.isNodeAt(travel_path_en.Current.x, travel_path_en.Current.z));
				
					enginePlayerS.popFrontOfRoute();	
				}
				
			}
			else
			if(!instantiated_selection_meshes_already)
			{
//				Debug.Log("BEGIN INSTANTIATING SELECTION MESHES");
//				foreach(HexData ath in getAdjacentTraversableHexes())
//				{
//					
//					Debug.Log("making a mesh for selection");
//					selection_hexes.Add(InstantiateSelectionHex(ath.x, ath.z));
//					selectionHexS hex_select = selection_hexes[selection_hexes.Count-1].GetComponent<selectionHexS>();
//					hex_select.direction_from_center = ath.direction_from_central_hex;
//					
//					hex_select.x = ath.x;
//					hex_select.z = ath.z;
//					
//					if(ath.traversal_cost <= 2)
//					{
//						hex_select.select_level = SelectLevel.Easy; 
//					}
//					else if(ath.traversal_cost <= 4)
//					{
//						hex_select.select_level = SelectLevel.Medium;
//					}
//					else
//					{
//						hex_select.select_level = SelectLevel.Hard; 
//					}
//					
//					hex_select.occupier      = ath.added_occupier;
//					if( ath.added_occupier == EntityE.Node)
//					{
//						hex_select.node_data = entityManagerS.getNodeInfoAt(ath.x, ath.z);
//					}
//					hex_select.hex_type      = ath.hex_type;
//					hex_select.action_cost = ath.traversal_cost;
//					hex_select.genTextString();
//					
//				}
				
			
				
//				//if we're on a resource node and we have enough ap to scavenge, then build a seclection hex for right here
//				if(entityManagerS.isEntityPos(x, z, EntityE.Node) && current_ap >= scavenge_base_cost)
//				{
//					NodeData nd = entityManagerS.getNodeInfoAt(x, z);
//					
//					//if the node isn't already empty
//					if(nd.node_level != NodeLevel.Empty)
//					{
//						selection_hexes.Add(InstantiateSelectionHex(x, z));
//						selectionHexS hex_select = selection_hexes[selection_hexes.Count-1].GetComponent<selectionHexS>();
//						
//						hex_select.x = x;
//						hex_select.z = z;
//						hex_select.direction_from_center = Facing.NorthWest;
//						hex_select.occupier      = EntityE.Node;
//						hex_select.hex_type      = hexManagerS.getHex(x, z).hex_type;
//						hex_select.action_cost   = scavenge_base_cost;
//						hex_select.select_level  = SelectLevel.Scavenge;
//						hex_select.node_data     = nd;
//						hex_select.genTextString();
//					}
//					
//				}
				
				//check for adjacent enemies
				if( current_ap >= getAttackAPCost())
				{
					foreach(HexData auh in getAdjacentUntraversableHexes())
					{
						entityEnemyS enemy = entityManagerS.getEnemyAt(auh.x, auh.z);
						if(enemy != null)
						{
							print ("adjacnet enemy!!!!!!!!!!!!!!!!!!!!!!");
							selection_hexes.Add(InstantiateSelectionHex(auh.x, auh.z));
							selectionHexS hex_select = selection_hexes[selection_hexes.Count-1].GetComponent<selectionHexS>();
							hex_select.x = auh.x;
							hex_select.z = auh.z;
							hex_select.direction_from_center = auh.direction_from_central_hex;
							hex_select.occupier      = EntityE.Enemy;
							hex_select.hex_type      = hexManagerS.getHex(x, z).hex_type;
							hex_select.action_cost   = getAttackAPCost();
							hex_select.select_level  = SelectLevel.Attack; 
							hex_select.genTextString();
							
						}
						
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
	
	public double getTraverseAPCostPathVersion (HexData hex_start, HexData hex_end)
	{	
		return (double) getTraverseAPCost(hex_end.hex_type);
	}
	
	public Path getPathFromMechTo(HexData destination)
	{
		Debug.LogWarning("getPathFromMechTo");
		return hexManagerS.getTraversablePath(
												hexManagerS.getHex(x, z), 
												hexManagerS.getHex(destination.x, destination.z),
												EntityE.Player,
												getTraverseAPCostPathVersion, 
												getAdjacentTraversableHexesPathVersion);
	}
	
	public List<HexData> getAdjacentTraversableHexesPathVersion (HexData hex, HexData destination, EntityE entity)
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hexManagerS.getAdjacentHexes(hex.x, hex.z);
		//Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(entityManagerS.isEntityPos(adjacent_hexes[i], entity) || (canTraverse(adjacent_hexes[i]) && adjacent_hexes[i].vision_state != Vision.Unvisted))
			{
				//add hex to traversable array
				result_hexes.Add(adjacent_hexes[i]); 
			}
		}
		
		//Debug.Log ("Number of result_hexes " + result_hexes.Count);
		return result_hexes;
		
//		return getAdjacentTraversableHexes();
	}
	
	public int attackEnemy(int att_x, int att_z)
	{
		Debug.LogWarning("ABOUT TO ATTCK ENTITY ON - "+ att_x + "," + att_z);
		entityEnemyS target = entityManagerS.getEnemyAt(att_x, att_z);
		current_ap -= getAttackAPCost();
		
		Debug.LogWarning("ABOUT TO ATTCK ENTITY "+ target.GetInstanceID());
		if(target != null)
			return target.acceptDamage(attack_damage);
		
		return 0; //nothing to damage if we get here			
	}
	
	
	public int getScavengeAPCost()
	{
		return scavenge_base_cost;
	}
	
	
	public bool scavengeParts(Node node_type, NodeLevel resource_level, int x, int z)
	{
		
		if(getScavengeAPCost() > current_ap)
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
		
		current_ap -= getScavengeAPCost();
		
		allowSelectionHexesDraw();
		
		entityManagerS.updateNodeLevel(x, z, resource_level);
		return true;
	}
	
	public void updateFoWStates()
	{
//		adjacent_visible_hexes
//			previous_adjacent_visible_hexes
		previous_adjacent_visible_hexes = adjacent_visible_hexes;
		adjacent_visible_hexes = hexManagerS.getAdjacentHexes(x, z, sight_range);
		List<HexData> previously_live_now_visited_hexes = new List<HexData>();
		
		//turn hexes within range now to Live state
		foreach(HexData hex in adjacent_visible_hexes)
		{ 
			hexManagerS.updateHexVisionState(hex, Vision.Live);
			if(hex.hex_script != null)
				hex.hex_script.updateFoWState();
		} 
		
		//turn hexes not within range now to Visisted state
		if(previous_adjacent_visible_hexes != null)
			foreach (HexData prev_hex in previous_adjacent_visible_hexes)
			{
			    if(!adjacent_visible_hexes.Contains(prev_hex) && !entityBaseS.adjacent_visible_hexes.Contains(prev_hex))
				{
					hexManagerS.updateHexVisionState(prev_hex, Vision.Visited);
					if(prev_hex.hex_script != null)
						prev_hex.hex_script.updateFoWState();
					previously_live_now_visited_hexes.Add(prev_hex);
				}	
			}
		
		entityManagerS.updateEntityFoWStates();
		
	}
	
	public void moveToHex(HexData location, bool _standing_on_top_of_node)
	{
		current_ap -= location.traversal_cost; 
		setLocation(location.x, location.z);	
		moveInWorld(location.x, location.z, 6F);
		setFacingDirection(location.direction_from_central_hex);
		updateFoWStates();
		is_standing_on_node = _standing_on_top_of_node;
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
	public bool lerp_move = false; 
	float time_to_complete = 2F;
	float moveTime = 0.0f;
 
	public override bool onDeath()
	{
		Application.Quit();
		return true;
	}
	
}
