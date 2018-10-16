using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerController : PseudoScene {
	public Image AvatarImage;
	public TextMeshProUGUI PointsText, WinnerText;
	
	private const int GOLD_MULTIPLIER = 20;

	public override Tween Arrive(bool animate = true) {
		Tween t = base.Arrive(animate);
		
		if (GameManager.Instance.PlayerInfos.Count <= 0)
			return t;

		PlayerInfo winner = GameManager.Instance.PlayerInfos.Aggregate((a, b) =>
			a.Points + a.Gold * GOLD_MULTIPLIER >= b.Points + b.Gold * GOLD_MULTIPLIER ? a : b);

		AvatarImage.sprite = winner.Avatar.Sprite;
		PointsText.text = string.Format("{0:N0} points", winner.Points + winner.Gold * GOLD_MULTIPLIER);
		WinnerText.text = string.Format("Player {0} is the WINNER!!!", winner.Index + 1);

		return t;
	}
}
