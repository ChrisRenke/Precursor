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
	
	private Facing direction_towards_actor = Facing.South;
	private bool   direction_in_range      = false; 
	
	private bool unreachable_hex_from_current_mech_location = false;
	
	private Facing direction_towards_enemy  = Facing.South;
	private bool   direction_in_range_enemy = false; 
  
	public List<HexData> town_adj_hexes;

	public bool node_occupier = false;
	private bool base_is_here = false;
	public bool mech_is_here = false;
		
	public gameManagerS  gm;
	public enginePlayerS ep;
	public entityManagerS em; 
	public hexManagerS hm; 
	
	private pathDrawS pd;
	void Awake(){ 
		gm = GameObject.Find("engineGameManager").GetComponent<gameManagerS>();
		ep = GameObject.Find("enginePlayer").GetComponent<enginePlayerS>();
		em = GameObject.Find("engineEntityManager").GetComponent<entityManagerS>();
//		print ("em is null: " + em == null);
		hm = GameObject.Find("engineHexManager").GetComponent<hexManagerS>();
//		print ("hm is null: " + hm == null);
		pd =  GameObject.Find("camera").GetComponent<pathDrawS>();
		
	}


	
	public void setDirectionTowardsActor(Facing direction){
		direction_towards_actor = direction;
		direction_in_range = true;	
	} 
	 
	public bool isLegitFacing(){
		return direction_in_range;
	}
	
	public Facing getDirectionTowardsActor(){
		return direction_towards_actor;
	}
	
	public void disableDirectionTowardsActor(){
		direction_in_range = false;
	}
	
	public void ControllerSelect(){ 
		createBorder();
		
		draw_mode = true;
			
		if(!border.active)
			border.active = true; 
		if(!glow.active)
			glow.active = true; 
		
		border.SetColor(ep.upgrade);
		border.Draw3DAuto();
		glow.Draw3DAuto();
	}
	
	public void ControllerDeselect(){ 
		border.active = false;
		glow.active = false; 
		
		draw_mode = false;
		clearBorders();
	}
	
	
	 
	public void setAsTownHex()
	{
		town_adj_hexes = new List<HexData>(hm.getAdjacentHexes(hex_data.x, hex_data.z));
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
		node_occupier = em.isNodeAt(hex_data.x, hex_data.z);
		if(node_occupier)
		{
			node_data = em.getNodeInfoAt(hex_data.x, hex_data.z);
		}
		
		return node_occupier;
	}
	
	public void updateFoWState()
	{
		hex_data = hm.getHex(hex_data.x, hex_data.z);
		
		switch(hex_data.vision_state)
		{
			case Vision.Live:
			renderer.material.SetColor("_Color", Color.white);
			Debug.Log("live!");
				break;
			case Vision.Visited:
			renderer.material.SetColor("_Color", Color.gray);
						Debug.Log("Visited!");
				break;
			case Vision.Unvisted:
			renderer.material.SetColor("_Color", Color.black);
						Debug.Log("Unvisted!");
				break;
		default: 
			renderer.material.SetColor("", Color.black);
				break;
		}
		
	}
	
	public void setBlack(){
		
		renderer.material.SetColor("", Color.black);
		
	}
	
	
	NodeData node_data;
	bool draw_mode = false;
	List<string> hex_display_text; 
	
	public void genTextString(SelectLevel select_level, int action_cost)
	{
		int current_ap = em.getMech().current_ap;
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
				
			case SelectLevel.AttempScavenge: 	
					if(current_ap >= action_cost)
						hex_display_text = new List<string>(){ "Attempt Scavenge",
															   node_data.node_level.ToString() + " " + node_data.node_type.ToString(),
															   "-" + action_cost + " AP"};//"<b><size=21></size></b>\n" ;
					else 
						hex_display_text = new List<string>(){ "<color=red>Attempt Scavenge</color>",
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
				
				if(current_ap >= em.getMech().getRecallAPCost()) 
					hex_display_text = new List<string>(){ "Recall",
															   "Town",
															   "-" + em.getMech().getRecallAPCost() + " AP"};
				else
					hex_display_text = new List<string>(){ "<color=red>Recall</color>",
															   "Town",
															   "<color=red>-" + em.getMech().getRecallAPCost() + " AP</color>"};
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
//				ep.hover_text);
//		}
//		 
//	}
	
	void createBorder()
	{ 
		border = pd.getSelectHex(hex_data);
		border.SetColor(ep.select_);
		glow = pd.getGlowHex(hex_data);
		glow.lineWidth = border.lineWidth * 1.75F;
		glow.SetColor(ep.glow);
	}
	
	void popFrontOfPath() 
	{
		path_display.removeFrontNode();
	}
	
	void OnMouseEnter()
	{
		if(!gm.mouse_over_gui)
		{
			createBorder();

			if(direction_in_range && !em.getMech().lerp_move)
				em.getMech().setFacingDirection(direction_towards_actor);
			
		}
		  
	}
	
	void OnMouseOver()
	{
		if(!gm.mouse_over_gui)
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
				
				border.SetColor(ep.select_);
				border.Draw3DAuto();
				glow.Draw3DAuto();
				HexData mech_hex = hm.getHex(em.getMech().x, em.getMech().z);
				
				
				if(base_is_here && Input.GetMouseButtonUp(1))
				{
					if(town_adj_hexes.Contains(mech_hex)){
						ep.displayBaseUpgradeMenu();
					}
					return;
				}
									
				if(mech_is_here && Input.GetMouseButtonUp(1))
				{
					ep.displayMechUpgradeMenu();
					return;
				}
				
				
				
				if(base_is_here){
					if(!town_adj_hexes.Contains(mech_hex)){
						genTextString(SelectLevel.Town, -1);
						ep.setRoute(null, hex_display_text, hex_data);	
					} 
					else	
					{ 
						genTextString(SelectLevel.TownUpgrade, -1);
						ep.setRoute(null, hex_display_text, hex_data);
					}
					border.SetColor(ep.upgrade); 
					return;
				}
				
				
				
				
				 
				//if mech standing on this hex
				if(mech_hex.Equals(hex_data))
				{
					
					//if the mech is on this hex, and this hex has a node as well
					if(node_occupier)
					{
	//					Debug.LogWarning("STANDING ON HEX WITH NODE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
						genTextString(SelectLevel.Scavenge, em.getMech().getScavengeAPCost());
						
						if(node_data.node_level != NodeLevel.Empty)
							border.SetColor(ep.scavenge);
						else
						{ 
							if(em.getMech().upgrade_scavenge_empty)
							{
								genTextString(SelectLevel.AttempScavenge, em.getMech().getScavengeAPCost());
								border.SetColor(ep.scavenge); 
							}
							else{
								
								genTextString(SelectLevel.Scavenge, -1);
								border.SetColor(ep.disable);
								
							}
						}
						ep.setRoute(null, hex_display_text, hex_data);
						return;
					}
					
					if(mech_is_here){
						genTextString(SelectLevel.MechUpgrade, -1);
						ep.setRoute(null, hex_display_text, hex_data);
						border.SetColor(ep.upgrade); 
						return;
					}
					
					ep.setRoute(null, null, hex_data);
					
					return;
				} 
				
				 
				
				if(can_attack_hex)
				{  
					genTextString(SelectLevel.Attack, em.getMech().getAttackAPCost());
					ep.setRoute(null, hex_display_text, hex_data);
					if(em.getMech().current_ap >= em.getMech().getAttackAPCost()){
						border.SetColor(ep.attack);
					}
					else
						border.SetColor(ep.disable);
						
					return;
				} 
				
				
				if(!em.getMech().canTraverse(hex_data))
					return;
				
				
				//if the path is no longer starting from where the player mechu currently is...
				
				  //the mech has moved                                //the mech has made another upgrade
				if(!mech_location_when_path_made.Equals(mech_hex)){
					
						if(path_display != null)
								path_display.destroySelf();
						
						can_attack_hex = false;
						
						
						path_from_mech = em.getMech().getPathFromMechTo(hm.getHex (hex_data.x, hex_data.z));
						path_display = pd.getPathLine(path_from_mech);
						
						last_mech_upgrade_count = em.getMech().getUpgradeCount();
						mech_location_when_path_made = mech_hex; 
						
						Debug.Log("Currently trying to find a path from hex " + x_DISPLAYONLY + "|" + z_DISPLAYONLY + " for round " + gm.current_round);
						if(path_from_mech == null)
						{
							unreachable_hex_from_current_mech_location = true; 
						}
						else{
							unreachable_hex_from_current_mech_location = false;
						}
				}
				else
				if((last_mech_upgrade_count != em.getMech().getUpgradeCount())) //|| path_display == null || path_display.path_line == null)
				{	
					if(path_display != null)
							path_display.destroySelf();
					
					can_attack_hex = false;
					
					
//					if(last_mech_upgrade_count != em.getMech().getUpgradeCount() && unreachable_hex_from_current_mech_location)
//					{
						path_from_mech = em.getMech().getPathFromMechTo(hm.getHex (hex_data.x, hex_data.z));
						path_display = pd.getPathLine(path_from_mech);
						
						last_mech_upgrade_count = em.getMech().getUpgradeCount();
						mech_location_when_path_made = mech_hex; 
						
						Debug.Log("Currently trying to find a path from hex " + x_DISPLAYONLY + "|" + z_DISPLAYONLY + " for round " + gm.current_round);
						if(path_from_mech == null)
						{
							unreachable_hex_from_current_mech_location = true; 
						}
						else{
							unreachable_hex_from_current_mech_location = false;
						}
//					}
				}
				else
				{
					nothing_changed_this_pass = true;
				}
				
//				if(built_path_with_current_mech_state  && ( path_display == null || path_display.path_line == null))
//				{
//					
//				}
				
					
//					if(!unreachable_hex_from_current_mech_location &&  last_mech_upgrade_count == em.getMech().getUpgradeCount())    //mech_location_when_path_made.Equals(mech_hex))
//					{
//						 
//						if(em.canTraverseHex(hex_data))
//						{
//							path_from_mech = em.getMech().getPathFromMechTo(hm.getHex (hex_data.x, hex_data.z));
//							path_display = pd.getPathLine(path_from_mech);
//							
//							last_mech_upgrade_count = em.getMech().getUpgradeCount();
//							mech_location_when_path_made = mech_hex; 
//							
//							Debug.Log("Currently trying to find a path from hex " + x_DISPLAYONLY + "|" + z_DISPLAYONLY + " for round " + gameManagerS.current_round);
//							if(path_from_mech == null)
//							{
//								unreachable_hex_from_current_mech_location = true; 
//							}
//							else{
//								unreachable_hex_from_current_mech_location = false;
//							}
//						}
						
//					}
				
				//if the mech could possibly walk here
				if(path_display != null && em.getMech().canTraverse(hex_data))
				{
					genTextString(SelectLevel.Move, (int) path_from_mech.TotalCost);
					
					if(path_display == null || path_display.path_line == null)
						path_display = pd.getPathLine(path_from_mech);
					
					ep.setRoute(path_display, hex_display_text, hex_data);
				}
				else
				{
					genTextString(SelectLevel.Disabled, -1);
					ep.setRoute(null, hex_display_text, hex_data);
				}
			}
		}
	}
	
	
	public bool nothing_changed_this_pass = false;
	
	void OnMouseUpAsButton()
	{		
		
		if(!gm.mouse_over_gui && !em.getMech().lerp_move)
		{
			HexData mech_hex = hm.getHex(em.getMech().x, em.getMech().z);//if mech standing on this hex
			
		
			
			if(mech_hex.Equals(hex_data))
			{
				
				//if the mech is on this hex, and this hex has a node as well
				if(node_occupier && em.getMech().current_ap >= em.getMech().getScavengeAPCost())
				{
					
					node_data = em.getNodeInfoAt(hex_data.x, hex_data.z);
					if(node_data.node_level!=NodeLevel.Empty)
					{
						em.getMech().scavengeParts(node_data.node_type, node_data.node_level, node_data.x, node_data.z);
						node_data = em.getNodeInfoAt(hex_data.x, hex_data.z);
						return;
					}
					else{ 
						if(em.getMech().upgrade_scavenge_empty)
						{ 
							Debug.Log("Scavenge empty attempt!");
							em.getMech().attemptScavenge(node_data.node_type, node_data.node_level, node_data.x, node_data.z);
						} 
						return;
					}
				}
				
			}
//			 
//			if(mech_is_here)
//			{  
//				em.getPlayer().displayMechUpgradeMenu();
//				return;
//			}
			
			//if your're selecting an enemy within range
			if(can_attack_hex)
			{ 
				if(em.getMech().current_ap >= em.getMech().getAttackAPCost())
				{
					em.getMech().attackEnemy(hex_data.x, hex_data.z);
					return;
				}
			}
			else
			if(path_from_mech != null && em.getMech().canTraverse(hex_data))
			{
				em.getMech().moveToHexViaPath(path_from_mech);
			}
		}
	}
	
	private int last_mech_upgrade_count = 0;
	
	
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
//		renderer.material.SetColor("", Color.gray);
	}
}
