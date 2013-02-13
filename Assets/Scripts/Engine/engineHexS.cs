using UnityEngine;
using System.Collections;

public class engineHexS : MonoBehaviour {
	 
	public  int x, z;
	public  Hex hex_type;
	public  LineRenderer lr; 
	
	public void buildHexData(int _x, int _z, Hex _type)
	{
		hex_type = _type;
		x = _x;
		z = _z; 
	}
//	
//	void Awake()
//	{
//// 		lr = GameObject.Find("lr").GetComponent<LineRenderer>();
//		lr = gameObject.AddComponent<LineRenderer>();
//		lr.useWorldSpace = false;
//		lr.SetVertexCount(7);
//	}
//	
//	void Update()
//	{
//
////        lr.SetPosition(0, entityManagerS.mech_s.gameObject.transform.position);
////
////        lr.SetPosition(1, entityManagerS.base_s.gameObject.transform.position);
//		
////		Mesh mesh = GetComponent<MeshFilter>().mesh;
////        Vector3[] vertices = mesh.vertices;
////        int i = 0;
//////		
//////            lr.SetPosition(0, vertices[0]+new Vector3(0,1,0));
//////		
//////            lr.SetPosition(1, vertices[3]+new Vector3(0,1,0));
////        while (i < vertices.Length) {
////            lr.SetPosition(i, vertices[i]+new Vector3(0,1,0)); 
////            i++;
////        }
////		lr.SetPosition(6, vertices[0]+new Vector3(0,1,0));
////        mesh.vertices = vertices;
////        mesh.RecalculateBounds();
//		
////		gameObject.transform.position.x +  (x * 2.30024F)/2 + z * -0.841947F, 0, z * 1.81415F + x * 1.3280592F);
//	}
	 
}
