using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarConfirmationController : PseudoScene {
	public GameObject LayoutObject, AvatarPrefab;

	public AvatarSelectController AvatarSelectController;


	public void GenerateGame() {
		GameDatabase gd = Resources.Load<GameDatabase>("GameDatabase");
		// Good enough for now...
		int numberOfPlayers = AvatarSelectController.SelectedAvatars.Count;
		
		GameManager.Instance.RoundQueue = new Queue<RoundInfo>();
		GameManager.Instance.PlayerInfos = new List<PlayerInfo>();
		for (int i = 0; i < numberOfPlayers; i++) {
			// TODO: change gem type depending on avatar.
			GameManager.Instance.PlayerInfos.Add(new PlayerInfo(i, AvatarSelectController.SelectedAvatars[i], Item.GemType.Ruby));
		}
		
		// Create a queue of all the games and who should play them.
        for (int i = 0; i < gd.GameCount; i++) {
	        for (int j = 0; j < numberOfPlayers; j++) {
			    GameManager.Instance.RoundQueue.Enqueue(new RoundInfo(j, i, gd.Games[i].SceneName));
	        }
        }
	}

	public override void Arrive() {
		base.Arrive();
		
		// Clear any already selected avatars.
		int childCount = LayoutObject.transform.childCount;
		for (int i = childCount-1; i >= 0; i--) {
			Destroy(LayoutObject.transform.GetChild(i).gameObject);
		}
		
		// Display each selected sprite.
		foreach (var sprite in AvatarSelectController.SelectedAvatars) {
			GameObject clone = Instantiate(AvatarPrefab);
			clone.GetComponent<Image>().sprite = sprite;
			// Must use worldPositionStays = false or Unity FUCKS you... https://answers.unity.com/questions/868484/why-is-instantiated-objects-scale-changing.html
			clone.transform.SetParent(LayoutObject.transform, false);
		}
		
		// NOTE: is there a better place for this than Arrive()?
		GenerateGame();
	}
}
