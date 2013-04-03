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
	
	public Material basic_n;
	public Material basic_ne;
	public Material basic_se;
	public Material basic_s;
	public Material basic_sw;
	public Material basic_nw;
	
	public Material armor_n;
	public Material armor_ne;
	public Material armor_se;
	public Material armor_s;
	public Material armor_sw;
	public Material armor_nw;
	
	public Material legs_n;
	public Material legs_ne;
	public Material legs_se;
	public Material legs_s;
	public Material legs_sw;
	public Material legs_nw;
	
	public Material gunsize_n;
	public Material gunsize_ne;
	public Material gunsize_se;
	public Material gunsize_s;
	public Material gunsize_sw;
	public Material gunsize_nw;
	
	public Material gunrange_n;
	public Material gunrange_ne;
	public Material gunrange_se;
	public Material gunrange_s;
	public Material gunrange_sw;
	public Material gunrange_nw;
	
	public Material mountain_n;
	public Material mountain_ne;
	public Material mountain_se;
	public Material mountain_s;
	public Material mountain_sw;
	public Material mountain_nw;
	
	public Material water_n;
	public Material water_ne;
	public Material water_se;
	public Material water_s;
	public Material water_sw;
	public Material water_nw;
	
//	public Material fin_walk_n;
//	public Material fin_walk_ne;
//	public Material fin_walk_se;
//	public Material fin_walk_s;
//	public Material fin_walk_sw;
//	public Material fin_walk_nw;
	
	public bool is_walking = false;
	
	private Dictionary<Facing, Material> facing_basic;
	private Dictionary<Facing, Material> facing_water;
	private Dictionary<Facing, Material> facing_mountain;
	private Dictionary<Facing, Material> facing_legs;
	private Dictionary<Facing, Material> facing_armor;
	private Dictionary<Facing, Material> facing_gunsize;
	private Dictionary<Facing, Material> facing_gunrange;
	
//	private bool draw_mode = false; 
	
	
	void Awake()
	{ 
		facing_basic = new Dictionary<Facing, Material>();
		facing_basic.Add (Facing.North, basic_n);
		facing_basic.Add (Facing.SouthEast, basic_se);
		facing_basic.Add (Facing.NorthWest, basic_nw);
		facing_basic.Add (Facing.NorthEast, basic_ne);
		facing_basic.Add (Facing.South, basic_s);
		facing_basic.Add (Facing.SouthWest, basic_sw);
		
		facing_armor.Add (Facing.North, armor_n);
		facing_armor.Add (Facing.SouthEast, armor_se);
		facing_armor.Add (Facing.NorthWest, armor_nw);
		facing_armor.Add (Facing.NorthEast, armor_ne);
		facing_armor.Add (Facing.South, armor_s);
		facing_armor.Add (Facing.SouthWest, armor_sw);
		
		facing_legs.Add (Facing.North, legs_n);
		facing_legs.Add (Facing.SouthEast, legs_se);
		facing_legs.Add (Facing.NorthWest, legs_nw);
		facing_legs.Add (Facing.NorthEast, legs_ne);
		facing_legs.Add (Facing.South, legs_s);
		facing_legs.Add (Facing.SouthWest, legs_sw);
		
		facing_gunrange.Add (Facing.North, gunrange_n);
		facing_gunrange.Add (Facing.SouthEast, gunrange_se);
		facing_gunrange.Add (Facing.NorthWest, gunrange_nw);
		facing_gunrange.Add (Facing.NorthEast, gunrange_ne);
		facing_gunrange.Add (Facing.South, gunrange_s);
		facing_gunrange.Add (Facing.SouthWest, gunrange_sw);
		
		facing_gunsize.Add (Facing.North, gunsize_n);
		facing_gunsize.Add (Facing.SouthEast, gunsize_se);
		facing_gunsize.Add (Facing.NorthWest, gunsize_nw);
		facing_gunsize.Add (Facing.NorthEast, gunsize_ne);
		facing_gunsize.Add (Facing.South, gunsize_s);
		facing_gunsize.Add (Facing.SouthWest, gunsize_sw);
		
		facing_mountain.Add (Facing.North, mountain_n);
		facing_mountain.Add (Facing.SouthEast, mountain_se);
		facing_mountain.Add (Facing.NorthWest, mountain_nw);
		facing_mountain.Add (Facing.NorthEast, mountain_ne);
		facing_mountain.Add (Facing.South, mountain_s);
		facing_mountain.Add (Facing.SouthWest, mountain_sw);
		
		facing_water.Add (Facing.North, water_n);
		facing_water.Add (Facing.SouthEast, water_se);
		facing_water.Add (Facing.NorthWest, water_nw);
		facing_water.Add (Facing.NorthEast, water_ne);
		facing_water.Add (Facing.South, water_s);
		facing_water.Add (Facing.SouthWest, water_sw);
	}
	
	
	
	//Update
	void Update () {  
		setFacing();
		
		if(owner.lerp_move)
		{
			 SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells,fps);
		}
		else
		{
			
		    Vector2 offset = new Vector2(0,0);
		    renderer.material.SetTextureOffset ("_MainTex", offset);
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
	
	void setToStanding(int colCount ,int rowCount ,int rowNumber ,int colNumber,int totalCells,int fps ){
	 
	    // Calculate index
//	    int index  = (int)(Time.time * fps);
	    // Repeat when exhausting all cells
	    int index =  17;
	 
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
	    renderer.material = facing_basic[owner.facing_direction];
		setToStanding(colCount,rowCount,rowNumber,colNumber,totalCells,fps);
		
	}
}
