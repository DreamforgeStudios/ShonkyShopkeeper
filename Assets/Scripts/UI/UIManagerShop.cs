using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManagerShop : MonoBehaviour {
	//Order is WickedGrove, FlamingPeak, GiantsPass, SkyCity
    public List<GameObject> animatedTowns;


	// Use this for initialization
	void Start () {
	    SetTownBackground(Inventory.Instance.currentTown);
		SetTownMusic(Inventory.Instance.currentTown);
	}

	//Set all backgrounds as inactive then activate select one
    private void SetTownBackground(Travel.Towns currentTown)
    {
	    foreach (GameObject townBG in animatedTowns)
	    {
		    townBG.SetActive(false);
	    }

	    switch (currentTown)
	    {
		    case Travel.Towns.WickedGrove:
			    animatedTowns[0].SetActive(true);
			    break;
		    case Travel.Towns.FlamingPeak:
			    animatedTowns[1].SetActive(true);
			    break;
		    case Travel.Towns.GiantsPass:
			    animatedTowns[2].SetActive(true);
			    break;
		    case Travel.Towns.SkyCity:
			    animatedTowns[3].SetActive(true);
			    break;
		    default: 
			    animatedTowns[0].SetActive(true);
			    break;
	    }
    }

	private void SetTownMusic(Travel.Towns currentTown)
	{
		switch (currentTown)
		{
			case Travel.Towns.WickedGrove:
				SFX.Play("WickedGroveTrack",1f,1f,0f,true,0f);
				break;
			case Travel.Towns.FlamingPeak:
				SFX.Play("FlamingPeakTrack",1f,1f,0f,true,0f);
				break;
			case Travel.Towns.GiantsPass:
				SFX.Play("GiantsPassTrack",1f,1f,0f,true,0f);
				break;
			case Travel.Towns.SkyCity:
				SFX.Play("SkyCityTrack",1f,1f,0f,true,0f);
				break;
			default: 
				SFX.Play("WickedGroveTrack",1f,1f,0f,true,0f);
				break;
		}
	}
}