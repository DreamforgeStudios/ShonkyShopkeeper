using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWalker : MonoBehaviour {
	public int walkDirection;
	// TODO: random walk speed variance?
	public float walkSpeed;

	private bool enteredScreen = false;

	// Use this for initialization
	void Start () {
		InvokeRepeating("TestAndDestroy", 2f, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(walkDirection * walkSpeed * Time.deltaTime, 0, 0);
	}

	// Check if the object is on the screen.  If not, destroy.
	private void TestAndDestroy() {
		bool onScreen = TestOnScreen();
		if (onScreen && !enteredScreen) {
			enteredScreen = true;
		} else if (!onScreen && enteredScreen) {
			Destroy(this.gameObject);
		}
	}

	private bool TestOnScreen() {
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
		return screenPoint.z > 0 && screenPoint.x > 0 &&
			   screenPoint.x < 1 && screenPoint.y > 0 &&
			   screenPoint.y < 1;
	}

	public void SetWalkDirection(int walkDirection) {
		this.walkDirection = walkDirection;
	}

	public void SetWalkSpeed(float walkSpeed) {
		this.walkSpeed = walkSpeed;
	}

	public void SetScale(float scale) {
		transform.localScale = new Vector3(scale * transform.localScale.x,
										   scale * transform.localScale.y,
										   transform.localScale.z);
	}

	// Later...???
	/*
	Raycast to this object and run this function.
	public void LoadRealVersion() {
		??
	}
	*/
}
