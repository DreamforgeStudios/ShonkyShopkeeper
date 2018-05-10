using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TravelSceneManager : MonoBehaviour {
    //Object used to represent player
    public GameObject player;
    public float playerMoveSpeed;

    //Objects used as representations of towns on map
    public GameObject town1;
    public GameObject town2;
    public GameObject town3;

    //Materials to represent unlocked towns
    public Material unlocked;
    public Material locked;

    //UI Text
    public TextMeshProUGUI helperText;
    public TextMeshProUGUI goldAmount;

    //Last item clicked
    public GameObject lastTownClicked = null;

    //Current Town
    public Travel.Towns currentTown = Travel.currentTown;
    public Travel.Towns newTown = Travel.currentTown;

    // Use this for initialization
    void Start () {
        helperText.enabled = false;
        Setup();
        //Remove these 2 lines in actual game
        Travel.unlockedTowns.Add(Travel.Towns.Town1);
        //Inventory.AddGold(500000);

        CheckUnlockedTowns();
	}

    // Update is called once per frame
    void Update () {
        CheckForInput();
        UpdateUI();
        CheckUnlockedTowns();
        MovePlayerToNewTown(newTown);
        CheckPosition();
	}

    private void CheckForInput() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 30)) {
                Debug.Log(hit.transform.gameObject.name);
                if (lastTownClicked == null && hit.transform.gameObject.tag == "Town") {
                    if (!Travel.unlockedTowns.Contains(CurrentTown(hit.transform.gameObject))) {
                        lastTownClicked = hit.transform.gameObject;
                        helperText.enabled = true;
                        helperText.text = "Click this town again if you wish to purchase it";
                    } else {
                        lastTownClicked = hit.transform.gameObject;
                        helperText.enabled = true;
                        helperText.text = "Click this town again if you wish to travel to it";
                    }      
                } else if (hit.transform.gameObject == lastTownClicked && lastTownClicked.tag == "Town") {
                    Travel.Towns selectedTown = CurrentTown(lastTownClicked);
                    if (!Travel.unlockedTowns.Contains(selectedTown)) {
                        bool completeTransaction = Travel.UnlockNewTown(selectedTown);
                        lastTownClicked = null;
                        if (completeTransaction) {
                            helperText.text = "Town " + selectedTown + " is now avaliable to travel to";
                        } else {
                            helperText.text = "Insufficent gold to travel to next town";
                        }
                    } else {
                        Travel.ChangeCurrentTown(selectedTown);
                        newTown = selectedTown;
                        helperText.enabled = false;
                        lastTownClicked = null;
                    }
                }
            }
        }
    }

    //Position player sprite over relevant town at scene load
    private Vector3 ReturnTownPosition(Travel.Towns town) {
        Vector3 newPosition;   
        switch (town) {
            case Travel.Towns.Town1:
                newPosition = town1.transform.position;
                newPosition.z = 18;
                return newPosition;
            case Travel.Towns.Town2:
                newPosition = town2.transform.position;
                newPosition.z = 18;
                return newPosition;
            case Travel.Towns.Town3:
                newPosition = town3.transform.position;
                newPosition.z = 18;
                return newPosition;
            case Travel.Towns.Town4:
                return player.transform.position;
            case Travel.Towns.Town5:
                return player.transform.position;
            default:
                return player.transform.position;
        }
    }

    //Move player to new town using movetowards
    private void MovePlayerToNewTown(Travel.Towns newTown) {
        if (currentTown != newTown) {
            player.transform.position = Vector3.MoveTowards(player.transform.position, ReturnTownPosition(newTown), playerMoveSpeed);
        }
    }

    //Used to update currentTown once reached
    private void CheckPosition() {
        if (player.transform.position == ReturnTownPosition(newTown)) {
            currentTown = newTown;
        }
    }
    //Update Visuals to show which towns are unlocked
    private void CheckUnlockedTowns() {
        foreach (Travel.Towns town in Travel.unlockedTowns) {
            switch (town) {
                case Travel.Towns.Town1:
                    town1.GetComponent<Renderer>().material = unlocked;
                    break;
                case Travel.Towns.Town2:
                    town2.GetComponent<Renderer>().material = unlocked;
                    break;
                case Travel.Towns.Town3:
                    town3.GetComponent<Renderer>().material = unlocked;
                    break;
                case Travel.Towns.Town4:
                    break;
                case Travel.Towns.Town5:
                    break;
                default:
                    break;
            }
        }
    }

    //Assign materials to objects
    private void Setup() {
        town1.GetComponent<Renderer>().material = locked;
        town2.GetComponent<Renderer>().material = locked;
        town3.GetComponent<Renderer>().material = locked;
        player.transform.position = ReturnTownPosition(Travel.currentTown);
    }

    //Return current town
    private Travel.Towns CurrentTown(GameObject townObject) {
        switch (townObject.name) {
            case "Town1":
                return Travel.Towns.Town1;
            case "Town2":
                return Travel.Towns.Town2;
            case "Town3":
                return Travel.Towns.Town3;
            default:
                return currentTown;
        } 
    }

    private void UpdateUI() {
        //goldAmount.text = "Gold Amount = " + Inventory.goldAmount;
    }
}
