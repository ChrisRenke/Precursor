	using UnityEngine;
using System.Collections;

public class selectionHexS : MonoBehaviour {
		//vars for the whole sheet
	public int colCount    = 4;
	public int rowCount    = 2;
	 
	//vars for animation
	public int rowNumber   = 0; //Zero Indexed
	public int colNumber   = 0; //Zero Indexed
	public int totalCells  = 8;
	
	public int frame_index = 0;
	
  	//Maybe this should be a private var
    private Vector2 offset;
	
	public SelectLevel select_level = SelectLevel.Easy;
	
	//Update
	void Update () { 
		SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells);  
	}
 
	//SetSpriteAnimation
	void SetSpriteAnimation(int colCount ,int rowCount ,int rowNumber ,int colNumber,int totalCells){

	    // Size of every cell
	    float sizeX = 1.0f / colCount;
	    float sizeY = 1.0f / rowCount;
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
	
	void OnMouseOver()
	{
		if(Input.GetMouseButton(0))
		{
			switch(select_level)
			{
				case SelectLevel.Disabled:
					frame_index = 7;
					break;
				case SelectLevel.Easy:
					frame_index = 4;
					break;
				case SelectLevel.Medium:
					frame_index = 5;
					break;
				case SelectLevel.Hard:
					frame_index = 6;
					break;
			}
			Debug.Log("Hex selected");
		}
		else
		{
			switch(select_level)
			{
				case SelectLevel.Disabled:
					frame_index = 5;
					break;
				case SelectLevel.Easy:
					frame_index = 1;
					break;
				case SelectLevel.Medium:
					frame_index = 2;
					break;
				case SelectLevel.Hard:
					frame_index = 3;
					break;
			}
		}
	}
	
	void OnMouseExit()
	{
		if(select_level == SelectLevel.Disabled)
			frame_index = 7;		
		else
			frame_index = 0;
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
}
