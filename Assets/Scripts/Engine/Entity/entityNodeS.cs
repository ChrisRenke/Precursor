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
}
