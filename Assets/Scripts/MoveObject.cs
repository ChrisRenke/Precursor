//using UnityEngine;
//using System.Collections;
//
//public class MoveObject : MonoBehaviour {
//
//	function Start () {
//		// Starting from the origin, move this object to 5 units along X by 10 units along Z, at 2.5 units per second
//		yield MoveObject.use.Translation(transform, Vector3.zero, Vector3(5.0, 0.0, 10.0), 2.5, MoveType.Speed);
//		// When that's done, simultaneously move up one unit and flip 180 degrees along the Z axis, doing both in half a second
//		MoveObject.use.Translation(transform, Vector3.up, .5, MoveType.Time);
//		MoveObject.use.Rotation(transform, Vector3.forward * 180.0, .5);
//	}
//	// Update is called once per frame
//	void Update () {
//	
//	}
//}
