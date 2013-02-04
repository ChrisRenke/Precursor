using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class circleMenuTerrainScript : MonoBehaviour {
	
	public GameObject[] hexes;
	public float radial_distance;
	
	private LinkedList<GameObject> objs;
	
	// Use this for initialization
	void Start () {
		playerScript.selection_menu_displayed = true;
		int size = hexes.Length;
		float interval = (Mathf.PI * 2)/size;
		
//		print ("BUILDING MENU :");
		for(int i = 0; i < size; ++i)
		{
			float x = Mathf.Cos(interval*i + (Mathf.PI/2))*radial_distance          + transform.position.x;
			float z = ((Mathf.Sin(interval*i + (Mathf.PI/2))*radial_distance)/1.16F) + transform.position.z;
			GameObject new_hex = (GameObject) Instantiate(hexes[i], new Vector3(x, transform.position.y + (.01F * i), z), Quaternion.identity);	
			hexScript new_hex_script = new_hex.GetComponent<hexScript>();
			new_hex_script.menu_item = true;
			new_hex_script.menu_item_num = i;
//			new_hex_script.name      = "MENU ITEM : " + i;
//			print ("-----TILE TYPE : " + new_hex.GetType());
//			new_hex.
			new_hex.transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
