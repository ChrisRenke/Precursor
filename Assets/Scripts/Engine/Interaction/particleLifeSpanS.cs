using UnityEngine;
using System.Collections;

public class particleLifeSpanS : MonoBehaviour {
	
	ParticleSystem ps;
	
	// Use this for initialization
	void Start () {
		ps = particleSystem;
	}
	
	// Update is called once per frame
	void Update () {
		if(ps)
		{
		    if(!ps.IsAlive())
		    {
		       Destroy(gameObject);
		    }
		}
	}
}
