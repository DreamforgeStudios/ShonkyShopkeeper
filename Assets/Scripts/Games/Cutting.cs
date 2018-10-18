using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class DifficultyCuttingDictionary : SerializableDictionary<Difficulty, CuttingDifficultySettings> {}

[System.Serializable]
public class CuttingDifficultySettings {
	public float InitialSpawnInterval;
	public float EndSpawnInterval;
	public AnimationCurve SpawnCurve;
	// If the length of the player's cut vector is longer / shorter by this amount, they will receive a 1 (0%).
	public float MaximumLengthDifference;
	// If (one minus) (player's vector (dot) optimal vector) is larger than this amount, they will receive a 1 (0%).
	public float MaximumVectorCloseness;
	// The value which determines whether or not a particular cut was a success or fail.  Calculated based on the average
	//  of the previous two criterion.
	public float AcceptanceThreshold;
	// The overall closeness value (0-1) is multiplier by this value and added to the points system.
	public float CutRewardMultiplier;
}

public class Cutting : MonoBehaviour {
	[BoxGroup("Game Properties")]
	[MinMaxSlider(2.5f, 6f)]
	public Vector2 MinMaxDistance;
	[BoxGroup("Game Properties")]
	public Vector3 MaxStartPoint;
	[BoxGroup("Game Properties")]
	public float MaxAngle;
	[BoxGroup("Game Properties")]
	public float CutTimeout;

	[BoxGroup("Balance")]
	public DifficultyCuttingDictionary DifficultySettings;
	[BoxGroup("Balance")]
	public bool ManualDifficultyOverride;
	[BoxGroup("Balance")]
	[EnableIf("ManualDifficultyOverride")]
	public Difficulty ManualDifficulty;
	/*
	[BoxGroup("Balance")]
	public float InitialSpawnInterval;
	[BoxGroup("Balance")]
	public float EndSpawnInterval;
	[BoxGroup("Balance")]
	public AnimationCurve SpawnCurve;
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
	*/
	
	[BoxGroup("Feel")]
	[Tooltip("The punch duration is decided by the closeness of the cut.  Multiply it by something else using this value.")]
	public float PunchDurationMultiplier = 1f;
	[BoxGroup("Feel")]
	[Range(0, 20)]
	public int PunchVibration;
	[BoxGroup("Feel")]
	[Range(0, 1)]
	public float PunchElasticity;
	[BoxGroup("Feel")]
	public Ease RotateEase;
	[BoxGroup("Feel")]
	public float RotateDurationMultiplier;
	[BoxGroup("Feel")]
	[Range(0, 360)]
	public float RotatePower;
	[BoxGroup("Feel")]
	[Tooltip("How long should the player not have any successes before showing the instruction text again?")]
	public float MissDurationTimeout;
	
	//[BoxGroup("Object Assignments")]
	//public QualityBar QualityBar;
	[BoxGroup("Object Assignments")]
	public PointsManager PointsManager;
	[BoxGroup("Object Assignments")]
	public GameObject ReturnOrRetryButtons;
	[BoxGroup("Object Assignments")]
	public GameObject PartyReturnButtons;
	[BoxGroup("Object Assignments")]
	public Countdown CountdownObj;
	[BoxGroup("Object Assignments")]
	public NewCutPoint CutPrefab;
	[BoxGroup("Object Assignments")]
	public GemSpawnManager GemSpawnManager;
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI GradeText;
	[BoxGroup("Object Assignments")]
	public InstructionHandler InstructionManager;
	
	
	// List of all cuts.
	private LinkedList<NewCutPoint> activeCuts;
	
