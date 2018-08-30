using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class Tracing : MonoBehaviour {
    public TextMeshProUGUI qualityText;
    public TextMeshProUGUI scoreText;

    //Tracing & Vector Lists
    private List<Vector3> playerPoints = new List<Vector3>();
    private List<Vector3> optimalPoints = new List<Vector3>();
    private List<int> optimalPointIndex = new List<int>();

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

    //Distance and tracking variables
    public int hitPoints = 0;
    public float maxDistanceAway;
    private bool foundNumber = false;
    private int lastIndex = 0;
    private float bestDistanceSoFar;
    private float totalDistanceAway = 0;
    private float averageDistanceAway = 0;

    //Score Variables
    private float score = 0;
    private float _finalScore = 0;

    //Tming Variables
    public float startTime;
    public float currentTime;
    public float finishTime;
    private float finalTime;
    public float timeLimit = 10.00f;
    private bool startTimer = false;

    //Quality bar.
    public QualityBar qualityBar;
    public GameObject returnOrRetryButtons;
    public BrickSpawnManager brickSpawnmanager;
    private bool start = false;
    
    //Canvas flash
    public RawImage WhiteFlash;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += (() => { Time.timeScale = 1; start = true; });
        //qualityBar.Subtract(1f,false);
    }

    // Use this for initialization
    void Start() {
        SFX.Play("CraftingOre",1f,1f,0f,true,0f);
        //SFX.Play();
        Countdown.onComplete += GameOver;
        finishTime = Time.time + 10f;
        GeneralSetup();
        SetupLineRenderer();
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
        
    }

    private void SplitRuneObject()
    {
        _currentRuneColliders = _currentRune.transform.GetChild(0).gameObject;
        _currentRuneSprite = _currentRune.transform.GetChild(1).gameObject;
        _currentRuneHitPoints = _currentRune.transform.GetChild(2).gameObject;
        _currentRuneSpriteRenderer = _currentRuneSprite.GetComponent<SpriteRenderer>();
    }

    /*
    //Helper method to showcase optimal points
    private void DrawOptimalLines() {
        int ID = 0;
        foreach (Vector3 position in optimalPoints) {
            if (!playerPoints.Contains(position)) {
                playerPoints.Add(position);
                lineRenderer.positionCount = playerPoints.Count;
                lineRenderer.SetPosition(playerPoints.Count - 1, playerPoints[playerPoints.Count - 1]);
                ID++;
            }
        }
    }
    */

    private void SetupLineRenderer() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.positionCount = 0;
        lineRenderer.startColor = chosenStartColour;
        lineRenderer.endColor = chosenFinishColour;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.sortingLayerName = "LineRenderer";
    }
    private void GetNecessaryPositions() {
        for (int i = 0; i < _currentRuneHitPoints.transform.childCount; i++)
        {
            Vector3 position = _currentRuneHitPoints.transform.GetChild(i).gameObject.transform.position;
            position.z = 10;
            optimalPoints.Add(position);
        }       
        //DrawOptimalLines();
    }

    private void SetupColliders()
    {
        for (int i = 0; i < _currentRuneColliders.transform.childCount; i++)
        {
            GameObject collider = _currentRuneColliders.transform.GetChild(i).gameObject;
            Vector3 colliderTransform = collider.transform.position;
            colliderTransform.z = 10;
            collider.transform.position = colliderTransform;
        }
    }
    
    private void GetInput() {
        Vector3 mPosition = Input.mousePosition;
        Vector3 mWorldPosition = _mainCamera.ScreenToWorldPoint(mPosition);
        mWorldPosition.z = 10;
        FollowSphere.transform.position = mWorldPosition;

        if (Input.GetMouseButtonDown(0)) {
            //SFX.Play("sound");
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
            if (score > 0)
            {
                Debug.Log("adding score of " + score);
                qualityBar.Add(score/1000,true);
                _finalScore += score;
                scoreText.text = string.Format("Final score is {0}", _finalScore);
                NextRune();
            }
        }

        if (isMouseDown && (Time.time - mouseDownTime) > 0.02) {
            if (!playerPoints.Contains(mWorldPosition)) {
                FollowSphere.SetActive(true);
                playerPoints.Add(mWorldPosition);
                lineRenderer.positionCount = playerPoints.Count;
                lineRenderer.SetPosition(playerPoints.Count - 1, playerPoints[playerPoints.Count - 1]);
            }
        }

    }

	public Quality.QualityGrade grade = Quality.QualityGrade.Unset;
    private void GameOver()
    {
        Countdown.onComplete -= GameOver;
        grade = qualityBar.Finish();
        qualityText.text = Quality.GradeToString(grade);
        qualityText.color = Quality.GradeToColor(grade);
        qualityText.gameObject.SetActive(true);
        qualityBar.Disappear();
        ResetOptimalPoints();
        _currentRuneSprite.SetActive(false);
		grade = Quality.CalculateCombinedQuality(GameManager.Instance.QualityTransfer, grade);
        ShowUIButtons();
        _dataBase.HideUI();
        _canTrace = false;
        
        brickSpawnmanager.Upgrade();
    }

    /*
    private void DetermineQuality(float finalScore) {
        float decimalScore = finalScore / 1000;
        // For transferring quality between scenes.
        if (GameManager.instance) {
            GameManager.instance.UpdateQuality(decimalScore, 1);
        }
        grade = Quality.FloatToGrade(decimalScore, 3);
    }
    */

    private int CalculateAccuracy(bool success) {
        averageDistanceAway = totalDistanceAway / hitPoints;// optimalPointIndex.Count;
        //Debug.Log("avg dist away = " + averageDistanceAway);

        if (success) {
            averageDistanceAway = totalDistanceAway / optimalPointIndex.Count;
            //Debug.Log("avg dist away = " + averageDistanceAway);
            if (averageDistanceAway >= 0 && averageDistanceAway <= 0.63) {
                return 1000;
            }
            else if (averageDistanceAway > 0.63 && averageDistanceAway < 0.67) {
                return 850;
            }
            else if (averageDistanceAway > 0.67 && averageDistanceAway < 0.7) {
                return 700;
            }
            else if (averageDistanceAway > 0.7 && averageDistanceAway < 0.77) {
                return 550;
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
        //Debug.Log("collider hits: " + colliderHits);
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
                    //Debug.Log("Found close point");
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
        lineRenderer.positionCount = 0;
        optimalPoints.Clear();
        playerPoints.RemoveRange(0, playerPoints.Count);
        optimalPointIndex.RemoveRange(0, optimalPointIndex.Count);
    }

    private void NextRune()
    {
        Flash();
        ResetOptimalPoints();
        _currentRune = _dataBase.GetRandomRune();
        //SFX.Play("sound");
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
        WhiteFlash.DOFade(0.8f, 0.15f).OnComplete(() => WhiteFlash.DOFade(0f, 1f));
        
    }

    public void Return() {
		ReturnOrRetry.Return("shell", grade);
	}

	public void Retry() {
		ReturnOrRetry.Retry();
	}

    public void ShowUIButtons() {
	    returnOrRetryButtons.SetActive(true);
        returnOrRetryButtons.GetComponent<UpdateRetryButton>().SetText();
    }

    

   
}
