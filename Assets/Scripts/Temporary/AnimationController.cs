using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Animation>().Play("Default Take");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
