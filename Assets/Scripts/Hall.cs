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
	public Vector3 frontPos, backPos, inspectPos;
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
	
	//Tutorial Element
	public MapTutorial mapTutorialManager;
	
	//Default inv to fix reference issues
	public Inventory defaultInv;
	
	//New system for map screen and objects going around it
	public List<GameObject> townObjects, startingPositions, townCanvasElements;
	public GameObject startingPosition, townInspectPosition;
	private bool canMoveAround = false;
	//Need to keep track of select hit town due to scaling
	private RaycastHit townHit;
	
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
		goldAmount.text = string.Format("<sprite=0> {0}",Inventory.Instance.goldCount);
		if (_mapManager.EnableGlobe)
		{
			CheckCamera();
			if (!GameManager.Instance.InMap)
				ShopButton.SetActive(true);
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
	}

	private void Setup()
	{
		SaveManager.LoadFromTemplate(defaultInv);
		goldAmount.enabled = false;
		ShopButton.SetActive(false);
		_mapManager = MapManager.GetComponent<ShowMap>();
	}

	private void MoveCamera()
	{
		Debug.Log("Map Interaction is " + mapInteraction);
		if (!mapInteraction && MapTutorial.Instance.CanMoveCamera)
		{
			//Debug.Log("Not interacting with map");
			if (!forward)
			{
				Debug.Log("Moving Forward");
				Camera.main.transform.DOMove(frontPos, 1f).SetEase(Ease.InOutSine).OnComplete(() => forward = true);
				goldAmount.enabled = true;
				MoveAroundGlobe();
				if (GameManager.Instance.InMap)
				{
					if (mapTutorialManager.clone != null)
						mapTutorialManager.NextInstruction();
				}
			}
			else
			{
				Debug.Log("Moving Back");
				Camera.main.transform.DOMove(backPos, 1f).SetEase(Ease.InOutSine).OnComplete(() => forward = false);
				goldAmount.enabled = false;
				_click = false;
				canMoveAround = false;
			}
		}
	}

	private void RayCastSphere()
	{
		if (forward)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawLine(ray.origin,ray.direction,Color.red,4f);
			if (Physics.Raycast(ray, out hit, 1))
			{
				//Debug.Log(hit.transform.gameObject.name + " hit");
				if (hit.transform.gameObject.CompareTag("Town") && canMoveAround)
				{
					ChooseTown(hit);
				}
				else if (mapInteraction && !hit.transform.gameObject.CompareTag("Town"))
				{
					Debug.Log("Exiting map interaction");
					ExitMapInteraction();
				}
			}
			else
			{	
				if (mapInteraction)
				{
					Debug.Log("Exiting map interaction");
					ExitMapInteraction();
				}			
			}
		}
		else
		{
			mapInteraction = false;
		}
	}

	private void ChooseTown(RaycastHit hit)
	{
		mapInteraction = true;
		canMoveAround = false;
		townHit = hit;
		
		//Set select town sprite to render above others
		hit.transform.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
		
		//Move all townObjs to globe by first killing current tweens and then sending them back to the initial pos
		ReturnToGlobe();
		
		//Move camera to inspect pos and make chosen town larger
		Camera.main.transform.DOMove(inspectPos, 1f, false).OnComplete(() => 
			hit.transform.DOMove(townInspectPosition.transform.position,0.55f,false)).OnComplete(()=> hit.transform.DOScale(hit.transform.localScale * 2f,1f));
		
		//Load Relevant canvas elements
		Travel.Towns currentTown = CurrentTownObject(hit.transform.gameObject);
		ShowRelevantTownUI(currentTown, true);
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
	
	//Moves town objects from the centre of the sphere to each of their respective starting points
	private void MoveAroundGlobe()
	{
		canMoveAround = true;
		/*
		 * Really dirty right now as for loops + OnCompleteTweens don't work well
		 */
		townObjects[0].transform.DOMove(startingPositions[0].transform.position, 0.5f, false).OnComplete(() =>
			RotateAroundGlobe(townObjects[0].transform, 0));
		townObjects[1].transform.DOMove(startingPositions[1].transform.position, 0.5f, false).OnComplete(() =>
			RotateAroundGlobe(townObjects[1].transform,1));
		townObjects[2].transform.DOMove(startingPositions[2].transform.position, 0.5f, false).OnComplete(() =>
			RotateAroundGlobe(townObjects[2].transform,2));
		townObjects[3].transform.DOMove(startingPositions[3].transform.position, 0.5f, false).OnComplete(() =>
			RotateAroundGlobe(townObjects[3].transform,3));
	}

	//Recursive function that rotates the town objects around the sphere
	private void RotateAroundGlobe(Transform townTransform, int currentPositionIndex)
	{
		int nextPosition = currentPositionIndex + 1;
		if (nextPosition == 4)
			nextPosition = 0;
		if (canMoveAround)
		{
			townTransform.DOMove(startingPositions[nextPosition].transform.position, 5f, false).SetEase(Ease.InOutQuad)
				.OnComplete(() => RotateAroundGlobe(townTransform, nextPosition));
		}
	}

	//Sends all the town objects back into the sphere
	private void ReturnToGlobe()
	{
		canMoveAround = false;
		foreach (GameObject town in townObjects)
			town.transform.DOKill();

		foreach (GameObject town in townObjects)
			town.transform.DOMove(startingPosition.transform.position, 0.5f, false);
	}

	//Rescales the selected town, moves the camera back and then starts them circling the sphere again
	private void ExitMapInteraction()
	{
		townHit.transform.DOScale(townHit.transform.localScale / 2f, 0.1f).OnComplete(()=> MoveAroundGlobe());
		townHit.transform.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
		Camera.main.transform.DOMove(frontPos, 1f).SetEase(Ease.InOutSine);
		mapInteraction = false;
	}
	
	//Return current town
	private Travel.Towns CurrentTownObject(GameObject hitObject)
	{
		int index = townObjects.IndexOf(hitObject);
		switch (index) {
			case 1:
				return Travel.Towns.WickedGrove;
			case 0:
				return Travel.Towns.FlamingPeak;
			case 3:
				return Travel.Towns.GiantsPass;
			case 2:
				return Travel.Towns.SkyCity;
			default:
				return Travel.Towns.WickedGrove;
		}
	}

	private void ShowRelevantTownUI(Travel.Towns town, bool enable)
	{
		//Disable all elements
		foreach (GameObject element in townCanvasElements)
			element.SetActive(false);
		
		//Enable selected element
		switch (town)
		{
			case Travel.Towns.FlamingPeak:

				break;
			case Travel.Towns.GiantsPass:

				break;
			case Travel.Towns.SkyCity:

				break;
			case Travel.Towns.WickedGrove:

				break;
			default:

				break;
		}
	}
	
	//Method used when attempting to buy a new town through button. Also handles UI at same time
	public void PurchaseOrTravelTown()
	{
		Travel.Towns currentTownSelected = CurrentTownObject(townHit.transform.gameObject);
		bool completeTransaction = Travel.UnlockNewTown(currentTownSelected);
		//If this was the first town unlocked, make it the current
		Debug.Log(Inventory.Instance.GetUnlockedTowns().Count + " unlocked towns");
		Debug.Log("Complete transaction " + completeTransaction);
		if (Inventory.Instance.GetUnlockedTowns().Count == 1 && completeTransaction) {
			Travel.ChangeCurrentTown(currentTownSelected);
			SaveManager.SaveInventory();
			PlayerPrefs.SetInt("FirstStart", 1);
			if (GameManager.Instance.InMap)
			{
				GameManager.Instance.InMap = false;
				GameManager.Instance.BarterTutorial = true;
			}
			Initiate.Fade("Shop", Color.black, 2f);
		}
		//Else if it was a subsequent town, check the purchase was successful
		else {
			if (completeTransaction) {
                //SFX.Play("sound");
                SFX.Play("location_purchase", 1f, 1f, 0f, false, 0f);
                if (GameManager.Instance.InMap)
				{
					GameManager.Instance.InMap = false;
					GameManager.Instance.BarterTutorial = true;
				}
				Travel.ChangeCurrentTown(currentTownSelected);
				Initiate.Fade("Shop", Color.black, 2f);
				SaveManager.SaveInventory();
			}
			else {
                SFX.Play("Fail_Tap", 1f, 1f, 0f, false, 0f);
            }
		}
	
	}
}
