using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class editorMenuHexS : MonoBehaviour {
	
	public GameObject[] hexes;
	public float radial_distance;
	
	// Use this for initialization
	void Start () {
		editorUserS.selection_menu_displayed = true;
		int size = hexes.Length;
		float interval = (Mathf.PI * 2)/size;
		
		for(int i = 0; i < size; ++i)
		{
			float x =   Mathf.Cos(interval*i + (Mathf.PI/2))*radial_distance         + transform.position.x;
			float z = ((Mathf.Sin(interval*i + (Mathf.PI/2))*radial_distance)/1.16F) + transform.position.z;
			 
			GameObject new_hex            = (GameObject) Instantiate(hexes[i], new Vector3(x,  5 + (.01F * i) + .01F, z), Quaternion.identity);	
			
			editorHexS new_hex_script     = new_hex.GetComponent<editorHexS>();
			new_hex_script.menu_item 	  = true;
			new_hex_script.menu_item_num  = i;
//			new_hex_script.hex_type       = hexes[i].GetComponent<editorHexS>().hex_type;
			
			new_hex.transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
