using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PartyHelper {
    public static void InsertMockPlayers() {
		GameManager.Instance.PlayerInfos = new List<PlayerInfo>();
		for (int i = 0; i < 4; i++) {
			Texture2D tex = Texture2D.whiteTexture;
			Sprite s = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f));
			Avatar a = ScriptableObject.CreateInstance<Avatar>();
			a.Color = Color.red;
			a.Sprite = s;
			a.GemType = Item.GemType.Ruby;
			GameManager.Instance.PlayerInfos.Add(new PlayerInfo(i, a, 0, 0));
			GameManager.Instance.PlayerInfos[i].Points = Random.Range(3000, 30000);
			GameManager.Instance.PlayerInfos[i].Gold = Random.Range(100, 600);
		}
    }

	public static void InsertMockQueue() {
		GameDatabase gd = Resources.Load<GameDatabase>("GameDatabase");
		
		GameManager.Instance.RoundQueue = new Queue<RoundInfo>();
		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 4; j++) {
				GameManager.Instance.RoundQueue.Enqueue(new RoundInfo(j, i, gd.Games[i].SceneName));
			}
			
		}
	}

	public static void InsertMockHistory() {
		GameManager.Instance.RoundHistory = new LinkedList<PostRoundInfo>();
		for (int i = 0; i < 4; i++) {
			GameManager.Instance.RoundHistory.AddFirst(new PostRoundInfo(i, 0, "Smelting", Random.Range(1000, 8000), 0));
		}
	}
}
