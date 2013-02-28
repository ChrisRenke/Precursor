using UnityEngine;
using System.Collections;

public class engineHexS : MonoBehaviour {
	 
	private HexData hex_data;
//	public  LineRenderer lr; 
	
	private VectorLine border;
	private VectorLine glow;
	private PathDisplay path_display;
	private Path path_from_mech;
	private HexData mech_location_when_path_made = new HexData(-10,-10, true);
	
	public bool node_occupier = false;
	
	void Start(){
		
//		border.minDrawIndex = 50;
		
//		glow = pathDrawS.outlineHex(hex_data);
//		glow.lineWidth = border.lineWidth * 1.45F;
//		glow.SetColor(enginePlayerS.glow_color);
//		glow.minDrawIndex = 49;
		
		
	}
	
	public void clearBorders()
	{
		VectorLine.Destroy(ref border);
		border = null;
	}
	
	public void assignHexData_IO_LOADER_ONLY(HexData _hex_data)
	{ 
		hex_data = _hex_data;
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
	string display_text; 
	
	public void genTextString(SelectLevel select_level, int action_cost)
	{
		
		switch(select_level)
		{
				
			case SelectLevel.Scavenge:
				if(action_cost == -999)					
					display_text = node_data.node_level.ToString() + " " + node_data.node_type.ToString();
				else
					display_text = node_data.node_level.ToString() + " " + node_data.node_type.ToString() + "\nScavenge Parts\n-" + action_cost + " AP";
				break;
				
			case SelectLevel.Attack:
				display_text = "Attack Enemy\n-" + action_cost + " AP";
				break;
				
			case SelectLevel.Travel:
				
				if(!node_occupier)
				{
					display_text = hex_data.hex_type.ToString() + "\n" + "-" + action_cost + " AP";
				}
				else
				{
					display_text = hex_data.hex_type.ToString() + "\n"  + node_data.node_level.ToString() + " " + node_data.node_type.ToString() + "\n-" + action_cost + " AP";
				}
				break;
			
			case SelectLevel.Disabled:
				display_text = "";
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
		border = pathDrawS.outlineHex(hex_data);
		border.SetColor(enginePlayerS.select_color);
	}
	
//	void popFrontOfPath() 
//	{
//		path_display.removeFrontNode();
//	}
//	
	void OnMouseEnter()
	{
		createBorder();
	}
	
	void OnMouseOver()
	{
		if(hex_data.vision_state != Vision.Unvisted)
		{
			
			
			draw_mode = true;
			
			if(!border.active)
				border.active = true; 
//			if(!glow.active)
//				glow.active = true; 
			
			border.SetColor(enginePlayerS.select_color);
			border.Draw3DAuto();
//			glow.Draw3DAuto();
			
			
			HexData mech_hex = hexManagerS.getHex(entityManagerS.getMech().x, entityManagerS.getMech().z);
			
			
			//if mech standing on this hex
			if(mech_hex.Equals(hex_data))
			{
				
				//if the mech is on this hex, and this hex has a node as well
				if(node_occupier)
				{
//					Debug.LogWarning("STANDING ON HEX WITH NODE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
					genTextString(SelectLevel.Scavenge, entityManagerS.getMech().getScavengeAPCost());
					
					if(entityManagerS.getMech().current_ap > entityManagerS.getMech().getScavengeAPCost() && node_data.node_level != NodeLevel.Empty)
						border.SetColor(enginePlayerS.scavenge_color);
					else
					{
						genTextString(SelectLevel.Scavenge, -999);
						border.SetColor(enginePlayerS.disable_color);
					}
					enginePlayerS.setRoute(null, display_text, hex_data);
					return;
				}
				enginePlayerS.setRoute(null, "", hex_data);
				
				return;
			}
			
			
			//if the path is no longer starting from where the player mechu currently is...
			if(!mech_location_when_path_made.Equals(mech_hex) || path_display == null || path_display.path_line == null)
			{	
				if(path_display != null)
					path_display.destroySelf();
				
				path_from_mech = entityManagerS.getMech().getPathFromMechTo(hexManagerS.getHex (hex_data.x, hex_data.z));
				path_display = pathDrawS.getPathLine(path_from_mech);
				mech_location_when_path_made = mech_hex;
			}
			
			
			//if the mech could possibly walk here
			if(path_display != null && entityManagerS.getMech().canTraverse(hex_data))
			{
				genTextString(SelectLevel.Travel, (int) path_from_mech.TotalCost);
				enginePlayerS.setRoute(path_display, display_text, hex_data);
			}
			else
			{
				genTextString(SelectLevel.Disabled, -1);
				enginePlayerS.setRoute(null, display_text, hex_data);
			}
		}
	}
	
	void OnMouseUpAsButton()
	{		
		
		HexData mech_hex = hexManagerS.getHex(entityManagerS.getMech().x, entityManagerS.getMech().z);//if mech standing on this hex
		if(mech_hex.Equals(hex_data))
		{
			//if the mech is on this hex, and this hex has a node as well
			if(node_occupier && entityManagerS.getMech().current_ap > entityManagerS.getMech().getScavengeAPCost())
			{
				entityManagerS.getMech().scavengeParts(node_data.node_type, node_data.node_level, node_data.x, node_data.z);
				node_data = entityManagerS.getNodeInfoAt(hex_data.x, hex_data.z);
			}
			return;
		}
		else
		if(path_from_mech != null && entityManagerS.getMech().canTraverse(hex_data))
		{
			entityMechS.moveToHexViaPath(path_from_mech);
		}
	}
	
	
	void OnMouseExit()
	{
		if(hex_data.vision_state != Vision.Unvisted)
		{ 
			border.active = false;
//			glow.active = false; 
			
			draw_mode = false;
			 clearBorders();
			
//			if(path_display != null)
//				path_display.hidePath();
//			border.StopDrawing3DAuto();
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
		   
		frame_index = (((9 - (int) hex_data.hex_type) * 3 ) - 1 ) - UnityEngine.Random.Range(0,3);  //pick a random variant
		
		print ("HEX TYPE: " +  hex_data.hex_type + " | FINDEX: " + frame_index);
	    
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
	  
	    renderer.material.SetTextureOffset ("_MainTex", offset); 
	    renderer.material.SetTextureScale  ("_MainTex", size); 
	}
}
