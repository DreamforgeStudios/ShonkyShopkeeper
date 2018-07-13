using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateRetriesText : MonoBehaviour {
	private TextMeshProUGUI text;

	void Start() {
		this.text = this.GetComponent<TextMeshProUGUI>();
	}

	void Update () {
		// Don't need to call this every frame, but it's fine for now.
		text.text = "Retries Remaining: " + GameManager.Instance.RetriesRemaining.ToString();
	}
}
