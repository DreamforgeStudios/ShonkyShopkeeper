using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;
    
	// TODO, this is no longer needed.
	private float[] gameScores;

	// For transfering tapped personalities to the bidding game from the shop screen.
	public Personality currentPersonality = null;
	public Shonky currentShonky = null;
	public Sprite currentSprite = null;
    public Travel.Towns currentTown;

	void Awake () {
		if (instance == null) {
			instance = this;
			SetupGameManager();
		} else if (instance != this) {
			Destroy(gameObject);
		}
	}

	private void SetupGameManager() {
		DontDestroyOnLoad(gameObject);
		Application.targetFrameRate = 60;
		gameScores = new float[4];
	}

	public void UpdateQuality(float grade, int index) {
		gameScores[index] = grade;
	}

	public Quality.QualityGrade GetQuality() {
		if (currentShonky != null) {
			// TODO: need a way to get shonky quality.
		}

		return Quality.QualityGrade.Sturdy;
	}

    public void UpdateCurrentTown() {
        currentTown = Travel.ReturnCurrentTown();
    }
}
