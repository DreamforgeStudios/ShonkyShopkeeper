using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class AvatarSelectController : PseudoScene {
	public Image AvatarImage;
	public List<Avatar> AvailableAvatars;
	public PlayerSelectController PlayerSelectController;
	//public PseudoSceneManager PseudoSceneManager;

	[ReadOnly]
	public List<Avatar> SelectedAvatars = new List<Avatar>();
	[ReadOnly]
	public List<Avatar> AvailableAvatarsInstance = new List<Avatar>();
	[ReadOnly]
	public int ActiveIndex = 0;

	// Use this for initialization
	void Start () {
		UpdateImage();
	}

	public void SelectAvatar() {
		SelectedAvatars.Add(AvailableAvatarsInstance[ActiveIndex]);

		if (SelectedAvatars.Count >= PlayerSelectController.ActiveNumberOfPlayers) {
			// TODO: don't hard code this?
			PseudoSceneManager.ChangeScene("AvatarConfirmation");
		}

		AvailableAvatarsInstance.RemoveAt(ActiveIndex);
		if (ActiveIndex < 0) {
			ActiveIndex = 0;
		} else if (ActiveIndex > AvailableAvatarsInstance.Count - 1) {
			ActiveIndex = AvailableAvatarsInstance.Count - 1;
		}

		Sequence seq = DOTween.Sequence();
		seq.Append(AvatarImage.transform.DOScale(0, .3f).SetEase(Ease.InBack));
		seq.AppendCallback(() => UpdateImage());
		seq.Append(AvatarImage.transform.DOScale(1, .5f).SetEase(Ease.OutBack));
		seq.Play();
	}
	
	public void NextAvatar() {
		if (ActiveIndex >= AvailableAvatarsInstance.Count - 1) {
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
		AvatarImage.sprite = AvailableAvatarsInstance[ActiveIndex].Sprite;
	}
	
	public override Tween Arrive(bool animate = true) {
		Tween t = base.Arrive(animate);
		
		// Reset avatars each time we load in.
		SelectedAvatars.Clear();
		AvailableAvatarsInstance.Clear();
		AvailableAvatarsInstance.AddRange(AvailableAvatars);
		UpdateImage();

		return t;
	}
}
