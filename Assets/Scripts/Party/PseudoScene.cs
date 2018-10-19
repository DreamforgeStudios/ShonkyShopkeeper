using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NaughtyAttributes;

public class PseudoScene : MonoBehaviour {
	public GameObject WorldObjects;
	public PseudoSceneManager PseudoSceneManager;

	// Insert scene.
	public virtual Tween Arrive(bool animate = true) {
		// Not the cleanest solution to allowing animations to be stopped, but a reasonable one.
		if (!animate) {
			SetObjectsActive(true);
			return null;
		}
		
		PseudoSceneManager.Fader.fillAmount = 1f;
		PseudoSceneManager.Fader.fillOrigin = (int) Image.OriginHorizontal.Right;
		SFX.Play("Screen_wipe");
		
		SetObjectsActive(true);
		
		return PseudoSceneManager.Fader.DOFillAmount(0, .5f).SetEase(Ease.InCubic)
			.OnComplete(() => PseudoSceneManager.Fader.raycastTarget = false); // Allow UI interaction.
	}

	// TODO, implement animations for each scene through this.
	// Remove scene.
	public virtual Tween Depart(bool animate = true) {
		if (!animate) {
			SetObjectsActive(false);
			return null;
		}

		PseudoSceneManager.Fader.raycastTarget = true; // Prevent button presses.
        PseudoSceneManager.Fader.fillAmount = 0f;
        PseudoSceneManager.Fader.fillOrigin = (int) Image.OriginHorizontal.Left;
		SFX.Play("Screen_wipe");

        return PseudoSceneManager.Fader.DOFillAmount(1, .5f).SetEase(Ease.InCubic)
            .OnComplete(() => { SetObjectsActive(false); });
	}

	private void SetObjectsActive(bool value) {
        gameObject.SetActive(value);
        if (WorldObjects != null) 
            WorldObjects.SetActive(value);
	}
}
