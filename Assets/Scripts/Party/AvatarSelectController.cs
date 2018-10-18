using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class AvatarSelectController : PseudoScene {
	public Image AvatarImage;
	public List<Avatar> AvailableAvatars;
	public PlayerSelectController PlayerSelectController;
	//public PseudoSceneManager PseudoSceneManager;

	[ReadOnly]
	public List<Avatar> SelectedAvatars = new List<Avatar>();
	[ReadOnly]
	public int ActiveIndex = 0;

	// Use this for initialization
	void Start () {
		UpdateImage();
	}

	public void SelectAvatar() {
		SelectedAvatars.Add(AvailableAvatars[ActiveIndex]);

		if (SelectedAvatars.Count >= PlayerSelectController.ActiveNumberOfPlayers) {
			// TODO: don't hard code this?
			PseudoSceneManager.ChangeScene("AvatarConfirmation");
		}

		//AvailableAvatars.RemoveAt(ActiveIndex);
	}
	
	public void NextAvatar() {
		if (ActiveIndex >= AvailableAvatars.Count - 1) {
			return;
		}

		ActiveIndex++;
		UpdateImage();
	}

	public void PreviousAvatar() {
		if (ActiveIndex <= 0) {
			return;
		}

		ActiveIndex--;
		UpdateImage();
	}

	public void UpdateImage() {
		AvatarImage.sprite = AvailableAvatars[ActiveIndex].Sprite;
	}
	
	public override Tween Arrive(bool animate = true) {
		Tween t = base.Arrive(animate);
		
		// Reset avatars each time we load in.
		SelectedAvatars.Clear();

		return t;
	}
}
