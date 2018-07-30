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

	
	[BoxGroup("Balance")]
	[Tooltip("If the length of the player's cut vector is longer / shorter by this amount, they will receive a 1 (0%)" +
	         "for that cut.")]
	[Slider(0, 15)]
	public float MaximumLengthDifference = 5;
	[BoxGroup("Balance")]
	[Tooltip("If (one minus) the player's vector dot product the optimal vector is larger than this amount, they will" +
	         "receive a 1 (0%) for that cut.")]
	[Slider(0f, 1)]
	public float MaximumVectorCloseness = 1;
	[BoxGroup("Balance")]
	[Tooltip("The value which determines whether or not a particular cut was a success or fail.  Calculated based on" +
	         " the average of the previous two criterion.")]
	[Slider(0f, 1)]
	public float AcceptanceThreshold = .2f;
	[BoxGroup("Balance")]
	[Tooltip("The overall closeness value (0-1) is multiplied by this value and added to the quality bar.")]
	public float CutRewardMultiplier = .25f;
	
	
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
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI GradeText;
	
	
	// List of all cuts.
	private LinkedList<CutPoint> activeCuts;
	
	private bool start = false;
	// Keeps track of the time the game has been ongoing.
	private float timeCounter = 0;
	// Keeps track of the time ongoing between each interval.
	private float timeIntervalCounter = 0;
	// Touch origin needs to be passed between frames.
	private Vector3 touchOrigin;
	// Keeps track of what cut is currently active.
	private CutPoint activeCut = null;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += () => { Time.timeScale = 1; start = true; };
    }

    void Start () {
		Countdown.onComplete += GameOver;
	    activeCuts = new LinkedList<CutPoint>();
    }

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
	
	private void GameLoop() {
		// If it's time to spawn another cut.
		if (timeIntervalCounter > Mathf.Lerp(InitialSpawnInterval, EndSpawnInterval, SpawnCurve.Evaluate(timeCounter)) &&
		    CutPrefab.SpawnTime < CountdownObj.CurrentTimeRemaining) {
			// TODO: maybe add a parent to keep the scene clean.
			var cutPosition = GenerateNewCutPosition();
			CutPoint clone = Instantiate(CutPrefab, cutPosition, Quaternion.identity);
			// TODO: this is a bit messy, move GemObject calculation somewhere else.
			clone.CutVector = -(cutPosition - GemObject.transform.position)*1.8f;
			clone.onSpawnComplete += cut => activeCuts.AddLast(cut);

			timeIntervalCounter = 0;
		}
		
		timeIntervalCounter += Time.deltaTime;
		timeCounter += Time.deltaTime;
	}

	private void ProcessTouch() {
		// Don't let the player use multiple fingers, and don't run if there's no input.
		if (Input.touchCount > 1 || Input.touchCount == 0) {
			return;
		}

		Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began) {
	        // Touch pos in world space.
	        touchOrigin = Utility.ConvertToWorldPoint(touch.position);
	        // Find closest cut based on world space touch pos.
			activeCut = FindClosestCutPoint(touchOrigin);
	        // If no cuts available (list must be empty), don't select.
	        if (activeCut != null)
		        activeCut.SetSelected();
	        
        } else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
	        // If no cut selected, don't do anything to avoid null error.
	        if (activeCut == null)
		        return;
	        
	        var touchVector = Utility.ConvertToWorldPoint(touch.position) - touchOrigin;
	        // The touch has ended -- try to perform a cut -> doesn't mean that this cut will be a success.
	        PerformCut(activeCut, touchVector);
        }
	}

	// Only here for debugging on PC.
	//private bool holding = false;
	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			//holding = true;
			touchOrigin = Utility.ConvertToWorldPoint(Input.mousePosition);
			activeCut = FindClosestCutPoint(touchOrigin);
			
			if (activeCut != null)
				activeCut.SetSelected();
			
			//Debug.DrawLine(touchOrigin, activeCut.transform.position, Color.red, 10f);
		} else if (Input.GetMouseButtonUp(0)) {
			//holding = false;
			if (activeCut == null)
				return;
			
			var touchVector = Utility.ConvertToWorldPoint(Input.mousePosition) - touchOrigin;
			PerformCut(activeCut, touchVector);
		}
	}

	private void PerformCut(CutPoint activeCut, Vector3 cutVector) {
        float val = CalculateCloseness(activeCut.CutVector, cutVector);
        //Debug.Log("Calculated a closeness value of: " + val);
        if (val < AcceptanceThreshold) {
	        // TODO: animation.
	        SFX.Play("bump_small");
			QualityBar.Add((1-val) * CutRewardMultiplier);
			activeCuts.Remove(activeCut);
			Destroy(activeCut.gameObject);
	        activeCut = null;
        } else {
	        // TODO: fail sound / animation.
			activeCut.UnsetSelected();
		}
	}
	
	/* Utility ------ */
	// Generate a new random cut position based on the constraints listed in properties.
	private Vector3 GenerateNewCutPosition() {
		float distance = Random.Range(MinMaxDistance.x, MinMaxDistance.y);
		Vector3 vecPos = Utility.RotateAroundPivot(MaxStartPoint.normalized * distance, Vector3.forward,
			new Vector3(0, 0, Random.Range(0f, MaxAngle)));
		    
		return vecPos + GemObject.transform.position;
	}

	// Find cut point closest to another position.
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
	
	// Produce a scalar value representing how well a user performed a cut.
	private float CalculateCloseness(Vector3 guideVector, Vector3 userVector) {
		float vectorCloseness = 0f;
		float lengthCloseness = 0f;

		Vector3 gvn = Vector3.Normalize(guideVector);
		Vector3 uvn = Vector3.Normalize(userVector);
		vectorCloseness = 1 - Vector3.Dot(gvn, uvn);
		vectorCloseness = Mathf.InverseLerp(0, MaximumVectorCloseness, vectorCloseness);
		//Debug.Log("Vector similarity: " + vectorCloseness);

		float gvl = Vector3.Magnitude(guideVector);
		float uvl = Vector3.Magnitude(userVector);
		lengthCloseness = Mathf.InverseLerp(0, 5f, Mathf.Abs(gvl - uvl));
		//Debug.Log("Length similarity: " + lengthCloseness);

		return (vectorCloseness + lengthCloseness) / 2f;
	}
	
	private Quality.QualityGrade grade = Quality.QualityGrade.Unset;
	private void GameOver() {
		Countdown.onComplete -= GameOver;
		start = false;
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

	private void OnDrawGizmos() {
	}
}