	private bool start = false;
	// Keeps track of the time the game has been ongoing.
	private float timeCounter = 0;
	// Keeps track of the time ongoing between each interval.
	private float timeIntervalCounter = 0;
	// Touch origin needs to be passed between frames.
	private Vector3 touchOrigin;
	// Keeps track of what cut is currently active.
	private NewCutPoint activeCut = null;
	// Keeps track of the active punch tween, so we don't do multiple at once.
	private Tween activePunch = null;
	// Keeps track of the active rotation tween, so we don't do multiple at once.
	private Tween activeRotation = null;
	// Holds cut points, and keeps the scene a bit tidier.
	private GameObject cutContainer;
	// Keeps track of how long it's been since the player's last successful swipe.  If too long, show text again.
	private float missDurationCounter;
	// Active difficulty setting.
	private CuttingDifficultySettings activeDifficultySettings;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += () => { Time.timeScale = 1; start = true; };
    }

    void Start ()
    {
	    SFX.Play("CraftingGem", 1f, 1f, 0f, true, 0f);
	    if (GameManager.Instance.ActiveGameMode == GameMode.Story) {
			Countdown.onComplete += GameOver;
	    } else if (GameManager.Instance.ActiveGameMode == GameMode.Party) {
		    Countdown.onComplete += GameOverParty;
	    }

	    Difficulty d = ManualDifficultyOverride ? ManualDifficulty : PersistentData.Instance.Difficulty;
	    if (!DifficultySettings.TryGetValue(d, out activeDifficultySettings)) {
		    Debug.LogError("The current difficulty (" + PersistentData.Instance.Difficulty.ToString() +
		                     ") does not have a CuttingDifficultySettings associated with it.");
	    }
	    
	    activeCuts = new LinkedList<NewCutPoint>();
	    cutContainer = new GameObject("CutContainer");

	    // Spawn our first cut straight away instead of waiting.
	    timeIntervalCounter = activeDifficultySettings.InitialSpawnInterval;
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
		if (timeIntervalCounter > Mathf.Lerp(activeDifficultySettings.InitialSpawnInterval,
											 activeDifficultySettings.EndSpawnInterval,
											 activeDifficultySettings.SpawnCurve.Evaluate(timeCounter / CountdownObj.StartTime)) &&
		    CutPrefab.SpawnTime < CountdownObj.CurrentTimeRemaining) {
			// TODO: maybe add a parent to keep the scene clean.
			var cutPosition = GenerateNewCutPosition();
			NewCutPoint clone = Instantiate(CutPrefab, cutPosition, Quaternion.identity, cutContainer.transform);
			// TODO: this is a bit messy, move GemObject calculation somewhere else.
			clone.CutVector = -(cutPosition - GemSpawnManager.Gem.transform.position)*1.8f; // make the vector a bit longer.
			clone.onSpawnComplete += cut => activeCuts.AddLast(cut);
			clone.onTimeoutComplete += cut => activeCuts.Remove(cut);
            SFX.Play("Cutting_circle_appears");

            timeIntervalCounter = 0;
		}
		
		// If player is struggling, show instructions again.
		if (missDurationCounter > MissDurationTimeout) {
			missDurationCounter = 0;
			InstructionManager.PushInstruction();
		}
		
		timeIntervalCounter += Time.deltaTime;
		timeCounter += Time.deltaTime;
	    missDurationCounter += Time.deltaTime;
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

	private void PerformCut(NewCutPoint cut, Vector3 cutVector) {
        float val = CalculateCloseness(cut.CutVector, cutVector);
        //Debug.Log("Calculated a closeness value of: " + val);
        if (val < activeDifficultySettings.AcceptanceThreshold) {
	        // TODO: animation.
	        SFX.Play("Cutting_good_cut");
	        missDurationCounter = 0;
	        PushGem(cutVector, 1 - val);
	        PointsManager.AddPoints((1-val) * activeDifficultySettings.CutRewardMultiplier);
			//QualityBar.Add((1-val) * CutRewardMultiplier, true);
			activeCuts.Remove(cut);
			Destroy(cut.gameObject);
	        activeCut = null;
        } else {
	        // TODO: fail sound / animation.
			cut.UnsetSelected();
		}
	}

	// TODO: make it so that score affects gem push more.
	private void PushGem(Vector3 direction, float score = .5f) {
		// Prevent the gem from moving off-center.
		if (activePunch != null)
			activePunch.Complete();
		if (activeRotation != null)
			activeRotation.Complete();

		activePunch = GemSpawnManager.Gem.transform.DOPunchPosition(direction.normalized,
			score * PunchDurationMultiplier, PunchVibration, PunchElasticity);

		// Calculate the vector which is perpendicular to cut so that we can rotate around it.
		// https://gamedev.stackexchange.com/questions/70075/how-can-i-find-the-perpendicular-to-a-2d-vector
		Vector3 perpendicular = new Vector3(direction.y, -direction.x, direction.z) * RotatePower;
		activeRotation = GemSpawnManager.Gem.transform.DORotate(perpendicular, score * RotateDurationMultiplier, RotateMode.WorldAxisAdd)
			.SetEase(RotateEase);
	}
	
	/* Utility ------ */
	// Generate a new random cut position based on the constraints listed in properties.
	private Vector3 GenerateNewCutPosition() {
		float distance = Random.Range(MinMaxDistance.x, MinMaxDistance.y);
		Vector3 vecPos = Utility.RotateAroundPivot(MaxStartPoint.normalized * distance, Vector3.forward,
			new Vector3(0, 0, Random.Range(0f, MaxAngle)));
		    
		// Move the cut to be relative to the gem.
		return vecPos + GemSpawnManager.Gem.transform.position;
	}

	// Find cut point closest to another position.
	private NewCutPoint FindClosestCutPoint(Vector3 worldPoint) {
		if (activeCuts.Count == 0)
			return null;

		NewCutPoint closest = null;
		float minDistance = Mathf.Infinity;
		foreach (var cut in activeCuts) {
			float dist = Vector3.Distance(cut.transform.position, worldPoint);
			if (dist < minDistance) {
				closest = cut;
				minDistance = dist;
			}
		}

		//SFX.Play("sound");
		return closest;
	}
	
	// Produce a scalar value representing how well a user performed a cut.
	private float CalculateCloseness(Vector2 guideVector, Vector2 userVector) {
		float lengthCloseness = 0f;
		float vectorCloseness = 0f;

		float gvl = guideVector.magnitude;
		float uvl = userVector.magnitude;
		lengthCloseness = Mathf.InverseLerp(0, activeDifficultySettings.MaximumLengthDifference, Mathf.Abs(gvl - uvl));
		
		guideVector.Normalize();
		userVector.Normalize();;
		vectorCloseness = 1 - Vector3.Dot(guideVector, userVector);
		vectorCloseness = Mathf.InverseLerp(0, activeDifficultySettings.MaximumVectorCloseness, vectorCloseness);
		//Debug.Log("Vector similarity: " + vectorCloseness);

		//Debug.Log("Length similarity: " + lengthCloseness);

		return (vectorCloseness + lengthCloseness) / 2f;
	}
	
	private Quality.QualityGrade grade = Quality.QualityGrade.Unset;
	private void GameOver() {
		Countdown.onComplete -= GameOver;
		start = false;
		grade = Quality.CalculateGradeFromPoints(PointsManager.GetPoints());
		PointsManager.onFinishLeveling += () => {
            GemSpawnManager.UpgradeGem(grade);
			
			PointsManager.gameObject.SetActive(false);
            GradeText.text = Quality.GradeToString(grade);
            GradeText.color = Quality.GradeToColor(grade);
            GradeText.gameObject.SetActive(true);
		};
		
		PointsManager.DoEndGameTransition();
		
		foreach (NewCutPoint cut in activeCuts) {
			Destroy(cut.gameObject);
		}

		ShowUIButtons();
	}

	public void JunkReturn()
	{
		ReturnOrRetry.Return("Cut " + GameManager.Instance.GemTypeTransfer, grade);
	}

	public void Return() {
		if (grade != Quality.QualityGrade.Junk)
			ReturnOrRetry.Return("Cut " + GameManager.Instance.GemTypeTransfer, grade);
		else
			ReturnOrRetryButtons.GetComponent<UpdateRetryButton>().WarningTextEnable();
	}

	public void Retry() {
		ReturnOrRetry.Retry();
	}

    public void ShowUIButtons() {
	    ReturnOrRetryButtons.SetActive(true);
        ReturnOrRetryButtons.GetComponent<UpdateRetryButton>().SetText();
    }
	
	private void GameOverParty() {
		Countdown.onComplete -= GameOverParty;
		start = false;

		PointsManager.onFinishLeveling += () => GemSpawnManager.UpgradeGem(Quality.QualityGrade.Mystic);
		PointsManager.DoEndGameTransitionParty();
		
		foreach (NewCutPoint cut in activeCuts) {
			Destroy(cut.gameObject);
		}
		
		ShowUIButtonsParty();
	}
	
	public void PartyModeReturn() {
		ReturnOrRetry.ReturnParty(PointsManager.GetPoints());
	}

	public void ShowUIButtonsParty() {
		PartyReturnButtons.SetActive(true);
	}
}