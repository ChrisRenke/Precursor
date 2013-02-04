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
			
			editorEntityS new_entity_s = new_entity.GetComponent<editorEntityS>();
			new_entity_s.menu_item = true;
			new_entity_s.menu_item_num = i; 
			
			new_entity.transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
