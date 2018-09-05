using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NPCWalker : MonoBehaviour {
	public int walkDirection;
	// TODO: random walk speed variance?
	public float walkSpeed;

	public WizardSprite WizardSprite;

    private SpriteRenderer wizard;

	private bool enteredScreen = false;

    //Variables for walking animation and going to shop front when clicked
    private bool walkCycle = false;
    public bool walkNormal = true;

	// Use this for initialization
	void Start () {
        wizard = GetComponent<SpriteRenderer>();
		//InvokeRepeating("TestAndDestroy", 2f, 2f);
        walkNormal = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (walkNormal)
		    transform.position += new Vector3(walkDirection * walkSpeed * Time.deltaTime, 0, 0);

        if (!walkCycle)
            WizardPunch(0.1f, 0.5f);
	}

	// Check if the object is on the screen.  If not, set inactive.
	private void TestAndDestroy() {
		bool onScreen = TestOnScreen();
		if (onScreen && !enteredScreen) {
			enteredScreen = true;
		} else if (!onScreen && enteredScreen) {
			this.gameObject.SetActive(false);
		}
	}

	private bool TestOnScreen() {
		Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
		return screenPoint.z > 0 && screenPoint.x > 0 &&
			   screenPoint.x < 1 && screenPoint.y > 0 &&
			   screenPoint.y < 1;
	}

    public void WizardPunch(float strength, float duration) {
        walkCycle = true;
        //wizard.transform.DOPunchRotation(Vector3.forward * 25 * strength, duration, 0).SetEase(Ease.InOutBack).OnComplete(() => walkCycle = false);
    }

    public void SetWalkDirection(int walkDirection) {
		this.walkDirection = walkDirection;
	    if (walkDirection == -1)
	    {
		    transform.eulerAngles = new Vector3(0f,180f,0f);
	    }
	    else
	    {
		    transform.eulerAngles = new Vector3(0f,0f,0f);
	    }
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
