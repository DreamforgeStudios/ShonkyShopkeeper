using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TODOList : MonoBehaviour {
	public TextMeshProUGUI TODOText;

	public void UpdateScroll() {
		Debug.Log("Updating...");
		// Mess.............
		string txt = "";
		if (PersistentData.Instance.TownsUnlocked < 5) {
			txt += "Travel to a new town\n" +
			       "<color=#D2009D>" + Mathf.Clamp(Inventory.Instance.goldCount, 0, Travel.NextPurchaseCost()) + "</color>"  +
				   "/" + "<color=#D2009D>" + Travel.NextPurchaseCost() + "</color> gold\n\n";
		}

		if (PersistentData.Instance.TrueGolemsCrafted == 0) {
			txt += "Create <color=#" + ColorUtility.ToHtmlStringRGB(Quality.GradeToColor(Quality.QualityGrade.Mystic)) +
			       ">Mystic</color> Components\n" + "+\n" +
			       "Create more Golems";
		} else if (PersistentData.Instance.TrueGolemsCrafted < 4) {
			txt += "Create remaining " + (4 - PersistentData.Instance.TrueGolemsCrafted) + "True Golems";
		}

		TODOText.text = txt;
	}

	// Use this for initialization
	void Start () {
		InvokeRepeating("UpdateScroll", 0, 3);
	}
}
