using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarterManager : MonoBehaviour {
	public GameObject GolemSpawnPoint;
	public RadialBar RadialBar;
	public RadialSlider RadialSlider;
	
	private Quality.QualityGrade golemQuality;
	private ItemInstance golem;
	private bool start = false;

    void Awake() {
        // Don't start until we're ready.
        //Time.timeScale = 0;
        //ReadyGo.onComplete += () => { Time.timeScale = 1; start = true; };
	    start = true;
    }
	
	// Use this for initialization
	void Start () {
		Countdown.onComplete += GameOver;
		
        ItemInstance tmp;
		// TODO: ShonkyInventory and Inventory should automatically initialise, without having to use InitializeFromDefault somewhere.
        if (ShonkyInventory.Instance != null && ShonkyInventory.Instance.GetItem(GameManager.Instance.ShonkyIndexTransfer, out tmp)) {
            golem = tmp;
	        golemQuality = golem.Quality;
        } else {
	        Debug.LogWarning("Shonky index was not transfered, is incorrect, or ShonkyInventory not initialized.  Will use default Shonky.");
            golem = new ItemInstance("rubygolem1", 1, Quality.QualityGrade.Sturdy, false);
	        golemQuality = golem.Quality;
        }

		GameObject clone = Instantiate(golem.item.physicalRepresentation, GolemSpawnPoint.transform.position, GolemSpawnPoint.transform.rotation, GolemSpawnPoint.transform);
		clone.GetComponent<Rigidbody>().isKinematic = true;
	}
	
	// Update is called once per frame
	void Update () {
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

	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			RadialSlider.Pause();
		}

		if (Input.GetMouseButtonUp(0)) {
			RadialSlider.Play();
		}
	}

	private void ProcessTouch() {
	}

	private void CalculateHit() {
		
	}

	private void GameOver() {
		Countdown.onComplete -= GameOver;
		start = false;
	}
}
