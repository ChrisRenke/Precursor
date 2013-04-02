using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text; 

public abstract class entity_core : MonoBehaviour {
	
	public int x;
	public int z;
	public editor_entity ent_type;
	public string name;
	public bool menu_item;
	
	public abstract string getOutput();
	
	public StringBuilder makeOutputHeaderSB(){
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("\t\tENTITY{ " ); 
		sb.AppendLine("\t\t\tX          = " + x);
		sb.AppendLine("\t\t\tZ          = " + z);
		sb.AppendLine("\t\t\tType       = " + ent_type);
		return sb;
	}
		
	void OnMouseEnter()
	{
		if(menu_item)
		{
			transform.localScale += new Vector3(.15F, 0F, .15F); 
			transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
		}
	}
	
	void OnMouseExit()
	{
		if(menu_item)
		{
			transform.localScale -= new Vector3(.15F, 0F, .15F); 
			transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
		}
	}
}
