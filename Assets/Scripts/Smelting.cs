using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Smelting : MonoBehaviour {
	// Timer object.
	public TextMeshProUGUI timer;
	// Grade object.
	public TextMeshProUGUI grade;
	// Debug object.
	public TextMeshProUGUI debug;
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

	// The rigidbody attached to this game object.
	private Rigidbody rb;
	// Previous rotation.
	private Vector3 prevRotation;

    //Two objects to show and hide for restart and scene change
    public GameObject nextScene;
    public GameObject retryScene;

	private bool started;
	private float runningTotal;

	void Start () {
		rb = transform.GetComponent<Rigidbody>();
		prevRotation = transform.eulerAngles;
		timeToGo = holdTime;
		started = false;
        nextScene.SetActive(false);
        retryScene.SetActive(false);
	}
	
	// Don't waste frames on mobile...
	void FixedUpdate() {
		// Continually rotate backwards.
		transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + negativeMomentum);
		// Alternative method.
		//transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, maxRotation, negativeMomentum, negativeMomentum);
		// Constrain.
		Constrain();
		UpdateDebug();
		UpdateTimer();

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
		float closeness = 1 - Mathf.Abs(transform.eulerAngles.z - successPoint) / successRange;
		Color lerped = Color.Lerp(Color.red, Color.green, closeness);
		debugMaterial.color = lerped;
		//Debug.Log("closeness:" + closeness + " pos: " + transform.eulerAngles.z);
	}

	public void Stow() {
		rb.AddTorque(0, 0, -tapForce);

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
}
