using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    public GameObject target;
    private Camera mainCamera;
	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;
        mainCamera.transform.LookAt(target.transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
