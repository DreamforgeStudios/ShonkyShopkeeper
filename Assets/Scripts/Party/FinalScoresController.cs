using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;

public class FinalScoresController : PseudoScene {
	[BoxGroup("Object Assignments")]
	public GameObject PlayerScoresLayout;
	[BoxGroup("Object Assignments")]
	public PlayerScoreElement PlayerScoreElementPrefab;
	[BoxGroup("Object Assignments")]
    public TextMeshProUGUI GoldBonusText;

	[BoxGroup("Feel")]
	public float InitialRiseDuration;
	[BoxGroup("Feel")]
	public float FinalRiseDuration;
	[BoxGroup("Feel")]
	public Ease RiseEase;
	[BoxGroup("Feel")]
	public float AvatarPopDuration;
	[BoxGroup("Feel")]
	public Ease AvatarPopEase;
	[BoxGroup("Feel")]
	public float GoldBonusDuration;
	[BoxGroup("Feel")]
	public Ease GoldBonusEase;
	[BoxGroup("Feel")]
	public float GoldBonusInbetweenDuration;

	public override Tween Arrive(bool animate = true) {
		Tween t = base.Arrive(animate);

		// Clear any previous items (although there shouldn't be any).
		int childCount = PlayerScoresLayout.transform.childCount;
		for (int i = childCount-1; i >= 0; i--) {
			Destroy(PlayerScoresLayout.transform.GetChild(i).gameObject);
		}

		Sequence seq = DOTween.Sequence();

		var orderedPlayers = GameManager.Instance.PlayerInfos.OrderByDescending(x => x.Points).ToList();
		//float highestScore = GameManager.Instance.PlayerInfos.Max((x,y) => x.Points > y.Po);
		var playerInfos = GameManager.Instance.PlayerInfos;
		for (int i = 0; i < playerInfos.Count; i++) {
			PlayerScoreElement clone = Instantiate(PlayerScoreElementPrefab);
			clone.Avatar.sprite = playerInfos[i].Avatar.Sprite;
			clone.PointsText.text = "0";
			clone.FillFG.color = playerInfos[i].Avatar.Color;

			// Avatars pop in at the very start.
			clone.Avatar.transform.localScale = Vector3.zero;
			seq.Insert(0, clone.Avatar.transform.DOScale(Vector3.one, AvatarPopDuration).SetEase(AvatarPopEase));
				
			float maxHeight = clone.FillBG.rectTransform.rect.height;
			float initialFill = Mathf.Lerp(0, maxHeight, playerInfos[i].Points / orderedPlayers[0].AggregatePoints);
			float finalFill = Mathf.Lerp(0, maxHeight, playerInfos[i].AggregatePoints / orderedPlayers[0].AggregatePoints);
			
			Vector2 initialFillSizeDelta = new Vector2(clone.FillFG.rectTransform.sizeDelta.x, initialFill);
			Vector2 finalFillSizeDelta = new Vector2(clone.FillFG.rectTransform.sizeDelta.x, finalFill);
			Vector2 initialPos = new Vector2(0, initialFillSizeDelta.y);
			Vector2 finalPos = new Vector2(0, finalFillSizeDelta.y);
			
			// Find position of the player.
			int position = orderedPlayers.FindIndex(x => x == playerInfos[i]);
			// Find reverse position of the player for use in calculations (0 is worst -- descending order).
			int index = (orderedPlayers.Count-1) - position;
			int initialMultiplier = position == 0 || position == 1 ? orderedPlayers.Count - 2 : index;
			int finalMultiplier = orderedPlayers.Count - 1;
			
			// We want to raise first and second place at the same time, so use the same calculation in that situation.
            seq.Insert(AvatarPopDuration + InitialRiseDuration * initialMultiplier,
                clone.FillFG.rectTransform.DOSizeDelta(initialFillSizeDelta, InitialRiseDuration).SetEase(RiseEase));
            seq.Insert(AvatarPopDuration + InitialRiseDuration * initialMultiplier,
                clone.Avatar.rectTransform.DOAnchorPos(initialPos, InitialRiseDuration).SetEase(RiseEase));
            seq.Insert(AvatarPopDuration + InitialRiseDuration * initialMultiplier,
                DOTween.To(() => 0, x => clone.PointsText.text = x.ToString("N0"), playerInfos[i].Points, InitialRiseDuration));

			// These need to be callbacks because they must be formulated at runtime (they add to the existing position / sizedelta).
			//  If they aren't callbacks, then they do nothing because they're made at compile time, which has different values.
			seq.InsertCallback(AvatarPopDuration + InitialRiseDuration * finalMultiplier + index * GoldBonusInbetweenDuration + GoldBonusDuration,
				() => clone.FillFG.rectTransform.DOSizeDelta(finalFillSizeDelta, FinalRiseDuration).SetEase(RiseEase));
			seq.InsertCallback(AvatarPopDuration + InitialRiseDuration * finalMultiplier + index * GoldBonusInbetweenDuration + GoldBonusDuration,
				() => clone.Avatar.rectTransform.DOAnchorPos(finalPos, FinalRiseDuration).SetEase(RiseEase));
			// Clone i because it may change between now and when the closure is run.
			var iclone = i;
			seq.InsertCallback(AvatarPopDuration + InitialRiseDuration * finalMultiplier + index * GoldBonusInbetweenDuration + GoldBonusDuration,
                () => DOTween.To(() => playerInfos[iclone].Points, x => clone.PointsText.text = x.ToString("N0"), playerInfos[i].AggregatePoints, FinalRiseDuration));

			clone.transform.SetParent(PlayerScoresLayout.transform, false);
		}

		GoldBonusText.transform.localScale = Vector3.zero;
		seq.Insert(AvatarPopDuration + InitialRiseDuration * (playerInfos.Count - 1),
			GoldBonusText.transform.DOScale(Vector3.one, GoldBonusDuration).SetEase(GoldBonusEase));
		
		// For now, just do this.
		seq.OnComplete(() => Invoke("LoadNextScene", 3f));

		if (t != null) {
            seq.Pause();
            t.onComplete += () => seq.Play();
		} else if (Initiate.IsLoading) {
			seq.Pause();
			Initiate.onFinishFading += () => seq.Play();
		}
		
		return t;
	}

	// Temp function for now.
	private void LoadNextScene() {
		PseudoSceneManager.ChangeScene("Winner");
	}
}
