using UnityEngine;
using System.Collections;

public class editorHexS : MonoBehaviour {
	
	public int x_coord;
	public int z_coord; 
	 
	public Hex hex_type;   
	
	public int         menu_item_num; 
	public bool        menu_item = false;
	private bool 	   sampling_mode = false;
	
//	
//		
//	void OnGUI()
//	{
//		if(menu_item || sampling_mode)
//		{
//			Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (transform.position);
//			GUI.Label(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15,200,30), hex_type.ToString(), editorHexManagerS.tooltip);
//		}
//		 
//	}

	
	void OnMouseEnter()
{		// this hex is a menu or we're sampling textures with the hex eyedropper
		if(menu_item)
		{
			transform.localScale += new Vector3(.15F, 0F, .15F); 
			transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
		}
		
	}
	
	void OnMouseOver()
	{
		if((Input.GetKey(KeyCode.LeftAlt) && !editorUserS.entity_mode))
		{
			sampling_mode = true;
		}
		else
		{ 
			sampling_mode = false;
		}
	}
	
	void OnMouseExit()
	{
		if(menu_item)
		{
			transform.localScale -= new Vector3(.15F, 0F, .15F); 
			transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
		}
		sampling_mode = false;
	}
	
//	void OnMouseDrag()
//	{
//		print ("shitttt");
//		
//	}
	
}
