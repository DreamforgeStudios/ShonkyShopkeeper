using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;


public class Hall : MonoBehaviour
{

	//Variables for handling camera and globe movement
	private bool forward, mapInteraction, townInteraction = false;
	public Vector3 frontPos, backPos;
	public GameObject globe;
	public float speed = 100.0f;
	public float Xrot, Yrot = 0f;

	private GameObject lastTownClicked;
	
	
	
	//UI Text
	public TextMeshProUGUI helperText;
	public TextMeshProUGUI goldAmount;
	private string spriteString = "<sprite=0>";
	
	//Current Town
	public Travel.Towns currentTown {
		get { return Inventory.Instance.GetCurrentTown(); }
	}
	
	//Player representation
	public GameObject player;
	//Town Representation
	public GameObject town1, town2, town3, town4;
	
	//Default inventories
	public Inventory inventory;
	public ShonkyInventory shonkyInventory;
	public Mine mineInventory;
	
	// Use this for initialization
	void Start ()
	{
		Camera.main.transform.position = backPos;

		Setup();
		//Load the shop screen in the background as that is the only one which can be travelled to
		StartCoroutine(LoadAsyncScene("Shop"));
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckCamera();
		//MoveCamera();
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
		if(PlayerPrefs.GetInt("FirstStart") == 0) {
			helperText.text = "Select the town where you wish to begin your journey";
			helperText.enabled = false;
			goldAmount.enabled = false;
			//Also need to load default inventory to reset towns
			Debug.Log("Loading Initial Inventories");
			SaveManager.LoadOrInitializeInventory(inventory);
			SaveManager.LoadOrInitializeShonkyInventory(shonkyInventory);
			SaveManager.LoadOrInitializeMineInventory(mineInventory);
			SaveManager.SaveInventory();
			SaveManager.SaveShonkyInventory();
			SaveManager.SaveMineInventory();
			Debug.Log("Saved Initial Inventories");
            
		}
		helperText.enabled = false;
		player.SetActive(false);
		player.transform.position = ReturnTownPosition(currentTown);
	}

	private void MoveCamera()
	{
		//Debug.Log("Map Interaction is " + mapInteraction + " and forward is " + forward);
		if (!mapInteraction && !townInteraction)
		{
			//Debug.Log("Not interacting with map");
			if (!forward)
			{
				Debug.Log("Moving Forward");
				Camera.main.transform.DOMove(frontPos, 1f).SetEase(Ease.InOutSine).OnComplete(() => forward = true);
				helperText.enabled = true;
				goldAmount.enabled = true;
			}
			else
			{
				Debug.Log("Moving Back");
				Camera.main.transform.DOMove(backPos, 1f).SetEase(Ease.InOutSine).OnComplete(() => forward = false);
				helperText.enabled = false;
				goldAmount.enabled = false;
			}
		}
	}

