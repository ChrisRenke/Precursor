using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text; 

public class info_mech : entity_core {
	
//public enum BaseUpgrade { Level0, Level1, Level2, Level3}
//public enum BaseCategories  { Structure, Walls, Defense }; 
	   
	public int      current_hp  = 30;
	public int 	    max_hp 	    = 30;
	
	public bool		gun_range	= false;
	public bool		gun_cost	= false;
	public bool		gun_damage	= false;
	
	public bool		mobi_cost	= false;
	public bool		mobi_water	= false;
	public bool		mobi_mntn	= false;

	public bool		ex_tele		= false;
	public bool		ex_armor	= false;
	public bool		ex_scav		= false;
	
	public override string getOutput()
	{
		StringBuilder sb = makeOutputHeaderSB();
		sb.AppendLine("\t\t\tcurrent_hp = " + current_hp);
		sb.AppendLine("\t\t\tmax_hp     = " + max_hp);
		
		sb.AppendLine("\t\t\tgun_range  = " + gun_range);
		sb.AppendLine("\t\t\tgun_cost   = " + gun_cost);
		sb.AppendLine("\t\t\tgun_damage = " + gun_damage);
		
		sb.AppendLine("\t\t\tmobi_cost  = " + mobi_cost);
		sb.AppendLine("\t\t\tmobi_water = " + mobi_water);
		sb.AppendLine("\t\t\tmobi_mntn  = " + mobi_mntn);
		
		sb.AppendLine("\t\t\tex_tele    = " + ex_tele);
		sb.AppendLine("\t\t\tex_armor   = " + ex_armor);
		sb.AppendLine("\t\t\tex_scav    = " + ex_scav);
        sb.AppendLine("\t\t}");
		return sb.ToString();
	}
	
}
