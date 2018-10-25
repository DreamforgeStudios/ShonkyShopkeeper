using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DifficultySmeltingDictionary : SerializableDictionary<Difficulty, SmeltingDifficultySettings> {}

[System.Serializable]
public class SmeltingDifficultySettings {
	public AnimationCurve ClosenessPointsCurve;
	// Points multiplier..
	public float ClosenessContribution;

	public float MinVolatilityNegativeMomentum,
		MaxVolatilityNegativeMomentum,
		MinVolatilityTapForce,
		MaxVolatilityTapForce;

	public AnimationCurve VolatilityCurve;
}

public class Smelting : MonoBehaviour {

	[BoxGroup("Game Properties")]
	// The maximum jump we can make in a single frame.  This is mostly to avoid looping.
	// Set equal to maxRotation.z if causing problems.
	public float maxJump;
	[BoxGroup("Game Properties")]
	// Success point (z rotation);
	public float successPoint;
	[BoxGroup("Game Properties")]
	// "Green" range.
	public float successRange;

	/*
	[BoxGroup("Balance")]
    // Curve that defines the multiplier for closeness (how much we should take away).
    public AnimationCurve closenessCurve;
	[BoxGroup("Balance")]
	public float MinVolatilityNegativeMomentum, MaxVolatilityNegativeMomentum,
				 MinVolatilityTapForce, MaxVolatilityTapForce;
	[BoxGroup("Balance")]
	// Curve that defines how volatile the dial becomes over time.
	public AnimationCurve volatilityCurve;
	[BoxGroup("Balance")]
	public float closenessContribution;
	[BoxGroup("Balance")]
    // The amount of negative momentum we should apply each tick.
    public float negativeMomentum;
	[BoxGroup("Balance")]
	// The amount of positive momentum that we should apply on a tap.
	public float tapForce;
	*/

	[BoxGroup("Balance")]
	public DifficultySmeltingDictionary DifficultySettings;
	[BoxGroup("Balance")]
	public bool ManualDifficultyOverride;
	[BoxGroup("Balance")]
	[EnableIf("ManualDifficultyOverride")]
	public Difficulty ManualDifficulty;
	
	[BoxGroup("Feel")]
    public float heldTickrate;
	[BoxGroup("Feel")]
    // Curve that defines the impact of holding.
    public AnimationCurve accelerationCurve;
	[BoxGroup("Feel")]
	public float MissDurationTimout;

	[BoxGroup("Object Assignments")]
    public PointsManager pointsManager;
	[BoxGroup("Object Assignments")]
	public UpdateRetryButton returnOrRetryButtons;
	[BoxGroup("Object Assignments")]
	public PartyButtonManager PartyReturnButtons;
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI qualityText;
	[BoxGroup("Object Assignments")]
    public Sprite feedbackPositive;
	[BoxGroup("Object Assignments")]
    public Sprite feedbackNegative;
	[BoxGroup("Object Assignments")]
    public Material feedbackMaterial;
	[BoxGroup("Object Assignments")]
	public ParticleSystem feedbackParticleSystem;
	[BoxGroup("Object Assignments")]
	public InstructionHandler instructionManager;
	[BoxGroup("Object Assignments")]
	public OreSpawnManager OreSpawnManager;
	[BoxGroup("Object Assignments")]
	public Countdown CountdownObj;
	[BoxGroup("Object Assignments")]
	public Transform Dial;

	private Quality.QualityGrade grade = Quality.QualityGrade.Unset;
	private float missDurationCounter;
	// The rigidbody attached to this game object.
	private Rigidbody rb;
	// Previous rotation.
	private Vector3 prevRotation;
	private SmeltingDifficultySettings activeDifficultySettings;

	private float timeCounter;

