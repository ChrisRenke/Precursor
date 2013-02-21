	using UnityEngine;
using System.Collections;

public class selectionHexS : Entity {
		//vars for the whole sheet
	public int colCount    = 4;
	public int rowCount    = 2;
	 
	//vars for animation
	public int rowNumber   = 0; //Zero Indexed
	public int colNumber   = 0; //Zero Indexed
	public int totalCells  = 8;
	
	public int frame_index = 1;
	
  	//Maybe this should be a private var
    private Vector2 offset;
	
	public SelectLevel select_level = SelectLevel.Easy;
	public Facing      direction_from_center;
	
	public  int action_cost;
	public  Hex hex_type;
	private bool draw_mode = false;
	public  EntityE occupier;
	public  NodeData node_data;
	
	private string display_text = "blank";
	private entityMechS mech;
	
	void Awake()
	{	
		mech    = GameObject.FindGameObjectWithTag("player_mech").GetComponent<entityMechS>();
	}
	
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
	
	void OnMouseEnter()
	{
		mech.setFacingDirection(direction_from_center);
//		entityMechS.a
	}
	
	public void genTextString()
	{
		if(select_level == SelectLevel.Scavenge)
		{
			display_text = node_data.node_level.ToString() + " " + node_data.node_type.ToString() + "\nScavenge Parts\n-" + action_cost + " AP";
		}else
		if(select_level == SelectLevel.Attack)
		{
			display_text = "Attack Enemy\n-" + action_cost + " AP";
		}
		else
		{
			if(occupier == EntityE.None)
			{
				display_text = hex_type.ToString() + "\n" + "-" + action_cost + " AP";
			}
			else if((occupier == EntityE.Node))
			{
				display_text = hex_type.ToString() + "\n"  + node_data.node_level.ToString() + " " + node_data.node_type.ToString() + "\n-" + action_cost + " AP";
			}
		}
	}
	
	void OnMouseOver()
	{
		
		if(Input.GetMouseButton(0))
		{
			switch(select_level)
			{
				case SelectLevel.Disabled:
					frame_index = 12;
					break;
				case SelectLevel.Easy:
					frame_index = 8;
					break;
				case SelectLevel.Medium:
					frame_index = 9;
					break;
				case SelectLevel.Hard:
					frame_index = 10;
					break;
				case SelectLevel.Scavenge:
					frame_index = 11;
					break;
				case SelectLevel.Attack:
					frame_index = 7;
					break;
			}
			Debug.Log("Hex selected");
		}
		else
		{
			switch(select_level)
			{
				case SelectLevel.Disabled:
					frame_index = 12;
					break;
				case SelectLevel.Easy:
					frame_index = 3;
					break;
				case SelectLevel.Medium:
					frame_index = 4;
					break;
				case SelectLevel.Hard:
					frame_index = 5;
					break;
				case SelectLevel.Scavenge:
					frame_index = 6;
					break;
				case SelectLevel.Attack:
					frame_index = 2;
					break;
			}
		}
		
		draw_mode = true;
	}
	
	void OnMouseUpAsButton()
	{
		if(select_level == SelectLevel.Scavenge)
		{
			mech.scavengeParts(node_data.node_type, node_data.node_level, node_data.x, node_data.z);
		}
		else if(select_level == SelectLevel.Attack)
		{
			mech.attackEnemy(x, z);
			Debug.LogWarning("ATTACKING TARGET @ " + x + "," + z);
			mech.allowSelectionHexesDraw();
		}
		else
		{
			mech.moveToHex(x, z, action_cost, occupier);
		}
		
		mech.destroySelectionHexes();
	}
	
		
	void OnGUI()
	{
		if(draw_mode)
		{
			Vector3 spot_on_screen = Camera.main.WorldToScreenPoint (transform.position);
//			GUI.DrawTexture(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15, 200,30), 
			GUI.Label(new Rect(spot_on_screen.x - 100, Screen.height - spot_on_screen.y - 15, 200,30),
				display_text, 
				enginePlayerS.hover_text);
		}
		 
	}
	void OnMouseExit()
	{
		if(select_level == SelectLevel.Disabled)
			frame_index = 12;		
		else if(select_level == SelectLevel.Attack)
			frame_index = 0;	
		else 
			frame_index = 1;
		
		draw_mode = false;
	}
	
	 
}
	