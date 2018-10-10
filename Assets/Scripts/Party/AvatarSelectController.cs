﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class AvatarSelectController : PseudoScene {
	public Image AvatarImage;
	public List<Sprite> AvailableAvatars;
	public PlayerSelectController PlayerSelectController;
	public PseudoSceneManager PseudoSceneManager;

	[ReadOnly]
	public List<Sprite> SelectedAvatars = new List<Sprite>();
	[ReadOnly]
	public int ActiveIndex = 0;

	// Use this for initialization
	void Start () {
		UpdateImage();
	}

	public void SelectAvatar() {
		SelectedAvatars.Add(AvailableAvatars[ActiveIndex]);

		if (SelectedAvatars.Count >= PlayerSelectController.ActiveNumberOfPlayers) {
			// TODO: don't hard code this.
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
		AvatarImage.sprite = AvailableAvatars[ActiveIndex];
	}
	
	public override void Arrive() {
		base.Arrive();
		// Reset avatars each time we load in.
		SelectedAvatars.Clear();
	}
}
