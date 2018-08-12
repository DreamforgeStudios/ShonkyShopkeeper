using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;

[System.Serializable]
public class SegmentInfoDictionary : SerializableDictionary<BarterManager.Segment, SegmentInfo> {}

[System.Serializable]
public class SegmentInfo {
	//[SerializeField]
	//public BarterManager.Segment Segment;
	[SerializeField]
	public float PriceAdd;
	[SerializeField]
	public float TimeAdd;
	[SerializeField]
	[Range(-1, 1)]
	public float HappinessAdd;
	[SerializeField]
	public Vector2 MinMaxSize;
	[SerializeField]
	public Color Color;

}

public class BarterManager : MonoBehaviour {
	private Quality.QualityGrade golemQuality;
	private ItemInstance golem;
	private bool start = false;
	private float price = 0f;
	private float happiness = 0.5f;

	[BoxGroup("Balance")]
	public int NumberOfSegments;
	[BoxGroup("Balance")]
	public SegmentInfoDictionary SegmentInfoDict;

	[BoxGroup("Object Assignments")]
	public GameObject GolemSpawnPoint;
	[BoxGroup("Object Assignments")]
	public RadialBar RadialBar;
	[BoxGroup("Object Assignments")]
	public RadialSlider RadialSlider;
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI DebugText;
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI PriceText;
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI HappinessText;
	[BoxGroup("Object Assignments")]
	public Countdown HourGlass;
	[BoxGroup("Object Assignments")]
	public Image WizardRenderer;
	
	public enum Segment {
		Best, Good, Bad, Ok
	}

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

		if (GameManager.Instance.SpriteTransfer != null) {
			WizardRenderer.sprite = GameManager.Instance.SpriteTransfer;
		} else {
			Debug.LogWarning("Wizard sprite was not transfered, will use defaut.");
		}

		// Build a dictionary based on the list, which is easier for code to use.
		//SegmentInfosDict = BuildDictionary(SegmentInfos);
		// TODO: this isn't very obvious, is there a better way?
		RadialBar.DefaultColor = SegmentInfoDict[Segment.Ok].Color;

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
	}

	private void ProcessTouch() {
	}

	private void CalculateAndDoHit() {
		float sliderAngle = RadialSlider.Angle;
		
		var points = RadialBar.Points;
		foreach (var point in points) {
			if (sliderAngle > point.x - point.y * .5f && sliderAngle < point.x + point.y * .5f) {
				Hit((Segment) point.z);
				return;
			}
		}
		
		Hit(Segment.Ok);
	}

	private void Hit(Segment value) {
		//SegmentInfo pt = SegmentInfos.Find(x => Segment.Bad == value);
		SegmentInfo info = SegmentInfoDict[value];
		
		switch (value) {
			case Segment.Best:
				DebugText.color = Color.green;
				break;
			case Segment.Good:
				DebugText.color = new Color(.6f, 1, 0);
				break;
			case Segment.Ok:
				DebugText.color = Color.yellow;
				break;
			case Segment.Bad:
				DebugText.color = Color.red;
				break;
			default:
				DebugText.text = "Undefined.";
				DebugText.color = Color.gray;
				break;
		}

		HourGlass.CurrentTimeRemaining += info.TimeAdd;
		price += info.PriceAdd;
		happiness = Mathf.Clamp01(happiness + info.HappinessAdd);

		
		PriceText.text = "Price: $" + price;
		HappinessText.text = "Happy: " + (happiness * 100).ToString("0") + "%";
		DebugText.text = value.ToString();
		RadialSlider.PauseForDuration(.2f);
	}

	private void GeneratePoints() {
		// TODO: quality and golem type should affect this?
		float occupied = RadialSlider.MinRotation;
		float between = Mathf.Abs(RadialSlider.MinRotation) + Mathf.Abs(RadialSlider.MaxRotation) ;
		float segmentSize = between / NumberOfSegments;
		for (int i = 0; i < NumberOfSegments; i++) {
			Segment segmentType = (Segment) Random.Range((int) Segment.Best, (int) Segment.Bad + 1);
			SegmentInfo si = SegmentInfoDict[segmentType];
			
			float size = Random.Range(si.MinMaxSize.x, si.MinMaxSize.y);
			float angle = Random.Range(occupied + size * .5f, occupied + segmentSize);
			
			RadialBar.AddPoint(new Vector4(angle, size, (float) segmentType, 1), si.Color);

			occupied = angle + size * .5f;
		}
	}
	
	private void GameOver() {
		Countdown.onComplete -= GameOver;
		start = false;
	}
}
