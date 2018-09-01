using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Smelting : MonoBehaviour {
	// Timer object.
	//public TextMeshProUGUI timer;
	// Grade object.
	public TextMeshProUGUI qualityText;
	// Debug object.
	public TextMeshProUGUI debug;
    //Two sprites and their holder used to give feedback
    public Sprite more;
    public Sprite less;
    public Image holder;

    public Sprite feedbackPositive;
    public Sprite feedbackNegative;
    public Material feedbackMaterial;

	public OreSpawnManager OreSpawnManager;
    //private Image feedbackContainer;

	// Amount of time that the player should hold the position.
	//public float holdTime;
	//private float timeToGo;
    // The amount of negative momentum we should apply each tick.
    public float negativeMomentum;
	// The amount of positive momentum that we should apply on a tap.
	public float tapForce;
	// The maximum jump we can make in a single frame.  This is mostly to avoid looping.
	// Set equal to maxRotation.z if causing problems.
	public float maxJump;
	// Success point (z rotation);
	public float successPoint;
	// "Green" range.
	public float successRange;
	// Debug.
	public Material debugMaterial;
	// Grade based on how close player was to the middle.
    // Curve that defines the impact of holding.
    public AnimationCurve accelerationCurve;
    // Curve that defines the multiplier for closeness (how much we should take away).
    public AnimationCurve closenessCurve;
	public float closenessContribution;

	// The rigidbody attached to this game object.
	private Rigidbody rb;
	// Previous rotation.
	private Vector3 prevRotation;

    //Particle System
    public ParticleSystem particle;
    public int amountOfParticles = 5;
    //private ParticleSystem.EmitParams emitParams;

    public QualityBar qualityBar;
	public GameObject returnOrRetryButtons;

    // For looking up items.


    private bool start;
	//private float runningTotal;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += (() => { Time.timeScale = 1; start = true; });
    }

	void Start () {
		SFX.Play("CraftingOre", looping: true);
		SFX.Play("fire_loop", looping: true);
		
		rb = GetComponent<Rigidbody>();
		prevRotation = transform.eulerAngles;
        Countdown.onComplete += GameOver;
    }
	
	// Don't waste frames on mobile...
	void FixedUpdate() {
        if (!start)
            return;
		
        // Continually rotate backwards
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + negativeMomentum);
        // Alternative method.
        //transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, maxRotation, negativeMomentum, negativeMomentum);
        
		// Constrain to rotation boundaries.
		Constrain();
		UpdateDebug();

        UpdateBar();
        UpdateFeedback();

		// Record previous location.
		prevRotation = transform.eulerAngles;
	}

    void Update() {
        if (!start)
            return;
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
    public float heldTickrate;
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
		if (Mathf.Abs(transform.eulerAngles.z - prevRotation.z) > maxJump) {
			transform.eulerAngles = prevRotation;
		}
	}

    private void UpdateBar() {
		float closeness = Mathf.Min(Mathf.Abs(transform.eulerAngles.z - successPoint) / successRange, 2);
        if (closeness < 1) {
            feedbackMaterial.SetTexture("_MainTex", feedbackPositive.texture);
			// Using a curve seemed like a good idea at the time...
			qualityBar.Add(closenessCurve.Evaluate(closeness) * closenessContribution * Time.deltaTime, allowMoveUp: true);
        } else {
            feedbackMaterial.SetTexture("_MainTex", feedbackNegative.texture);
			qualityBar.Subtract((1-closenessCurve.Evaluate(closeness - 1)) * closenessContribution * Time.deltaTime, allowMoveDown: true);
        }
        //Debug.Log("closeness: " + closeness);
        //Debug.Log("closeness evaluation: " + closenessCurve.Evaluate(closeness));
	    //qualityBar.TickSubtraction = closenessCurve.Evaluate(closeness);
	    
    }

	private void UpdateDebug() {
        float closeness = 1 - Mathf.Abs(transform.eulerAngles.z - successPoint) / successRange;
        Color lerped = Color.Lerp(Color.red, Color.green, closeness);
        debugMaterial.color = lerped;
	}

	public void Stow() {
        float amountToStow = accelerationCurve.Evaluate(heldTime);
        particle.Emit((int)(amountToStow * amountOfParticles));
        SFX.Play("bump_small");
        
        rb.AddTorque(0, 0, -tapForce * amountToStow);
		//SFX.Play("sound");

        // Alternate approach.
        /*
		Vector3 rotAdd = new Vector3(0, 0, tapForce);
		Vector3 rot = transform.eulerAngles;
		rot = rot + rotAdd;
		transform.eulerAngles = rot;
		transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, transform.eulerAngles + rotAdd, negativeMomentum, negativeMomentum);
		*/
    }

    private void UpdateFeedback() {
        if(transform.eulerAngles.z < successPoint) {
            holder.enabled = true;
            holder.sprite = less;
        } else if (transform.eulerAngles.z > successPoint) {
            holder.enabled = true;
            holder.sprite = more;
        } else {
            holder.enabled = false;
        }
    }

	
	private Quality.QualityGrade grade = Quality.QualityGrade.Unset;
    private void GameOver() {
        Countdown.onComplete -= GameOver;

        grade = qualityBar.Finish();
        qualityText.text = Quality.GradeToString(grade);
        qualityText.color = Quality.GradeToColor(grade);
        qualityText.gameObject.SetActive(true);
        qualityBar.Disappear();
	    if (grade == Quality.QualityGrade.Junk)
			OreSpawnManager.Upgrade(false);
	    else
		    OreSpawnManager.Upgrade(true);
        ShowUIButtons();
    }
	
	public void JunkReturn()
	{
		ReturnOrRetry.Return("Brick", grade);
	}

	public void Return() {
		if (grade != Quality.QualityGrade.Junk)
			ReturnOrRetry.Return("Brick", grade);
		else
			returnOrRetryButtons.GetComponent<UpdateRetryButton>().WarningTextEnable();
	}

	public void Retry() {
		ReturnOrRetry.Retry();
	}

    public void ShowUIButtons() {
	    returnOrRetryButtons.SetActive(true);
        returnOrRetryButtons.GetComponent<UpdateRetryButton>().SetText();
    }
}
