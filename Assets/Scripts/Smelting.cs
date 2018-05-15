using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Smelting : MonoBehaviour {
	// Timer object.
	public TextMeshProUGUI timer;
	// Grade object.
	public TextMeshProUGUI grade;
	// Debug object.
	public TextMeshProUGUI debug;
    //Two sprites and their holder used to give feedback
    public Sprite more;
    public Sprite less;
    public Image holder;
	// Amount of time that the player should hold the position.
	public float holdTime;
	private float timeToGo;
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

    private bool holding = false;

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

    private bool started;
	private float runningTotal;

	void Start () {
		rb = transform.GetComponent<Rigidbody>();
        transform.eulerAngles = new Vector3(0, 0, 354);
		prevRotation = transform.eulerAngles;
		timeToGo = holdTime;
		started = false;
        nextScene.SetActive(false);
        retryScene.SetActive(false);
        emitParams = new ParticleSystem.EmitParams();
    }
	
	// Don't waste frames on mobile...
	void FixedUpdate() {
        // Continually rotate backwards
        if (transform.eulerAngles.z < 355 )
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + negativeMomentum);

        // Alternative method.
        //transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, maxRotation, negativeMomentum, negativeMomentum);
        if (holding)
            Stow();
		// Constrain.
		Constrain();
		UpdateDebug();
		UpdateTimer();

        GiveFeedback();

		// Record previous location.
		prevRotation = transform.eulerAngles;
	}

	private void Constrain() {
		// If we've made too big of a jump (probably looped), then don't allow the rotation.
		if (Mathf.Abs(transform.eulerAngles.z - prevRotation.z) > maxJump) {
			transform.eulerAngles = prevRotation;
		}
	}

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

	private void UpdateDebug() {
        if (transform.eulerAngles.z > 140) {
            float closeness = 1 - Mathf.Abs(transform.eulerAngles.z - successPoint) / successRange;
            Color lerped = Color.Lerp(Color.red, Color.green, closeness);
            debugMaterial.color = lerped;
        } else if (transform.eulerAngles.z < 130) {
            float closeness = 1 - Mathf.Abs(transform.eulerAngles.z - successPoint) / successRange;
            Color lerped = Color.Lerp(Color.black, Color.green, closeness);
            debugMaterial.color = lerped;
        } else {
            debugMaterial.color = Color.green;
        }
		//Debug.Log("closeness:" + closeness + " pos: " + transform.eulerAngles.z);
	}

    public void ButtonDown() {
        holding = true;
    }

    public void ButtonUp() {
        holding = false;
    }
	public void Stow() {
        Debug.Log(transform.eulerAngles.z + " current vs max z " + maxRotation.z);
        if (transform.eulerAngles.z > maxRotation.z) {
            rb.AddTorque(0, 0, -tapForce);
            particle.Emit(10);
        }
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

    private void GiveFeedback() {
        if(transform.eulerAngles.z < 130) {
            holder.enabled = true;
            holder.sprite = less;
        } else if (transform.eulerAngles.z > 140) {
            holder.enabled = true;
            holder.sprite = more;
        } else {
            holder.enabled = false;
        }
    }

}
