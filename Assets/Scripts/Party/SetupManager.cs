﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour {
	// Use this for initialization
	void Start () {
		SFX.Play("Game_instrumental", looping: true);
	}
}