    private bool start;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += () => { Time.timeScale = 1; start = true; };
    }

	void Start () {
		//Debug.Log(gameObject.name);
		SFX.Play("CraftingOre", looping: true);
		SFX.Play("fire_loop", looping: true);
	    if (GameManager.Instance.ActiveGameMode == GameMode.Story) {
			Countdown.onComplete += GameOver;
	    } else if (GameManager.Instance.ActiveGameMode == GameMode.Party) {
		    Countdown.onComplete += GameOverParty;
	    }
		
	    Difficulty d = ManualDifficultyOverride ? ManualDifficulty : PersistentData.Instance.Difficulty;
	    if (!DifficultySettings.TryGetValue(d, out activeDifficultySettings)) {
		    Debug.LogError("The current difficulty (" + PersistentData.Instance.Difficulty.ToString() +
		                     ") does not have a SmeltingDifficultySettings associated with it.");
	    }
		
		rb = Dial.GetComponent<Rigidbody>();
		prevRotation = Dial.transform.localEulerAngles;
    }
	
	// Don't waste frames on mobile...
	void FixedUpdate() {
        if (!start)
            return;

		float volatility = Mathf.Lerp(activeDifficultySettings.MinVolatilityNegativeMomentum,
			activeDifficultySettings.MaxVolatilityNegativeMomentum,
			activeDifficultySettings.VolatilityCurve.Evaluate(timeCounter / CountdownObj.StartTime));
		
        // Continually rotate backwards
		//transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + volatility);// * negativeMomentum);
        rb.AddTorque(0, 0, volatility);
        // Alternative method.
        //transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, maxRotation, negativeMomentum, negativeMomentum);
        
		// Constrain to rotation boundaries.
		Constrain();

        UpdateBar();

		// Record previous location.
		prevRotation = Dial.transform.localEulerAngles;
	}

    void Update() {
        if (!start)
            return;

	    // If player seems to be struggling, show instructions again.
	    if (missDurationCounter > MissDurationTimout) {
		    missDurationCounter = 0;
		    instructionManager.PushInstruction();
	    }
	    
        missDurationCounter += Time.deltaTime;
	    timeCounter += Time.deltaTime;
	    
		// Check where we are running the program.
		RuntimePlatform p = Application.platform;
		if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer || p == RuntimePlatform.OSXEditor || p == RuntimePlatform.OSXPlayer)
			// Process mouse inputs.
			ProcessMouse();
		else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
			// Process touch inputs.
			ProcessTouch();
    }

    private bool holding = false;
    private float heldTime;
    private float nextTick;
    private void ProcessMouse() {
        if (Input.GetMouseButtonDown(0)) {
            Stow();
            holding = true;
            heldTime = 0f;
            nextTick = heldTickrate;
        }

        if (Input.GetMouseButtonUp(0)) {
            holding = false;
        }

        if (holding) {
            heldTime += Time.deltaTime;
            if (heldTime > nextTick) {
                Stow();
                nextTick += heldTickrate;
            }
        }
    }

    private void ProcessTouch() {
        // You can use multiple fingers to stow faster... good or bad?
        foreach (Touch touch in Input.touches) {
            if (touch.phase == TouchPhase.Began) {
                Stow();
                holding = true;
                heldTime = 0f;
                nextTick = heldTickrate;
            }

            if (touch.phase == TouchPhase.Ended) {
                holding = false;
            }

            if (holding) {
                heldTime += Time.deltaTime;
                if (heldTime > nextTick) {
                    Stow();
                    nextTick += heldTickrate;
                }
            }
        }
    }

	private void Constrain() {
		// If we've made too big of a jump (probably looped), then don't allow the rotation.
		if (Mathf.Abs(Dial.localEulerAngles.z - prevRotation.z) > maxJump) {
			Dial.localEulerAngles = prevRotation;
		}
	}

    private void UpdateBar() {
		float closeness = Mathf.Min(Mathf.Abs(Dial.eulerAngles.z - successPoint) / successRange, 2);
        if (closeness < 1) {
	        missDurationCounter = 0;
            feedbackMaterial.SetTexture("_MainTex", feedbackPositive.texture);
			// Using a curve seemed like a good idea at the time...
			pointsManager.AddPoints(activeDifficultySettings.ClosenessPointsCurve.Evaluate(closeness) * activeDifficultySettings.ClosenessContribution * Time.deltaTime);
        } else {
            feedbackMaterial.SetTexture("_MainTex", feedbackNegative.texture);
			//qualityBar.Subtract((1-closenessCurve.Evaluate(closeness - 1)) * closenessContribution * Time.deltaTime, allowMoveDown: true);
        }
        //Debug.Log("closeness: " + closeness);
        //Debug.Log("closeness evaluation: " + closenessCurve.Evaluate(closeness));
	    //qualityBar.TickSubtraction = closenessCurve.Evaluate(closeness);
	    
    }

	public void Stow() {
		SFX.Play("Bidding_FailTap",0.5f,1f,0f,false);
		
		float volatility = Mathf.Lerp(activeDifficultySettings.MinVolatilityTapForce,
			activeDifficultySettings.MaxVolatilityTapForce,
			activeDifficultySettings.VolatilityCurve.Evaluate(timeCounter / CountdownObj.StartTime));
        float amountToStow = accelerationCurve.Evaluate(heldTime);
        //SFX.Play("bump_small");
        
        //rb.AddTorque(0, 0, (-tapForce * volatility) * amountToStow);
        rb.AddTorque(0, 0, -volatility * amountToStow);

        // Alternate approach.
        /*
		Vector3 rotAdd = new Vector3(0, 0, tapForce);
		Vector3 rot = transform.eulerAngles;
		rot = rot + rotAdd;
		transform.eulerAngles = rot;
		transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, transform.eulerAngles + rotAdd, negativeMomentum, negativeMomentum);
		*/
    }

    private void GameOver() {
        Countdown.onComplete -= GameOver;
	    start = false;
        grade = Quality.CalculateGradeFromPoints(pointsManager.GetPoints());
	    feedbackParticleSystem.Stop();
	    pointsManager.onFinishLeveling += () => {
		    OreSpawnManager.Upgrade(grade);
		    pointsManager.gameObject.SetActive(false);
		    qualityText.text = Quality.GradeToString(grade);
		    qualityText.color = Quality.GradeToColor(grade);
		    qualityText.gameObject.SetActive(true);
	    	SFX.Play(Quality.ReturnSFXName(grade),1f,1f,0f,false,0f);
	    };
	    
	    pointsManager.DoEndGameTransition();
	    
        ShowUIButtons();
    }
	
	public void JunkReturn()
	{
		ReturnOrRetry.Return("Brick", grade);
	}

	public void Return() {
		returnOrRetryButtons.DisableButtons();
		if (grade != Quality.QualityGrade.Junk)
			ReturnOrRetry.Return("Brick", grade);
		else
			returnOrRetryButtons.GetComponent<UpdateRetryButton>().WarningTextEnable();
	}

	public void Retry() {
		returnOrRetryButtons.DisableButtons();
		ReturnOrRetry.Retry();
	}

    public void ShowUIButtons() {
	    returnOrRetryButtons.gameObject.SetActive(true);
        returnOrRetryButtons.GetComponent<UpdateRetryButton>().SetText();
    }
	
	private void GameOverParty() {
		Countdown.onComplete -= GameOverParty;
		start = false;
		feedbackParticleSystem.Stop();

		pointsManager.onFinishLeveling += () => OreSpawnManager.Upgrade(Quality.QualityGrade.Mystic);
		pointsManager.DoEndGameTransitionParty();
	    
	    ShowUIButtonsParty();
	}
	
	public void PartyModeReturn() {
		PartyReturnButtons.DisableButtons();
	    ReturnOrRetry.ReturnParty(pointsManager.GetPoints());
	}

	public void ShowUIButtonsParty() {
		PartyReturnButtons.gameObject.SetActive(true);
	}
}
