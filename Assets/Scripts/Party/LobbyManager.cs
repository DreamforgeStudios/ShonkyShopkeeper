using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {
	public PseudoSceneManager PseudoSceneManager;
	public bool BuildMockData = false;

	// Use this for initialization
	void Start () {
        if (BuildMockData) {
            PartyHelper.InsertMockPlayers();
            PartyHelper.InsertMockHistory();
            PartyHelper.InsertMockQueue();
        }
		
		// Game has not been initialized, probably starting from the editor with mock data disabled.
		if (GameManager.Instance.RoundQueue == null || GameManager.Instance.RoundHistory == null || GameManager.Instance.PlayerInfos == null) {
			Debug.LogWarning("Game has not been initialised.  Probably starting from the editor without mock data enabled.");
			return;
		}
		
		if (GameManager.Instance.RoundQueue.Count <= 0) {
			PseudoSceneManager.ChangeSceneWithoutAnimation("FinalScores");
		// If the round has ended and a new round is about to begin.
		} else if (GameManager.Instance.RoundHistory.First != null &&
		           GameManager.Instance.RoundHistory.First.Value.RoundNumber !=
		           GameManager.Instance.RoundQueue.Peek().RoundNumber) {
			PseudoSceneManager.ChangeSceneWithoutAnimation("Intermission");
		} else {
			PseudoSceneManager.ChangeSceneWithoutAnimation("RoundView");
		}
	}
}
