using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mechDisplayS : MonoBehaviour {
		//vars for the whole sheet
	public int colCount ;
	public int rowCount ;
	 
	//vars for animation
	public int rowNumber   = 0; //Zero Indexed
	public int colNumber   = 0; //Zero Indexed
	public int totalCells;
	
	public int fps;
	
	public int frame_index;
	
  	//Maybe this should be a private var
    private Vector2 offset;
	
	private entityMechS owner; 
	
	public Material walk_n;
	public Material walk_ne;
	public Material walk_se;
	public Material walk_s;
	public Material walk_sw;
	public Material walk_nw;
	
//	public Material fin_walk_n;
//	public Material fin_walk_ne;
//	public Material fin_walk_se;
//	public Material fin_walk_s;
//	public Material fin_walk_sw;
//	public Material fin_walk_nw;
	
	public bool is_walking = false;
	
	private Dictionary<Facing, Material> facing_walks;
	
//	private bool draw_mode = false; 
	
	
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
		setFacing();
		
		if(owner.lerp_move)
		{
			 SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells,fps);
		}
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
	
	
	void Start()
	{
		owner = gameObject.GetComponent<entityMechS>();
	}
 
	void setFacing()
	{
	    renderer.material = facing_walks[owner.facing_direction];
		
	}
}
