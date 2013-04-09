using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class pathDrawS : MonoBehaviour {
	
	static Vector3 y_adj_line =new Vector3(0,1,0);
	static Vector3 y_adj_hex =new Vector3(0,10,0);
	static Vector3 y_adj_hex_glow =new Vector3(0,9.9F,0);
	static Vector3 y_adj_hex_sel =new Vector3(0,9.95F,0);
	static Vector3 xz_norm =new Vector3(1,0,1);
	
	public static float max_path_width = 12;
	 
	private static entityMechS  mech;
	static VectorLine player_route;
	// Use this for initialization
	void Start () { 
//		mech = GameObject.FindWithTag("player_mech").GetComponent<entityMechS>();
		mech = entityManagerS.getMech();
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	public static Color getColorFromCost(int cost)
	{	 
		if(cost <= 2)
		{
			return enginePlayerS.easy_color;
		}
		else if(cost <= 4)
		{
			return  enginePlayerS.medium_color;
		}
		else
		{
			return  enginePlayerS.hard_color;
		}
	
	}
	
	
	public static VectorLine getGlowHex(HexData hex)
	{
		MeshFilter ms = (MeshFilter)hex.hex_object.GetComponent("MeshFilter");
		Vector3[] verts = new Vector3[(ms.mesh.vertices.Length + 1)];
//		VectorLine
		for(int i = 0; i <  ms.mesh.vertices.Length; ++i)
			verts[i] = ms.mesh.vertices[i] + y_adj_hex + hex.hex_object.transform.position;
		
		verts[ms.mesh.vertices.Length] = ms.mesh.vertices[0] + y_adj_hex_glow + hex.hex_object.transform.position;
		
		
		for(int i = 0; i < verts.Length; ++i)
			verts[i].y = 1;
			
		return new VectorLine("border", verts, null, 8F,LineType.Continuous, Joins.Weld); 
//		VectorLine.MakeLine("border", verts).Draw3DAuto();
	}
	
	
	public static VectorLine getSelectHex(HexData hex)
	{
		MeshFilter ms = (MeshFilter)hex.hex_object.GetComponent("MeshFilter");
		Vector3[] verts = new Vector3[(ms.mesh.vertices.Length + 1)];
//		VectorLine
		for(int i = 0; i <  ms.mesh.vertices.Length; ++i)
			verts[i] = ms.mesh.vertices[i] + y_adj_hex + hex.hex_object.transform.position;
		
		verts[ms.mesh.vertices.Length] = ms.mesh.vertices[0] + y_adj_hex_sel + hex.hex_object.transform.position;
		
		
		for(int i = 0; i < verts.Length; ++i)
			verts[i].y = 1;
			
		return new VectorLine("border", verts, null, 8F,LineType.Continuous, Joins.Weld); 
//		VectorLine.MakeLine("border", verts).Draw3DAuto();
	}
	
	public static VectorLine outlineHex(HexData hex)
	{
		MeshFilter ms = (MeshFilter)hex.hex_object.GetComponent("MeshFilter");
		Vector3[] verts = new Vector3[(ms.mesh.vertices.Length + 1)];
//		VectorLine
		for(int i = 0; i <  ms.mesh.vertices.Length; ++i)
			verts[i] = ms.mesh.vertices[i] + y_adj_hex + hex.hex_object.transform.position;
		
		verts[ms.mesh.vertices.Length] = ms.mesh.vertices[0] + y_adj_hex + hex.hex_object.transform.position;
		
		
		for(int i = 0; i < verts.Length; ++i)
			verts[i].y = 1;
			
		return new VectorLine("border", verts, null, 8F,LineType.Continuous, Joins.Weld); 
//		VectorLine.MakeLine("border", verts).Draw3DAuto();
	}
	
	
	public static PathDisplay getPathLine(Path path)
	{
//			Debug.Log("drawPath numba J");
//			Debug.Log("awkward length " + 	path.getPathLength());
		
		float usable_ap = mech.current_ap;
		float max_ap = mech.max_ap;
	
		if(path != null)
		{
			Vector3[] path_spots = new Vector3[path.getPathLength()];
			Color[] path_colors  = new Color[path_spots.Length-1];
			float[] path_widths   = new float[path_spots.Length-1];
			int i = 0;
			int this_turns_travelable = -1;
			
			List<VectorLine> spots = new List<VectorLine>();
			
			
			foreach(HexData path_pos in path.getTraverseOrderList())
			{
//				path_spots[i] = Camera.mainCamera.WorldToScreenPoint((hexManagerS.CoordsGameTo3D(path_pos.x, path_pos.z)));
				
				path_spots[i] = ((hexManagerS.CoordsGameTo3D(path_pos.x, path_pos.z) + y_adj_line));
				
				if(i != 0)
				{
					path_colors[i-1] = getColorFromCost(path_pos.traversal_cost);
					
					usable_ap-=path_pos.traversal_cost;
					if(usable_ap >= 0)
					{
						path_widths[i-1] = usable_ap/max_ap * max_path_width/2 + max_path_width/2;
								
					
					}
					else
					{
						usable_ap = 1;
						if(this_turns_travelable == -1)
							this_turns_travelable = i-1;
						path_widths[i-1] = 2;
						path_colors[i-1] = enginePlayerS.disable_color;
						
						
//						break;
					}
					
					
//					var circle = new VectorLine("Circle", new Vector3[30], path_colors[i-1], null, 10F, LineType.Continuous, Joins.Weld);
//					circle.MakeCircle(path_spots[i], Vector3.up, .3F);
//					spots.Add(circle);
					VectorLine outline = outlineHex(path_pos);
					outline.SetColor(path_colors[i-1]);
					outline.lineWidth = usable_ap/max_ap * max_path_width/2 + max_path_width/4;
					spots.Add (outline);
				}
				
				
				
//				Debug.Log(path_spots[i] + " | " + path_pos.traversal_cost);
				i++;
			}
			 
			Array.Resize(ref path_spots, i+1);
			Array.Resize(ref path_colors, i);
			Array.Resize(ref path_widths, i);
			
//			for(int vc = 0; vc < lineColors.Length; ++vc)
//				lineColors[vc] = Color.red;
////				lineColors[vc] = new Color(UnityEngine.Random.Range(0,255), UnityEngine.Random.Range(0,255), UnityEngine.Random.Range(0,255));
			
			
//			VectorLine.SetLine (Color.green, path_spots);
//			VectorLine.Destroy(ref player_route);
			
			player_route = new VectorLine("Line", path_spots, path_colors, null, 5F, LineType.Continuous, Joins.Weld); 
//			player_route.MakeCircle(path_spots[3], Vector3.up, 2F, 10);
			player_route.SetColorsSmooth(path_colors);
			player_route.SetWidths(path_widths);
			player_route.MakeSpline(path_spots);
			player_route.smoothWidth = true;
			
			
//			if(this_turns_travelable != -1)
//			{
//				var circle = new VectorLine("Circle", new Vector3[30], null, 10F, LineType.Continuous, Joins.Weld);
//				circle.MakeCircle(path_spots[this_turns_travelable], Vector3.up, .3F);
//			
//			}
//			
			
//		VectorLine	rings = new VectorLine("Ring1", new Vector2[8], Color.white, null, 10F, LineType.Continuous);
//   
//	       rings.MakeCircle(path_spots[3], Vector3.up, 8F);
//	       rings.Draw3DAuto();
			
			return new PathDisplay(spots, player_route, new List<Color>(path_colors), new List<float>(path_widths));
//			player_route.Draw3DAuto();
		}
		
		return null;
		
	}
	
	
	
}

public class PathDisplay{
		
	public VectorLine       path_line;
	private List<VectorLine> spot_list;
	private List<Color>		 color_list;
	private List<float>      width_list;
	
	public PathDisplay(List<VectorLine> visits, VectorLine line,
					List<Color> _color, List<float> _widths)
	{
		path_line = line;
		spot_list = visits;
		color_list = _color;
		width_list = _widths;
	}

	public void removeFrontNode()
	{
		color_list.RemoveAt(0);
		width_list.RemoveAt(0);
		Vector3[] new_points = new Vector3[path_line.points3.Length -1];
		for(int i = 0; i < new_points.Length - 1; ++i)
		{
			new_points[i] = path_line.points3[i+1];
		}
		
		VectorLine.Destroy(ref path_line);
		path_line = new VectorLine("path", new_points, null, 5F ,LineType.Continuous,Joins.Weld);
		path_line.SetColorsSmooth(color_list.ToArray());
		path_line.SetWidths(width_list.ToArray());
		path_line.Draw3DAuto();
		
		VectorLine del_spot = spot_list[0];
		spot_list.RemoveAt(0);
		VectorLine.Destroy(ref del_spot);
	}
	
	public void destroySelf()
	{
		
		for(int i = 0; i < spot_list.Count; ++i)
		{ 
			VectorLine vl =  spot_list[i];
			VectorLine.Destroy(ref vl);
		}
		VectorLine.Destroy(ref path_line); 
	}
	
	
	public void displayPath()
	{
		foreach(VectorLine vl in spot_list)
		{
			vl.active = true;
			vl.Draw3DAuto();
		}
		
		path_line.active = true; 
		path_line.Draw3DAuto();
	}
	
	public void hidePath()
	{
		foreach(VectorLine vl in spot_list)
		{
			vl.active = false; 
		}
		if(path_line!=null)
			path_line.active = false; 
	}
	
	
	public bool Equals(object obj) 
	{
		if(obj == null)
			return false;
		
		if (!(obj is PathDisplay))
			return false;
		
		PathDisplay other_path = (PathDisplay) obj;
		return this.spot_list.Equals(other_path.spot_list); 
	}
	
}