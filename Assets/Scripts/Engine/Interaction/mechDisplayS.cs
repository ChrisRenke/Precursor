using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mechDisplayS : MonoBehaviour {
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
	
	private entityMechS owner;
	private int col_index=0;
	private int row_index=0;
	
//	private bool draw_mode = false;
	
	private Dictionary<Facing, int> direction_to_index;
	
	
	void Awake()
	{
		direction_to_index = new Dictionary<Facing, int>();
		direction_to_index.Add(Facing.North, 0);
		direction_to_index.Add(Facing.NorthEast, 1);
		direction_to_index.Add(Facing.SouthEast, 2);
		direction_to_index.Add(Facing.South, 3);
		direction_to_index.Add(Facing.SouthWest, 4);
		direction_to_index.Add(Facing.NorthWest, 5);
	}
	
	
	
	//Update
	void Update () { 
		SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells);  
	}
	
	void Start()
	{
		owner = gameObject.GetComponent<entityMechS>();
	}
 
	//SetSpriteAnimation
	void SetSpriteAnimation(int colCount ,int rowCount ,int rowNumber ,int colNumber,int totalCells){
		
		col_index = (int) owner.facing_direction;
		row_index = owner.upgrade_traverse_water ? 1 : 0;
		
		
		frame_index = col_index + 6 * row_index;
		frame_index +=  owner.upgrade_traverse_water ? 2 : 0;
		
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
//	
//	void OnMouseOver()
//	{
//		if(Input.GetMouseButton(0))
//		{
//			switch(select_level)
//			{
//				case SelectLevel.Disabled:
//					frame_index = 7;
//					break;
//				case SelectLevel.Easy:
//					frame_index = 4;
//					break;
//				case SelectLevel.Medium:
//					frame_index = 5;
//					break;
//				case SelectLevel.Hard:
//					frame_index = 6;
//					break;
//			}
//			Debug.Log("Hex selected");
//		}
//		else
//		{
//			switch(select_level)
//			{
//				case SelectLevel.Disabled:
//					frame_index = 5;
//					break;
//				case SelectLevel.Easy:
//					frame_index = 1;
//					break;
//				case SelectLevel.Medium:
//					frame_index = 2;
//					break;
//				case SelectLevel.Hard:
//					frame_index = 3;
//					break;
//			}
//		}
//		draw_mode = true;
//	}
//	
//		
//	void OnGUI()
//	{
//		if(draw_mode)
//		{
//			Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (transform.position);
//			GUI.Label(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15, 200,30),
//				hex_type.ToString() + "\n" + "-" + movement_cost + " AP", 
//				enginePlayerS.hover_text);
//		}
//		 
//	}
//	void OnMouseExit()
//	{
//		if(select_level == SelectLevel.Disabled)
//			frame_index = 7;		
//		else
//			frame_index = 0;
//		
//		draw_mode = false;
//	}
//	
//	
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
}
