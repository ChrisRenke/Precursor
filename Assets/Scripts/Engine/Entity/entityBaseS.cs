using UnityEngine;
using System.Collections;

public class entityBaseS : Combatable {
	
	// Use this for initialization
	void Start () {
		x = 1;
		z = 5;
	}
	// Update is called once per frame
	void Update () {
	
	}

	
	#region implemented abstract members of Combatable
	public override int attackTarget (Combatable target)
	{
		throw new System.NotImplementedException ();
	}
	#endregion
}
