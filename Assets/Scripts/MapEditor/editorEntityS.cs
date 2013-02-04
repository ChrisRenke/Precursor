using UnityEngine;
using System.Collections;

public class editorEntityS : MonoBehaviour {
	
	public int x_coord;
	public int z_coord; 
	
	public editorEntityManagerS.Entity entity_type;
	public GUIStyle tooltipStyle;
	
	public int 			tile_num;
	public  int        menu_item_num; 
	public  bool       menu_item = false;
	  
	void OnGUI()
	{
		if(menu_item)
		{
			Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (transform.position);
			GUI.Label(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15,200,30), entity_type.ToString(), tooltipStyle);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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