using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowMap : MonoBehaviour
{
	public RawImage Map;
	public Button Exit;
	public GameObject ShopButton;
	public bool EnableGlobe;
	public LayerMask Mask;
	
	private GameObject lastTownClicked;
	
	//Current Town
	private Travel.Towns currentTown {
		get { return Inventory.Instance.GetCurrentTown(); }
	}
	
	//Player representation
	public GameObject player;
	//Town Representation
	public GameObject town1, town2, town3, town4;
	
	//UI Text
	public TextMeshProUGUI helperText;
	public TextMeshProUGUI goldAmount;
	private string spriteString = "<sprite=0>";

	private void Start()
	{
		EnableGlobe = true;
		Map.DOFade(0f, 0.05f);
		player.SetActive(false);
		FadeButton(Exit,0f,0.05f);
		Exit.enabled = false;
		ShopButton.SetActive(false);
	}

	public void ShowMapOnScreen()
	{
		EnableGlobe = false;
		Map.DOFade(1f, 2.0f);
		FadeButton(Exit,1f,2.0f);
		Exit.enabled = true;
		player.SetActive(true);
		MovePlayerToTown(ReturnTownPosition(currentTown));
	}
	
	private void Setup()
	{
		if(PlayerPrefs.GetInt("FirstStart") == 0) {
			helperText.text = "Select the town where you wish to begin your journey";
			helperText.enabled = false;
			goldAmount.enabled = false;   
		}
		helperText.enabled = false;
	}

	private void Update()
	{
		if (!EnableGlobe && Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(ray.origin,ray.direction);
			if (Physics.Raycast(ray, out hit, 1, Mask))
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
	}
	
	//Used when the player clicks on a gameobject that represents a town the first time
	private void FirstClick(RaycastHit hit)
	{
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
			PlayerPrefs.SetInt("FirstStart",1);
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
				newPosition.z = -9.072264f;
				return newPosition;
			case Travel.Towns.FlamingPeak:
				newPosition = town2.transform.position;
				newPosition.z = -9.072264f;
				return newPosition;
			case Travel.Towns.GiantsPass:
				newPosition = town3.transform.position;
				newPosition.z = -9.072264f;
				return newPosition;
			case Travel.Towns.SkyCity:
				newPosition = town4.transform.position;
				newPosition.z = -9.072257f;
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

	public void HideMap()
	{
		EnableGlobe = true;
		ShopButton.SetActive(true);
		Map.DOFade(0f, 0.05f);
		FadeButton(Exit,0f,0.05f);
		Exit.enabled = false;
		player.SetActive(false);
	}

	private void FadeButton(Button button, float alpha, float duration)
	{
		Image image = button.GetComponent<Image>();
		image.DOFade(alpha, duration);
	}

	private void MovePlayerToTown(Vector3 pos)
	{
		player.transform.position = pos;
	}
}
