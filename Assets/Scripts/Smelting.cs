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
	// Amount of time that the player should hold the position.
	//public float holdTime;
	//private float timeToGo;
    // The amount of negative momentum we should apply each tick.
    public float negativeMomentum;
	// The amount of positive momentum that we should apply on a tap.
	public float tapForce;
	// The maximum rotation that we should sit at.
	public Vector3 maxRotation;
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
	// The score that results in a perfect.
	public float bestScore;
	// The score that results in junk.
	public float worstScore;
    // Curve that defines the impact of holding.
    public AnimationCurve accelerationCurve;
    // Curve that defines the multiplier for closeness (how much we should take away).
    public AnimationCurve closenessMultiplier;

	// The rigidbody attached to this game object.
	private Rigidbody rb;
	// Previous rotation.
	private Vector3 prevRotation;

    //Two objects to show and hide for restart and scene change
    public GameObject nextScene;
    public GameObject retryScene;

    //Particle System
    public ParticleSystem particle;
    public int amountOfParticles = 5;
    private ParticleSystem.EmitParams emitParams;

    public QualityBar qualityBar;

    // For looking up items.
    public ItemDatabase db;


    //private bool started;
	//private float runningTotal;

	void Start () {
		rb = GetComponent<Rigidbody>();
        //transform.eulerAngles = new Vector3(0, 0, 354);
		prevRotation = transform.eulerAngles;
        Countdown.onComplete += GameOver;
		//timeToGo = holdTime;
		//started = false;
        nextScene.SetActive(false);
        retryScene.SetActive(false);
        emitParams = new ParticleSystem.EmitParams();
    }
	
	// Don't waste frames on mobile...
	void FixedUpdate() {
        // Continually rotate backwards
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + negativeMomentum);
        // Alternative method.
        //transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, maxRotation, negativeMomentum, negativeMomentum);
        
		// Constrain to rotation boundaries.
		Constrain();
		UpdateDebug();
		//UpdateTimer();

        UpdateQualityBar();
        UpdateFeedback();

		// Record previous location.
		prevRotation = transform.eulerAngles;
	}

    void Update() {
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

    private void UpdateQualityBar() {
		float closeness = Mathf.Abs(transform.eulerAngles.z - successPoint) / successRange;
        Debug.Log("closeness: " + closeness);
        Debug.Log("closeness evaluation: " + closenessMultiplier.Evaluate(closeness));
        qualityBar.SetFixedSubtraction(closeness * closenessMultiplier.Evaluate(closeness));
    }

    /*
	private void UpdateTimer() {
		float closeness = 1 - Mathf.Abs(transform.eulerAngles.z - successPoint) / successRange;
		if (closeness > 0 && timeToGo > 0) {
			started = true;
			float newTime = timeToGo - Time.deltaTime;
			timeToGo = newTime < 0 ? 0 : newTime;
			timer.text = timeToGo.ToString("n3");
		}
		
		if (started && timeToGo > 0) {
			// There should be a faster/more efficient way to do this.
			runningTotal += Mathf.Lerp(-1f, 1f, closeness);
			debug.text = runningTotal.ToString("n2");
		}

        if (timeToGo <= 0) {
			float quality = Mathf.InverseLerp(worstScore, bestScore, runningTotal);
			Quality.QualityGrade grade = Quality.FloatToGrade(quality, 3);
			this.grade.text = Quality.GradeToString(grade);
			this.grade.color = Quality.GradeToColor(grade);
			this.grade.gameObject.SetActive(true);
			if (GameManager.instance) {
				GameManager.instance.UpdateQuality(quality, 0);
			}
			
            ShowUIButtons();
        }
	}
    */

	private void UpdateDebug() {
        float closeness = 1 - Mathf.Abs(transform.eulerAngles.z - successPoint) / successRange;
        Color lerped = Color.Lerp(Color.red, Color.green, closeness);
        debugMaterial.color = lerped;
	}

	public void Stow() {
        float amountToStow = accelerationCurve.Evaluate(heldTime);
        particle.Emit((int)(amountToStow * amountOfParticles));
        
        rb.AddTorque(0, 0, -tapForce * amountToStow);

        // Alternate approach.
        /*
		Vector3 rotAdd = new Vector3(0, 0, tapForce);
		Vector3 rot = transform.eulerAngles;
		rot = rot + rotAdd;
		transform.eulerAngles = rot;
		transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, transform.eulerAngles + rotAdd, negativeMomentum, negativeMomentum);
		*/
    }

    public void ShowUIButtons() {
        nextScene.SetActive(true);
        retryScene.SetActive(true);
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

    private void GameOver() {
        Countdown.onComplete -= GameOver;

        var grade = qualityBar.Finish();
        qualityText.text = Quality.GradeToString(grade);
        qualityText.color = Quality.GradeToColor(grade);
        qualityText.gameObject.SetActive(true);
        qualityBar.Disappear();

        // TODO: back to shop button needs to change to facilitate restarting games.
        Inventory.Instance.InsertItem(new ItemInstance(db.GetActual("Brick"), 1, grade, true));

        ShowUIButtons();
    }
}
