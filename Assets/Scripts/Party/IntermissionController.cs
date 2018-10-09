using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermissionController : PseudoScene {
	public GameObject PlayerInfoLayout;
	public PlayerInfoElement PlayerInfoElementPrefab;
	public ItemDatabase ItemDB;

	public override void Arrive() {
		base.Arrive();
		
		// Clear any previous things.
		int childCount = PlayerInfoLayout.transform.childCount;
		for (int i = childCount-1; i >= 0; i--) {
			Destroy(PlayerInfoLayout.transform.GetChild(i).gameObject);
		}

		var lastGame = GameManager.Instance.RoundHistory.First.Value;
		
		foreach (var player in GameManager.Instance.PlayerInfos) {
			PlayerInfoElement clone = Instantiate(PlayerInfoElementPrefab);
			clone.Avatar.sprite = player.Avatar;
			clone.PointsText.text = string.Format("{0:N0}", player.Points);

			Sprite creationSprite;
			// Could use GameDatabase here, but it ends up being more code for the same effort (now and in the future).
			switch (lastGame.GameSceneName) {
				case "Smelting":
					// Player /played/ smelting, so /got/ brick.
					creationSprite = ItemDB.GetActual("brick").spriteRepresentation;
					break;
				case "Tracing":
					creationSprite = ItemDB.GetActual("shell").spriteRepresentation;
					break;
				case "Cutting":
					creationSprite = ItemDB.GetActual("cut" + player.GemType).spriteRepresentation;
					break;
				case "Polishing":
					creationSprite = ItemDB.GetActual("charged" + player.GemType).spriteRepresentation;
					break;
				default:
					creationSprite = ItemDB.GetActual("ore").spriteRepresentation;
					break;
			}

			clone.CreationImage.sprite = creationSprite;
			
			clone.transform.SetParent(PlayerInfoLayout.transform, false);
		}
	}
}
