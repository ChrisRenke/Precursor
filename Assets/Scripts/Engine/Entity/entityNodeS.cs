using UnityEngine;
using System.Collections;

public class entityNodeS : Entity {
	
	public readonly EntityE   entity_type = EntityE.Node;
	public Node   node_type;
	public NodeLevel node_level;
	
	public NodeData getNodeData()
	{
		return  new NodeData(x, z, node_type, node_level);
	}
	
	
	public void updateFoWState()
	{
		HexData occupying_hex = hexManagerS.getHex(x, z);
		switch(occupying_hex.vision_state)
		{
		case Vision.Live:
			gameObject.renderer.enabled = true;
			renderer.material.SetColor("_Color", Color.white);
			break;
		case Vision.Visited:
			gameObject.renderer.enabled = true;
			renderer.material.SetColor("_Color", Color.gray);
			break;
		case Vision.Unvisted:
			gameObject.renderer.enabled = false;
			break;
		default:
			throw new System.Exception("update FoW Combatable error!!");
		}
	}
	
	
	//vars for the whole sheet
	public int colCount    = 3;
	public int rowCount    = 3;
	 
	//vars for animation
	public int rowNumber   = 0; //Zero Indexed
	public int colNumber   = 0; //Zero Indexed
	public int totalCells  = 9;
	public int frame_index = 0;
	
	 
	//SetSpriteAnimation
	public void SetVisiual(){
		   
		frame_index = ((int)node_level) + (((int) node_type ) * 3);
			
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
}
