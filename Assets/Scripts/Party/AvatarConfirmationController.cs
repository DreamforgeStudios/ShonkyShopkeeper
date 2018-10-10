using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarConfirmationController : PseudoScene {
	public GameObject LayoutObject, AvatarPrefab;

	public AvatarSelectController AvatarSelectController;

	// Use this for initialization
	void Start () {
	}

	public override void Arrive() {
		base.Arrive();
		
		// Clear any already selected avatars.
		int childCount = LayoutObject.transform.childCount;
		for (int i = childCount; i >= 0; i--) {
			Destroy(LayoutObject.transform.GetChild(i));
		}
		
		// Display each selected sprite.
		foreach (var sprite in AvatarSelectController.SelectedAvatars) {
			GameObject clone = Instantiate(AvatarPrefab);
			clone.GetComponent<Image>().sprite = sprite;
			clone.transform.SetParent(LayoutObject.transform);
		}
	}
}
