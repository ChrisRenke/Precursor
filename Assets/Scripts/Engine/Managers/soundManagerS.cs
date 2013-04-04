using UnityEngine;
using System.Collections;

public class soundManagerS : MonoBehaviour {
	
	public AudioClip  button_press;
	public AudioClip  mech_walk;
	public AudioClip  explode_enemy;
	public AudioClip  gun_normal;
	public AudioClip  gun_big;
	public AudioClip  on_fire;
	public AudioClip  tile_desert;
	public AudioClip  tile_grass;
	public AudioClip  tile_water;
	public AudioClip  tile_swamp;
	public AudioClip  tile_mountain;
	public AudioClip  tile_farmland;
	public AudioClip  tile_forest;
	public AudioClip  teleport;
	public AudioClip  spawn_enemy;
	public AudioClip  heal;  
	
	public AudioClip  scavenge_1;
	public AudioClip  scavenge_2;
	public AudioClip  scavenge_3; 
	
	public AudioClip  upgrade_1;
	public AudioClip  upgrade_2;
	
	public AudioClip  upgrade_base_1;
	public AudioClip  upgrade_base_2;
	void Awake()
	{
		
//		DontDestroyOnLoad(transform.gameObject);
	}
	// Use this for initialization
	void Start () {
		audio.volume = 1F;
		audio.priority = 128;
		audio.ignoreListenerVolume = true;
		audio.rolloffMode = UnityEngine.AudioRolloffMode.Linear;
//		if(audio = null)
//		{
//			audio = new AudioSource();
//		}
	}
	
	public void playSpawn(){ 
		audio.PlayOneShot(spawn_enemy);
	}
	
	public void playTeleport(){ 
		audio.PlayOneShot(teleport);
	}
	
	public void playHeal(){audio.PlayOneShot(heal);}
	
	public void playScavenge(){
		int choice = (int)UnityEngine.Random.Range(1F,3.99999F);
		switch(choice)
		{
		case 1:
		audio.PlayOneShot(scavenge_1);
			break;
			
		case 2:
		audio.PlayOneShot(scavenge_2);
			break;
			
		case 3: 
		audio.PlayOneShot(scavenge_3);
			break;
		}
	}
	
	public void playUpgradeMech(){
		int choice = (int)UnityEngine.Random.Range(1F,2.99999F);
		switch(choice)
		{
		case 1:
		audio.PlayOneShot(upgrade_1);
			break;
			
		case 2:
		audio.PlayOneShot(scavenge_2);
			break;
			 
		}
	}
	
	
	public void playGunNormal(){
		audio.PlayOneShot(gun_normal);
	}	
	public void playGunBigl(){
		audio.PlayOneShot(gun_big);
	}
	public void playButtonPress(){
		audio.PlayOneShot(button_press);
	}
	public void playMechWalk(){
		
		if(audio == null)
			UnityEngine.AudioSource.Instantiate(gameObject);
		audio.PlayOneShot(mech_walk);
	}
	public void playExplodeEnemy(){
		audio.PlayOneShot(explode_enemy);
	}
}
