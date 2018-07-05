using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// INFO: Don't use InputManager (which will be event based) in this class.  It needs more precision and more details.
public class Cutting : MonoBehaviour {
	public Vector3[] cutOrigins;
	public Vector3[] cutVectors;

	// Persistent quality bar.
	public QualityBar qualityBar;

	// The maximum distance that the cut origin can be from the intended origin to be considered a fail.
	public float maximumDistance;
	// The amount that the distance should affect the final result.
	public float impactDistance;

	// The maximum difference that the closeness can be to be considered a fail.
	public float maximumCloseness;
	public float impactCloseness;

	public float maximumLength;
	public float impactLength;

	// The best possible swipe time that the player can get.
	public float baseTime;
	// The maximum time before the time component of a swipe is considered "worthless".
	public float maximumTime;
	// The impact that time should have on the score.
	public float impactTime;

	// TODO: this might not need to be a global.
	private float swipeTime;

	public TextMeshProUGUI directorText;
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

	public GameObject cutIndicator;

	public bool debug = false;

	// Object that holds return and retry buttons.
    public GameObject returnOrRetryButtons;

    //Particle System
    public ParticleSystem particle;
    public int amountOfParticles = 5;

	private bool start = false;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += (() => { Time.timeScale = 1; start = true; });
    }

    // Use this for initialization
    void Start () {
		Countdown.onComplete += GameOver;

		// Should probably do more initialization here...
		currentIndex = 0;
		SpawnCut(cutOrigins[currentIndex], cutVectors[currentIndex]);
    }

	// Update is called once per frame
	void Update () {
		// Don't do anything if the game hasn't started.
		if (!start)
			return;

		// Mostly to make it easier to place cut points.
		if (debug) DrawCuts(1);

		// Check where we are running the program.
		RuntimePlatform p = Application.platform;
		if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer || p == RuntimePlatform.OSXEditor || p == RuntimePlatform.OSXPlayer)
			// Process mouse inputs.
			ProcessMouse();
		else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
			// Process touch inputs.
			ProcessTouch();
	}

	private void ProcessTouch() {
		// Don't let the player use multiple fingers, and don't run if there's no input.
		if (Input.touchCount > 1 || Input.touchCount == 0) {
			return;
		}

		Touch touch = Input.GetTouch(0);
        particle.Emit(amountOfParticles);
        if (touch.phase == TouchPhase.Began) {
			InitiateTouch(touch);
		} else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
			ConcludeTouch(touch);

			if (currentIndex >= cutVectors.Length) {
				GameOver();
			} else {
				Destroy(currentCutPoint);
				SpawnCut(cutOrigins[currentIndex], cutVectors[currentIndex]);
			}
		} else {
			// The player must be holding down.
			// Perhaps we should move this to something like IdleTouch()... but no point rn.
			swipeTime += Time.deltaTime;
		}
	}

	private void InitiateTouch(Touch touch) {
		swipeTime = 0;
		touchOrigin = ConvertToWorldPoint(touch.position);
		if (currentIndex >= cutVectors.Length) {
			return;
		}
		CutPoint cut = currentCutPoint.GetComponent<CutPoint>();
		cut.SetCutVector(cutVectors[currentIndex]);
    }

	private void ConcludeTouch(Touch touch) {
		Vector2 touchPos = touch.position;
		touchVector = ConvertToWorldPoint(touchPos) - touchOrigin;
		float close = CalculateCloseness(touchOrigin.Value, touchVector.Value, swipeTime);
		qualityBar.Subtract(close);
		if (currentIndex < cutVectors.Length)
			currentIndex++;
		touchOrigin = null;
	}

	// Only here for debugging on PC.
	private bool holding = false;
	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			holding = true;
			swipeTime = 0;
			touchOrigin = ConvertToWorldPoint(Input.mousePosition);
			if (currentIndex >= cutVectors.Length) {
				return;
			}
			CutPoint cut = currentCutPoint.GetComponent<CutPoint>();
			cut.SetCutVector(cutVectors[currentIndex]);
        }

		if (Input.GetMouseButtonUp(0)) {
			// Protect against null value.
			// TODO: this is dirty.
			if (currentIndex >= cutVectors.Length) {
				return;
			}

			holding = false;
			Vector2 mousePos = Input.mousePosition;
			touchVector = ConvertToWorldPoint(mousePos) - touchOrigin;
			float close = CalculateCloseness(touchOrigin.Value, touchVector.Value, swipeTime);
			//Debug.Log(close);
			qualityBar.Subtract(close*close);
			if (currentIndex < cutVectors.Length)
				currentIndex++;
			DrawDebugLine(touchOrigin.Value);

			// Reset the origin.
			touchOrigin = null;

			if (currentIndex >= cutVectors.Length) {
				GameOver();
			} else {
				Destroy(currentCutPoint);
				SpawnCut(cutOrigins[currentIndex], cutVectors[currentIndex]);
			}
		}

        if (holding) {
            swipeTime += Time.deltaTime;
            particle.Emit(amountOfParticles);
        }
	}
	
	
	
	/* START CUT THINGS */
	private void SpawnCut(Vector3 origin, Vector3 cut) {
		// Instantiate cut at point.
		currentCutPoint = Instantiate(cutIndicator, origin, Quaternion.identity);
	}
	

	// TODO: this will eventually go in the input manager.
	private Vector3 ConvertToWorldPoint(Vector3 screenPoint) {
		// If this is changed from 10, it fucks up.
		// I think that 10 indicates 10 units FROM the camera, not 10 in the scene.
		screenPoint.z = 10;
		return Camera.main.ScreenToWorldPoint(screenPoint);
	}

	private float CalculateCloseness(Vector3 origin, Vector3 vec, float time) {

		// 0 = close, 1 = far.
		float originCloseness = 0f;
		float vectorCloseness = 0f;
		// We should measure length closeness because we normalize when calculating vector closeness.
		float lengthCloseness = 0f;

		// Calculate how close the player was to the starting point.
		float distance = Vector3.Distance(origin, cutOrigins[currentIndex]);
		originCloseness = Mathf.InverseLerp(0, maximumDistance, distance);
		// So the debugUI can use it.
		//this.oCloseness = originCloseness;

		// Calculate how similar the vectors are (how close the player was to the correct swipe).
		// Normalize so that distance doesn't affect the dot product.
		Vector3 vn = Vector3.Normalize(vec);
		Vector3 cn = Vector3.Normalize(cutVectors[currentIndex]);
		float vSimilarity = 1-Vector3.Dot(vn, cn);
		vectorCloseness = Mathf.InverseLerp(0, maximumCloseness, vSimilarity);
		//this.vCloseness = vectorCloseness;

		// Calculate how close the vector length is to the optimum length.  (did the player overshoot? undershoot? etc).
		float vl = Vector3.Magnitude(vec);
		float cl = Vector3.Magnitude(cutVectors[currentIndex]);
		lengthCloseness = Mathf.InverseLerp(0, maximumLength, Mathf.Abs(vl-cl));
		//this.lCloseness = lengthCloseness;

		float timeCloseness = Mathf.InverseLerp(baseTime, maximumTime, time);
		//this.tCloseness = timeCloseness;

		originCloseness *= impactDistance;
		vectorCloseness *= impactCloseness;
		lengthCloseness *= impactLength;
		timeCloseness *= impactTime;

		//Debug.LogFormat("Origin closeness: {0}, Vector closeness: {1}, LengthCloseness: {2}, TimeCloseness: {3}", originCloseness, vectorCloseness, lengthCloseness, timeCloseness);

		return (originCloseness + vectorCloseness + lengthCloseness + timeCloseness) / 4f;
	}
	
	private void DrawDebugLine(Vector3 origin) {
		Vector3 end = ConvertToWorldPoint(Input.mousePosition);
		Debug.DrawLine(origin, end, Color.yellow, 2);
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

	private Quality.QualityGrade grade = Quality.QualityGrade.Unset;
	private void GameOver() {
		Countdown.onComplete -= GameOver;
		grade = qualityBar.Finish();
		qualityBar.Disappear();
		//Quality.QualityGrade grade = Quality.FloatToGrade(grade, 3);
		gradeText.text = Quality.GradeToString(grade);
		gradeText.color = Quality.GradeToColor(grade);
		gradeText.gameObject.SetActive(true);

        // TODO: back to shop button needs to change to facilitate restarting games.
        //Inventory.Instance.InsertItem(new ItemInstance("Cut " + DataTransfer.GemType, 1, grade, true));

		ShowUIButtons();
	}

	public void Return() {
		ReturnOrRetry.Return("Cut " + DataTransfer.GemType, grade);
	}

	public void Retry() {
		ReturnOrRetry.Retry();
	}

    public void ShowUIButtons() {
	    returnOrRetryButtons.SetActive(true);
    }
}