using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShonkyPen : MonoBehaviour {
    public GameObject[] shonkys;

	// Use this for initialization
	void Start () {
        shonkys = GameObject.FindGameObjectsWithTag("Shonky");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
