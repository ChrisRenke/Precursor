using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text; 

public class info_town : entity_core {
	
//public enum BaseUpgrade { Level0, Level1, Level2, Level3}
//public enum BaseCategories  { Structure, Walls, Defense }; 
	
	public int       current_hp = 100;
	public int 	     max_hp = 100;
	
	public BaseUpgradeLevel structure_level = BaseUpgradeLevel.Level0;
	public BaseUpgradeLevel wall_level      = BaseUpgradeLevel.Level0;
	public BaseUpgradeLevel defense_level   = BaseUpgradeLevel.Level0;
	
	public  override string getOutput()
	{
		StringBuilder sb = makeOutputHeaderSB();
		sb.AppendLine("\t\t\tcurrent_hp = " + current_hp);
		sb.AppendLine("\t\t\tmax_hp     = " + max_hp);
		
		sb.AppendLine("\t\t\tWall_Lvl   = " + wall_level);
		sb.AppendLine("\t\t\tDef_lvl    = " + defense_level);
		sb.AppendLine("\t\t\tStruc_lvl  = " + structure_level);
        sb.AppendLine("\t\t}");
		return sb.ToString();
	}
	
}
