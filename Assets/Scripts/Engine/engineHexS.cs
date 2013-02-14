using UnityEngine;
using System.Collections;

public class engineHexS : MonoBehaviour {
	 
	public  int x, z;
	public  Hex hex_type;
//	public  LineRenderer lr; 
	
	public void buildHexData(int _x, int _z, Hex _type)
	{
		hex_type = _type;
		x = _x;
		z = _z; 
	}
	
 	void Start()
	{
		
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
		   
		frame_index = (((9 - (int) hex_type) * 3 ) - 1 ) - UnityEngine.Random.Range(0,2);  //pick a random variant
		
		print ("HEX TYPE: " + hex_type + " | FINDEX: " + frame_index);
	    
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
