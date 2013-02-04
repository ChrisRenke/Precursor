using UnityEngine;
using System.Collections;

public class hexS : MonoBehaviour {
	
    public GameObject prefab;
	
	
	public int x_coord;
	public int z_coord; 
	
	
	public tileManagerS.Tiles tile_type;
	public GUIStyle tooltipStyle;
	public int 			tile_num;
	
	public  int        menu_item_num; 
	public  bool       menu_item = false;
	
//	// Use this for initialization
//	void Start () { 
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
	
	void OnGUI()
	{
		if(menu_item)
		{
			Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (transform.position);
			GUI.Label(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15,200,30), tile_type.ToString(), tooltipStyle);
		}
	}

	
	
	
	
	
	
//	void OnMouseDown()
//	{
		
//		if(Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
//		{
//			playerS.tms.CreateHex(true, playerS.brush_size, playerS.last_created_hex_go, transform.position,  playerS.last_created_hex_type, x_coord, z_coord);
//		}
//		else
//		if(Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0))
//		{
//			playerS.last_created_hex_type = tile_type;
//			playerS.last_created_hex_go   = gameObject;
//		}
//		else
//		if(Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
//		{	
//			playerS.tms.CreateHex(true, 1, playerS.tms.border_hex, transform.position,  tileManagerS.Tiles.EditorTileA, x_coord, z_coord);
//		}
//		else
//		{
//			playerS.selected_hex = this.gameObject;
//			print (playerS.selected_hex);
//			if(terrain_menu != null)
//			{
//				Destroy(terrain_menu);
//			}
//			if(!menu_item)
//			{
//				terrain_menu = (GameObject) Instantiate(circleMenuPrefab, new Vector3(transform.position.x, transform.position.y + .5F, transform.position.z), Quaternion.identity);	
//			}
//		}
//	}
	
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
	{
		if(menu_item)
		{
			transform.localScale += new Vector3(.15F, 0F, .15F); 
		}
	}
	
	void OnMouseExit()
	{
		if(menu_item)
		{
			transform.localScale -= new Vector3(.15F, 0F, .15F); 
		}
	}
	
//	void OnMouseDrag()
//	{
//		print ("shitttt");
//		
//	}
	
}
