using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


public class editorEntityManagerS : MonoBehaviour {
	
	public static Dictionary<int, Dictionary<int, EntityData>> entity_db = new Dictionary<int, Dictionary<int, EntityData>>();  
	
	public enum 		Entity    {Player, Base, Enemy, Junkyard, Outpost, Factory};
	  
	public GameObject  	player_entity;
	public GameObject  	enemy_entity;
	public GameObject  	outpost_entity;
	public GameObject  	junkyard_entity; 
	public GameObject  	factory_entity; 
	public GameObject  	base_entity; 
	
	public static Dictionary<Entity, GameObject> entity_dict = new Dictionary<Entity, GameObject>();
	
	
	// Use this for initialization
	void Start () { 
		entity_dict.Add(Entity.Player, player_entity);
		entity_dict.Add(Entity.Enemy, enemy_entity);
		entity_dict.Add(Entity.Outpost, outpost_entity);
		entity_dict.Add(Entity.Junkyard, junkyard_entity); 
		entity_dict.Add(Entity.Factory, factory_entity); 
		entity_dict.Add(Entity.Base, base_entity); 
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
	
  	private GameObject InstantiateEntity(Vector3 pos, Entity ent_type, int x, int z)
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
	
	public GameObject AddEntity(Vector3 pos, Entity ent_type, int x, int z)
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
				entity_db[x].Add(z,  new EntityData(new_ent_s.name, new_ent, ent_type, x, z));   
				
			}
		}
		else
		{
			//nothing has ever been made in this x row, so make the z dict and the z entry
			print ("--nothing ever in that x.");  
			new_ent = InstantiateEntity(pos, ent_type, x, z);
			new_ent_s = new_ent.GetComponent<editorEntityS>();
			
			entity_db.Add(x, new Dictionary<int, EntityData>());
			entity_db[x].Add(z,  new EntityData(new_ent_s.name, new_ent, ent_type, x, z));  
			  
		}
		
		return entity_db[x][z].getEntity();
		 
	}
	
	
	public class EntityData {
		
		public int x_coord;
		public int z_coord;
		public string name;
		public editorEntityManagerS.Entity entity_type;   
		GameObject occupier;
		
		public EntityData(string _name, 
						GameObject _occupier,
						editorEntityManagerS.Entity   _entity_type,
						int    _x_coord,
						int    _z_coord)
		{
			name 		= _name;
			x_coord     = _x_coord;
			z_coord     = _z_coord; 
			occupier    = _occupier;
			entity_type = _entity_type;
		} 
		
		public editorEntityManagerS.Entity getEntityType()
		{
			return entity_type;
		}
		
		public GameObject getEntity()
		{
			return occupier;
		}
		
	}	
}
