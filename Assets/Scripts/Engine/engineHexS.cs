using UnityEngine;
using System.Collections;

public class engineHexS : MonoBehaviour {
	 
	private HexData hex_data;
//	public  LineRenderer lr; 
	
	private VectorLine border;
	private PathDisplay path_display;
	private Path path_from_mech;
	private HexData mech_location_when_path_made;
	
	void Start(){
		
		border = pathDrawS.outlineHex(hex_data);
	}
	
	public void assignHexData_IO_LOADER_ONLY(HexData _hex_data)
	{ 
		hex_data = _hex_data;
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
	
	
	
	void OnMouseEnter()
	{
		if(hex_data.vision_state != Vision.Unvisted)
		{
//			if(border == null)
//				border = pathDrawS.outlineHex(hex_data);
			HexData mech_hex = hexManagerS.getHex(entityManagerS.getMech().x, entityManagerS.getMech().z);
			
			if(mech_hex.Equals(hex_data))
				return;
			
			//if the path is no longer starting from where the player mechu currently is...
			if(!mech_location_when_path_made.Equals(mech_hex))
			{
				path_from_mech = entityManagerS.getMech().getPathFromMechTo(hexManagerS.getHex (hex_data.x, hex_data.z));
				path_display = pathDrawS.getPathLine(path_from_mech);
			}
			
			if(!border.active)
				border.active = true;
			
			border.SetColor(enginePlayerS.hard_color);
			border.Draw3DAuto();
			
			if(path_display != null)
			{
				path_display.displayPath(); 
			}
		}
	}
	
	void OnMouseUpAsButton()
	{
		entityMechS.moveToHexViaPath(path_from_mech);
	}
	
	
	void OnMouseExit()
	{
		if(hex_data.vision_state != Vision.Unvisted)
		{ 
			border.active = false;
			
			if(path_display != null)
				path_display.hidePath();
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
	 
//		renderer.material.s
	    renderer.material.SetTextureOffset ("_MainTex", offset); 
	    renderer.material.SetTextureScale  ("_MainTex", size);
//		renderer.material.SetColor("_Color", Color.gray);
	}
}
