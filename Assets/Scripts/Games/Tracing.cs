using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.XR;
using Random = UnityEngine.Random;

[System.Serializable]
public class DifficultyTracingDictionary : SerializableDictionary<Difficulty, TracingDifficultySettings> {}

[System.Serializable]
public class TracingDifficultySettings {
    public float ScoreMultiplier;
}

public class Tracing : MonoBehaviour {
    public TextMeshProUGUI qualityText;
    //public TextMeshProUGUI scoreText;
    public InstructionHandler instructionManager;

    //Tracing & Vector Lists
    private List<Vector3> playerPoints = new List<Vector3>();
    private List<Vector3> optimalPoints = new List<Vector3>();
    private List<int> optimalPointIndex = new List<int>();
    public List<Vector3> previousRuneLinger = new List<Vector3>();

    //Gameobject that holds the database of all Runes
    public GameObject TracingManager;
    private TracingDataBase _dataBase;
    
    //Current Rune GameObjects
    private GameObject _currentRune;
    private GameObject _currentRuneColliders;
    private GameObject _currentRuneSprite;
    private GameObject _currentRuneHitPoints;
    private SpriteRenderer _currentRuneSpriteRenderer;

    //Sphere that follows the player's finger
    public GameObject FollowSphere;

    //Line Renderer variables
    public LineRenderer lineRenderer;
    public float width;
    public Material material;
    public Color chosenStartColour;
    public Color chosenFinishColour;

    //Misc Variables
    private bool _canTrace = false;
    private Camera _mainCamera;
    private bool isMouseDown = false;
    private float mouseDownTime;
    private SceneManager sceneManager;
    private int designatedStartIndex;
    private float missDurationCounter;

    //Distance and tracking variables
    public int hitPoints = 0;
    public float maxDistanceAway = 0.5f;
    private bool foundNumber = false;
    private int lastIndex = 0;
    private float bestDistanceSoFar;
    private float totalDistanceAway = 0;
    private float averageDistanceAway = 0;

    //Score Variables
    public DifficultyTracingDictionary DifficultySettings;
	public bool ManualDifficultyOverride;
	[EnableIf("ManualDifficultyOverride")]
	public Difficulty ManualDifficulty;
    //public float ScoreMultiplier;
    private float score = 0;

    private TracingDifficultySettings activeDifficultySettings;
    //private float _finalScore = 0;

    //Tming Variables
    public float startTime;
    public float currentTime;
    public float finishTime;
    private float finalTime;
    public float timeLimit = 10.00f;
    private bool startTimer = false;
    public float MissDurationTimeout;

    //Quality bar.
    public PointsManager PointsManager;
    public GameObject returnOrRetryButtons;
    public GameObject PartyReturnButtons;
    public BrickSpawnManager brickSpawnmanager;
    private bool start = false;
    
    //Canvas flash
    public RawImage WhiteFlash;
    
