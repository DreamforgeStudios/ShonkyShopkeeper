using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class IntermissionController : PseudoScene {
	[BoxGroup("Object Assignments")]
	public GameObject PlayerInfoLayout;
	[BoxGroup("Object Assignments")]
	public PlayerInfoElement PlayerInfoElementPrefab;
	[BoxGroup("Object Assignments")]
	public ItemDatabase ItemDB;
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI HeadingText;

	public override Tween Arrive(bool animate = true) {
		Tween t = base.Arrive(animate);

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

		// Find the roundInfos of the previous round.
		var rounds = GameManager.Instance.RoundHistory.Where(x => x.RoundNumber == lastRoundNode.Value.RoundNumber).ToArray();
		float maxScore = rounds.Max(x => x.PointsGained);
		foreach (var round in rounds) {
			PlayerInfo player = GameManager.Instance.PlayerInfos[round.PlayerIndex];
			PlayerInfoElement clone = Instantiate(PlayerInfoElementPrefab);
			clone.Avatar.sprite = player.Avatar.Sprite;
			// TODO: have avatars pop in here.
			DOTween.To(() => 0, x => clone.PointsText.text = x.ToString(), round.PointsGained, 5f).SetEase(Ease.OutQuad)
				.SetDelay(1f);
			
			var m = clone.GlowParticles.main;
			m.startColor = new ParticleSystem.MinMaxGradient(Color.white, player.Avatar.Color);
			var em = clone.GlowParticles.emission;
			em.rateOverTime = Mathf.Lerp(0, 10, Mathf.Pow(round.PointsGained / maxScore, 4));

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
					creationSprite = ItemDB.GetActual("cut " + player.Avatar.GemType).spriteRepresentation;
					break;
				case "Polishing":
					creationSprite = ItemDB.GetActual("charged " + player.Avatar.GemType).spriteRepresentation;
					break;
				default:
					creationSprite = ItemDB.GetActual("ore").spriteRepresentation;
					break;
			}

			clone.CreationImage.sprite = creationSprite;
			
			clone.transform.SetParent(PlayerInfoLayout.transform, false);
			// We're actually working backwards here, so reorder the hierarchy.
			clone.transform.SetAsFirstSibling();
		}
		
		/*
		var idxRoundNode = lastRoundNode;
		// Go through all games in the past round.
		while (idxRoundNode != null && idxRoundNode.Value.RoundNumber == lastRoundNode.Value.RoundNumber) {
			var round = idxRoundNode.Value;
			
			PlayerInfo player = GameManager.Instance.PlayerInfos[round.PlayerIndex];
			PlayerInfoElement clone = Instantiate(PlayerInfoElementPrefab);
			clone.Avatar.sprite = player.Avatar;
			// TODO: have avatars pop in here.
			DOTween.To(() => 0, x => clone.PointsText.text = x.ToString(), round.PointsGained, 5f).SetEase(Ease.OutQuad)
				.SetDelay(1f);
			
			var em = clone.GlowParticles.emission;
			em.rateOverTime = Mathf.Lerp(0, 20, round.PointsGained / 8000f);

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
		*/

		return t;
	}
}
