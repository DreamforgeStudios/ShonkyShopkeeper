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
	public virtual Tween Arrive() {
		PseudoSceneManager.Fader.fillAmount = 1f;
		PseudoSceneManager.Fader.fillOrigin = (int) Image.OriginHorizontal.Right;
		
        gameObject.SetActive(true);
        if (WorldObjects != null) 
            WorldObjects.SetActive(true);
		
		return PseudoSceneManager.Fader.DOFillAmount(0, .5f).SetEase(Ease.InCubic)
			.OnComplete(() => PseudoSceneManager.Fader.raycastTarget = false); // Allow UI interaction.
	}
	
	// TODO, implement animations for each scene through this.
	// Remove scene.
	public virtual Tween Depart() {
		PseudoSceneManager.Fader.raycastTarget = true; // Prevent button presses.
		PseudoSceneManager.Fader.fillAmount = 0f;
		PseudoSceneManager.Fader.fillOrigin = (int) Image.OriginHorizontal.Left;

		return PseudoSceneManager.Fader.DOFillAmount(1, .5f).SetEase(Ease.InCubic)
			.OnComplete(() => {
				gameObject.SetActive(false);
				if (WorldObjects != null)
					WorldObjects.SetActive(false);
			});
	}
}
