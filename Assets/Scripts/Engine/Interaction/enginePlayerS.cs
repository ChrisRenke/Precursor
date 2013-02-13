using UnityEngine;
using System.Collections;

public class enginePlayerS : MonoBehaviour {
	
	public int 									maxZoom 	= 2;
	public int 									minZoom 	= 25;
	
	public GUIStyle								tooltip;
	
	
	public float								vSensitivity = 1.0F; 
	public float 								hSensitivity = 1.0F;
	public float 								zoomSensitivity = 1.0F;
	
	public static float  						camera_min_x_pos = 9999999999;
	public static float  						camera_max_x_pos = -999999999;
	public static float  						camera_min_z_pos = 9999999999;
	public static float  						camera_max_z_pos = -999999999;
	
	private static GameObject maincam;
	
	public GUIStyle getTooltip()
	{
		return tooltip;
	}
	
	// Use this for initialization
	void Awake () {  
		
		maincam = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update() {
		
		bool zoom_adjusted = false;
		
		Vector3 screenBottomLeft = maincam.camera.ScreenToWorldPoint(new Vector3(0,0,0));
		Vector3 screenTopRight = maincam.camera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0));
		
		Vector3 screenCenter = (screenBottomLeft + screenTopRight ) / 2;
		
		
		
		
		 
		if(( (Input.mousePosition.x > 30 &&  Input.mousePosition.x < 240 ) 
					&&
			 (Input.mousePosition.y < Screen.height - 30 &&  Input.mousePosition.y > Screen.height - 140 ))
			||
			((Input.mousePosition.x > Screen.width - 240 && Input.mousePosition.x < Screen.width - 30)
			&&
			 (Input.mousePosition.y < Screen.height - 30 &&  Input.mousePosition.y > Screen.height - 100 ))
			)
		{
			//do nothing if the mouse is over the gui element areas
		}
		else
		{
			//mouse elsewhere in screen
		}
		
		float w = Input.GetAxis("Mouse ScrollWheel");
		float zoom_adjust = w * zoomSensitivity;
		
		
		
		
		//zoom out
		if(Input.GetKey(KeyCode.Equals))
		{ 
			zoom_adjust += .5F * zoomSensitivity;
			zoom_adjusted = true;
		}
		
