using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance = null;

	private float[] gameScores;

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
		float sum = 0;
		foreach(float score in gameScores) {
			sum += score;
		}

		sum /= gameScores.Length;
		Debug.Log("Grade: " + sum);

		return Quality.FloatToGrade(sum, 3);
	}
}
