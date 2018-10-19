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
	public bool forward, mapInteraction = false;
	public Vector3  defaultRotation, frontPos, backPos, inspectPos;
	public GameObject globe;
	public float speed = 100.0f;
	public float Xrot, Yrot = 0f;
	
	//UI Text
	public TextMeshProUGUI goldAmount;
	private string spriteString = "<sprite=0>";
	public GameObject ShopButton, MoveCameraBackButton;
	
	//Tutorial Element
	public MapTutorial mapTutorialManager;
	
	//Default inv to fix reference issues
	public Inventory defaultInv;
	
	//New system for map screen and objects going around it
	public List<GameObject> townObjects, startingPositions, townCanvasElements;
	public GameObject startingPosition, townInspectPosition;
	private bool canMoveAround, playingLoop = false;
	
	//Need to keep track of select hit town due to scaling
	private RaycastHit townHit;
	
	//The buttons that appear on the town descriptions
	public Button purchaseButton, travelButton, backButton;
	
	//Gold Text
	public TextMeshProUGUI goldText;
	
	//True Golem Intro Handler
	public TrueGolemIntro trueGolemHandler;
	
	// Use this for initialization
	void Start ()
	{
		Camera.main.transform.position = backPos;
		Camera.main.transform.localRotation = Quaternion.Euler(defaultRotation);

		Setup();
		
		//ambient SFX
		SFX.Play("MainMenuTrack", 0.1f, 1f, 0f, true, 0f);
		
		//If introducing true golem, set the camera at the relevant position and load the relevant dialogue
		if (GameManager.Instance.introduceTrueGolem)
		{
			trueGolemHandler.IntroduceTrueGolem();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		goldAmount.text = string.Format("<sprite=0> {0}",Inventory.Instance.goldCount);
		
		//If not introducing a true golem, allow for normal movement.
		if (!GameManager.Instance.introduceTrueGolem)
		{
			CheckCamera();
			if ((!GameManager.Instance.InMap && !GameManager.Instance.firstTownSelect) && !forward)
				ShopButton.SetActive(true);
			else
				ShopButton.SetActive(false);
		}
	}

	private void CheckCamera()
	{
		if (Input.GetMouseButton(0))
		{
			RayCastSphere();
		}
	}

	private void Setup()
	{
		//SaveManager.LoadFromTemplate(defaultInv);
		goldAmount.enabled = false;
		ShopButton.SetActive(false);
		MoveCameraBackButton.SetActive(false);
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
				MoveCameraBackButton.SetActive(true);
				ShopButton.gameObject.SetActive(false);
				if (GameManager.Instance.InMap && !mapTutorialManager.clickedOrb)
				{
					mapTutorialManager.ClickedSphere();
					canMoveAround = false;
					MoveCameraBackButton.SetActive(false);
				}
				mapInteraction = false;
				
			}
			else
			{
				Debug.Log("Moving Back");
				Camera.main.transform.DOMove(backPos, 1f).SetEase(Ease.InOutSine).OnComplete(() => forward = false);
				Camera.main.transform.DORotate(defaultRotation, 1f, RotateMode.Fast);
				goldAmount.enabled = false;
				canMoveAround = false;
				MoveCameraBackButton.SetActive(false);
				ReturnToGlobe();
			}
		}
	}

	public void MoveCameraBack()
	{
		MoveCamera();
		trueGolemHandler.inspectingGolem = false;
	}

	private void RayCastSphere()
	{
		if (forward)
		{
			//Just need to handle the tutorial now that the system has changed.
			if (GameManager.Instance.InMap && mapTutorialManager.clone.Instruction)
			{
				canMoveAround = true;
			}
			
			//Raycast for the town  objects.
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawLine(ray.origin,ray.direction,Color.red,4f);
			if (Physics.Raycast(ray, out hit, 1))
			{
				Debug.Log(hit.transform.gameObject.name + " hit" + " can select towns " + canMoveAround);
				if (hit.transform.gameObject.CompareTag("Town") && canMoveAround)
				{
					ChooseTown(hit);
				} else if (GameManager.Instance.InMap)
				{
					mapInteraction = true;
				}
				/*else if (mapInteraction && !hit.transform.gameObject.CompareTag("Town"))
				{
					Debug.Log("Exiting map interaction");
					ExitMapInteraction();
				}*/ else if (trueGolemHandler.inspectingGolem &&
				           hit.transform.gameObject == trueGolemHandler.golemSelected && !trueGolemHandler.readingDialogue)
				{
					trueGolemHandler.ReshowDialogue();
				}
			}
		}
		else
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawLine(ray.origin,ray.direction,Color.red,4f);
			Debug.Log("Drawing ray");
			if (Physics.Raycast(ray, out hit, 5))
			{
				Debug.Log("Hit " + hit.transform.gameObject.tag );
				if (hit.transform.gameObject.CompareTag("Globe"))
				{
					MoveCamera();
				} else if (hit.transform.gameObject.CompareTag("TrueGolem"))
				{
					trueGolemHandler.HighlightTrueGolem(hit.transform.gameObject);
					forward = true;
					MoveCameraBackButton.SetActive(true);
				}
			}
			//mapInteraction = false;
		}
	}

	//When the player clicks on a town bubble, this handles the related actions such as moving the camera, scaling and 
	//sprite ordering
	private void ChooseTown(RaycastHit hit)
	{
		mapInteraction = true;
		canMoveAround = false;
		townHit = hit;
		
		//Get rid of back button (which moves camera to starting position)
		MoveCameraBackButton.SetActive(false);
		
		//Sound effect
		SFX.Play("Map_location_select", 1f, 1f, 0f, false, 0f);
		
		//Move all townObjs to globe by first killing current tweens and then sending them back to the initial pos
		ReturnToGlobe();
		
		//Change layer of chosen town to be above others
		hit.transform.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
		
		//Finish all previous tweens
		foreach (GameObject town in townObjects)
			town.transform.DOComplete();
		
		//Move camera to inspect pos and make chosen town larger
		Camera.main.transform.DOMove(inspectPos, 1f, false).OnComplete(() =>
			hit.transform.DOMove(townInspectPosition.transform.position, 0.1f, false).OnComplete(() =>
				hit.transform.DOScale(new Vector3(0.0014f, 0.0014f, 0.0014f), 1f)));
		
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
		mapInteraction = false;
		
		if (!GameManager.Instance.InMap)
			canMoveAround = true;
		
		//Make sure they are on the right layer
		foreach (GameObject town in townObjects)
		{
			town.transform.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
		}	
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
		
		//If in tutorial hide rune overlays
		if (GameManager.Instance.InMap)
		{
			mapTutorialManager.ReactivateALlTownHighlights();
		}
		if (canMoveAround)
		{
			townTransform.DOMove(startingPositions[nextPosition].transform.position, 5f, false).SetEase(Ease.InOutQuad)
				.OnComplete(() => RotateAroundGlobe(townTransform, nextPosition));
		} else if (GameManager.Instance.InMap)
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
		{
			town.transform.DOComplete();
			town.transform.DOMove(startingPosition.transform.position, 0.5f, false);
			town.transform.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Behind";
		}	
		//If in tutorial hide rune overlays
		if (GameManager.Instance.InMap)
		{
			mapTutorialManager.DeactivateAllTownHighlights();
		}
	}

	//Rescales the selected town, moves the camera back and then starts them circling the sphere again
	public void ExitMapInteraction()
	{
		//Show back button (which moves camera to starting position)
		MoveCameraBackButton.SetActive(true);
		
		//Finish all existing Tweens
		foreach (GameObject town in townObjects)
			town.transform.DOComplete();
		
		townHit.transform.DOScale(new Vector3(0.0007f,0.0007f,0.0007f), 0.1f).OnComplete(()=> MoveAroundGlobe());
		Camera.main.transform.DOMove(frontPos, 1f).SetEase(Ease.InOutSine);
		HideTownUIAndButtons();
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
		
		//Enable selected town element
		switch (town)
		{
			case Travel.Towns.FlamingPeak:
				townCanvasElements[0].SetActive(true);
				break;
			case Travel.Towns.GiantsPass:
				townCanvasElements[1].SetActive(true);
				break;
			case Travel.Towns.SkyCity:
				townCanvasElements[2].SetActive(true);
				break;
			case Travel.Towns.WickedGrove:
				townCanvasElements[3].SetActive(true);
				break;
			default:
				townCanvasElements[0].SetActive(true);
				break;
		}
		
		//Enable respective Buttons and gold text
		if (Inventory.Instance.unlockedTowns.Contains(town))
			travelButton.gameObject.SetActive(true);
		else
			purchaseButton.gameObject.SetActive(true);

		backButton.gameObject.SetActive(true);
		if (!GameManager.Instance.InMap)
		{
			goldText.gameObject.SetActive(true);
			goldText.text = "<sprite=0>" + Inventory.Instance.goldCount;
		}
	}
	
	//Method used by the back button to hide current town description
	public void HideTownUIAndButtons()
	{
		foreach (GameObject town in townCanvasElements)
		{
			town.SetActive(false);
		}
		purchaseButton.gameObject.SetActive(false);
		travelButton.gameObject.SetActive(false);
		backButton.gameObject.SetActive(false);
		goldText.gameObject.SetActive(false);
		
		//Need SFX for this back button
		SFX.Play("Fail_Tap", 1f, 1f, 0f, false, 0f);
	}
	
	//Method used to send user back to shop by 'travelling'
	public void TravelButton()
	{
		Travel.Towns currentTownSelected = CurrentTownObject(townHit.transform.gameObject);
		SFX.Play("Traveling_chimes", 1f, 1f, 0f, false, 0f);
		Travel.ChangeCurrentTown(currentTownSelected);
		SaveManager.SaveInventory();
		Initiate.Fade("Shop", Color.black, 2f);
	}
	
	//Method used when attempting to buy a new town through button. Also handles UI at same time
	public void PurchaseTown()
	{
		Travel.Towns currentTownSelected = CurrentTownObject(townHit.transform.gameObject);
		bool completeTransaction = Travel.UnlockNewTown(currentTownSelected);
		//If this was the first town unlocked, make it the current
		Debug.Log(Inventory.Instance.GetUnlockedTowns().Count + " unlocked towns");
		Debug.Log("Complete transaction " + completeTransaction);
		if (Inventory.Instance.GetUnlockedTowns().Count == 1 && completeTransaction) {
			Travel.ChangeCurrentTown(currentTownSelected);
			SFX.Play("Location_query_purchase", 1f, 1f, 0f, false, 0f);
			SaveManager.SaveInventory();
			PlayerPrefs.SetInt("ExistingSave", 1);
			if (GameManager.Instance.InMap)
			{
				GameManager.Instance.InMap = false;
				GameManager.Instance.BarterTutorial = true;
			}
			Travel.ChangeCurrentTown(currentTownSelected);
			SaveManager.SaveInventory();
			GameManager.Instance.firstTownSelect = false;
			Initiate.Fade("Shop", Color.black, 2f);
		}
		//Else if it was a subsequent town, check the purchase was successful
		else {
			if (completeTransaction) {
				PlayerPrefs.SetInt("ExistingSave", 1);
                SFX.Play("location_purchase", 1f, 1f, 0f, false, 0f);
                if (GameManager.Instance.InMap)
				{
					GameManager.Instance.InMap = false;
					GameManager.Instance.BarterTutorial = true;
				}
				Travel.ChangeCurrentTown(currentTownSelected);
				SaveManager.SaveInventory();
				GameManager.Instance.firstTownSelect = false;
				Initiate.Fade("Shop", Color.black, 2f);		
			}
			else {
                SFX.Play("Fail_Tap", 1f, 1f, 0f, false, 0f);
            }
		}
	}

	public void BackToShop()
	{
		Initiate.Fade("Shop",Color.black,2.0f);
	}
}