		//zoom in
		if(Input.GetKey(KeyCode.Minus))
		{
			zoom_adjust -= .5F * zoomSensitivity;	
			zoom_adjusted = true;
		}
		if(zoom_adjust != 0)
		{
			if(minZoom > maincam.camera.orthographicSize - zoom_adjust)
			{
				if(maxZoom < maincam.camera.orthographicSize - zoom_adjust)
				{
					maincam.camera.orthographicSize -= zoom_adjust;
					zoom_adjusted = true;
					
				}
				else
				{
					maincam.camera.orthographicSize = maxZoom;
					zoom_adjusted = true;
				}
			}
			else
			{	
				maincam.camera.orthographicSize = minZoom;
				zoom_adjusted = true;
			}
		}
		 

		
		if(Input.GetMouseButton(2) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
		{ 
			Vector3 trans_x = Vector3.back;
			Vector3 trans_y = Vector3.back;
			
			if(Input.GetMouseButton(2))
			{
				float h = Input.GetAxis("Mouse X");
				float v  = Input.GetAxis("Mouse Y");
				trans_y = Vector3.up * v * -1 * vSensitivity; 
				trans_x = Vector3.right * h * -1 * hSensitivity; 
			}
			
				
			if(Input.GetKey(KeyCode.D))
			{
				trans_x = Vector3.right * hSensitivity * 12F * Time.deltaTime; 
			} 
			if(Input.GetKey(KeyCode.A))
			{
				trans_x = Vector3.right * hSensitivity * -12F * Time.deltaTime; 
			} 
			
			
			if(Input.GetKey(KeyCode.W))
			{
				trans_y = Vector3.up * vSensitivity * 12F * Time.deltaTime; 
			} 
			if(Input.GetKey(KeyCode.S))
			{
				trans_y = Vector3.up * vSensitivity * -12F * Time.deltaTime; 
			} 
			
			if(trans_x != Vector3.back)
			{
				if(trans_x.x + screenTopRight.x > camera_max_x_pos)
					maincam.transform.position.Set(camera_max_x_pos, screenCenter.y, screenCenter.z);
				else if(trans_x.x + screenBottomLeft.x < camera_min_x_pos)
					maincam.transform.position.Set(camera_min_x_pos, screenCenter.y, screenCenter.z);
				else
					maincam.transform.Translate(trans_x);
			}
				
			if(trans_y != Vector3.back)
			{
				if(trans_y.y + screenTopRight.z > camera_max_z_pos)
					maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_max_z_pos);
				else
				if(trans_y.y + screenBottomLeft.z < camera_min_z_pos)
					maincam.transform.position.Set(screenCenter.x, screenCenter.y, camera_min_z_pos);
				else
					maincam.transform.Translate(trans_y);
			}
		}
		
		
		if(true)
		{ 
			Vector3 adjustedScreenBottomLeft = maincam.camera.ScreenToWorldPoint(new Vector3(0,0,0));
			Vector3 adjustedScreenTopRight = maincam.camera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,0));
			
			Vector3 adjustedScreenCenter = (adjustedScreenBottomLeft + adjustedScreenTopRight ) / 2;
			
			if(adjustedScreenTopRight.z > camera_max_z_pos)
			{
				print ("too far up"); 
				maincam.transform.position = new Vector3(maincam.transform.position.x, maincam.transform.position.y, maincam.transform.position.z -  (adjustedScreenTopRight.z - screenTopRight.z));
			}
			if(adjustedScreenBottomLeft.z <= camera_min_z_pos)
			{
				print ("too far down");
				maincam.transform.position = new Vector3(maincam.transform.position.x, maincam.transform.position.y, maincam.transform.position.z +  (adjustedScreenTopRight.z - screenTopRight.z));
			}
			
			if(adjustedScreenTopRight.x > camera_max_x_pos)
			{
				print ("too far right");
				maincam.transform.position = new Vector3(maincam.transform.position.x - (adjustedScreenTopRight.x - screenTopRight.x), maincam.transform.position.y, maincam.transform.position.z);
			}
			if(adjustedScreenBottomLeft.x <= camera_min_x_pos)
			{
				print ("too far left");
				maincam.transform.position = new Vector3(maincam.transform.position.x + (adjustedScreenTopRight.x - screenTopRight.x), maincam.transform.position.y, maincam.transform.position.z);
			}
		}
    }
	
	
	GameObject RaycastMouse(string _tag)
	{ 
		print (_tag);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
		RaycastHit[] results = Physics.RaycastAll(ray, 30);
		for(int i = 0; i < results.Length; ++i)
		{
			print (results[i].transform.gameObject.GetType() + " _ " + results[i].transform.gameObject.tag);
			if(results[i].transform.tag == _tag)
			{
            	print("Hit something with proper tag = " + _tag);
				return results[i].transform.gameObject;
			}
			
		} 
		print ("Nothing clicked.");
		return null; 
	}
//	{
//		RaycastHit hit;
//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
//		if (Physics.Raycast(ray, out hit, 100))
//		{
//			if(hit.transform.tag == TAG)
//			{
//            	print("Hit something with proper tag = " + TAG);
//				return hit.transform.gameObject;
////				editorHexS hex_tile = hit.transform.gameObject.GetComponent<editorHexS>();
////				if(!hex_tile.menu_item && hex_tile.tile_type != tileManagerS.Tiles.Perimeter)
////				{
////					audio.Play();
////					hex_tile.CloneRing(1);
////				}
//			}
//		} 
//		print ("nothing");
//		return null; 
//	}
	 
	
	
}

//
//					string nm = hit.transform.name;
//					GameObject hex = GameObject.Find(nm);
//					editorHexS hex_tile = hex.GetComponent<editorHexS>();
//					hex_tile.CloneNorth();

//-0.841947, 0, 1.81415