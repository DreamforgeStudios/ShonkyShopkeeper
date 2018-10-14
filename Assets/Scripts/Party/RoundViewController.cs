using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundViewController : PseudoScene {
	public TextMeshProUGUI RoundText, PlayerText, DescriptionText;
	public Image ScreenshotHolder;
	public Button BeginButton;

	public override Tween Arrive() {
		Tween t = base.Arrive();

		GameDatabase gd = Resources.Load<GameDatabase>("GameDatabase");
		
		// Only peek, because info is still needed during the games.
		RoundInfo nextRound = GameManager.Instance.RoundQueue.Peek();
		Game game = gd.GetGameBySceneName(nextRound.GameSceneName);
		
		// + 1 because computers start from 0 but humans dont.
		RoundText.text = string.Format("Round {0}", nextRound.RoundNumber + 1);
		PlayerText.text = string.Format("Player {0} is next...", nextRound.PlayerIndex + 1);
		DescriptionText.text = string.Format("{0}", game.Description);

		ScreenshotHolder.sprite = game.Screenshot;
		
		// Change button behaviour to match the correct scene.
		BeginButton.onClick.AddListener(() => {
			// Even though we don't need it every time, it's cleaner to transfer every time anyway.
			GameManager.Instance.GemTypeTransfer = GameManager.Instance.PlayerInfos[nextRound.PlayerIndex].GemType;
			Initiate.Fade(game.SceneName, Color.black, 2f);
		});

		return t;
	}
}
