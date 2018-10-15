using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntermissionController : PseudoScene {
	[BoxGroup("Object Assignments")]
	public GameObject PlayerInfoLayout;
	[BoxGroup("Object Assignments")]
	public ZapController Zap;
	[BoxGroup("Object Assignments")]
	public PlayerInfoElement PlayerInfoElementPrefab;
	[BoxGroup("Object Assignments")]
	public ItemDatabase ItemDB;
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI HeadingText;

	private const float POP_DURATION = .6f;

	private PlayerInfoElement winner = null;
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
		LinkedListNode<PostRoundInfo> lastRoundNode = GameManager.Instance.RoundHistory.First;
		Game game = gd.GetGameBySceneName(lastRoundNode.Value.GameSceneName);
		
        Zap.transform.localScale = Vector3.zero;
        Zap.Text.text = game.IntermissionText;
		HeadingText.text = string.Format("Round {0} -- {1} Game",
			lastRoundNode.Value.RoundNumber + 1, game.Name);

		
		Sequence seq = DOTween.Sequence();
		//PlayerInfoElement winner = null;
		// Find the roundInfos of the previous round.
		var rounds = GameManager.Instance.RoundHistory.Where(x => x.RoundNumber == lastRoundNode.Value.RoundNumber).ToArray();
		float maxScore = rounds.Max(x => x.PointsGained);
		for (int i = 0; i < rounds.Length; i++) {
			PlayerInfo player = GameManager.Instance.PlayerInfos[rounds[i].PlayerIndex];
			PlayerInfoElement clone = Instantiate(PlayerInfoElementPrefab);
			clone.Avatar.sprite = player.Avatar.Sprite;
			clone.CreationImage.sprite = FindSpriteRepresentation(rounds[i].GameSceneName, player.Avatar.GemType);
			
			// Store the variable at its current state, for use in the dotween closure later on.
			if (i == 0 || rounds[i].PointsGained == maxScore) {
				winner = clone;
				//Zap.transform.position = clone.Avatar.transform.position;
			}
			
			// Pop in avatar, points text, and creation image.
			clone.Avatar.transform.localScale = Vector3.zero;
			clone.PointsText.transform.localScale = Vector3.zero;
			clone.CreationImage.transform.localScale = Vector3.zero;
			clone.PointsText.text = "0";
			seq.Insert(POP_DURATION * i, clone.Avatar.transform.DOScale(1, POP_DURATION).SetEase(Ease.OutBack));
			seq.Insert(POP_DURATION * i + .2f, clone.PointsText.transform.DOScale(1, POP_DURATION).SetEase(Ease.OutBack));
			seq.Insert(POP_DURATION * i + .3f, clone.CreationImage.transform.DOScale(1, POP_DURATION).SetEase(Ease.OutBack));
			seq.Insert(POP_DURATION * i + .35f, DOTween.To(() => 0, x => clone.PointsText.text = x.ToString(), rounds[i].PointsGained, 5f)
				.SetEase(Ease.OutCirc));

			var m = clone.GlowParticles.main;
			m.startColor = new ParticleSystem.MinMaxGradient(Color.white, player.Avatar.Color);
			var em = clone.GlowParticles.emission;
			em.rateOverTime = Mathf.Lerp(0, 10, Mathf.Pow(rounds[i].PointsGained / maxScore, 4));

			clone.transform.SetParent(PlayerInfoLayout.transform, false);
			// We're actually working backwards here, so reorder the hierarchy.
			clone.transform.SetAsFirstSibling();
		}

		// Force Unity to update the layout so that we can position things.
		LayoutRebuilder.ForceRebuildLayoutImmediate(PlayerInfoLayout.GetComponent<RectTransform>());
		Zap.transform.position = winner.Avatar.transform.position;
		seq.Append(Zap.transform.DOScale(Vector3.one, .4f).SetEase(Ease.OutBack)
			.OnComplete(() =>
				Zap.Image.transform.DORotate(Vector3.forward * 360, 2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
					.SetLoops(-1))); // Loop won't be infinite if we add this to the sequence.

		seq.Play();
		
		return t;
	}
	
	/*
	private void Update() {
		Debug.Log("Zap pos: " + Zap.GetComponent<RectTransform>().position);
		if (winner != null) {
			Debug.Log("Winner pos: " + winner.Avatar.transform.position);
		}
	}
	*/

	private Sprite FindSpriteRepresentation(string gameSceneName, Item.GemType playerGemType) {
        switch (gameSceneName) {
            case "Smelting":
                // Player /played/ smelting, so /got/ brick.
                return ItemDB.GetActual("brick").spriteRepresentation;
            case "Tracing":
                return ItemDB.GetActual("shell").spriteRepresentation;
            case "Cutting":
                return ItemDB.GetActual("cut " + playerGemType).spriteRepresentation;
            case "Polishing":
                return ItemDB.GetActual("charged " + playerGemType).spriteRepresentation;
            default:
                return ItemDB.GetActual("ore").spriteRepresentation;
        }
	}
}
