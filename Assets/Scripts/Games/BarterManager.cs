using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class BarterManager : MonoBehaviour {
	public GameObject GolemSpawnPoint;
	public RadialBar RadialBar;
	public RadialSlider RadialSlider;
	public TextMeshProUGUI DebugText;
	
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
		
		GeneratePoints();
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
			//RadialSlider.Pause();
			CalculateAndDoHit();
		}

		if (Input.GetMouseButtonUp(0)) {
			//RadialSlider.Play();
		}
	}

	private void ProcessTouch() {
	}

	private void CalculateAndDoHit() {
		float sliderAngle = RadialSlider.Angle;
		
		var points = RadialBar.GetPoints();
		foreach (var point in points) {
			if (sliderAngle > point.x - point.y * .5f && sliderAngle < point.x + point.y * .5f) {
				Hit((int)point.z);
				return;
			}
		}
		
		Hit(2);
	}

	private void Hit(int value) {
		switch (value) {
			case 0:
				DebugText.text = "Best";
				DebugText.color = Color.green;
				break;
			case 1:
				DebugText.text = "Good";
				DebugText.color = new Color(.6f, 1, 0);
				break;
			case 2:
				DebugText.text = "OK";
				DebugText.color = Color.yellow;
				break;
			case 3:
				DebugText.text = "Bad";
				DebugText.color = Color.red;
				break;
			default:
				DebugText.text = "Undefined.";
				DebugText.color = Color.gray;
				break;
		}
		
		RadialSlider.PauseForDuration(.2f);
	}

	private void GeneratePoints() {
		// TODO: quality and golem type should affect this?
		const int NUM_POINTS = 7;
		const float MAX_ROTATION = 20f;
		float occupied = RadialSlider.MinRotation;
		float between = Mathf.Abs(RadialSlider.MinRotation) + Mathf.Abs(RadialSlider.MaxRotation) ;
		float segmentSize = between / NUM_POINTS;
		for (int i = 0; i < NUM_POINTS; i++) {
			float size = Random.Range(3f, 30f);
			float angle = Random.Range(occupied + size * .5f, occupied + segmentSize);
			//float angle = occupied + size * .5f;
			float weight = Random.Range(0, 4);
			RadialBar.AddPoint(new Vector4(angle, size, weight, 1));

			occupied = angle + size * .5f;
		}
	}

	private void GameOver() {
		Countdown.onComplete -= GameOver;
		start = false;
	}
}
