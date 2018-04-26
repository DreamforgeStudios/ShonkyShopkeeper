using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// INFO: Don't use InputManager (which will be event based) in this class.  It needs more precision and more details.
public class Cutting : MonoBehaviour {
	public Vector3[] cutOrigins;
	public Vector3[] cutVectors;

	// Player's score for each cut.
	private float[] scores;

	// The maximum distance that the cut origin can be from the intended origin to be considered a fail.
	public float maximumDistance;
	// The amount that the distance should affect the final result.
	public float impactDistance;

	// The maximum difference that the closeness can be to be considered a fail.
	public float maximumCloseness;
	public float impactCloseness;

	public float maximumLength;
	public float impactLength;

	public float maximumTimeDiff;


	// The time the player has to cut each line.
	public float timePerLine;
	private float currentLineTime;

	public TextMeshProUGUI directorText;
	public TextMeshProUGUI timerText;
	public TextMeshProUGUI gradeText;
	public TextMeshProUGUI percentText;

	// (?) Allow these vectors to be nullable, so we can reset them more conveniently later.
	// This also means that we have to use .Value to get the value of these vectors.
	private Vector3? touchOrigin;
	private Vector3? touchVector;

	// Current index of the point we're cutting.h
	private int currentIndex;

	// Keep a reference around to despawn later.
	private GameObject currentCutPoint;

	// Defecit towards the goal.
	public float defecit;

	// DEBUG.
	private float oCloseness;
	private float vCloseness;
	private float lCloseness;
	public GameObject cutIndicator;

	public bool debug;

	private enum Grades {
		// Mystic and magical won't exist in the vertical slice, and this will be moved by the time they do.
		Sturdy,
		Passable,
		Brittle,
		Junk
	}

	// Use this for initialization
	void Start () {
		currentIndex = 0;
		SpawnCut(cutOrigins[currentIndex], cutVectors[currentIndex]);

		currentLineTime = timePerLine;

		// Initialize to the size we need.
		scores = new float[cutOrigins.Length];

		UpdateDirector();
		timerTicking = true;
	}

	private void SpawnCut(Vector3 origin, Vector3 cut) {
		// Instantiate cut at point.
		currentCutPoint = Instantiate(cutIndicator, origin, Quaternion.identity);
		CutPoint point = currentCutPoint.GetComponent<CutPoint>();
		point.SetCutVector(cut);
	}
	
	// Update is called once per frame
	private bool timerTicking;
	void Update () {
		// Mostly to make it easier to place cut points.
		if (debug) DrawCuts(1);
		// Check if we are running either in the Unity editor or in a standalone build.
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		// Process mouse inputs.
		ProcessMouse();

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		// Process touch inputs.
		ProcessTouch();

		// End platform dependant input.
		#endif

		// Take away from the time given on the current line.
		if (timerTicking) {
			if (currentLineTime <= 0) {
				currentLineTime = 0;
				defecit += Time.deltaTime;
			} else {
				currentLineTime -= Time.deltaTime;
			}
		}

		UpdateTimerText();
	}

