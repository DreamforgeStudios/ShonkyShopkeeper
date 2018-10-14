using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerController : PseudoScene {
	public Image AvatarImage;
	public TextMeshProUGUI PointsText, WinnerText;

	public override Tween Arrive(bool animate = true) {
		Tween t = base.Arrive(animate);
		
		if (GameManager.Instance.PlayerInfos.Count <= 0)
			return t;

		// TODO factor gold into points calculation.
		PlayerInfo winner = GameManager.Instance.PlayerInfos[0];
		foreach (var playerInfo in GameManager.Instance.PlayerInfos) {
			if (playerInfo.Points > winner.Points) {
				winner = playerInfo;
			}
		}

		AvatarImage.sprite = winner.Avatar.Sprite;
		PointsText.text = string.Format("{0:N0} points", winner.Points);
		WinnerText.text = string.Format("Player {0} is the WINNER!!!", winner.Index + 1);

		return t;
	}
}
