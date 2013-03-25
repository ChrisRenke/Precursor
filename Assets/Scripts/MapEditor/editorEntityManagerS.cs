using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


public class editorEntityManagerS : MonoBehaviour {
	
	public static Dictionary<int, Dictionary<int, EntityData>> entity_db = new Dictionary<int, Dictionary<int, EntityData>>();  
	 
	  
	public GameObject  	player_entity;
	public GameObject  	enemy_entity;
	public GameObject  	outpost_entity;
	public GameObject  	junkyard_entity; 
	public GameObject  	factory_entity; 
	public GameObject  	base_entity; 
	
	public static Dictionary<EntityE, GameObject> entity_dict = new Dictionary<EntityE, GameObject>();
	public static Dictionary<Node, GameObject> node_dict	  = new Dictionary<Node, GameObject>();
	
	
	
	
	//player stuff
	public int     	   mech_starting_health_percentage = 100;
	
	//base stuff
	public int     	   base_starting_health_percentage = 100;
	
	//node stuff
	public int     	   node_starting_level = 2;  //0 empty, 1 sparse, 2 full	
	private string     node_label;
		
	//enemy stuff
	public bool 	   enemy_knows_mech_loc = false;
	public string	   enemy_knows_mech_loc_str_t = "Does NOT know MECH location";
	public string	   enemy_knows_mech_loc_str_f = "Knows MECH location";
	private string	   enemy_knows_mech_loc_str;
	public bool 	   enemy_knows_base_loc = false;
	public string	   enemy_knows_base_loc_str_t = "Does NOT know BASE location";
	public string	   enemy_knows_base_loc_str_f = "Knows BASE location";
	private string	   enemy_knows_base_loc_str;
	
	
	// Use this for initialization
	void Start () { 
		entity_dict.Add(EntityE.Player, player_entity);
		entity_dict.Add(EntityE.Enemy, enemy_entity);
		entity_dict.Add(EntityE.Base, base_entity); 
		
		node_dict.Add(Node.Outpost, outpost_entity);
		node_dict.Add(Node.Junkyard, junkyard_entity); 
		node_dict.Add(Node.Factory, factory_entity); 
		
		enemy_knows_mech_loc_str = enemy_knows_mech_loc_str_f;
		enemy_knows_base_loc_str = enemy_knows_base_loc_str_f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void playSelectTile()
	{
		audio.Play();
	}
	
	public bool occupied(int x, int z)
	{
		if(entity_db.ContainsKey(x))
		{
			if(entity_db[x].ContainsKey(z))
			{
				return true;
			}
		}
		return false;
	}
	
  	private GameObject InstantiateEntity(Vector3 pos, EntityE ent_type, int x, int z)
	{ 
		GameObject new_ent  = (GameObject) Instantiate(entity_dict[ent_type], pos, Quaternion.identity);
		editorEntityS ent_s = new_ent.GetComponent<editorEntityS>(); 
		
		ent_s.x_coord = x;
		ent_s.z_coord = z;
		ent_s.name = "entity("+ent_s.x_coord +"," + ent_s.z_coord+")";
		ent_s.entity_type = ent_type;
		
		return new_ent;
	}
	
	public void deleteEntity(editorEntityS entity_s)
	{
		entity_db[entity_s.x_coord].Remove(entity_s.z_coord);
		if(entity_db[entity_s.x_coord].Keys.Count == 0)
		{
			print("removing sub level");
			entity_db.Remove(entity_s.x_coord);
		}
		
		Destroy(entity_s.gameObject);
	}
	
	
	public void setEntityProperties(editorEntityS ent_s)
	{
		if(ent_s.entity_type == EntityE.Base)
		{
			ent_s.base_starting_health_percentage = base_starting_health_percentage;
		}
		else if(ent_s.entity_type == EntityE.Player)
		{
			ent_s.mech_starting_health_percentage = mech_starting_health_percentage;
		}
		else if(ent_s.entity_type == EntityE.Node)
		{
			ent_s.node_starting_level = node_starting_level;
		}
		else if(ent_s.entity_type == EntityE.Enemy)
		{
			ent_s.enemy_knows_base_loc = enemy_knows_base_loc;
			ent_s.enemy_knows_mech_loc = enemy_knows_mech_loc;
		} 
	}
	
	public void LoadEntity(EntityE ent_type, int x, int z)
	{
		Vector3 converted = editorUserS.CoordsGameTo3D(x, z);
		Vector3 adjusted  = new Vector3(converted.x, converted.y + 1F, converted.z + .5F);
		AddEntity(adjusted, ent_type, x, z);
		
	}
	
	
	public GameObject AddEntity(Vector3 pos, EntityE ent_type, int x, int z)
	{
	
		GameObject new_ent;
		editorEntityS new_ent_s;
		
		print ("ENTITY created @ " + x + ", " + z);
		
		if(entity_db.ContainsKey(x))
		{ 
			if(entity_db[x].ContainsKey(z))
			{
				
					//if there is already an entity there and we want to overwrite, overwrite it 
					if(entity_db[x][z].getEntityType() != editorUserS.last_created_entity_type)
					{
						new_ent   = InstantiateEntity(pos, ent_type, x, z);
						new_ent_s = new_ent.GetComponent<editorEntityS>();
						setEntityProperties(new_ent_s);
						
						//now delete what is already there
						Destroy(entity_db[x][z].getEntity());
						entity_db[x][z]= new EntityData(new_ent_s.name, new_ent, ent_type, x, z);  
						print ("--replacing : " + entity_db[x][z].getEntityType().ToString() + " with " + ent_type);
					}
					else
					{
					print ("--same Entity already exists there.");	
					}
			}
			else
			{
				//we've just gotta make a new z entry since nothing has ever been made in this z spot
				print ("--nothing ever in that z.");  
				
				new_ent = InstantiateEntity(pos, ent_type, x, z);
				new_ent_s = new_ent.GetComponent<editorEntityS>();
				setEntityProperties(new_ent_s);
				
				entity_db[x].Add(z,  new EntityData(new_ent_s.name, new_ent, ent_type, x, z));   
				
			}
		}
		else
		{
			//nothing has ever been made in this x row, so make the z dict and the z entry
			print ("--nothing ever in that x.");  
			new_ent = InstantiateEntity(pos, ent_type, x, z);
			new_ent_s = new_ent.GetComponent<editorEntityS>();
			setEntityProperties(new_ent_s);
			
			entity_db.Add(x, new Dictionary<int, EntityData>());
			entity_db[x].Add(z,  new EntityData(new_ent_s.name, new_ent, ent_type, x, z));  
			  
		}
		
		return entity_db[x][z].getEntity();
		 
	}
	
	
	public class EntityData {
		
		public int x_coord;
		public int z_coord;
		public string name;
		public EntityE entity_type;   
		public GameObject occupier;
		
		public EntityData(string _name, 
						GameObject _occupier,
						EntityE   _entity_type,
						int    _x_coord,
						int    _z_coord)
		{
			name 		= _name;
			x_coord     = _x_coord;
			z_coord     = _z_coord; 
			occupier    = _occupier;
			entity_type = _entity_type;
		} 
		
		public EntityE getEntityType()
		{
			return entity_type;
		}
		
		public GameObject getEntity()
		{
			return occupier;
		}
		
	}	
	
	void OnGUI()
	{
		if(editorUserS.entity_mode)
		{
			//draw Base config options
			if(editorUserS.last_created_entity_type == EntityE.Base)
			{
				base_starting_health_percentage = (int)GUI.HorizontalSlider(new Rect( 30, 70, 210, 30), base_starting_health_percentage, (float) 0, (float) 100);	
				GUI.Label(new Rect(250, 65, 150, 30),  base_starting_health_percentage + "% starting hp" );
			}
			else
			//draw Player config options
			if(editorUserS.last_created_entity_type == EntityE.Player)
			{ 
				mech_starting_health_percentage = (int)GUI.HorizontalSlider(new Rect( 30, 70, 210, 30), mech_starting_health_percentage, (float) 0, (float) 100);	
				GUI.Label(new Rect(250, 65, 150, 30),  mech_starting_health_percentage + "% starting hp");
			}
			else
			//draw node config options
			if(editorUserS.last_created_entity_type == EntityE.Node)
			{
				node_starting_level = (int)GUI.HorizontalSlider(new Rect( 30, 70, 210, 30), node_starting_level, (float) 0, (float) 2);	
				if(node_starting_level == 0)
					node_label = "Empty";
				else if(node_starting_level == 1)
					node_label = "Sparse";
				else
					node_label = "Full";
				GUI.Label(new Rect(250, 65, 100, 30),  node_label);
			}
			else
			//draw enemy config options
			if(editorUserS.last_created_entity_type == EntityE.Enemy)
			{
				if(GUI.Button(new Rect( 30, 70, 210, 30), enemy_knows_base_loc_str))
				{
					enemy_knows_base_loc  = !enemy_knows_base_loc;
					
					if(enemy_knows_base_loc)
						enemy_knows_base_loc_str = enemy_knows_base_loc_str_t;
					else
						enemy_knows_base_loc_str = enemy_knows_base_loc_str_f;
				} 
				
				if(GUI.Button(new Rect( 30, 110, 210, 30), enemy_knows_mech_loc_str))
				{
					enemy_knows_mech_loc  = !enemy_knows_mech_loc;
					
					if(enemy_knows_mech_loc)
						enemy_knows_mech_loc_str = enemy_knows_mech_loc_str_t;
					else
						enemy_knows_mech_loc_str = enemy_knows_mech_loc_str_f;
				} 
			} 
		}
			 
		
	}
}
