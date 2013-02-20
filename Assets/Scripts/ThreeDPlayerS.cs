using UnityEngine;
using System.Collections;

public class ThreeDPlayerS : MonoBehaviour {
	
	public Facing direction;
	bool rotating = false;
	public double moveTime = 0;
	public Quaternion ending_rot;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		 
			
//		if(rotating)
//		{	
//			transform.rotation = Vector3.Lerp(transform.rotation, ending_rot,  moveTime);
//     		moveTime += Time.deltaTime/3;
//			
//			
////			if(Vector3.Distance(transform.rotation, ending_rot) <= .05)
////			{ 
////				rotating = false;
////				transform.rotation = ending_rot;
////				moveTime = 0;
////			}	
//		} 
//		else
//		{
//				rotating = false;
//				transform.rotation = ending_rot;
//				moveTime = 0;
//		}
//		gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, to.rotation, Time.time * speed);
	}
	
	void OnGUI()
	{
		int gui_spacing = 20;
		if(GUI.Button(new Rect(gui_spacing, gui_spacing, 180, 40), "Random Rotate"))
		{ 
//			ending_rot = Orientation.facingOrientation((Facing)UnityEngine.Random.Range(0,5));
//			rotating = true;
		}
	}
}
