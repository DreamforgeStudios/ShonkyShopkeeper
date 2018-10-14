using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class IntermissionController : PseudoScene {
	public GameObject PlayerInfoLayout;
	public PlayerInfoElement PlayerInfoElementPrefab;
	public ItemDatabase ItemDB;
	public TextMeshProUGUI HeadingText;

	public override Tween Arrive() {
		Tween t = base.Arrive();

		if (GameManager.Instance.RoundHistory.Count <= 0) {
			return t;
		}

		// Clear any previous things.
		int childCount = PlayerInfoLayout.transform.childCount;
		for (int i = childCount-1; i >= 0; i--) {
			Destroy(PlayerInfoLayout.transform.GetChild(i).gameObject);
		}

		GameDatabase gd = Resources.Load<GameDatabase>("GameDatabase");
		var lastRoundNode = GameManager.Instance.RoundHistory.First;
		HeadingText.text = string.Format("Round {0} -- {1} Game",
			lastRoundNode.Value.RoundNumber + 1,
			gd.GetGameBySceneName(lastRoundNode.Value.GameSceneName).Name);
		
		var idxRoundNode = lastRoundNode;
		// Go through all games in the past round.
		while (idxRoundNode != null && idxRoundNode.Value.RoundNumber == lastRoundNode.Value.RoundNumber) {
			var round = idxRoundNode.Value;
			
			PlayerInfo player = GameManager.Instance.PlayerInfos[round.PlayerIndex];
			PlayerInfoElement clone = Instantiate(PlayerInfoElementPrefab);
			clone.Avatar.sprite = player.Avatar;
			clone.PointsText.text = string.Format("{0:N0}", round.PointsGained);

			Sprite creationSprite;
			// Could use GameDatabase here, but it ends up being more code for the same effort (now and in the future).
			switch (round.GameSceneName) {
				case "Smelting":
					// Player /played/ smelting, so /got/ brick.
					creationSprite = ItemDB.GetActual("brick").spriteRepresentation;
					break;
				case "Tracing":
					creationSprite = ItemDB.GetActual("shell").spriteRepresentation;
					break;
				case "Cutting":
					creationSprite = ItemDB.GetActual("cut " + player.GemType).spriteRepresentation;
					break;
				case "Polishing":
					creationSprite = ItemDB.GetActual("charged " + player.GemType).spriteRepresentation;
					break;
				default:
					creationSprite = ItemDB.GetActual("ore").spriteRepresentation;
					break;
			}

			clone.CreationImage.sprite = creationSprite;
			
			clone.transform.SetParent(PlayerInfoLayout.transform, false);
			// We're actually working backwards here, so reorder the hierarchy.
			clone.transform.SetAsFirstSibling();

			idxRoundNode = idxRoundNode.Next;
		}

		return t;
	}
}
