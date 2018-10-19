using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode {
	Story,
	Party
}

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
		// Prewarm the audio manager to reduce stuttering.
		// Not sure if this will work after scene changes because Awake() is only called once on GameManager -- do static C# variables get destroyed?
		SFX.Prewarm();
	}

	private void SetupGameManager() {
		DontDestroyOnLoad(gameObject);
		Application.targetFrameRate = 60;

		if (Debug.isDebugBuild) {
			// TODO: make this automatically switch on / off.
			//Debug.unityLogger.logEnabled = false;
		}
		string currentScene = SceneManager.GetActiveScene().name;

		if (currentScene == "Shop"){
			InTutorial = false;
			TutorialIntroComplete = true;
		}
	}

	public GameMode ActiveGameMode = GameMode.Story;
	public bool PlayingAudio = true;
	public bool InTutorial = false;
	public bool TutorialIntroTopComplete, TutorialIntroComplete, InMap, BarterTutorial, BarterNPC, OfferNPC, introducedNPC = false;
	public bool HasInspectedAllInventoryItems = false;
	public bool TutorialGolemMade = false;
	public bool MineGoleminteractGolem = false;
	public bool SendToMine,HasMinePouch, WaitingForTimer, TimerComplete, ReturnPouch, OpenPouch, firstTownSelect = false;
	public List<string> InspectedItems = new List<string>();
	public Item.GemType GemTypeTransfer;
	public Quality.QualityGrade QualityTransfer;
	public Personality PersonalityTransfer;
	public string WizardTransfer;
    public int RetriesRemaining = 0;
	public int ShonkyIndexTransfer = 0;
	public float CameraRotTransfer = 8;
	//Boolean to control tool use during golem combination
	public bool canUseTools = true;
	//Boolean to introduce true golem in hall and relevant string variable to state which
	public bool introduceTrueGolem;
	public string typeOfTrueGolem;
	
	
	// Party Variables.
	public Queue<RoundInfo> RoundQueue;
	public LinkedList<PostRoundInfo> RoundHistory;
	public List<PlayerInfo> PlayerInfos;
	
	public Travel.Towns CurrentTown {
		get { return Inventory.Instance.GetCurrentTown(); }
	}
	
}