	private void RayCastSphere()
	{
		Debug.Log("forward is " + forward);
		if (forward)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//Debug.Log("Casting Ray");
			//Debug.DrawLine(Camera.main.transform.position,transform.forward,Color.black);

			if (Physics.Raycast(ray, out hit, 1))
			{
				//Debug.Log(hit.transform.gameObject.name);
				if (hit.transform.gameObject.tag == "Globe" || mapInteraction)
				{
					Debug.Log("Hit globe");
					RotateSphere(hit);
				} else if (!mapInteraction)
				{
					if (lastTownClicked == null && hit.transform.gameObject.tag == "Town") {
						FirstClick(hit);
					}
					//If the player has double clicked on a town
					else if (hit.transform.gameObject == lastTownClicked && lastTownClicked.tag == "Town")
					{
						SecondClick(hit);
					}
					else if (hit.transform.gameObject.tag == "Town" && hit.transform.gameObject != lastTownClicked)
					{
						//Reset clicked object
						FirstClick(hit);
					}
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
					townInteraction = false;
				}
			}
		}
		else
		{
			mapInteraction = false;
			townInteraction = false;
		}
	}

	private void RotateSphere(RaycastHit hit)
	{
		mapInteraction = true;
		Xrot += -Input.GetAxis("Mouse Y") * Time.deltaTime * speed;
		Xrot = Mathf.Clamp(Xrot,-30, 30);
		Yrot += -Input.GetAxis("Mouse X") * Time.deltaTime * speed;
		Vector3 rotation = new Vector3(Xrot, Yrot, 0);
		//globe.transform.Rotate(rotation);
		globe.transform.localEulerAngles = rotation;
	}
	
	//Used when the player clicks on a gameobject that represents a town the first time
    private void FirstClick(RaycastHit hit)
    {
	    townInteraction = true;
	    mapInteraction = false;
        //If the town clicked is not currently unlocked
        if (!Travel.unlockedTowns.Contains(CurrentTownObject(hit.transform.gameObject))) {
            lastTownClicked = hit.transform.gameObject;
            Travel.Towns selectedTown = CurrentTownObject(lastTownClicked);
            helperText.enabled = true;
            helperText.text = "Click " + selectedTown + " again if you wish to purchase it for " + Travel.NextPurchaseCost() + " gold";
        }
        //If the town is unlocked and not the current town
        else if (currentTown != CurrentTownObject(hit.transform.gameObject)) {
            lastTownClicked = hit.transform.gameObject;
            Travel.Towns selectedTown = CurrentTownObject(lastTownClicked);
            helperText.enabled = true;
            helperText.text = "Click " + selectedTown + " again if you wish to travel to it";
        }
    }
    //Used when the player clicks on the same town object a second time
    private void SecondClick(RaycastHit hit)
    {
	    townInteraction = true;
	    mapInteraction = false;
        Travel.Towns selectedTown = CurrentTownObject(lastTownClicked);
        //If not unlocked, attempt to buy
        if (!Travel.unlockedTowns.Contains(selectedTown)) {
            AttemptToBuyTown(selectedTown);
        }
        //If the town has been unlocked, move to selected town
        else {
            //movementFinished = false;
            //StartCoroutine(MovePlayerToNewTown(selectedTown));
	        
            Travel.ChangeCurrentTown(selectedTown);
            helperText.enabled = false;
            lastTownClicked = null;
            SaveManager.SaveInventory();

        }
    }
    //Method used when attempting to buy a new town. Also handles UI at same time
    private void AttemptToBuyTown(Travel.Towns selectedTown) {
        bool completeTransaction = Travel.UnlockNewTown(selectedTown);
        //If this was the first town unlocked, make it the current
        if (Inventory.Instance.GetUnlockedTowns().Count == 1 && completeTransaction) {
            player.SetActive(true);
            player.transform.position = ReturnTownPosition(selectedTown);
            helperText.text = "Welcome to " + selectedTown;
            Travel.ChangeCurrentTown(selectedTown);
            SaveManager.SaveInventory();
            PlayerPrefs.SetInt("FirstStart", 1);
        }
        //Else if it was a subsequent town, check the purchase was successful
        else {
            if (completeTransaction) {
                helperText.text = selectedTown + " can now be travelled to";
                SaveManager.SaveInventory();
            }
            else {
                helperText.text = "Insufficent gold to travel to next town";
            }
        }
        lastTownClicked = null;
        //CheckUnlockedTowns();
    }
	
	//Position player sprite over relevant town at scene load
	private Vector3 ReturnTownPosition(Travel.Towns town) {
		Vector3 newPosition;
		switch (town) {
			case Travel.Towns.WickedGrove:
				newPosition = town1.transform.position;
				newPosition.z = 18;
				return newPosition;
			case Travel.Towns.FlamingPeak:
				newPosition = town2.transform.position;
				newPosition.z = 18;
				return newPosition;
			case Travel.Towns.GiantsPass:
				newPosition = town3.transform.position;
				newPosition.z = 18;
				return newPosition;
			case Travel.Towns.SkyCity:
				newPosition = town4.transform.position;
				newPosition.z = 18;
				return newPosition;
			default:
				return player.transform.position;
		}
	}
	
	//Return current town
	private Travel.Towns CurrentTownObject(GameObject townObject) {
		switch (townObject.name) {
			case "Town1":
				return Travel.Towns.WickedGrove;
			case "Town2":
				return Travel.Towns.FlamingPeak;
			case "Town3":
				return Travel.Towns.GiantsPass;
			case "Town4":
				return Travel.Towns.SkyCity;
			default:
				return currentTown;
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
