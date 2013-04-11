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
	
	
	Renderer water;
	Renderer mountain;
	Renderer legs;
	Renderer gunsize;
	Renderer gunrange;
	Renderer armor;
	Renderer basic;
	
	private Facing previous_facing = Facing.North;
	 
	
	public bool is_walking = false;
	
	private Dictionary<Facing, Material> facing_basic;
	private Dictionary<Facing, Material> facing_water;
	private Dictionary<Facing, Material> facing_mountain;
	private Dictionary<Facing, Material> facing_legs;
	private Dictionary<Facing, Material> facing_armor;
	private Dictionary<Facing, Material> facing_gunsize;
	private Dictionary<Facing, Material> facing_gunrange;
	
	Vector2 size; 
	
	
	void Awake()
	{ 
		facing_basic = new Dictionary<Facing, Material>();
		
		facing_armor = new Dictionary<Facing, Material>();
		facing_legs = new Dictionary<Facing, Material>();
		facing_gunrange = new Dictionary<Facing, Material>();
		facing_gunsize = new Dictionary<Facing, Material>();
		facing_water = new Dictionary<Facing, Material>();
		facing_mountain = new Dictionary<Facing, Material>();
		
		
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
		
		
		
		water 		= transform.FindChild("upgradeWater").renderer;
		mountain	= transform.FindChild("upgradeMountain").renderer;
		legs 		= transform.FindChild("upgradeLegs").renderer;
		gunsize 	= transform.FindChild("upgradeGunSize").renderer;
		gunrange 	= transform.FindChild("upgradeGunRange").renderer;
		armor 		= transform.FindChild("upgradeArmor").renderer;
		basic 		= transform.renderer;  	 
		
		
		
	    float sizeX = 1.0f / colCount;
	    float sizeY = 1.0f / rowCount;
	    size =  new Vector2(sizeX,sizeY);
		
	    basic.material.SetTextureScale  ("_MainTex", size);
	    water.material.SetTextureScale  ("_MainTex", size);
	    mountain.material.SetTextureScale  ("_MainTex", size);
	    legs.material.SetTextureScale  ("_MainTex", size);
	    gunsize.material.SetTextureScale  ("_MainTex", size);
	    gunrange.material.SetTextureScale  ("_MainTex", size);
	    armor.material.SetTextureScale  ("_MainTex", size);
		
		water.enabled = false;
		mountain.enabled = false;
		legs.enabled = false;
		gunrange.enabled = false;
		gunsize.enabled = false;
		armor.enabled = false;
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
//			if(previous_facing != owner.facing_direction)
				setToStanding(colCount,rowCount,rowNumber,colNumber,totalCells,fps);
			
				armor.enabled = owner.upgrade_combat_armor;
				legs.enabled = owner.upgrade_move_cost;
				mountain.enabled = owner.upgrade_move_mountain;
				water.enabled = owner.upgrade_move_water;
				gunrange.enabled = owner.upgrade_combat_range;
				gunsize.enabled = owner.upgrade_combat_damage;
			
//			previous_facing = owner.facing_direction;
//		    Vector2 offset = new Vector2(0,0);
//		    renderer.material.SetTextureOffset ("_MainTex", offset);
		}
	} 
 
	//SetSpriteAnimation
	void SetSpriteAnimation(int colCount ,int rowCount ,int rowNumber ,int colNumber,int totalCells,int fps ){
	 
	    // Calculate index
	    int index  = (int)(Time.time * fps);
	    // Repeat when exhausting all cells
	    index = totalCells - index % totalCells;
	  
	 
	    // split into horizontal and vertical index
	    var uIndex = index % colCount;
	    var vIndex = index / colCount;
	 
	    // build offset
	    // v coordinate is the bottom of the image in opengl so we need to invert.
	    float offsetX = (uIndex+colNumber) * size.x;
	    float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
	    Vector2 offset = new Vector2(offsetX,offsetY); 
		
	    basic.material.SetTextureOffset  ("_MainTex", offset);
	    water.material.SetTextureOffset  ("_MainTex", offset);
	    mountain.material.SetTextureOffset  ("_MainTex", offset);
	    legs.material.SetTextureOffset  ("_MainTex", offset);
	    gunsize.material.SetTextureOffset  ("_MainTex", offset);
	    gunrange.material.SetTextureOffset  ("_MainTex", offset);
	    armor.material.SetTextureOffset  ("_MainTex", offset);
	}
	
	
	
	void setToStanding(int colCount ,int rowCount ,int rowNumber ,int colNumber,int totalCells,int fps ){
	 
	    // Calculate index
//	    int index  = (int)(Time.time * fps);
	    // Repeat when exhausting all cells
	    int index =  17;
	  
	    // split into horizontal and vertical index
	    var uIndex = index % colCount;
	    var vIndex = index / colCount;
	 
	    // build offset
	    // v coordinate is the bottom of the image in opengl so we need to invert.
	    float offsetX = (uIndex+colNumber) * size.x;
	    float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
	    Vector2 offset = new Vector2(offsetX,offsetY);
	 
	    basic.material.SetTextureOffset  ("_MainTex", offset);
	    water.material.SetTextureOffset  ("_MainTex", offset);
	    mountain.material.SetTextureOffset  ("_MainTex", offset);
	    legs.material.SetTextureOffset  ("_MainTex", offset);
	    gunsize.material.SetTextureOffset  ("_MainTex", offset);
	    gunrange.material.SetTextureOffset  ("_MainTex", offset);
	    armor.material.SetTextureOffset  ("_MainTex", offset);
	}
	
	
	void Start()
	{
		owner = gameObject.GetComponent<entityMechS>();
	}
 
	void setFacing()
	{
	    basic.material = facing_basic[owner.facing_direction];
	    armor.material = facing_armor[owner.facing_direction];
	    legs.material = facing_legs[owner.facing_direction];
	    gunsize.material = facing_gunsize[owner.facing_direction];
	    gunrange.material = facing_gunrange[owner.facing_direction];
	    water.material = facing_water[owner.facing_direction];
	    mountain.material = facing_mountain[owner.facing_direction];
		
		
	}
}
