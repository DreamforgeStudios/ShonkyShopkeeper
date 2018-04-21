using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Smelting : MonoBehaviour {
	// Timer object.
	public TextMeshProUGUI timer;
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

	// The rigidbody attached to this game object.
	private Rigidbody rb;
	// Previous rotation.
	private Vector3 prevRotation;

	void Start () {
		rb = transform.GetComponent<Rigidbody>();
		prevRotation = transform.eulerAngles;
		timeToGo = holdTime;
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
			float newTime = timeToGo - Time.deltaTime;
			timeToGo = newTime < 0 ? 0 : newTime;
			timer.text = timeToGo.ToString("n3");
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
		//Vector3 rotAdd = new Vector3(0, 0, tapForce);
		//Vector3 rot = transform.eulerAngles;
		//rot = rot + rotAdd;
		//transform.eulerAngles = rot;
		//transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, transform.eulerAngles + rotAdd, negativeMomentum, negativeMomentum);
	}
}
