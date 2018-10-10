using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionController : PseudoScene {

	// Use this for initialization
	void Start () {
		// Temporary for now.
		// TODO: put this in menu screen?
		GameManager.Instance.ActiveGameMode = GameMode.Party;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
