using UnityEngine;
using System.Collections;

public class editorHexS : MonoBehaviour {
	
	public int x_coord;
	public int z_coord; 
	 
	public editorHexManagerS.Hex hex_type; 
	public GUIStyle tooltipStyle;
	
	public int 		   tile_num; 
	public  int        menu_item_num; 
	public  bool       menu_item = false;
 
	private bool 	   sampling_mode = false;
		
	void OnGUI()
	{
		if(menu_item)
		{
			Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (transform.position);
			GUI.Label(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15,200,30), hex_type.ToString(), tooltipStyle);
		}
		
		if(sampling_mode)
		{
			print ("mouseover alt sample text popup");
			Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (transform.position);
			GUI.Label(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15,200,30), hex_type.ToString(), tooltipStyle);
		}
	}

		
//	void OnMouseUp()
//	{
//		
//		//if the radial is open
//		if(playerS.selection_menu_displayed)
//		{
//			print ("menu is displayed and mouse released.");
//			RaycastHit hit;
//    		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
//			if (Physics.Raycast(ray, out hit, 100))
//			{ 
//				print ("Hit Something...");
//				print ("------------------tag = " + hit.transform.tag);
//				print ("-----playerS.TAG = " + playerS.TAG);
//				if(hit.transform.tag == playerS.TAG)
//				{
//				
//					print ("...with the proper tag");
//					hexS hit_hex_script = hit.transform.GetComponent<hexS>();
//					
//					print ("_____----------phew_hex_script.name = " + hit_hex_script.name);
//					print ("_____-----phew_hex_script.menu_item = " + hit_hex_script.menu_item);
//					if(hit_hex_script.menu_item)
//					{ 
//	            		print("_____...Menu Item!! YAY!");
//						print("_____HEX TYPE: " + hit.transform.gameObject.GetType());
//						circleMenuTerrainS cmts = terrain_menu.GetComponent<circleMenuTerrainS>();
//						
//						int x = x_coord;
//						int z = z_coord;
//						Vector3 pos = transform.position;
//						playerS.tms.CreateHex(true, 1, cmts.hexes[hit_hex_script.menu_item_num], pos, hit_hex_script.tile_type,  x, z);
//						playerS.tms.playSelectTile();
//						Destroy(terrain_menu);
//						playerS.selection_menu_displayed = false;
////			
//					}
//					else{
//						print ("...non menu item");
//						Destroy(terrain_menu);
//						playerS.selection_menu_displayed = false;
//								
//					}
//				}
//				else
//				{
//					print ("...with the wrong tag.");
//					Destroy(terrain_menu);
//					playerS.selection_menu_displayed = false;
//				}
//				
//			}
//			else
//			{
//				print ("Did not hit anything");
//				Destroy(terrain_menu);
//				playerS.selection_menu_displayed = false;
//			}
//			
//		}
//		else
//		{
//			print ("Menu not dispayed");
//		}
//		
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
