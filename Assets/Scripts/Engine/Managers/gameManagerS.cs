using UnityEngine;
using System.Collections;

public class gameManagerS : MonoBehaviour {
	
	public static Turn current_turn;
	public static int  current_round;
	
	public GameObject  selection_hex_input;
	public static GameObject selection_hex;
	
	void Awake()
	{
		selection_hex = selection_hex_input;
		current_turn  = Turn.Player;
		current_round = 1;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
