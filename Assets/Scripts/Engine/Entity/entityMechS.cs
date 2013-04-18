using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class entityMechS : Combatable, IMove { 
	 
//public enum MechUpgrade { Move_Water, Move_Mountain, Move_Marsh, Move_Legs, Combat_Damage, Combat_Cost, Combat_Range, Combat_Armor, Combat_Dodge, 
//		Scavenge_Combat, Scavenge_Greed, Scavenge_Empty, Scavenge_Cost, Util_Recall, Util_Vision, Util_AP, Util_Heal, Util_Idle };
	
	
	public AudioClip  sound_walk_norm;
	public AudioClip  sound_walk_water;
	public AudioClip  sound_walk_mountain;
	 
	public AudioClip  sound_attack_norm;
	public AudioClip  sound_attack_upgrade;
	
	public AudioClip  sound_repair;
	
	public AudioClip  sound_burning;
	
	public AudioClip  sound_recall;
	
	public AudioClip  sound_upgrade_mech; 
	
	public AudioClip  sound_scavenge_junk;
	public AudioClip  sound_scavenge_fact;
	public AudioClip  sound_scavenge_outpost;
	
	
	public bool upgrade_move_water		= false;
	public bool upgrade_move_mountain 	= false;
	public bool upgrade_move_cost 		= false;
	public bool upgrade_move_marsh 		= false;
	
	public bool upgrade_combat_damage 	= false;
	public bool upgrade_combat_cost     = false;
	public bool upgrade_combat_range    = false;
	public bool upgrade_combat_armor    = false;
	public bool upgrade_combat_dodge    = false;
	
	public bool upgrade_scavenge_combat  = false;
	public bool upgrade_scavenge_greed   = false;
	public bool upgrade_scavenge_empty   = false;
	public bool upgrade_scavenge_cost    = false;
	
	public bool upgrade_util_recall    = false;
	public bool upgrade_util_vision    = false;
	public bool upgrade_util_ap        = false;
	public bool upgrade_util_parts     = false;
	public bool upgrade_util_idle      = false;
	  
//	public bool upgrade_traverse_water		= false;
//	public bool upgrade_traverse_mountain 	= false;
//	public bool upgrade_traverse_cost 		= false;
//	
//	public bool upgrade_armor 	 = false;
//	public bool upgrade_scavenge = false;
//	public bool upgrade_teleport = false;
//	
//	public bool upgrade_weapon_range  = false;
//	public bool upgrade_weapon_damage = false;
//	public bool upgrade_weapon_cost   = false;
	 
	public readonly int traverse_upgrade_cost  		= -1;
	public readonly int traverse_upgrade_marsh  	= -1;
	public readonly int traverse_standard_cost 		=  2;
	public readonly int traverse_slow_cost     		=  4;
	public readonly int traverse_mountain_cost		=  5;
	public readonly int traverse_water_cost    		=  5;
	
	public readonly int combat_dodge_percent      = 20;
	
	public readonly int util_recall_cost 			= 6;
	
	public readonly int scavenge_core_cost          =  3;
	public readonly int scavenge_upgrade_cost     	=  2;
	
	public readonly int scavenge_upgrade_greed    	=  1;
	public readonly int scavenge_empty_percent	= 25;
	
	public readonly int repair_base_cost            =  1; 
		  
	public readonly int weapon_core_damage	  		= 5;
	public readonly int weapon_upgrade_damage 		= 8;
	
	public readonly int weapon_core_range 	  		= 2;
	public readonly int weapon_upgrade_range  		= 3;
	
	public readonly int weapon_core_cost  	  		= 5; 
	public readonly int weapon_upgrade_cost   		= 4;
	
	public readonly int repair_core_heal			= 2;
	
	public readonly int idle_heal_restore			= 1;
	
	public readonly int vision_core_range			= 5;
	public readonly int vision_upgrade_range		= 7;
	
	public readonly int armor_upgrade 				= 2; 
	
	public readonly int part_core_capacity          = 12;
	public readonly int part_upgrade_capacity       = 20;
	 
	public int getPartCapacity(){
		return upgrade_util_parts ? part_upgrade_capacity : part_core_capacity;
	}
	

	void Awake(){ 
	
 		child_shots = gameObject.transform.GetChild(1);//.GetComponentsInChildren<ParticleSystem>();
 		child_fire = gameObject.transform.GetChild(0);//.GetComponentsInChildren<ParticleSystem>(); 
		turnOffFire();
		
		part_count = new Dictionary<Part, int>();
		part_count.Add(Part.Gear, 0);
		part_count.Add(Part.Plate, 0);
		part_count.Add(Part.Piston, 0);
		part_count.Add(Part.Strut, 0);
		
		max_hp     = starting_hp_max;
		
		current_ap = starting_ap;
		max_ap = starting_ap_max;
		createUpgradeMenuEntries();
		
	}
	
	public List<HexData> adjacent_visible_hexes;
	public List<HexData> previous_adjacent_visible_hexes;
	
	public  bool moving_on_path = false;
	public  List<HexData> travel_path;
	public  IEnumerator<HexData> travel_path_en;
	public  bool is_standing_on_node;
	
	 
	public int starting_hp_max = 30;
	
	public int starting_ap = 18;
	public int starting_ap_max = 18;
	 
	public ParticleSystem fire;
		
	public Dictionary<Part, int> part_count;
	
	public int getPartCount(Part part_query)
	{
		return part_count[part_query];	
	}
	public void adjustPartCount(Part part_query, int delta)
	{
		part_count[part_query] += delta;	
	}
	
	public int getRepairAPCost(){
		return repair_base_cost;
	}
	
	public int getRepairHealAmount(){
		return repair_core_heal;
	}
	
	public void repair(Part part)
	{
		em.createHealEffect(x,z); 
		
		adjustPartCount(part, -1);
		current_ap -= getRepairAPCost();
		current_hp += getRepairHealAmount();
		
		if(current_hp > max_hp)
		{
			current_hp = max_hp;
		}
	}
	
	public  Transform child_fire; 
	public  Transform child_shots; 
	 
	
	//Use this for initialization
	void Start () {  
		gm = GameObject.Find("engineGameManager").GetComponent<gameManagerS>();
		ep = GameObject.Find("enginePlayer").GetComponent<enginePlayerS>();
		em = GameObject.Find("engineEntityManager").GetComponent<entityManagerS>();
		hm = GameObject.Find("engineHexManager").GetComponent<hexManagerS>();
		updateFoWStates();
}
	
	
	public  void shootEffect(Facing face_direction){  
		switch(face_direction){ 
			case Facing.North:
				child_shots.GetChild(0).transform.GetChild(0).particleSystem.enableEmission = true; 
				break;
			case Facing.NorthEast:
				child_shots.GetChild(1).transform.GetChild(0).particleSystem.enableEmission = true; 
				break;
			case Facing.NorthWest:
				child_shots.GetChild(2).transform.GetChild(0).particleSystem.enableEmission = true; 
				break;
			case Facing.South:
				child_shots.GetChild(3).transform.GetChild(0).particleSystem.enableEmission = true; 
				break;
			case Facing.SouthEast:
				child_shots.GetChild(4).transform.GetChild(0).particleSystem.enableEmission = true; 
				break;
			case Facing.SouthWest:
				child_shots.GetChild(5).transform.GetChild(0).particleSystem.enableEmission = true; 
				break;
		} 
	}
	
	public  void turnOnFire(){
		
		child_fire.transform.GetChild(0).particleEmitter.emit = true; 
		child_fire.transform.GetChild(1).particleEmitter.emit = true; 
		child_fire.transform.GetChild(2).particleEmitter.emit = true; 
	}
	
	public  void turnOffFire(){
		
		child_fire.transform.GetChild(0).particleEmitter.emit = false; 
		child_fire.transform.GetChild(1).particleEmitter.emit = false; 
		child_fire.transform.GetChild(2).particleEmitter.emit = false; 
	}
	
	public  void moveToHexViaPath(Path _travel_path)
	{
		travel_path = _travel_path.getTraverseOrderList();
		travel_path_en = travel_path.GetEnumerator();
		travel_path_en.MoveNext();
		moving_on_path = true;
	}
	 
	public void applyAPCost(int cost){
		current_ap -= cost;
	}
	
	public bool onFire = false;
	public bool is_dead = false;
	 
	//Update is called once per frame
	void Update () {
		
			
		if(!is_dead && checkIfDead())
		{
			Debug.Log("HOLY SHIT YOU'RE DEAD!!!");
			is_dead =onDeath();
			
		}
		
		
		if((float)current_hp/(float)max_hp < .5F )
		{
			if(!onFire)
				turnOnFire(); 
			onFire = true;
		}
		else{
			if(onFire)
				turnOffFire();
			onFire = false;
		}
		
		
		if(gm.current_turn == Turn.Player && current_ap <= 0 && !lerp_move)
		{
			moving_on_path = false;
			travel_path_en = null;
			travel_path    = null;
			gm.endPlayerTurn();
		}
		
		if(gm.current_turn == Turn.Player)
		{
			
		
				
				
			if(moving_on_path && !lerp_move)
			{
				
				if(travel_path_en.MoveNext() && travel_path_en.Current.traversal_cost <= current_ap)
				{ 	
					moveToHex(travel_path_en.Current, em.isNodeAt(travel_path_en.Current.x, travel_path_en.Current.z));
				
					ep.popFrontOfRoute();	
				}
				else{ 
					moving_on_path = false;
				}
				
			}

		}
		
		if(lerp_move)
		{	
			transform.position = Vector3.Lerp(transform.position, ending_pos,  moveTime);
     		moveTime += Time.deltaTime/dist;
			
			if( Vector3.Distance(transform.position, ending_pos) <= .05)
			{ 
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
//		Debug.LogWarning("getPathFromMechTo");
		return hm.getTraversablePath(	hm.getHex(x, z), 
												hm.getHex(destination.x, destination.z),
												EntityE.Player,
												getTraverseAPCostPathVersion, 
												getAdjacentTraversableHexesPathVersion);
	}
	
	public List<HexData> getAdjacentTraversableHexesPathVersion (HexData hex, HexData destination, EntityE entity)
	{
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hm.getAdjacentHexes(hex.x, hex.z);
		//Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++){
			if(em.isEntityPos(adjacent_hexes[i], entity) || (canTraverse(adjacent_hexes[i]) && adjacent_hexes[i].vision_state != Vision.Unvisted))
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
		shootEffect(facing_direction);
		
		if(upgrade_combat_damage)
			audio.PlayOneShot(sound_attack_upgrade);
		else
			audio.PlayOneShot(sound_attack_norm);
		
		Debug.LogWarning("ABOUT TO ATTCK ENTITY ON - "+ att_x + "," + att_z);
//		entityEnemyS target = em.getEnemyAt(att_x, att_z);
		
		Enemy target = em.getEnemyAt(att_x, att_z);
		current_ap -= getAttackAPCost();
		
		int enemy_hp_left = 15;
		Debug.LogWarning("ABOUT TO ATTCK ENTITY "+ target.GetInstanceID());
		if(target != null)
			enemy_hp_left = target.acceptDamage(getAttackDamage());
		
		if(enemy_hp_left <= 0 && upgrade_scavenge_combat)
		{
			int ind = (int) UnityEngine.Random.Range(0, 3.999999999999F);
			part_count[(Part) ind] += 1;
			em.createPartEffect(att_x,att_z,(Part) ind);
			parts_collected += 1;
			validatePartCounts();
		}
		if(enemy_hp_left <= 0)
			player_kills++;
		return 0; //nothing to damage if we get here			
	}
	
	public int getRecallAPCost()
	{
		return util_recall_cost;
	}
	
	public int getAttackDamage(){
		return upgrade_combat_damage ? weapon_upgrade_damage : weapon_core_damage;
	}
	
	public int getScavengeAPCost(){
		return  upgrade_scavenge_cost ? scavenge_upgrade_cost : scavenge_core_cost;
	}
	
	public int parts_collected = 0;
	public bool attemptScavenge(Node node_type, NodeLevel resource_level, int x, int z)
	{
		
		
		if(getScavengeAPCost() > current_ap)
			throw new System.Exception("Don't have enough AP to scavenge, why is this being offered????");
		 
		

		int num_of_each_type = 1;
		
		if(upgrade_scavenge_greed)
			num_of_each_type++;
			 
		if(((int) UnityEngine.Random.Range(0, 99.999999999999F)) < scavenge_empty_percent)
		{
				
		if(node_type == Node.Factory)
		{
			audio.PlayOneShot(sound_scavenge_fact);
			part_count[Part.Piston] += num_of_each_type;
				
			parts_collected += num_of_each_type;
			part_count[Part.Gear] += num_of_each_type;
			parts_collected += num_of_each_type;
				
			for(int i = 0; i < num_of_each_type; ++i)
			{
				em.createPartEffect(x,z,Part.Gear);
				em.createPartEffect(x,z,Part.Piston);
			}
					
			
		}
		else if(node_type == Node.Outpost)
		{
			audio.PlayOneShot(sound_scavenge_outpost);
			part_count[Part.Strut] += num_of_each_type;
			
			part_count[Part.Plate] += num_of_each_type;
			parts_collected += num_of_each_type;
			parts_collected += num_of_each_type;
			for(int i = 0; i < num_of_each_type; ++i)
			{
					
				em.createPartEffect(x,z,Part.Strut);
				em.createPartEffect(x,z,Part.Plate);
			}
		}
		else
		{
			audio.PlayOneShot(sound_scavenge_junk);
			//increase random part count by two, twice
			for(int i =0; i < 2; i++)
			{
				int ind = (int) UnityEngine.Random.Range(0, 3.999999999999F);
				part_count[(Part) ind] += num_of_each_type;
				parts_collected += num_of_each_type;
				for(int ix = 0; ix < num_of_each_type; ++ix)
				{
				 	em.createPartEffect(x,z,(Part) ind);
				}
			}
		}
			
		}
		else
		{
			audio.PlayOneShot(sound_negative);
		}
		current_ap -= getScavengeAPCost(); 
		validatePartCounts();
		return true;
		
	}
	
	public AudioClip sound_negative;
	
	public void validatePartCounts(){
		if(part_count[Part.Gear] > getPartCapacity())
			part_count[Part.Gear] = getPartCapacity();
		if(part_count[Part.Piston] > getPartCapacity())
			part_count[Part.Piston] = getPartCapacity();
		if(part_count[Part.Strut] > getPartCapacity())
			part_count[Part.Strut] = getPartCapacity();
		if(part_count[Part.Plate] > getPartCapacity())
			part_count[Part.Plate] = getPartCapacity(); 
	}
	
	
	
	public bool scavengeParts(Node node_type, NodeLevel resource_level, int x, int z)
	{
		
		if(getScavengeAPCost() > current_ap)
			throw new System.Exception("Don't have enough AP to scavenge, why is this being offered????");
		
		if(resource_level == NodeLevel.Empty)
			return false;
		

		int num_of_each_type = (int) resource_level;
		
		if(upgrade_scavenge_greed)
			num_of_each_type++;
		
		if(node_type == Node.Factory)
		{
			audio.PlayOneShot(sound_scavenge_fact);
			part_count[Part.Piston] += num_of_each_type;
			part_count[Part.Gear] += num_of_each_type;
			parts_collected += num_of_each_type;
			parts_collected += num_of_each_type;
			for(int i = 0; i < num_of_each_type; ++i)
			{
			em.createPartEffect(x,z,Part.Gear);
			em.createPartEffect(x,z,Part.Piston);
			}
					
			
		}
		else if(node_type == Node.Outpost)
		{
			audio.PlayOneShot(sound_scavenge_outpost);
			part_count[Part.Strut] += num_of_each_type;
			parts_collected += num_of_each_type;
			parts_collected += num_of_each_type;
			
			part_count[Part.Plate] += num_of_each_type;
			for(int i = 0; i < num_of_each_type; ++i)
			{
					
				em.createPartEffect(x,z,Part.Strut);
				em.createPartEffect(x,z,Part.Plate);
			}
		}
		else
		{
			audio.PlayOneShot(sound_scavenge_junk);
			//increase random part count by two, twice
			for(int i =0; i < 2; i++)
			{
				int ind = (int) UnityEngine.Random.Range(0, 3.999999999999F);
				part_count[(Part) ind] += num_of_each_type;
				for(int ix = 0; ix < num_of_each_type; ++ix)
				{
				 	em.createPartEffect(x,z,(Part) ind);
			parts_collected += num_of_each_type;
				}
			}
		}
		
		current_ap -= getScavengeAPCost(); 
		
		em.updateNodeLevel(x, z, resource_level);
		validatePartCounts();
		return true;
	}
	
	
//	public void attackMech(int x, int z)
//	{
//		
//		entityEnemyS enemy_to_attack = em.getEnemyAt(x,z);
//		enemy_to_attack.acceptDamage(getAttackDamage());
//		//TODO ADD HOOKS FOR SOUNDS 
//	}
	
	List<HexData> attackable_hexes;
	public void updateAttackableEnemies()
	{
		//invalidate the previous hexes
		if(attackable_hexes != null)
		{	
			foreach(HexData hex in attackable_hexes)
			{
				hex.hex_script.can_attack_hex = false; 
			} 
		}
		
		//now update the current ones
		Debug.Log("Updating attackable enemies iasdfasdkjflasdjflkasjd fkasjdlf kajsdlfkaj slkfjalsn range " +  getAttackRange() + "...");
		if(em == null)
			em = GameObject.Find("engineEntityManager").GetComponent<entityManagerS>();
		attackable_hexes = em.getEnemyLocationsInRange(hm.getHex(x,z), getAttackRange());
		
		foreach(HexData hex in attackable_hexes)
		{
			hex.hex_script.can_attack_hex = true;
			Debug.LogWarning("entityEnemyS at hex :" + hex.x + "," + hex.z);
		} 
	}
	
	public void updateFoWStates()
	{
//		adjacent_visible_hexes
//			previous_adjacent_visible_hexes
		previous_adjacent_visible_hexes = adjacent_visible_hexes; 
			
		adjacent_visible_hexes = hm.getAdjacentHexes(x, z, sight_range);
		List<HexData> previously_live_now_visited_hexes = new List<HexData>();
		
		//turn hexes within range now to Live state
		foreach(HexData hex in adjacent_visible_hexes)
		{ 
			hm.updateHexVisionState(hex, Vision.Live);
			if(hex.hex_script != null)
				hex.hex_script.updateFoWState();
		} 
		
		//turn hexes not within range now to Visisted state
		if(previous_adjacent_visible_hexes != null)
			foreach (HexData prev_hex in previous_adjacent_visible_hexes)
			{
			    if(!adjacent_visible_hexes.Contains(prev_hex) && !em.getBase().adjacent_visible_hexes.Contains(prev_hex))
				{
					hm.updateHexVisionState(prev_hex, Vision.Visited);
					if(prev_hex.hex_script != null){
						prev_hex.hex_script.updateFoWState();
					}
					previously_live_now_visited_hexes.Add(prev_hex);
				}	
			}
		
		em.updateEntityFoWStates();
		
	}
	
	public void moveToHex(HexData location, bool _standing_on_top_of_node)
	{
		
		em.disableFacingDirectionsRange(hm.getHex(x,z), sight_range);
		
		hm.getHex(x,z).hex_script.mech_is_here = false;
		current_ap -= location.traversal_cost;
		setLocation(location.x, location.z);	
		moveInWorld(location.x, location.z, 6F);
		
		
		if(location.hex_type == Hex.Mountain || location.hex_type == Hex.Hills)
			audio.PlayOneShot(sound_walk_mountain);
		else if(location.hex_type == Hex.Marsh || location.hex_type == Hex.Marsh) 
			audio.PlayOneShot(sound_walk_water);
		else
			audio.PlayOneShot(sound_walk_norm);
		
		setFacingDirection(location.direction_from_central_hex);
		updateFoWStates();
		updateAttackableEnemies();
		is_standing_on_node = _standing_on_top_of_node;
		hm.getHex(x,z).hex_script.mech_is_here = true;
		
		em.updateFacingDirectionsRange(hm.getHex(x,z), sight_range);
		
	}
	
	public void transportToHex(HexData location, bool _standing_on_top_of_node)
	{
		setLocation(location.x, location.z);	
		moveInWorld(location.x, location.z, 6F);
		
		
		audio.PlayOneShot(sound_recall);
		
		//setFacingDirection(location.direction_from_central_hex);
		updateFoWStates();
		updateAttackableEnemies();
		is_standing_on_node = _standing_on_top_of_node;
	}

	public List<HexData> getAdjacentTraversableHexes ()
	{ 
		List<HexData> result_hexes = new List<HexData>(); //hold resulting hexes
		
		//Get adjacent tiles around player mech
		HexData[] adjacent_hexes = hm.getAdjacentHexes(x, z);
		Debug.Log(adjacent_hexes.Length + " found adjacent");
		
		//See which of the adjacent hexes are traversable
		for(int i = 0; i < adjacent_hexes.Length; i++)
			if(canTraverse(adjacent_hexes[i]))
			{
				adjacent_hexes[i].traversal_cost = getTraverseAPCost(adjacent_hexes[i].hex_type);
				adjacent_hexes[i] = em.fillEntityData(adjacent_hexes[i]);
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
		HexData[] adjacent_hexes = hm.getAdjacentHexes(x, z);
		
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
		HexData hex = hm.getHex(hex_x, hex_z);
		
		//if its a perimeter tile
		if(hex.hex_type == Hex.Perimeter)
			return false;
		
		//if it has a player, base, or enemy on it
		if(!em.canTraverseHex(hex_x, hex_z))
			return false;
			
		//account for upgrades here
		if(hex.hex_type == Hex.Water && !upgrade_move_water)
				return false;
		
		if(hex.hex_type == Hex.Mountain && !upgrade_move_mountain)
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
		return upgrade_combat_cost ? weapon_upgrade_cost : weapon_core_cost;
	}
	
	public int getAttackRange()
	{ 
		return upgrade_combat_range ? weapon_upgrade_range : weapon_core_range;
	}
	
	public int getTraverseAPCost(Hex hex_type)
	{
		
		switch(hex_type)
		{
		case Hex.Desert:
		case Hex.Farmland:
		case Hex.Grass:
			return traverse_standard_cost + (upgrade_move_cost ? traverse_upgrade_cost : 0);
		
		case Hex.Marsh:
			return traverse_slow_cost + (upgrade_move_cost ? traverse_upgrade_cost : 0) +  (upgrade_move_marsh ? traverse_upgrade_marsh : 0);
		case Hex.Hills:
		case Hex.Forest:
			return traverse_slow_cost + (upgrade_move_cost ? traverse_upgrade_cost : 0);
			
		case Hex.Mountain:
			return traverse_mountain_cost + (upgrade_move_cost ? traverse_upgrade_cost : 0);
		
		case Hex.Water:
			return traverse_water_cost + (upgrade_move_cost ? traverse_upgrade_cost : 0);
			
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
		Debug.Log ("Mech attacking target"); 
		current_ap -= getAttackAPCost();
		int hp =  target.acceptDamage(getAttackDamage());
		
		if(hp <= 0)
			player_kills++;
		
		return hp;
	}
	
	
	public int player_kills = 0;
	
	public void idleHeal()
	{
		int hpblockstoheal = getCurrentAP() / 5;
		if(hpblockstoheal > 0)
		{
			em.createHealEffect(x,z);
			audio.PlayOneShot(sound_repair);
			current_hp += hpblockstoheal;
			
			if(current_hp > max_hp)
				current_hp = max_hp;
		}
	}
	
	
	
	public void moveInWorld(int _destination_x, int _destination_z, float _time_to_complete)
	{
		lerp_move = true;
		starting_pos =  em.CoordsGameTo3DEntiy(x, z);
		ending_pos = em.CoordsGameTo3DEntiy(_destination_x, _destination_z);
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
//		Application.Quit();
		gm.endGame(false);
		return true;
	}
	
	private Dictionary<MechUpgradeMode, List<UpgradeEntry>> mechupgrademode_entrieslists;
	private Dictionary<MechUpgrade, UpgradeEntry> mechupgrade_to_entries;
	
	public Texture upgrade_menu_entry_fins;
	public Texture upgrade_menu_entry_legs;
	public Texture upgrade_menu_entry_mountains;
	public Texture upgrade_menu_entry_marsh;
	
	public Texture upgrade_menu_entry_gundamage;
	public Texture upgrade_menu_entry_guncost;
	public Texture upgrade_menu_entry_gunrange;
	public Texture upgrade_menu_entry_armor;
	public Texture upgrade_menu_entry_dodge;
	
	public Texture upgrade_menu_entry_killscavenge;
	public Texture upgrade_menu_entry_extrapartsscavenge;
	public Texture upgrade_menu_entry_emptyscavenge;
	public Texture upgrade_menu_entry_costscavenge;
	
	public Texture upgrade_menu_entry_recallbase;
	public Texture upgrade_menu_entry_visionrange;
	public Texture upgrade_menu_entry_ap;
	public Texture upgrade_menu_entry_part_capacity;
	public Texture upgrade_menu_entry_healidle;
	
	public List<UpgradeEntry> getUpgradeEntries(MechUpgradeMode mode)
	{
		return mechupgrademode_entrieslists[mode];
	}
	
	public UpgradeEntry getUpgradeEntries(MechUpgrade  upgrade)
	{
		return mechupgrade_to_entries[upgrade];
	}
	
	public bool checkMechDodge()
	{ 
		Debug.Log("Custom dodge damage being used!!!!!!!!!!!!!!!!!!!!!");
		if(upgrade_combat_dodge && ((int) UnityEngine.Random.Range(0, 99.999999999999F)) < combat_dodge_percent)
		{
			return true;
		}
		return false;
	}
	
//public enum MechUpgrade { Move_Water, Move_Mountain, Move_Marsh, Move_Legs, Combat_Damage, Combat_Cost, Combat_Range, Combat_Armor, Combat_Dodge, 
//		Scavenge_Combat, Scavenge_Greed, Scavenge_Empty, Scavenge_Cost, Util_Recall, Util_Vision, Util_AP, Util_Heal, Util_Idle };
	private void createUpgradeMenuEntries()
	{
		mechupgrademode_entrieslists = new Dictionary<MechUpgradeMode, List<UpgradeEntry>>(); 
		mechupgrade_to_entries = new Dictionary<MechUpgrade, UpgradeEntry>();
		
		List<UpgradeEntry> movement_upgrades = new List<UpgradeEntry>();
		List<UpgradeEntry> combat_upgrades = new List<UpgradeEntry>();
		List<UpgradeEntry> scavenge_upgrades = new List<UpgradeEntry>();
		List<UpgradeEntry> utility_upgrades = new List<UpgradeEntry>();
//		 																					 			 		gear pis plt stru
		movement_upgrades.Add (new UpgradeEntry("Aquatic Fins", 		"Allows Water hex traversal (5 AP)",	1,	5,	1,	1,	2, upgrade_menu_entry_fins, MechUpgrade.Move_Water));
		movement_upgrades.Add (new UpgradeEntry("Mountaineering Claws",	"Allows Mountain hex traversal (5 AP)",	2,	4,	0,	2,	2, upgrade_menu_entry_mountains, MechUpgrade.Move_Mountain));
		movement_upgrades.Add (new UpgradeEntry("Marsh Stabilizers",	"Reduces Marsh hex traversal by 1 AP",	0,	1,	4,	4,	2, upgrade_menu_entry_marsh, MechUpgrade.Move_Marsh));
		movement_upgrades.Add (new UpgradeEntry("Re-engineered Frame",	"Reduces all hex traversal by 1 AP",	6,	3,	1,	3,	2, upgrade_menu_entry_legs, MechUpgrade.Move_Legs));
			 
		combat_upgrades.Add (new UpgradeEntry("Howizter Bore",			"Increases attack damage by 3",			1,	2,	4,	2,	2, upgrade_menu_entry_gundamage, MechUpgrade.Combat_Damage));
		combat_upgrades.Add (new UpgradeEntry("Efficient Reload", 		"Reduces attack cost by 2 AP",			2,	2,	0,	2,	2, upgrade_menu_entry_guncost,   MechUpgrade.Combat_Cost));
		combat_upgrades.Add (new UpgradeEntry("Targeting Optics", 		"Increases attack range by 1",			4,	1,	1,	2,	2, upgrade_menu_entry_gunrange,  MechUpgrade.Combat_Range));
		combat_upgrades.Add (new UpgradeEntry("Gilded Armor", 			"Reduces damage recieved by 2",			2,	0,	6,	0,	2, upgrade_menu_entry_armor,     MechUpgrade.Combat_Armor));
		combat_upgrades.Add (new UpgradeEntry("Reactive Manuever", 		"Gives 20% change to dodge attacks",	0,	4,	0,	4,	2, upgrade_menu_entry_dodge,     MechUpgrade.Combat_Dodge));
					 
		scavenge_upgrades.Add (new UpgradeEntry("Combat Salvage",		"Gain one random part from kills",		2,	3,	0,	2,	2, upgrade_menu_entry_killscavenge, MechUpgrade.Scavenge_Combat));
		scavenge_upgrades.Add (new UpgradeEntry("Greedy Gather", 		"Gain one extra part per scavenge",		0,	2,	1,	4,	2, upgrade_menu_entry_extrapartsscavenge, MechUpgrade.Scavenge_Greed));
		scavenge_upgrades.Add (new UpgradeEntry("Probing Sensors", 		"25% change to scavenge empty nodes",	4,	2,	0,	0,	2, upgrade_menu_entry_ap,            MechUpgrade.Scavenge_Empty));
		scavenge_upgrades.Add (new UpgradeEntry("Efficient Scavenge", 	"Reduceds cost of scavenge by 1",		2,	2,	2,	2,	2, upgrade_menu_entry_costscavenge,  MechUpgrade.Scavenge_Cost));
	  	scavenge_upgrades.Add (new UpgradeEntry("Expanded Cargohold", 	"Increase part capacity to 20",			2,	2,	2,	2,	2, upgrade_menu_entry_part_capacity, MechUpgrade.Util_Parts));
	 	
		utility_upgrades.Add (new UpgradeEntry("Augemented Perception",	"Increase vision range by 2",			1,	2,	0,	3,	2, upgrade_menu_entry_visionrange, MechUpgrade.Util_Vision));
		utility_upgrades.Add (new UpgradeEntry("Redline Reactor", 		"Increase max AP by 5",					3,	6,	2,	2,	2, upgrade_menu_entry_ap, MechUpgrade.Util_AP));
		utility_upgrades.Add (new UpgradeEntry("Town Recall",			"Click base to teleport home (6 AP)",	1,	4,	5,	0,	2, upgrade_menu_entry_recallbase, MechUpgrade.Util_Recall));
		utility_upgrades.Add (new UpgradeEntry("Idle Reconstruction", 	"Covert 5 AP into 1 HP at end of turn",	4,	0,	0,	4,	2, upgrade_menu_entry_healidle, MechUpgrade.Util_Idle));
	 
		mechupgrademode_entrieslists.Add(MechUpgradeMode.Combat, combat_upgrades);
		mechupgrademode_entrieslists.Add(MechUpgradeMode.Movement, movement_upgrades);
		mechupgrademode_entrieslists.Add(MechUpgradeMode.Scavenge, scavenge_upgrades);
		mechupgrademode_entrieslists.Add(MechUpgradeMode.Utility, utility_upgrades); 
		
		foreach(UpgradeEntry entry in movement_upgrades) 
			mechupgrade_to_entries.Add(entry.upgrade_type, entry); 
		
		foreach(UpgradeEntry entry in combat_upgrades) 
			mechupgrade_to_entries.Add(entry.upgrade_type, entry); 
		
		foreach(UpgradeEntry entry in scavenge_upgrades) 
			mechupgrade_to_entries.Add(entry.upgrade_type, entry); 
		
		foreach(UpgradeEntry entry in utility_upgrades) 
			mechupgrade_to_entries.Add(entry.upgrade_type, entry); 
		
		
	} 
	
	public void subtractPartCosts(UpgradeEntry entry)
	{
		part_count[Part.Gear] -= entry.gear_cost;
		part_count[Part.Plate] -= entry.plate_cost;
		part_count[Part.Piston] -= entry.piston_cost;
		part_count[Part.Strut] -= entry.strut_cost;
		current_ap -= entry.ap_cost;
	}
	
	public UpgradeCostFeedback checkUpgradeAffordable(UpgradeEntry entry)
	{  
		
		if(entry.ap_cost <= getCurrentAP())
			if(	part_count[Part.Gear] >= entry.gear_cost && 
				part_count[Part.Plate] >= entry.plate_cost && 
				part_count[Part.Piston] >= entry.piston_cost && 
				part_count[Part.Strut] >= entry.strut_cost)
				return UpgradeCostFeedback.Success;
			else 
				return UpgradeCostFeedback.NeedMoreParts; 
		else
			return UpgradeCostFeedback.NeedMoreAP;
	}
	
	
	
	public UpgradeCostFeedback checkUpgradeAffordable(MechUpgrade upgrade)
	{ 
		UpgradeEntry entry = mechupgrade_to_entries[upgrade];  
		
		if(entry.ap_cost <= getCurrentAP())
			if(	part_count[Part.Gear] >= entry.gear_cost && 
				part_count[Part.Plate] >= entry.plate_cost && 
				part_count[Part.Piston] >= entry.piston_cost && 
				part_count[Part.Strut] >= entry.strut_cost)
				return UpgradeCostFeedback.Success;
			else 
				return UpgradeCostFeedback.NeedMoreParts; 
		else
			return UpgradeCostFeedback.NeedMoreAP;
	}
	
	public bool canAffordUpgrade(MechUpgrade upgrade){
		return (checkUpgradeAffordable(upgrade) == UpgradeCostFeedback.Success);
	}
	public bool canAffordUpgrade(UpgradeEntry upgrade){
		return (checkUpgradeAffordable(upgrade) == UpgradeCostFeedback.Success);
	}
	
	
	private int mech_upgrade_count = 0;
	
	public int getUpgradeCount(){ return mech_upgrade_count; }
	
	
	public void applyUpgrade(MechUpgrade upgrade)
	{
		mech_upgrade_count++;
		audio.PlayOneShot(sound_upgrade_mech);
		subtractPartCosts(mechupgrade_to_entries[upgrade]); 
		switch(upgrade)
		{
			case MechUpgrade.Move_Legs:
				upgrade_move_cost = true;
				break;
			
			case MechUpgrade.Move_Marsh:
				upgrade_move_marsh = true;
				break;
			
			case MechUpgrade.Move_Mountain:
				upgrade_move_mountain = true;
				break;
			
			case MechUpgrade.Move_Water:
				upgrade_move_water = true;
				break;
			
			case MechUpgrade.Combat_Armor:
				upgrade_combat_armor = true;
			    base_armor += armor_upgrade;
				break;
			
			case MechUpgrade.Combat_Cost:
				upgrade_combat_cost = true;
				break;
			
			case MechUpgrade.Combat_Damage:
				upgrade_combat_damage = true;
				break;
			
			case MechUpgrade.Combat_Dodge:
				upgrade_combat_dodge = true;
				break;
			
			case MechUpgrade.Combat_Range:
				upgrade_combat_range = true;
				break;
			
			case MechUpgrade.Util_AP:
				max_ap += 5;
				upgrade_util_ap = true;				
				break;
			
			case MechUpgrade.Util_Parts:
				
				upgrade_util_parts = true;	
				break;
			
			case MechUpgrade.Util_Idle:
				upgrade_util_idle = true;	
				break;
			
			case MechUpgrade.Util_Recall:
				upgrade_util_recall = true;	
				break;
			
			case MechUpgrade.Util_Vision:
				sight_range += 2;
				updateFoWStates();
				upgrade_util_vision = true;	
				break;
			
			case MechUpgrade.Scavenge_Combat:
				upgrade_scavenge_combat = true;	
				break;
			
			case MechUpgrade.Scavenge_Cost:
				upgrade_scavenge_cost = true;	
				break;
			
			case MechUpgrade.Scavenge_Empty:
				upgrade_scavenge_empty = true;	
				break;
			
			case MechUpgrade.Scavenge_Greed:
				upgrade_scavenge_greed = true;	
				break; 
			
		}
	}
	
	
	public bool checkUpgrade(MechUpgrade upgrade)
	{ 
		switch(upgrade)
		{
			case MechUpgrade.Move_Legs:
				return upgrade_move_cost; 
			
			case MechUpgrade.Move_Marsh:
				return upgrade_move_marsh;
			
			case MechUpgrade.Move_Mountain:
				return upgrade_move_mountain;
			
			case MechUpgrade.Move_Water:
				return upgrade_move_water;
			
			case MechUpgrade.Combat_Armor:
				return upgrade_combat_armor;
			
			case MechUpgrade.Combat_Cost:
				return upgrade_combat_cost;
			
			case MechUpgrade.Combat_Damage:
				return upgrade_combat_damage;
			
			case MechUpgrade.Combat_Dodge:
				return upgrade_combat_dodge;
			
			case MechUpgrade.Combat_Range:
				return upgrade_combat_range;
			
			case MechUpgrade.Util_AP:
				return upgrade_util_ap;
			
			case MechUpgrade.Util_Parts:
				return upgrade_util_parts;
			
			case MechUpgrade.Util_Idle:
				return upgrade_util_idle ;
			
			case MechUpgrade.Util_Recall:
				return upgrade_util_recall;
			
			case MechUpgrade.Util_Vision:
				return upgrade_util_vision ;
			
			case MechUpgrade.Scavenge_Combat:
				return upgrade_scavenge_combat;
			
			case MechUpgrade.Scavenge_Cost:
				return upgrade_scavenge_cost;
			
			case MechUpgrade.Scavenge_Empty:
				return upgrade_scavenge_empty;
			
			case MechUpgrade.Scavenge_Greed:
				return upgrade_scavenge_greed; 
		}
		
		throw new System.Exception("Attempted to see if mech has an upgrade that doesn't exist.  Did you change the MechUpgrade enum to add moar st00f?");
	}
	
	public bool checkUpgradeEnabled(MechUpgrade upgrade)
	{
		//TODO add system for disabling upgrades based on level
		return true;
	}
}

