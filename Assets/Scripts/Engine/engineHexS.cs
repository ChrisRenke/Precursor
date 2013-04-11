using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class engineHexS : MonoBehaviour {
	 
	private HexData hex_data; 
	public int x_DISPLAYONLY;
	public int z_DISPLAYONLY;
	private VectorLine border;
	private VectorLine glow;
	private PathDisplay path_display;
	private Path path_from_mech;
	private HexData mech_location_when_path_made = new HexData(-10,-10, true);
	public bool can_attack_hex = false;
	
	public List<HexData> town_adj_hexes;
	
	public bool node_occupier = false;
	private bool base_is_here = false;
	public bool mech_is_here = false;
	 
	
	public void ControllerSelect(){ 
		createBorder();
		
		draw_mode = true;
			
		if(!border.active)
			border.active = true; 
		if(!glow.active)
			glow.active = true; 
		
		border.SetColor(enginePlayerS.upgrade_color);
		border.Draw3DAuto();
		glow.Draw3DAuto();
	}
	
	public void ControllerDeselect(){ 
		border.active = false;
		glow.active = false; 
		
		draw_mode = false;
		clearBorders();
	}
	
	
	
	void Start(){
		
//		border.minDrawIndex = 50;
		
//		glow = pathDrawS.outlineHex(hex_data);
//		glow.lineWidth = border.lineWidth * 1.45F;
//		glow.SetColor(enginePlayerS.glow_color);
//		glow.minDrawIndex = 49;
		
		
	}
	
	public void setAsTownHex()
	{
		town_adj_hexes = new List<HexData>(hexManagerS.getAdjacentHexes(hex_data.x, hex_data.z));
		base_is_here = true;
	}
	 
	
	public void clearBorders()
	{
		VectorLine.Destroy(ref border);
		border = null;
		VectorLine.Destroy(ref glow);
		glow = null;
	}
	
	public void assignHexData_IO_LOADER_ONLY(HexData _hex_data)
	{ 
		hex_data = _hex_data;
		z_DISPLAYONLY = hex_data.z;
		x_DISPLAYONLY = hex_data.x;
	}
	
	public bool setNodePresence()
	{
		node_occupier = entityManagerS.isNodeAt(hex_data.x, hex_data.z);
		if(node_occupier)
		{
			node_data = entityManagerS.getNodeInfoAt(hex_data.x, hex_data.z);
		}
		
		return node_occupier;
	}
	
	public void updateFoWState()
	{
		hex_data = hexManagerS.getHex(hex_data.x, hex_data.z);
		
		switch(hex_data.vision_state)
		{
			case Vision.Live:
			renderer.material.SetColor("_Color", Color.white);
				break;
			case Vision.Visited:
			renderer.material.SetColor("_Color", Color.gray);
				break;
			case Vision.Unvisted:
			renderer.material.SetColor("_Color", Color.black);
				break;
		}
		
	}
	
	
	NodeData node_data;
	bool draw_mode = false;
	List<string> hex_display_text; 
	
	public void genTextString(SelectLevel select_level, int action_cost)
	{
		int current_ap = entityManagerS.getMech().current_ap;
		switch(select_level)
		{
				
			case SelectLevel.Scavenge: 	
				if(action_cost < 0)
					hex_display_text = new List<string>(){ "",
														   node_data.node_level.ToString() + " " + node_data.node_type.ToString(),
														   ""};//"<b><size=21></size></b>\n" ;
				else
					if(current_ap >= action_cost)
						hex_display_text = new List<string>(){ "Scavenge",
															   node_data.node_level.ToString() + " " + node_data.node_type.ToString(),
															   "-" + action_cost + " AP"};//"<b><size=21></size></b>\n" ;
					else 
						hex_display_text = new List<string>(){ "<color=red>Scavenge</color>",
															   node_data.node_level.ToString() + " " + node_data.node_type.ToString(),
															   "<color=red>-" + action_cost + " AP</color>"};//"<b><size=21></size></b>\n" ;
				 break;
				
			case SelectLevel.Attack: 
				if(current_ap >= action_cost)
					hex_display_text = new List<string>(){ "Attack", 
														   "-" + action_cost + " AP"};//"<b><size=21></size></b>\n" ;
				else
					hex_display_text = new List<string>(){ "<color=red>Attack</color>", 
														   "<color=red>-" + action_cost + " AP</color>"};//"<b><size=21></size></b>\n" ;
					
				break;
				
			case SelectLevel.Move:
				
				if(!node_occupier) 
					if(current_ap >= action_cost) 
						hex_display_text = new List<string>(){ "Move", 
															   "-" + action_cost + " AP"}; 
					else 
						hex_display_text = new List<string>(){ "<color=red>Move</color>",  "<color=red>-" + action_cost + " AP</color>"}; 
				else
					if(current_ap >= action_cost) 
						hex_display_text = new List<string>(){ "Move",
															   node_data.node_level.ToString() + " " + node_data.node_type.ToString(),
															   "-" + action_cost + " AP"};//"<b><size=21></size></b>\n" ;
					else
						hex_display_text = new List<string>(){ "<color=red>Move</color>",
															   node_data.node_level.ToString() + " " + node_data.node_type.ToString(),
															   "<color=red>-" + action_cost + " AP</color>"};//"<b><size=21></size></b>\n" ;
				break;
			
			case SelectLevel.Disabled:
				hex_display_text = new List<string>();
				break;
			
			case SelectLevel.Town:
				hex_display_text = new List<string>(){ "", "Town", "" };
				break;
			case SelectLevel.TownRecall:
				
				if(current_ap >= entityManagerS.getMech().getRecallAPCost()) 
					hex_display_text = new List<string>(){ "Recall",
															   "Town",
															   "-" + entityManagerS.getMech().getRecallAPCost() + " AP"};
				else
					hex_display_text = new List<string>(){ "<color=red>Recall</color>",
															   "Town",
															   "<color=red>-" + entityManagerS.getMech().getRecallAPCost() + " AP</color>"};
				break;
			case SelectLevel.TownUpgrade:
				hex_display_text = new List<string>(){ "Upgrade",
														   "Town" };
				break;
			case SelectLevel.MechUpgrade:
				hex_display_text = new List<string>(){ "Upgrade",
														  "Mech"};
				break;
			
		}
		
	}
	
//	void OnGUI()
//	{
//		if(draw_mode)
//		{
//			Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (transform.position);
////			GUI.DrawTexture(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15, 200,30), 
//			GUI.Label(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15, 200,30),
//				display_text, 
//				enginePlayerS.hover_text);
//		}
//		 
//	}
	
	void createBorder()
	{ 
		border = pathDrawS.getSelectHex(hex_data);
		border.SetColor(enginePlayerS.select_color);
		glow = pathDrawS.getGlowHex(hex_data);
		glow.lineWidth = border.lineWidth * 1.75F;
		glow.SetColor(enginePlayerS.glow_color);
	}
	
	void popFrontOfPath() 
	{
		path_display.removeFrontNode();
	}
	
	void OnMouseEnter()
	{
		if(!gameManagerS.mouse_over_gui)
			createBorder();
//		print (node_occupier + " node for this hex");
	}
	
	void OnMouseOver()
	{
		if(!gameManagerS.mouse_over_gui)
		{
			if(border == null)
				createBorder();
			
			if(hex_data.vision_state != Vision.Unvisted)
			{
				
				
				draw_mode = true;
				
				if(!border.active)
					border.active = true; 
				if(!glow.active)
					glow.active = true; 
				
				border.SetColor(enginePlayerS.select_color);
				border.Draw3DAuto();
				glow.Draw3DAuto();
				HexData mech_hex = hexManagerS.getHex(entityManagerS.getMech().x, entityManagerS.getMech().z);
				
				
				if(base_is_here){
					if(!town_adj_hexes.Contains(mech_hex)){
						genTextString(SelectLevel.Town, -1);
						enginePlayerS.setRoute(null, hex_display_text, hex_data);	
					} 
					else	
					{ 
						genTextString(SelectLevel.TownUpgrade, -1);
						enginePlayerS.setRoute(null, hex_display_text, hex_data);
					}
					border.SetColor(enginePlayerS.upgrade_color); 
					return;
				}
				
				
				
				
				 
				//if mech standing on this hex
				if(mech_hex.Equals(hex_data))
				{
					
					//if the mech is on this hex, and this hex has a node as well
					if(node_occupier)
					{
	//					Debug.LogWarning("STANDING ON HEX WITH NODE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
						genTextString(SelectLevel.Scavenge, entityManagerS.getMech().getScavengeAPCost());
						
						if(node_data.node_level != NodeLevel.Empty)
							border.SetColor(enginePlayerS.scavenge_color);
						else
						{ 
							genTextString(SelectLevel.Scavenge, -1);
							border.SetColor(enginePlayerS.disable_color);
						}
						enginePlayerS.setRoute(null, hex_display_text, hex_data);
						return;
					}
					
					if(mech_is_here){
						genTextString(SelectLevel.MechUpgrade, -1);
						enginePlayerS.setRoute(null, hex_display_text, hex_data);
						border.SetColor(enginePlayerS.upgrade_color); 
						return;
					}
					
					enginePlayerS.setRoute(null, null, hex_data);
					
					return;
				} 
				
				 
				
				if(can_attack_hex)
				{  
					genTextString(SelectLevel.Attack, entityManagerS.getMech().getAttackAPCost());
					enginePlayerS.setRoute(null, hex_display_text, hex_data);
					if(entityManagerS.getMech().current_ap >= entityManagerS.getMech().getAttackAPCost()){
						border.SetColor(enginePlayerS.attack_color);
					}
					else
						border.SetColor(enginePlayerS.disable_color);
						
					return;
				} 
				
				
				if(!entityManagerS.getMech().canTraverse(hex_data))
					return;
				
				
				//if the path is no longer starting from where the player mechu currently is...
				if(!mech_location_when_path_made.Equals(mech_hex) || path_display == null || path_display.path_line == null)
				{	
					if(path_display != null)
						path_display.destroySelf();
					
					if(entityManagerS.canTraverseHex(hex_data))
					{
						path_from_mech = entityManagerS.getMech().getPathFromMechTo(hexManagerS.getHex (hex_data.x, hex_data.z));
						path_display = pathDrawS.getPathLine(path_from_mech);
					}
					mech_location_when_path_made = mech_hex;
					can_attack_hex = false;
				}
				
				//if the mech could possibly walk here
				if(path_display != null && entityManagerS.getMech().canTraverse(hex_data))
				{
					genTextString(SelectLevel.Move, (int) path_from_mech.TotalCost);
					enginePlayerS.setRoute(path_display, hex_display_text, hex_data);
				}
				else
				{
					genTextString(SelectLevel.Disabled, -1);
					enginePlayerS.setRoute(null, hex_display_text, hex_data);
				}
			}
		}
	}
	
	void OnMouseUpAsButton()
	{		
		
		if(!gameManagerS.mouse_over_gui)
		{
			HexData mech_hex = hexManagerS.getHex(entityManagerS.getMech().x, entityManagerS.getMech().z);//if mech standing on this hex
			
			
			if(base_is_here)
			{
				if(town_adj_hexes.Contains(mech_hex)){
					enginePlayerS.displayBaseUpgradeMenu();
				}
				return;
			}
			
			if(mech_hex.Equals(hex_data))
			{
				
				//if the mech is on this hex, and this hex has a node as well
				if(node_occupier && entityManagerS.getMech().current_ap >= entityManagerS.getMech().getScavengeAPCost())
				{
					
					node_data = entityManagerS.getNodeInfoAt(hex_data.x, hex_data.z);
					if(node_data.node_level!=NodeLevel.Empty)
					{
						entityManagerS.getMech().scavengeParts(node_data.node_type, node_data.node_level, node_data.x, node_data.z);
						node_data = entityManagerS.getNodeInfoAt(hex_data.x, hex_data.z);
						return;
					}
					else{ 
						enginePlayerS.displayMechUpgradeMenu();
						return; 
					}
				}
				
			}
			 
			if(mech_is_here)
			{  
				enginePlayerS.displayMechUpgradeMenu();
				return;
			}
			
			//if your're selecting an enemy within range
			if(can_attack_hex)
			{ 
				if(entityManagerS.getMech().current_ap >= entityManagerS.getMech().getAttackAPCost())
				{
					entityManagerS.getMech().attackEnemy(hex_data.x, hex_data.z);
					return;
				}
			}
			else
			if(path_from_mech != null && entityManagerS.getMech().canTraverse(hex_data))
			{
				entityMechS.moveToHexViaPath(path_from_mech);
			}
		}
	}
	
	
	void OnMouseExit()
	{
		
			if(hex_data.vision_state != Vision.Unvisted)
			{ 
				if(border != null)
					border.active = false;
				if(glow != null)
					glow.active = false; 
				
				draw_mode = false;
				clearBorders();
				
//				if(path_display != null)
//					path_display.hidePath();
//				border.StopDrawing3DAuto();
			}
	}
	
	
	
	
	
	
	//vars for the whole sheet
	public int colCount    = 4;
	public int rowCount    = 8;
	 
	//vars for animation
	public int rowNumber   = 0; //Zero Indexed
	public int colNumber   = 0; //Zero Indexed
	public int totalCells  = 8;
	 
	
	
	public int frame_index = 0;
	 
	//SetSpriteAnimation
	public void SetVisiual(){
		   
		frame_index = (((9 - (int) hex_data.hex_type) * 3 ) - 1 ) - (int)UnityEngine.Random.Range(0,2.99999999999999F);  //pick a random variant
		
//		print ("HEX TYPE: " +  hex_data.hex_type + " | FINDEX: " + frame_index);
	    
		// Size of every cell
	    float sizeX = -1.0f / colCount;
	    float sizeY = -1.0f / rowCount;
	    Vector2 size =  new Vector2(sizeX,sizeY);
	 
	    // split into horizontal and vertical index
	    var uIndex = frame_index % colCount;
	    var vIndex = frame_index / colCount;
	 
	    // build offset
	    // v coordinate is the bottom of the image in opengl so we need to invert.
	    float offsetX = (uIndex+colNumber) * size.x;
	    float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
	    Vector2 offset = new Vector2(offsetX,offsetY);
	 
//		renderer.material.s
	    renderer.material.SetTextureOffset ("_MainTex", offset); 
	    renderer.material.SetTextureScale  ("_MainTex", size);
//		renderer.material.SetColor("_Color", Color.gray);
	}
}
