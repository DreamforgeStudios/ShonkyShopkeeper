using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManagerShop : MonoBehaviour {
    private Camera main;
    //public Canvas mineDesignTravelIcons;
    //public GameObject toolboxTools;
    public SpriteRenderer shopBG;
    public Sprite town1, town2, town3, town4;


	// Use this for initialization
	void Start () {
        main = Camera.main;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        /*
        if (main.GetComponent<CameraTap>().AtTopScreen()) {
            mineDesignTravelIcons.enabled = false;
            toolboxTools.SetActive(false);
        } else {
            mineDesignTravelIcons.enabled = true;
            toolboxTools.SetActive(true);
        }
        */
        DetermineBG();
	}

    private void DetermineBG() {
        //Debug.Log(Travel.ReturnCurrentTown());
        switch (Travel.ReturnCurrentTown()) {
            case Travel.Towns.WickedGrove:
                shopBG.sprite = town1;
                break;
            case Travel.Towns.FlamingPeak:
                shopBG.sprite = town2;
                break;
            case Travel.Towns.GiantsPass:
                shopBG.sprite = town3;
                break;
            case Travel.Towns.SkyCity:
                shopBG.sprite = town4;
                break;
        }
    }
}