    //Particle system used to give feedback
    public GameObject particlePrefab;
    private bool startedParticle;
    private GameObject feedbackParticleSystem;
    public Material goodTraceFeedback, badTraceFeedback;
    public LayerMask runeCollisionMask;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += (() => { Time.timeScale = 1; start = true; });
    }

    // Use this for initialization
    void Start() {
        SFX.Play("CraftingOre",1f,1f,0f,true,0f);
	    if (GameManager.Instance.ActiveGameMode == GameMode.Story) {
			Countdown.onComplete += GameOver;
	    } else if (GameManager.Instance.ActiveGameMode == GameMode.Party) {
		    Countdown.onComplete += GameOverParty;
	    }
        
	    Difficulty d = ManualDifficultyOverride ? ManualDifficulty : PersistentData.Instance.Difficulty;
	    if (!DifficultySettings.TryGetValue(d, out activeDifficultySettings)) {
		    Debug.LogError("The current difficulty (" + PersistentData.Instance.Difficulty.ToString() +
		                     ") does not have a TracingDifficultySettings associated with it.");
	    }
        
        finishTime = Time.time + 10f;
        GeneralSetup();
        //SetupLineRenderer();
        GetNecessaryPositions();
        SetupColliders();
    }

    // Update is called once per frame
    void Update() {
        if (!start)
        {
            _currentRuneSprite.SetActive(false);
            return;
        }

        if (_canTrace)
        {
            FadeInRune();
            currentTime = Time.time;
            GetInput();

            if (missDurationCounter > MissDurationTimeout) {
                missDurationCounter = 0;
                instructionManager.PushInstruction();
            }

            missDurationCounter += Time.deltaTime;
        }

        if (Time.time > finishTime && _canTrace)
        {
            GameOver();
        }
    }

    private void GeneralSetup()
    {
        _mainCamera = Camera.main;
        _dataBase = TracingManager.GetComponent<TracingDataBase>();
        _currentRune = _dataBase.GetRandomRune();
        SplitRuneObject();
        FollowSphere.SetActive(false);
        _canTrace = true;
        hitPoints = 0;
        averageDistanceAway = 0;
        score = 0;
        totalDistanceAway = 0;
        //Setup feedback Particle system
        feedbackParticleSystem = Instantiate(particlePrefab);
        feedbackParticleSystem.GetComponent<ParticleSystem>().Stop();
    }

    private void SplitRuneObject()
    {
        _currentRuneColliders = _currentRune.transform.GetChild(0).gameObject;
        _currentRuneSprite = _currentRune.transform.GetChild(1).gameObject;
        _currentRuneHitPoints = _currentRune.transform.GetChild(2).gameObject;
        _currentRuneSpriteRenderer = _currentRuneSprite.GetComponent<SpriteRenderer>();
    }
    
    private void GetNecessaryPositions() {
        for (int i = 0; i < _currentRuneHitPoints.transform.childCount; i++)
        {
            Vector3 position = _currentRuneHitPoints.transform.GetChild(i).gameObject.transform.position;
            optimalPoints.Add(position);
        }       
    }

    private void SetupColliders()
    {
        for (int i = 0; i < _currentRuneColliders.transform.childCount; i++)
        {
            GameObject collider = _currentRuneColliders.transform.GetChild(i).gameObject;
        }
    }
    
    private void GetInput() {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 mWorldPosition = ray.GetPoint(0.3f);
        FollowSphere.transform.position = mWorldPosition;

        if (Input.GetMouseButtonDown(0)) {
            SFX.Play("Tracing_taphold");
            isMouseDown = true;
            mouseDownTime = Time.time;
            if (!startTimer) {
                startTime = Time.time;
                finishTime = currentTime + timeLimit;
                startTimer = true;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            isMouseDown = false;
            FollowSphere.SetActive(false);
            hitPoints = 0;
            CheckPositions();
            score = CalculateColliderPenalties(CalculateAccuracy(CalculateWin()));
            //Reset Particle System
            if (startedParticle)
            {
                feedbackParticleSystem.GetComponent<ParticleSystem>().Stop();
                startedParticle = false;
            }
            //Add Score if greater than 0
            if (score > 0)
            {
                PointsManager.AddPoints(score * activeDifficultySettings.ScoreMultiplier);
                GiveFeedback();
                NextRune();
                missDurationCounter = 0;
            }
        }

        if (isMouseDown && (Time.time - mouseDownTime) > 0.02) {
            if (!playerPoints.Contains(mWorldPosition)) {
                FollowSphere.SetActive(true);
                playerPoints.Add(mWorldPosition);
                CheckForRune(mWorldPosition, ray);
            }
        }

    }

    private void GiveFeedback()
    {
        previousRuneLinger = ReverseList(playerPoints);
        lineRenderer.positionCount = previousRuneLinger.Count;
        lineRenderer.startWidth = 0.008f;
        lineRenderer.endWidth = 0.008f;
        lineRenderer.SetPositions(previousRuneLinger.ToArray());
        //Debug.Log("Score is " + score);
        Color customColor = Color.Lerp(Color.red,Color.green, score / 1200);
        lineRenderer.startColor = customColor;
        lineRenderer.endColor = customColor;
        //Stop previous coroutine and start new one
        StopCoroutine(FadePosition());
        StartCoroutine(FadePosition());
    }

    private List<Vector3> ReverseList(List<Vector3> listToReverse)
    {
        listToReverse.Reverse();
        return listToReverse;
    }

    private IEnumerator FadePosition()
    {
        while (lineRenderer.positionCount > 1)
        {
            lineRenderer.positionCount = lineRenderer.positionCount - 1;
            yield return new WaitForSeconds(0.05f);
        }
    }

	public Quality.QualityGrade grade = Quality.QualityGrade.Unset;
    private void GameOver()
    {
        Countdown.onComplete -= GameOver;

        var tmpGrade = Quality.CalculateGradeFromPoints(PointsManager.GetPoints());
        PointsManager.onFinishLeveling += () =>
        {
            brickSpawnmanager.Upgrade(tmpGrade);
            PointsManager.gameObject.SetActive(false);
            qualityText.text = Quality.GradeToString(tmpGrade);
            qualityText.color = Quality.GradeToColor(tmpGrade);
            qualityText.gameObject.SetActive(true);
        };
        
        PointsManager.DoEndGameTransition();
        FollowSphere.SetActive(false);
        _currentRuneSprite.SetActive(false);
        ResetOptimalPoints();
        
		grade = Quality.CalculateCombinedQuality(GameManager.Instance.QualityTransfer, tmpGrade);
        ShowUIButtons();
        _dataBase.HideUI();
        _canTrace = false;
    }

    private int CalculateAccuracy(bool success) {
        averageDistanceAway = totalDistanceAway / hitPoints;

        if (success) {
            averageDistanceAway = totalDistanceAway / optimalPointIndex.Count;
            Debug.Log("avg dist away = " + averageDistanceAway);
            if (averageDistanceAway >= 0 && averageDistanceAway <= 0.025) {
                return 1200;
            }
            else if (averageDistanceAway > 0.025 && averageDistanceAway < 0.05) {
                return 1000;
            }
            else if (averageDistanceAway > 0.05 && averageDistanceAway < 0.067) {
                return 850;
            }
            else if (averageDistanceAway > 0.067 && averageDistanceAway < 0.5) {
                return 600;
            }
            else {
                return 400;
            }
        }
        else {
            return 0;
        }
    }

    private int CalculateColliderPenalties(int score) {
        int colliderHits = FollowSphere.GetComponent<TracingColliding>().counter;
        FollowSphere.GetComponent<TracingColliding>().ResetCounter();
        Debug.Log("collider hits: " + colliderHits);
        // TODO: this is a bit rough...
        //qualityBar.Subtract(colliderHits * 0.1f);
        if (colliderHits == 0)
            return score;
        else if (colliderHits < 10)
            return score - (score / 4);
        else
            return score / 5;
    }

    private bool CalculateWin() {
        //Debug.Log(string.Format("Optimal points is {0} and hitpoints is {1}", optimalPoints.Count,hitPoints));
        if (optimalPoints.Count != hitPoints || currentTime > finishTime) {
            return false;
        }
        else {
            return true;
        }
    }
    //Check positions for Rune 1. This is a good system for runes that use 2 pieces
    private void CheckPositions() {
        lastIndex = 0;
        if (playerPoints.Count > 0 && playerPoints != null) {
            for (int i = 0; i < optimalPoints.Count; i++) {
                //Find the player point which is closest to this point if any
                if (FoundClosestPlayerPoint(i)) {
                    AddPoint();
                }
            }
            // TODO: remove previous quality calculation?
            //qualityBar.Subtract(totalDistanceAway / hitPoints);
        }
    }

    private void AddPoint() {
        //Debug.Log("Optimal points is " + optimalPoints.Count + " hitpoints is " + hitPoints + "Last index was " + lastIndex);
        optimalPointIndex.Add(lastIndex);
        hitPoints += 1;
        totalDistanceAway += bestDistanceSoFar;
    }

    private bool FoundClosestPlayerPoint(int currentPointToCompare) {
        Vector3 positionArea = optimalPoints[currentPointToCompare];
        foundNumber = false;
        bestDistanceSoFar = maxDistanceAway;
        for (int j = 0; j < playerPoints.Count; j++) {
            if (Vector3.Distance(playerPoints[j], positionArea) < maxDistanceAway)
                if (Vector3.Distance(playerPoints[j], positionArea) <= bestDistanceSoFar && j > lastIndex) {
                    bestDistanceSoFar = Vector3.Distance(playerPoints[j], positionArea);
                    lastIndex = j;
                    foundNumber = true;
                }
        }
        
        if (foundNumber)
            return true;
        else
            return false;
    }

    private void ResetOptimalPoints() {
        hitPoints = 0;
        //lineRenderer.positionCount = 0;
        optimalPoints.Clear();
        playerPoints.RemoveRange(0, playerPoints.Count);
        optimalPointIndex.RemoveRange(0, optimalPointIndex.Count);
        averageDistanceAway = 0f;
        totalDistanceAway = 0f;
    }

    private void NextRune()
    {
        Flash();
        ResetOptimalPoints();
        _currentRune = _dataBase.GetRandomRune();
        SFX.Play("Tracing_nxtshape");
        SplitRuneObject();
        GetNecessaryPositions();
        SetupColliders();
        score = 0;
    }

    private void FadeInRune()
    {
        _currentRuneSprite.SetActive(true);
        var alpha = _currentRuneSpriteRenderer.color;
        if (alpha.a < 1)
        {
            alpha.a = Mathf.Lerp(alpha.a, 1, Time.deltaTime * 2.5f);
            _currentRuneSpriteRenderer.color = alpha;
        }
    }

    private void Flash()
    {
        Color flashAlpha = WhiteFlash.color;
        flashAlpha.a = 0f;
        WhiteFlash.color = flashAlpha;
        WhiteFlash.enabled = true;
        WhiteFlash.DOFade(0.95f, 0.15f).OnComplete(() => WhiteFlash.DOFade(0f, 1f));   
    }
    
    //Method that raycasts the rune sprite to see if on the rune or on alpha
    private void CheckForRune(Vector3 mousePosition, Ray originalRaycast)
    {
        //Debug.Log("Added point was " + mousePosition);
        Debug.DrawRay(originalRaycast.origin,mousePosition,Color.red,1f);
        //If particle system not started, start it
        if (!startedParticle)
        {
            feedbackParticleSystem.GetComponent<ParticleSystem>().Play();
            startedParticle = true;
        }
        //Place particle system at point
        feedbackParticleSystem.transform.position = mousePosition;

        RaycastHit hit;
        if (Physics.Raycast(originalRaycast.origin,originalRaycast.direction,out hit,1f,runeCollisionMask))
        {
            Debug.Log("Collider tag " + hit.collider.tag);
            if (hit.collider.CompareTag("TracingRune"))
                feedbackParticleSystem.GetComponent<ParticleSystemRenderer>().material = goodTraceFeedback;
            else 
                feedbackParticleSystem.GetComponent<ParticleSystemRenderer>().material = badTraceFeedback;
        }
        else
        {
            feedbackParticleSystem.GetComponent<ParticleSystemRenderer>().material = badTraceFeedback;
        }
        
    }

    public void JunkReturn()
    {
        ReturnOrRetry.Return("shell", grade);
    }

    public void Return() {
        if (grade != Quality.QualityGrade.Junk)
		    ReturnOrRetry.Return("shell", grade);
        else
            returnOrRetryButtons.GetComponent<UpdateRetryButton>().WarningTextEnable();
	}

	public void Retry() {
		ReturnOrRetry.Retry();
	}

    public void ShowUIButtons() {
	    returnOrRetryButtons.SetActive(true);
        returnOrRetryButtons.GetComponent<UpdateRetryButton>().SetText();
    }

	private void GameOverParty() {
		Countdown.onComplete -= GameOverParty;

	    PointsManager.onFinishLeveling += () => brickSpawnmanager.Upgrade(Quality.QualityGrade.Mystic);
		PointsManager.DoEndGameTransitionParty();
	    
	    FollowSphere.SetActive(false);
	    _currentRuneSprite.SetActive(false);
	    ResetOptimalPoints();
	    
	    ShowUIButtonsParty();
	    _dataBase.HideUI();
		_canTrace = false;
	}
	
	public void PartyModeReturn() {
	    ReturnOrRetry.ReturnParty(PointsManager.GetPoints());
	}

	public void ShowUIButtonsParty() {
		PartyReturnButtons.SetActive(true);
	}
}
