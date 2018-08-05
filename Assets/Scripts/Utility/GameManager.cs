﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour {
	private static GameManager _instance;

	// Lazy instantiation of GameManager.
	// Means that we don't have to manually place GameManager in each scene.
	// Not sure if this is the best way to do this yet...
	public static GameManager Instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<GameManager>();

				if (_instance == null) {
					var container = new GameObject("GameManager");
					_instance = container.AddComponent<GameManager>();
					_instance.SetupGameManager();
				}
			}

			return _instance;
		}
	}
    public static bool pickedUpGolem = false;

	// This is so that we support manually placing GameManagers in scenes.
	// NOTE: GameManagers placed in the scene will be destroyed if _instance is already set.
	// So this only really works in the "main" scene.
	// TODO: probably delete this and remove GameManager from shop scene.
	private void Awake() {
		if (_instance == null) {
			_instance = this;
		} else if (_instance != this) {
			Destroy(this.gameObject);
		}
		
		SetupGameManager();
	}

	private void SetupGameManager() {
		DontDestroyOnLoad(gameObject);
		Application.targetFrameRate = 60;

		if (Debug.isDebugBuild) {
			// TODO: make this automatically switch on / off.
			//Debug.unityLogger.logEnabled = false;
		}
	}

	public Item.GemType GemTypeTransfer;
	public Quality.QualityGrade QualityTransfer;
	public Personality PersonalityTransfer;
	public Sprite SpriteTransfer;
    public int RetriesRemaining = 0;
	public int ShonkyIndexTransfer = -1;
	public float CameraRotTransfer = 8;
	
	public Travel.Towns CurrentTown {
		get { return Inventory.Instance.GetCurrentTown(); }
	}
	
}
