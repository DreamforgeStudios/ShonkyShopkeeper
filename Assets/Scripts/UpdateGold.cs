using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateGold : MonoBehaviour {
	private TextMeshProUGUI text;
	private string spriteString = "<sprite=0>";

	// Use this for initialization
	void Start () {
		text = GetComponent<TextMeshProUGUI>();
	}
	
	// Update is called once per frame
	void Update () {
		// For optimal performance, it would probably be better to trigger
		//  an update rather than doing it every frame.
		text.text = spriteString + " " + Inventory.Instance.goldCount;
	}
}
