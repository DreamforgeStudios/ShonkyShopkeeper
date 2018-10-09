using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {
	public PseudoSceneManager PseudoSceneManager;

	// Use this for initialization
	void Start () {
		if (GameManager.Instance.RoundQueue.Peek().RoundNumber != GameManager.Instance.CurrentRound) {
			PseudoSceneManager.ChangeScene("Intermission");
		} else {
			PseudoSceneManager.ChangeScene("RoundView");
		}
	}
}
