using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGlassButton : MonoBehaviour {
	public TutorialGlass Glass;
	public Sprite EnabledSprite, DisabledSprite;

	private bool glassEnabled = false;
	private Image image;

	void Start() {
		image = GetComponent<Image>();
	}

	public void Press() {
		glassEnabled = !glassEnabled;
		
		if (glassEnabled) {
			image.sprite = EnabledSprite;
			// TODO: set index depending on scene view...
			Glass.EnableOverlay();
		} else {
			image.sprite = DisabledSprite;
			Glass.DisableOverlay();
		}
	}
}
