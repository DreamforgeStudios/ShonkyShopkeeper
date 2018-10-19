using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cinematic : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SFX.Play("MainMenuTrack", 1f, 1f, 0f, true, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ExitToShop()
	{
		Initiate.Fade("Shop", Color.black,2f);
	}
}
