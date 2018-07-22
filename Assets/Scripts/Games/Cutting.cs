using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// INFO: Don't use InputManager (which will be event based) in this class.  It needs more precision and more details.
public class Cutting : MonoBehaviour {
	private List<Vector3> cutOrigins;
	private List<Vector3> cutVectors;

	public GameObject gemObject;
	public int NumberOfCuts;
	[MinMaxSlider(0f, 10f)]
	public Vector2 MinMaxDistance;

	// Persistent quality bar.
	public QualityBar QualityBar;

	// The maximum distance that the cut origin can be from the intended origin to be considered a fail.
	public float MaximumDistance;
	// The amount that the distance should affect the final result.
	public float ImpactDistance;

	// The maximum difference that the closeness can be to be considered a fail.
	public float MaximumCloseness;
	public float ImpactCloseness;

	public float MaximumLength;
	public float ImpactLength;

	// The best possible swipe time that the player can get.
	public float BaseTime;
	// The maximum time before the time component of a swipe is considered "worthless".
	public float MaximumTime;
	// The impact that time should have on the score.
	public float ImpactTime;

	// The time that the user has been swiping for...
	// ... could make this not a global, but it's not worth it.
	private float swipeTime;

	public TextMeshProUGUI DirectorText;
	public TextMeshProUGUI GradeText;
	public TextMeshProUGUI PercentText;

	// (?) Allow these vectors to be nullable, so we can reset them more conveniently later.
	// This also means that we have to use .Value to get the value of these vectors.
	private Vector3? touchOrigin;
	private Vector3? touchVector;

	// Current index of the point we're cutting.h
	private int currentIndex;

	// Keep a reference around to despawn later.
	private GameObject currentCutPoint;

	public GameObject CutIndicator;

	public bool Debug = false;

	// Object that holds return and retry buttons.
    public GameObject ReturnOrRetryButtons;

    //Particle System
    public ParticleSystem Particle;
    public int AmountOfParticles = 5;

