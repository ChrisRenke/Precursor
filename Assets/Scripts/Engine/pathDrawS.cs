using UnityEngine;
using System.Collections;

public class pathDrawS : MonoBehaviour {
	
	static Vector3 y_adj =new Vector3(0,10,0);
	static Vector3 xz_norm =new Vector3(1,0,1);
	 
	
	static VectorLine player_route;
	// Use this for initialization
	void Start () { 
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	public static Color getColorFromCost(int cost)
	{	
//		if(cost <= 2)
//		{
//			return Color.green;
//		}
//		else if(cost <= 4)
//		{
//			return Color.yellow;
//		}
//		else
//		{
//			return Color.red;
//		}
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
	
	
	public static void drawPath(Path path)
	{
			Debug.Log("drawPath numba J");
			Debug.Log("awkward length " + 	path.getPathLength());
	
		if(path != null)
		{
			Vector3[] path_spots = new Vector3[path.getPathLength()];
			Color[] lineColors = new Color[path.getPathLength()-1];
			int i = 0;
			
			foreach(HexData path_pos in path)
			{
//				path_spots[i] = Camera.mainCamera.WorldToScreenPoint((hexManagerS.CoordsGameTo3D(path_pos.x, path_pos.z)));
				
				path_spots[i] = ((hexManagerS.CoordsGameTo3D(path_pos.x, path_pos.z) + y_adj));
				
				if(i != 0)
					lineColors[i-1] = getColorFromCost(path_pos.traversal_cost);
				
				Debug.Log(path_spots[i] + " | " + path_pos.traversal_cost);
				i++;
			}
			 
			
//			for(int vc = 0; vc < lineColors.Length; ++vc)
//				lineColors[vc] = Color.red;
////				lineColors[vc] = new Color(UnityEngine.Random.Range(0,255), UnityEngine.Random.Range(0,255), UnityEngine.Random.Range(0,255));
			
			
//			VectorLine.SetLine (Color.green, path_spots);
			VectorLine.Destroy(ref player_route);
			player_route = new VectorLine("Line", path_spots, lineColors, null, 3F, LineType.Continuous, Joins.Weld); 
			player_route.SetColorsSmooth(lineColors);
			player_route.Draw3DAuto();
		}
		
	}
}
