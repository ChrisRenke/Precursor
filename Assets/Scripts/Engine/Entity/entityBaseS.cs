using UnityEngine;
using System.Collections;

public class entityBaseS : Combatable {
	
	// Use this for initialization
	void Start () {
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