	private bool start = false;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += (() => { Time.timeScale = 1; start = true; });
    }

    // Use this for initialization
    void Start () {
		Countdown.onComplete += GameOver;

	    for (int i = 0; i < NumberOfCuts; i++) {
		    float distance = Random.Range(MinMaxDistance.x, MinMaxDistance.y);
		    var vecPos = Utility.RotateAroundPivot(Vector3.right * distance, Vector3.forward,
			    new Vector3(0, 0, Random.Range(0f, 360f)));
		    
		    cutOrigins.Add(vecPos);
		    cutVectors.Add(-vecPos*2);
	    }

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
		if (Debug) DrawCuts(1);

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
        Particle.Emit(AmountOfParticles);
        if (touch.phase == TouchPhase.Began) {
			InitiateTouch(touch);
		} else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
			ConcludeTouch(touch);

			if (currentIndex >= cutVectors.Count) {
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
		if (currentIndex >= cutVectors.Count) {
			return;
		}
		CutPoint cut = currentCutPoint.GetComponent<CutPoint>();
		cut.SetCutVector(cutVectors[currentIndex]);
    }

	private void ConcludeTouch(Touch touch) {
		Vector2 touchPos = touch.position;
		touchVector = ConvertToWorldPoint(touchPos) - touchOrigin;
		float close = CalculateCloseness(touchOrigin.Value, touchVector.Value, swipeTime);
		QualityBar.Subtract(close);
		if (currentIndex < cutVectors.Count)
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
			if (currentIndex >= cutVectors.Count) {
				return;
			}
			CutPoint cut = currentCutPoint.GetComponent<CutPoint>();
			cut.SetCutVector(cutVectors[currentIndex]);
        }

		if (Input.GetMouseButtonUp(0)) {
			// Protect against null value.
			// TODO: this is dirty.
			if (currentIndex >= cutVectors.Count) {
				return;
			}

			holding = false;
			Vector2 mousePos = Input.mousePosition;
			touchVector = ConvertToWorldPoint(mousePos) - touchOrigin;
			float close = CalculateCloseness(touchOrigin.Value, touchVector.Value, swipeTime);
			//Debug.Log(close);
			QualityBar.Subtract(close*close);
			if (currentIndex < cutVectors.Count)
				currentIndex++;
			DrawDebugLine(touchOrigin.Value);

			// Reset the origin.
			touchOrigin = null;

			if (currentIndex >= cutVectors.Count) {
				GameOver();
			} else {
				Destroy(currentCutPoint);
				SpawnCut(cutOrigins[currentIndex], cutVectors[currentIndex]);
			}
		}

        if (holding) {
            swipeTime += Time.deltaTime;
            Particle.Emit(AmountOfParticles);
        }
	}
	
	
	
	/* START CUT THINGS */
	private void SpawnCut(Vector3 origin, Vector3 cut) {
		// Instantiate cut at point.
		currentCutPoint = Instantiate(CutIndicator, origin, Quaternion.identity);
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
		originCloseness = Mathf.InverseLerp(0, MaximumDistance, distance);
		// So the debugUI can use it.
		//this.oCloseness = originCloseness;

		// Calculate how similar the vectors are (how close the player was to the correct swipe).
		// Normalize so that distance doesn't affect the dot product.
		Vector3 vn = Vector3.Normalize(vec);
		Vector3 cn = Vector3.Normalize(cutVectors[currentIndex]);
		float vSimilarity = 1-Vector3.Dot(vn, cn);
		vectorCloseness = Mathf.InverseLerp(0, MaximumCloseness, vSimilarity);
		//this.vCloseness = vectorCloseness;

		// Calculate how close the vector length is to the optimum length.  (did the player overshoot? undershoot? etc).
		float vl = Vector3.Magnitude(vec);
		float cl = Vector3.Magnitude(cutVectors[currentIndex]);
		lengthCloseness = Mathf.InverseLerp(0, MaximumLength, Mathf.Abs(vl-cl));
		//this.lCloseness = lengthCloseness;

		float timeCloseness = Mathf.InverseLerp(BaseTime, MaximumTime, time);
		//this.tCloseness = timeCloseness;

		originCloseness *= ImpactDistance;
		vectorCloseness *= ImpactCloseness;
		lengthCloseness *= ImpactLength;
		timeCloseness *= ImpactTime;

		//Debug.LogFormat("Origin closeness: {0}, Vector closeness: {1}, LengthCloseness: {2}, TimeCloseness: {3}", originCloseness, vectorCloseness, lengthCloseness, timeCloseness);

		return (originCloseness + vectorCloseness + lengthCloseness + timeCloseness) / 4f;
	}
	
	private void DrawDebugLine(Vector3 origin) {
		Vector3 end = ConvertToWorldPoint(Input.mousePosition);
		UnityEngine.Debug.DrawLine(origin, end, Color.yellow, 2);
	}

	// Debug function.
	private void DrawCuts(float lifetime) {
		for (int i = 0; i < cutOrigins.Count; i++) {
			// Place objects at start and end positions.
			Destroy(Instantiate(CutIndicator, cutOrigins[i], Quaternion.identity), lifetime);
			//Instantiate(debugObject, cutOrigins[i] + cutVectors[i], Quaternion.identity);
			Vector3 start = cutOrigins[i];
			Vector3 end = start + cutVectors[i];
			if (i == currentIndex)
				UnityEngine.Debug.DrawLine(start, end, Color.red, lifetime);
			else
				UnityEngine.Debug.DrawLine(start, end, Color.blue, lifetime);
		}
	}

	private Quality.QualityGrade grade = Quality.QualityGrade.Unset;
	private void GameOver() {
		Countdown.onComplete -= GameOver;
		grade = QualityBar.Finish();
		QualityBar.Disappear();
		//Quality.QualityGrade grade = Quality.FloatToGrade(grade, 3);
		GradeText.text = Quality.GradeToString(grade);
		GradeText.color = Quality.GradeToColor(grade);
		GradeText.gameObject.SetActive(true);

		ShowUIButtons();
	}

	public void Return() {
		ReturnOrRetry.Return("Cut " + GameManager.Instance.GemTypeTransfer, grade);
	}

	public void Retry() {
		ReturnOrRetry.Retry();
	}

    public void ShowUIButtons() {
	    ReturnOrRetryButtons.SetActive(true);
        ReturnOrRetryButtons.GetComponent<UpdateRetryButton>().SetText();
    }
}