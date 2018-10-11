using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class FinalScoresController : PseudoScene {
	public GameObject PlayerScoresLayout;
	public PlayerScoreElement PlayerScoreElementPrefab;
	public TextMeshProUGUI RoundText;
	public PseudoSceneManager PseudoSceneManager;

	public override void Arrive() {
		base.Arrive();

		// Clear any previous items (although there shouldn't be any).
		int childCount = PlayerScoresLayout.transform.childCount;
		for (int i = childCount-1; i >= 0; i--) {
			Destroy(PlayerScoresLayout.transform.GetChild(i).gameObject);
		}

		float highestScore = GameManager.Instance.PlayerInfos.Max(x => x.Points);
		foreach (var player in GameManager.Instance.PlayerInfos) {
			PlayerScoreElement clone = Instantiate(PlayerScoreElementPrefab);
			clone.Avatar.sprite = player.Avatar;
			clone.PointsText.text = player.Points.ToString("N0");
			float maxHeight = clone.FillBG.rectTransform.rect.height;
			float fill = Mathf.Lerp(0, maxHeight, player.Points / highestScore);
			//Rect rect = clone.FillFG.rectTransform.rect;
			//rect.height = fill;
			//clone.FillFG.rectTransform.rect = rect;
			clone.FillFG.rectTransform.sizeDelta = new Vector2(clone.FillFG.rectTransform.sizeDelta.x, fill);

			Vector2 pos;
			pos = new Vector2(0, clone.FillFG.rectTransform.rect.height);
			clone.Avatar.rectTransform.anchoredPosition = pos;
			
			clone.transform.SetParent(PlayerScoresLayout.transform, false);
		}
		
		// For now, just do this.
		Invoke("LoadNextScene", 4f);
	}

	// Temp function for now.
	private void LoadNextScene() {
		PseudoSceneManager.ChangeScene("Winner");
	}
}