	// TODO, program for touch events.
	private void ProcessTouch() {
		// Loop through all current touch events.
		foreach (Touch touch in Input.touches) {
			Debug.Log("Touched the screen at position: " + touch.position);
			// Construct a ray from the current touch coordinates.
			Ray ray = Camera.main.ScreenPointToRay(touch.position);
			if (Physics.Raycast(ray)) {
				// Do something if hit.
			}
		}
	}

	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			//Debug.Log("Clicked the screen at position: " + Input.mousePosition);
			touchOrigin = ConvertToWorldPoint(Input.mousePosition);
		}

		if (Input.GetMouseButtonUp(0)) {
			Vector2 mousePos = Input.mousePosition;
			touchVector = ConvertToWorldPoint(mousePos) - touchOrigin;
			float close = CalculateCloseness(touchOrigin.Value, touchVector.Value);
			Debug.Log(close);
			scores[currentIndex++] = close;
			DrawDebugLine(touchOrigin.Value);
			UpdateDirector();
			// Reset the timer but keep it ticking.
			ResetTimer(true);

			// Reset the origin.
			touchOrigin = null;

			/* DEBUG --
			Vector2 mousePos = Input.mousePosition;
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
			transform.position = pos;

			// Get the vector of the current input.
			touchVector = touchOrigin - Input.mousePosition;
			Vector3 start = touchOrigin.Value;
			start.z = 10;

			Vector3 end = Input.mousePosition;
			end.z = 10;
			Debug.DrawLine(Camera.main.ScreenToWorldPoint(start), Camera.main.ScreenToWorldPoint(end), Color.blue, 1f);
			*/

			if (currentIndex >= cutVectors.Length) {
				GradeAndFinish();
			} else {
				Debug.Log("Spawning");
				Destroy(currentCutPoint);
				SpawnCut(cutOrigins[currentIndex], cutVectors[currentIndex]);
			}
		}
	}

	private void UpdateTimerText() {
		timerText.text = "Time: " + currentLineTime;
	}

	private void ResetTimer(bool ticking) {
		currentLineTime = timePerLine;
		timerTicking = ticking;
	}

	private void GradeAndFinish() {
		// Calculate the average cut grade.
		float sum = 0;
		foreach (float score in scores) {
			sum += score;
		}

		// Average.
		sum /= scores.Length;
		// Use as a percentage.
		sum = 1-sum;
		// Take away using the time defecit.
		sum -= (.01f * defecit);
		Debug.Log(sum);

		percentText.text = ((int)(sum*100f)).ToString() + "%";
		percentText.color = Color.Lerp(Color.red, Color.green, sum);

		// TODO, this will go in the grading class.
		// TODO, in the grading class, should be able to get string version of grade...
		// leaving this here as a reminder.
		Grades grade;
		if (sum >= 0.95) {
			grade = Grades.Sturdy;
		} else if (sum >= 0.85) {
			grade = Grades.Passable;
		} else if (sum >= 0.20) {
			grade = Grades.Brittle;
		} else {
			grade = Grades.Junk;
		}

		switch (grade) {
			case Grades.Sturdy:
				gradeText.text = "Sturdy";
				gradeText.color = Color.green;
				break;
			case Grades.Passable:
				gradeText.text = "Passable";
				gradeText.color = Color.white;
				break;
			case Grades.Brittle:
				gradeText.text = "Brittle";
				gradeText.color = Color.yellow;
				break;
			case Grades.Junk:
				gradeText.text = "Junk";
				gradeText.color = Color.red;
				break;
		}

		gradeText.gameObject.SetActive(true);

		//TODO: build an enum class for the grading.
	}

	private void UpdateDirector() {
		directorText.text = "Line to cut: " + (currentIndex+1);
	}

	private void DrawDebugLine(Vector3 origin) {
		Vector3 end = ConvertToWorldPoint(Input.mousePosition);
		Debug.DrawLine(origin, end, Color.yellow, 2);
	}

	// TODO: this will eventually go in the input manager.
	private Vector3 ConvertToWorldPoint(Vector3 screenPoint) {
		// If this is changed from 10, it fucks up.
		// I think that 10 indicates 10 units FROM the camera, not 10 in the scene.
		screenPoint.z = 10;
		return Camera.main.ScreenToWorldPoint(screenPoint);
	}

	private float CalculateCloseness(Vector3 origin, Vector3 vec) {
		//origin = ConvertToWorldPoint(origin);

		// 0 = close, 1 = far.
		float originCloseness = 0f;
		float vectorCloseness = 0f;
		// We should measure length closeness because we normalize when calculating vector closeness.
		float lengthCloseness = 0f;

		// Calculate how close the player was to the starting point.
		float distance = Vector3.Distance(origin, cutOrigins[currentIndex]);
		originCloseness = Mathf.InverseLerp(0, maximumDistance, distance);
		// So the debugUI can use it.
		this.oCloseness = originCloseness;

		// Calculate how similar the vectors are (how close the player was to the correct swipe).
		// Normalize so that distance doesn't affect the dot product.
		Vector3 vn = Vector3.Normalize(vec);
		Vector3 cn = Vector3.Normalize(cutVectors[currentIndex]);
		float vSimilarity = 1-Vector3.Dot(vn, cn);
		vectorCloseness = Mathf.InverseLerp(0, maximumCloseness, vSimilarity);
		this.vCloseness = vectorCloseness;

		// Calculate how close the vector length is to the optimum length.  (did the player overshoot? undershoot? etc).
		float vl = Vector3.Magnitude(vec);
		float cl = Vector3.Magnitude(cutVectors[currentIndex]);
		lengthCloseness = Mathf.InverseLerp(0, maximumLength, Mathf.Abs(vl-cl));
		this.lCloseness = lengthCloseness;

		originCloseness *= impactDistance;
		vectorCloseness *= impactCloseness;
		lengthCloseness *= impactLength;

		return (originCloseness + vectorCloseness + lengthCloseness) / 3f;
	}

	// Debug function.
	private void DrawCuts(float lifetime) {
		for (int i = 0; i < cutOrigins.Length; i++) {
			// Place objects at start and end positions.
			Destroy(Instantiate(cutIndicator, cutOrigins[i], Quaternion.identity), lifetime);
			//Instantiate(debugObject, cutOrigins[i] + cutVectors[i], Quaternion.identity);
			Vector3 start = cutOrigins[i];
			Vector3 end = start + cutVectors[i];
			if (i == currentIndex)
				Debug.DrawLine(start, end, Color.red, lifetime);
			else
				Debug.DrawLine(start, end, Color.blue, lifetime);
		}
	}

    void OnGUI()
    {
        Camera  c = Camera.main;
        Event   e = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Origin Closeness: " + oCloseness);
        GUILayout.Label("Vector Closeness: " + vCloseness);
        GUILayout.Label("Length Closeness: " + lCloseness);
        GUILayout.EndArea();
    }
}
