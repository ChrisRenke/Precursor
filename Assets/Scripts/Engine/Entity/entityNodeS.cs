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
}
