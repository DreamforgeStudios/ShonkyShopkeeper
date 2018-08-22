using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;


public class Hall : MonoBehaviour
{

	//Variables for handling camera and globe movement
	private bool forward, mapInteraction = false;
	public Vector3 frontPos, backPos;
	public GameObject globe, townColliders;
	public float speed = 100.0f;
	public float Xrot, Yrot = 0f;
	
	//Object used to manage map enlargement
	public GameObject MapManager;
	private ShowMap _mapManager;
	
	//UI Text
	public TextMeshProUGUI goldAmount;
	private string spriteString = "<sprite=0>";
	public GameObject ShopButton;
	
	//Variables used to distinguish between click and holding
	private float _holdThreshold = 0.5f;
	private bool _click = false;
	private float _currentHoldDuration;
	
	// Use this for initialization
	void Start ()
	{
		Camera.main.transform.position = backPos;

		Setup();
		//Load the shop screen in the background as that is the only one which can be travelled to
		//StartCoroutine(LoadAsyncScene("Shop"));
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_mapManager.EnableGlobe)
		{
			CheckCamera();
			//ShopButton.SetActive(true);
			townColliders.SetActive(false);
			if (Input.GetMouseButtonUp(0))
			{
				if (_click && (Time.time - _currentHoldDuration) < _holdThreshold)
				{
					Debug.Log("Showing map");
					_mapManager.ShowMapOnScreen();
				}
				else
				{
					_click = false;
				}
			}
		}
		else
		{
			ShopButton.SetActive(false);
			townColliders.SetActive(true);
		}
	}

	private void CheckCamera()
	{
		if (Input.GetMouseButton(0))
		{
			RayCastSphere();
			MoveCamera();
		}
		else
		{
			mapInteraction = false;
		}
	}

	private void Setup()
	{
		goldAmount.enabled = false;
		ShopButton.SetActive(false);
		_mapManager = MapManager.GetComponent<ShowMap>();
	}

	private void MoveCamera()
	{
		//Debug.Log("Map Interaction is " + mapInteraction + " and forward is " + forward);
		if (!mapInteraction && MapTutorial.Instance.CanMoveCamera)
		{
			//Debug.Log("Not interacting with map");
			if (!forward)
			{
				Debug.Log("Moving Forward");
				Camera.main.transform.DOMove(frontPos, 1f).SetEase(Ease.InOutSine).OnComplete(() => forward = true);
				goldAmount.enabled = true;
			}
			else
			{
				Debug.Log("Moving Back");
				Camera.main.transform.DOMove(backPos, 1f).SetEase(Ease.InOutSine).OnComplete(() => forward = false);
				goldAmount.enabled = false;
				_click = false;
			}
		}
	}

	private void RayCastSphere()
	{
		if (forward)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawLine(ray.origin,ray.direction,Color.red);
			if (Physics.Raycast(ray, out hit, 1))
			{
				Debug.Log(hit.transform.gameObject.name + " hit");
				//Debug.Log(hit.transform.gameObject.name);
				if (hit.transform.gameObject.CompareTag("Globe") || mapInteraction)
				{
					RotateSphere(hit);
				}
			}
			else
			{
				if (mapInteraction)
				{
					RotateSphere(hit);
				}
				else
				{
					mapInteraction = false;
				}
			}
		}
		else
		{
			mapInteraction = false;
		}
	}

	private void RotateSphere(RaycastHit hit)
	{
		mapInteraction = true;
		
		if (Input.touchCount > 0)
		{
			Debug.Log("Touching globe with touch input");
			Touch touch = Input.GetTouch(0);
			Xrot += -touch.deltaPosition.y * Time.deltaTime * speed/30f;
			Xrot = Mathf.Clamp(Xrot, -30, 30);
			Yrot += -touch.deltaPosition.x * Time.deltaTime * speed/30f;
			Vector3 rotation = new Vector3(Xrot, Yrot, 0);
			globe.transform.localEulerAngles = rotation;
			if (!_click)
			{
				_click = true;
				_currentHoldDuration = Time.time;
			}
		}
		else if (Input.GetMouseButton(0))
		{
			Xrot += -Input.GetAxis("Mouse Y") * Time.deltaTime * speed;
			Xrot = Mathf.Clamp(Xrot, -30, 30);
			Yrot += -Input.GetAxis("Mouse X") * Time.deltaTime * speed;
			Vector3 rotation = new Vector3(Xrot, Yrot, 0);
			globe.transform.localEulerAngles = rotation;
			
			if (!_click)
			{
				_click = true;
				_currentHoldDuration = Time.time;
			}
		}
	}
	
	// Load a sync in the background.
	private AsyncOperation asyncLoad;
	IEnumerator LoadAsyncScene(string sceneName) {
		asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		asyncLoad.allowSceneActivation = false;

		// Wait until the asynchronous scene fully loads.
		// This includes actually starting the scene, so the coroutine wont stop until the scene is changed.
		while (!asyncLoad.isDone) {
			yield return new WaitForSeconds(.1f);
		}
	}
	/*
	//Update Visuals to show which towns are unlocked
	private void CheckUnlockedTowns() {
		List<Travel.Towns> unlockList = Inventory.Instance.GetUnlockedTowns();
		if (unlockList != null) {
			foreach (Travel.Towns town in unlockList) {
				switch (town) {
					case Travel.Towns.WickedGrove:
						town1.GetComponent<Renderer>().material = unlocked;
						break;
					case Travel.Towns.FlamingPeak:
						town2.GetComponent<Renderer>().material = unlocked;
						break;
					case Travel.Towns.GiantsPass:
						town3.GetComponent<Renderer>().material = unlocked;
						break;
					case Travel.Towns.SkyCity:
						town4.GetComponent<Renderer>().material = unlocked;
						break;
					default:
						break;
				}
			}
		}
	}
	*/
}
