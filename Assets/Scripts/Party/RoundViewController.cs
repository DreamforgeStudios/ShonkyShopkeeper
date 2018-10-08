using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundViewController : PseudoScene {
	public TextMeshProUGUI RoundText, PlayerText, DescriptionText;
	public Image ScreenshotHolder;
	public Button BeginButton;

	// Use this for initialization
	void Start () {
		GameDatabase gd = Resources.Load<GameDatabase>("GameDatabase");
		
		// Only peek, because info is still needed during the games.
		RoundInfo nextRound = GameManager.Instance.RoundQueue.Peek();
		Game game = gd.Games.Find(x => x.SceneName == nextRound.GameSceneName);
		
		// + 1 because computers start from 0 but humans dont.
		RoundText.text = string.Format("Round {0}", nextRound.RoundNumber + 1);
		PlayerText.text = string.Format("Player {0} is next...", nextRound.PlayerIndex + 1);
		DescriptionText.text = string.Format("{0}", game.Description);

		ScreenshotHolder.sprite = game.Screenshot;
		
		// Change button behaviour to match the correct scene.
		BeginButton.onClick.AddListener(() => Initiate.Fade(game.SceneName, Color.black, 2f));
	}
}
