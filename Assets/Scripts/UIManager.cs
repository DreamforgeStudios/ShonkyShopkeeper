using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour {
    //public Image Arrow;
    private bool topScreen;
    private Camera main;
    private Quaternion rot1;
    private Quaternion rot2;
    private Quaternion currentRotaton;
    public Canvas mineDesignTravelIcons;
    public GameObject toolboxTools;

	// Use this for initialization
	void Start () {
        main = Camera.main;
        //rot1 = Arrow.transform.rotation;
        //rot2 = Quaternion.Euler(0, 0, 180);
        //currentRotaton = rot1;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //topScreen = main.GetComponent<CameraSwipe>().startingPosition;
        //if (topScreen) {
            //Arrow.transform.DORotate(rot1.eulerAngles, 0.7f);
        //} else {
            //Arrow.transform.DORotate(rot2.eulerAngles, 0.7f);
        //}
        //currentRotaton = Arrow.transform.rotation;
        if (main.GetComponent<CameraTap>().AtTopScreen()) {
            mineDesignTravelIcons.enabled = false;
            toolboxTools.SetActive(false);
        } else {
            mineDesignTravelIcons.enabled = true;
            toolboxTools.SetActive(true);
        }
	}
}
