﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TODOList : MonoBehaviour {
	public TextMeshProUGUI TODOText;

	// Use this for initialization
	void Start () {
		// Mess.............
		string txt = "";
		if (PersistentData.Instance.TownsUnlocked < 4) {
			txt += "Travel to a new town.\n" +
			       "<color=#ffd253>" + Mathf.Clamp(0, Travel.NextPurchaseCost(), Inventory.Instance.goldCount) + "</color>"  +
				   "/" + "<color=#ffd253>" + Travel.NextPurchaseCost() + "</color> gold.\n\n";
		}

		if (PersistentData.Instance.TrueGolemsCrafted == 0) {
			txt += "Create <color=#" + ColorUtility.ToHtmlStringRGB(Quality.GradeToColor(Quality.QualityGrade.Mystic)) +
			       ">Mystic</color> Components.\n" + "+\n" +
			       "Create more Golems.";
		} else if (PersistentData.Instance.TrueGolemsCrafted < 4) {
			txt += "Create remaining " + (4 - PersistentData.Instance.TrueGolemsCrafted) + "True Golems";
		}

		TODOText.text = txt;
	}
}
