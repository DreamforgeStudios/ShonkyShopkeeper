using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Linq;
using System.Security.Cryptography;
using DG.Tweening;
using Random = UnityEngine.Random;

[System.Serializable]
public class SegmentInfoDictionary : SerializableDictionary<BarterManager.Segment, SegmentInfo> {}

[System.Serializable]
public class PriceInfoDictionary : SerializableDictionary<Quality.QualityGrade, float> {}

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
	private GameObject golemClone;
	private bool start = false;
	private float price = 0f;
	private float happiness = 0.5f;

	[BoxGroup("Balance")]
	public int NumberOfSegments;
	[BoxGroup("Balance")]
	public SegmentInfoDictionary SegmentInfoDict;
	[BoxGroup("Balance")]
	public PriceInfoDictionary PriceInfoDict;

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
	public SpriteRenderer WizardRenderer;
	[BoxGroup("Object Assignments")]
	public GameObject Wand;
	[BoxGroup("Object Assignments")]
	public GameObject BackToShop;
	[BoxGroup("Object Assignments")]
	public ParticleSystem CoinFallParticles;
	
	public enum Segment {
		Best, Good, Bad, Ok
	}

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += () => { Time.timeScale = 1; start = true; };
	    //start = true;
    }
	
	// Use this for initialization
	void Start () {
		SFX.Play("BiddingTrack",1f,1f,0f,true,0f);
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

		golemClone = Instantiate(golem.item.physicalRepresentation, GolemSpawnPoint.transform.position, GolemSpawnPoint.transform.rotation, GolemSpawnPoint.transform);
		golemClone.GetComponent<Rigidbody>().isKinematic = true;

		if (GameManager.Instance.SpriteTransfer != null) {
			WizardRenderer.sprite = GameManager.Instance.SpriteTransfer;
		} else {
			Debug.LogWarning("Wizard sprite was not transfered, will use defaut.");
		}

		// Build a dictionary based on the list, which is easier for code to use.
		//SegmentInfosDict = BuildDictionary(SegmentInfos);
		// TODO: this isn't very obvious, is there a better way?
		RadialBar.DefaultColor = SegmentInfoDict[Segment.Ok].Color;

		price = PriceInfoDict[golemQuality];
		PriceText.text = "$" + price;
		c = PriceText.color;

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
		// Don't let the player use multiple fingers, and don't run if there's no input.
		if (Input.touchCount > 1 || Input.touchCount == 0) {
			return;
		}

		Touch touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began) {
			CalculateAndDoHit();
		}
	}

	private void CalculateAndDoHit() {
		float sliderAngle = RadialSlider.Angle;
		
		var points = RadialBar.Points;
		foreach (var point in points) {
			if (sliderAngle > point.x - point.y * .5f && sliderAngle < point.x + point.y * .5f) {
				Hit(point, (Segment) point.z);
				return;
			}
		}
		
		// This happens when the user hits nothing, which defaults as "OK".
		Hit(Vector4.zero, Segment.Ok);
	}

	private Color c;
	private void Hit(Vector4 point, Segment value) {
		//SegmentInfo pt = SegmentInfos.Find(x => Segment.Bad == value);
		SegmentInfo info = SegmentInfoDict[value];

		// TODO, base speed and stuff off how well they're doing.
		Animator anim = golemClone.GetComponent<Animator>();
		anim.SetBool("Pickup", true);
		anim.speed += .5f;
		
		RadialBar.ChangePoint(point, SegmentInfoDict[Segment.Bad].Color);

		HourGlass.CurrentTimeRemaining += info.TimeAdd;
		price += info.PriceAdd;
		happiness = Mathf.Clamp01(happiness + info.HappinessAdd);

		PriceText.text = "Price: $" + price;
		// TODO: when the design is finalised, allow these to be altered through parameters.
		// BUG: its possible that the transform gets punched into space if the user spams hard enough.
		PriceText.rectTransform.DOPunchAnchorPos(Vector2.up * 5, .7f);
		PriceText.DOColor(info.Color, .5f).SetEase(Ease.OutCirc).OnComplete(() => PriceText.DOColor(c, .5f).SetEase(Ease.OutCirc));
			
		HappinessText.text = "Happy: " + (happiness * 100).ToString("0") + "%";
		HappinessText.rectTransform.DOPunchAnchorPos(Vector2.up * 5, .7f);
		HappinessText.DOColor(info.Color, .5f).SetEase(Ease.OutCirc).OnComplete(() => HappinessText.DOColor(c, .5f).SetEase(Ease.OutCirc));
		
		DebugText.text = value.ToString();
		DebugText.color = info.Color;

		Wand.transform.DORotate(Vector3.right * 150f, .7f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCirc);

		if (info.PriceAdd > 0) {
			var burst = CoinFallParticles.emission.GetBurst(0);
			burst.cycleCount = (int) (info.PriceAdd * .1f);
			CoinFallParticles.emission.SetBurst(0, burst);
			CoinFallParticles.Stop();
			CoinFallParticles.time = 0;
			CoinFallParticles.Play();
		}
		
		RadialSlider.PauseForDuration(.2f);
	}

	private void GeneratePoints() {
		// TODO: quality and golem type should affect this?
		// TODO: add likelyhood for each segment to appear.
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
	
	public void Return() {
		ReturnOrRetry.Return((int)price, GameManager.Instance.ShonkyIndexTransfer);
	}
	
	private void GameOver() {
		Countdown.onComplete -= GameOver;
		start = false;
		
		BackToShop.SetActive(true);
	}
}
