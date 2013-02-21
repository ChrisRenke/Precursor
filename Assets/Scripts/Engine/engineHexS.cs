using UnityEngine;
using System.Collections;

public class engineHexS : MonoBehaviour {
	 
	private HexData hex_data;
//	public  LineRenderer lr; 
	
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
//		renderer.material.SetColor("_Color", Color.gray);
	}
}
