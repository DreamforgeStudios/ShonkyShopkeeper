using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerController : PseudoScene {
	public Image AvatarImage;
	public TextMeshProUGUI PointsText, WinnerText;

	public override void Arrive() {
		base.Arrive();
		
		if (GameManager.Instance.PlayerInfos.Count <= 0)
			return;

		// TODO factor gold into points calculation.
		PlayerInfo winner = GameManager.Instance.PlayerInfos[0];
		foreach (var playerInfo in GameManager.Instance.PlayerInfos) {
			if (playerInfo.Points > winner.Points) {
				winner = playerInfo;
			}
		}

		AvatarImage.sprite = winner.Avatar;
		PointsText.text = string.Format("{0:N0} points", winner.Points);
		WinnerText.text = string.Format("Player {0} is the WINNER!!!", winner.Index + 1);
	}
}
