﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManagerShop : MonoBehaviour {
    //private Camera main;
    //public Canvas mineDesignTravelIcons;
    //public GameObject toolboxTools;
    public SpriteRenderer shopBG;
    public Sprite town1, town2, town3, town4;
    public GameObject frostBG;


	// Use this for initialization
	void Start () {
        //main = Camera.main;
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

    // TODO: running and changing sprites every frame is bad for performance, try to run this once.
    private void DetermineBG() {
        //Debug.Log(Travel.ReturnCurrentTown());
        switch (Travel.ReturnCurrentTown()) {
            case Travel.Towns.WickedGrove:
                shopBG.sprite = town1;
                SetFrostBG(true);
                break;
            case Travel.Towns.FlamingPeak:
                shopBG.sprite = town2;
                SetFrostBG(false);
                break;
            case Travel.Towns.GiantsPass:
                shopBG.sprite = town3;
                SetFrostBG(true);
                break;
            case Travel.Towns.SkyCity:
                shopBG.sprite = town4;
                SetFrostBG(false);
                break;
        }
    }

    private void SetFrostBG(bool activate)
    {
        frostBG.SetActive(activate);
    }
}