using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public Image Arrow;
    private bool topScreen;
    private Camera main;
    private Quaternion rot1;
    private Quaternion rot2;
    private Quaternion currentRotaton;
	// Use this for initialization
	void Start () {
        main = Camera.main;
        rot1 = Arrow.transform.rotation;
        rot2 = Quaternion.Euler(0, 0, 180);
        currentRotaton = rot1;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        topScreen = main.GetComponent<CameraSwipe>().startingPosition;
        if (topScreen) {
            Arrow.transform.rotation = Quaternion.Lerp(currentRotaton, rot1, 0.05f);
        } else {
            Arrow.transform.rotation = Quaternion.Lerp(currentRotaton, rot2, 0.05f);
        }
        currentRotaton = Arrow.transform.rotation;
	}
}
