using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class editorMenuEntityS : MonoBehaviour {
	
	public GameObject[] entities;
	public float radial_distance; 
	
	// Use this for initialization
	void Start () {
		editorUserS.selection_menu_displayed = true;
		int size = entities.Length;
		float interval = (Mathf.PI * 2)/size;
		 
		for(int i = 0; i < size; ++i)
		{
			float x =   Mathf.Cos(interval*i + (Mathf.PI/2))*radial_distance         + transform.position.x;
			float z = ((Mathf.Sin(interval*i + (Mathf.PI/2))*radial_distance)/1.16F) + transform.position.z;
			
			GameObject new_entity = (GameObject) Instantiate(entities[i], new Vector3(x, 5 + (.01F * i) + .01F, z), Quaternion.identity);	
			
			entity_core new_entity_s = new_entity.GetComponent<entity_core>();
			new_entity_s.menu_item = true; 
			switch(i){
			case 0:
			new_entity_s.ent_type = editor_entity.Town; break;
			case 1:
			new_entity_s.ent_type = editor_entity.Mech;break;
			case 2:
			new_entity_s.ent_type = editor_entity.Factory;break;
			case 3:
			new_entity_s.ent_type = editor_entity.Junkyard;break;
			case 4:
			new_entity_s.ent_type = editor_entity.Outpost;break;
			case 5:
			new_entity_s.ent_type = editor_entity.Enemy;break;
			case 6:
			new_entity_s.ent_type = editor_entity.Spawn;break;
			} 
			new_entity.transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
