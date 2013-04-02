using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text; 

public class info_node : entity_core {
	
//public enum BaseUpgrade { Level0, Level1, Level2, Level3}
//public enum BaseCategories  { Structure, Walls, Defense }; 
	
	public NodeLevel node_level = NodeLevel.Full;
	
	public override string getOutput()
	{
		StringBuilder sb = makeOutputHeaderSB();
		sb.AppendLine("\t\t\tnode_level = " + node_level); 
        sb.AppendLine("\t\t}");
		return sb.ToString();
	}
	
}
