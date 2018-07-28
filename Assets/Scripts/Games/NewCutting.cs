using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// INFO: Don't use InputManager (which will be event based) in this class.  It needs more precision and more details.
public class NewCutting : MonoBehaviour {
	[BoxGroup("Game Properties")]
	public float InitialSpawnInterval;
	[BoxGroup("Game Properties")]
	public float EndSpawnInterval;
	[BoxGroup("Game Properties")]
	public AnimationCurve SpawnCurve;
	[BoxGroup("Game Properties")]
	[MinMaxSlider(2.5f, 6f)]
	public Vector2 MinMaxDistance;
	[BoxGroup("Game Properties")]
	public Vector3 MaxStartPoint;
	[BoxGroup("Game Properties")]
	public float MaxAngle;
	
	[BoxGroup("Object Assignments")]
	public QualityBar QualityBar;
	[BoxGroup("Object Assignments")]
	public GameObject ReturnOrRetryButtons;
	[BoxGroup("Object Assignments")]
	public Countdown CountdownObj;
	[BoxGroup("Object Assignments")]
	public CutPoint CutPrefab;
	[BoxGroup("Object Assignments")]
	public GameObject GemObject;
	
	private LinkedList<CutPoint> activeCuts;
	
	private bool start = false;
	// Keeps track of the time the game has been ongoing.
	private float timeCounter = 0;
	// Keeps track of the time ongoing between each interval.
	private float timeIntervalCounter = 0;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += () => { Time.timeScale = 1; start = true; };
    }

    // Use this for initialization
    void Start () {
		Countdown.onComplete += GameOver;
	    activeCuts = new LinkedList<CutPoint>();
    }

	private Vector3 GenerateNewCutPosition() {
		float distance = Random.Range(MinMaxDistance.x, MinMaxDistance.y);
		Vector3 vecPos = Utility.RotateAroundPivot(MaxStartPoint.normalized * distance, Vector3.forward,
			new Vector3(0, 0, Random.Range(0f, MaxAngle)));
		    
		return vecPos + GemObject.transform.position;
	}

	private void GameLoop() {
		// If it's time to spawn another cut.
		if (timeIntervalCounter > Mathf.Lerp(InitialSpawnInterval, EndSpawnInterval, SpawnCurve.Evaluate(timeCounter))) {
			// TODO: maybe add a parent to keep the scene clean.
			var cutPosition = GenerateNewCutPosition();
			CutPoint clone = Instantiate(CutPrefab, cutPosition, Quaternion.identity);
			// TODO: this is a bit messy, move GemObject calculation somewhere else.
			clone.CutVector = -(cutPosition - GemObject.transform.position)*1.8f;
			clone.onSpawnComplete += AddCut;

			timeIntervalCounter = 0;
		}
		
		//UpdateSelected();

		timeIntervalCounter += Time.deltaTime;
		timeCounter += Time.deltaTime;
	}

	private CutPoint FindClosestCutPoint(Vector3 worldPoint) {
		if (activeCuts.Count == 0)
			return null;

		CutPoint closest = null;
		float minDistance = Mathf.Infinity;
		foreach (CutPoint cut in activeCuts) {
			float dist = Vector3.Distance(cut.transform.position, worldPoint);
			if (dist < minDistance) {
				closest = cut;
				minDistance = dist;
			}
		}

		return closest;
	}

	private void UpdateSelected() {
		if (activeCut != null) {
			activeCut.SetSelected();
		} else if (activeCuts.Count > 0) {
			activeCuts.Last.Value.SetSelected();
		}
	}

	private void AddCut(CutPoint cut) {
		activeCuts.AddLast(cut);
	}

	// Update is called once per frame
	void Update () {
		// Don't do anything if the game hasn't started.
		if (!start)
			return;

		GameLoop();

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
        if (touch.phase == TouchPhase.Began) {
		} else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
		} else {
		}
	}

	private float CalculateCloseness(Vector3 guideVector, Vector3 userVector) {
		float vectorCloseness = 0f;
		float lengthCloseness = 0f;

		Vector3 gvn = Vector3.Normalize(guideVector);
		Vector3 uvn = Vector3.Normalize(userVector);
		vectorCloseness = 1 - Vector3.Dot(gvn, uvn);
		Debug.Log("Vector similarity: " + vectorCloseness);

		float gvl = Vector3.Magnitude(guideVector);
		float uvl = Vector3.Magnitude(userVector);
		lengthCloseness = Mathf.InverseLerp(0, 5f, Mathf.Abs(gvl - uvl));
		Debug.Log("Length similarity: " + vectorCloseness);

		return (vectorCloseness + lengthCloseness) / 2f;
	}


	// Only here for debugging on PC.
	private bool holding = false;
	private Vector3 touchOrigin;
	private Vector3 touchVector;
	private CutPoint activeCut = null;
	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			holding = true;
			touchOrigin = Utility.ConvertToWorldPoint(Input.mousePosition);
			activeCut = FindClosestCutPoint(touchOrigin);//activeCuts.Last.Value;
			activeCut.SetSelected();
			Debug.DrawLine(touchOrigin, activeCut.transform.position, Color.red, 10f);
		}

		if (Input.GetMouseButtonUp(0)) {
			holding = false;
			Vector2 mousePos = Input.mousePosition;
			touchVector = Utility.ConvertToWorldPoint(mousePos) - touchOrigin;
			float val = CalculateCloseness(activeCut.CutVector, touchVector);
			Debug.Log("Calculated a closeness value of: " + val);
			if (val < 0.2f) {
				//float close = CalculateCloseness(touchOrigin, touchVector);
				activeCuts.Remove(activeCut);
				Destroy(activeCut.gameObject);
				activeCut = null;
			} else {
				activeCut.UnsetSelected();
			}
		}

        if (holding) {
        }
	}
	
	
	
	private Quality.QualityGrade grade = Quality.QualityGrade.Unset;
	private void GameOver() {
		Countdown.onComplete -= GameOver;
		start = false;
		grade = QualityBar.Finish();
		QualityBar.Disappear();
		//Quality.QualityGrade grade = Quality.FloatToGrade(grade, 3);
		//GradeText.text = Quality.GradeToString(grade);
		//GradeText.color = Quality.GradeToColor(grade);
		//GradeText.gameObject.SetActive(true);

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

	private void OnDrawGizmos() {
	}
}