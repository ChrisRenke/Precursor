using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemyDisplayS : MonoBehaviour {
		//vars for the whole sheet
	public int colCount    = 4;
	public int rowCount    = 2;
	 
	//vars for animation
	public int rowNumber   = 0; //Zero Indexed
	public int colNumber   = 0; //Zero Indexed
	public int totalCells  = 8;
	
	public int fps = 10;
	
	public int frame_index = 0;
	
  	//Maybe this should be a private var
    private Vector2 offset;
	
	private entityEnemyS owner;
	private int col_index=0;
	private int row_index=0;
	
	public Material walk_n;
	public Material walk_ne;
	public Material walk_se;
	public Material walk_s;
	public Material walk_sw;
	public Material walk_nw;
//	private bool draw_mode = false; 
		
	private Dictionary<Facing, Material> facing_walks;
	
	void Awake()
	{ 
		facing_walks = new Dictionary<Facing, Material>();
		facing_walks.Add (Facing.North, walk_n);
		facing_walks.Add (Facing.SouthEast, walk_se);
		facing_walks.Add (Facing.NorthWest, walk_nw);
		facing_walks.Add (Facing.NorthEast, walk_ne);
		facing_walks.Add (Facing.South, walk_s);
		facing_walks.Add (Facing.SouthWest, walk_sw);
	}
	
	
	//Update
	void Update () { 
		setMaterial();
		
		if(owner.lerp_move)
		{
			 SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells,fps);
		}
//		SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells);  
	}
	
	void Start()
	{
		owner = gameObject.GetComponent<entityEnemyS>();
	}
 

	//SetSpriteAnimation
	void SetSpriteAnimation(int colCount ,int rowCount ,int rowNumber ,int colNumber,int totalCells,int fps ){
	 
	    // Calculate index
	    int index  = (int)(Time.time * fps);
	    // Repeat when exhausting all cells
	    index = index % totalCells;
	 
	    // Size of every cell
	    float sizeX = 1.0f / colCount;
	    float sizeY = 1.0f / rowCount;
	    Vector2 size =  new Vector2(sizeX,sizeY);
	 
	    // split into horizontal and vertical index
	    var uIndex = index % colCount;
	    var vIndex = index / colCount;
	 
	    // build offset
	    // v coordinate is the bottom of the image in opengl so we need to invert.
	    float offsetX = (uIndex+colNumber) * size.x;
	    float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
	    Vector2 offset = new Vector2(offsetX,offsetY);
	 
	    renderer.material.SetTextureOffset ("_MainTex", offset);
	    renderer.material.SetTextureScale  ("_MainTex", size);
	}
	void setMaterial()
	{
	    renderer.material = facing_walks[owner.facing_direction];
		
		if(hexManagerS.getHex(owner.x, owner.z).vision_state == Vision.Live)
	  		renderer.material.SetColor ("_Color", Color.red);
		else
			renderer.enabled = false;
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