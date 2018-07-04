using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private string spriteString = "<sprite=0>";
    public Image prototypeOver;
    public TextMeshProUGUI prototypeEndText;

    //Last item clicked
    public GameObject lastTownClicked = null;

    //Current Town
    public Travel.Towns currentTown = Travel.currentTown;
    public Travel.Towns newTown = Travel.currentTown;

    // Use this for initialization
    void Start() {
        Setup();

        // NOTE: when using inventory, remember to save
        CheckUnlockedTowns();

        //Load the shop screen in the background as that is the only one which can be travelled to
        StartCoroutine(LoadAsyncScene("Shop"));
    }

    // Update is called once per frame
    void Update() {
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
                //Really dirty right now, will improve when full details of town system are known
                //If nothing has been clicked previously and clicked on town object
                if (lastTownClicked == null && hit.transform.gameObject.tag == "Town") {
                    //If the town clicked is not currently unlocked
                    if (!Travel.unlockedTowns.Contains(CurrentTown(hit.transform.gameObject))) {
                        lastTownClicked = hit.transform.gameObject;
                        Travel.Towns selectedTown = CurrentTown(lastTownClicked);
                        helperText.enabled = true;
                        helperText.text = "Click " + selectedTown + " again if you wish to purchase it for " + Travel.NextPurchaseCost() + " gold";
                    }
                    //If the town is unlocked and not the current town
                    else if (currentTown != CurrentTown(hit.transform.gameObject)){
                        lastTownClicked = hit.transform.gameObject;
                        Travel.Towns selectedTown = CurrentTown(lastTownClicked);
                        helperText.enabled = true;
                        helperText.text = "Click " + selectedTown + " again if you wish to travel to it";
                    }
                }
                //If the player has double clicked on a town
                else if (hit.transform.gameObject == lastTownClicked && lastTownClicked.tag == "Town") {
                    Travel.Towns selectedTown = CurrentTown(lastTownClicked);
                    //If not unlocked, attempt to buy
                    if (!Travel.unlockedTowns.Contains(selectedTown)) {
                        bool completeTransaction = Travel.UnlockNewTown(selectedTown);
                        lastTownClicked = null;
                        //If this was the first town unlocked, make it the current
                        if (Travel.unlockedTowns.Count == 1 && completeTransaction) {
                            player.SetActive(true);
                            player.transform.position = ReturnTownPosition(selectedTown);
                            helperText.text = "Welcome to " + selectedTown;
                            Travel.ChangeCurrentTown(selectedTown);
                            currentTown = selectedTown;
                            newTown = selectedTown;
                        }
                        //Else if it was a subsequent town, check the purchase was successful
                        else {
                            if (completeTransaction) {
                                helperText.text = selectedTown + " can now be travelled to";
                            }
                            else {
                                helperText.text = "Insufficent gold to travel to next town";
                            }
                        }
                    }
                    //If the town has been unlocked, move to selected town
                    else {
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
            case Travel.Towns.WickedGrove:
                newPosition = town1.transform.position;
                newPosition.z = 18;
                return newPosition;
            case Travel.Towns.Chelm:
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
            /*
             * This was used in the prototype to create an 'end'
            if (currentTown == Travel.Towns.Chelm) {
                PrototypeEnd();
            }
            */
        }
    }
    //Update Visuals to show which towns are unlocked
    private void CheckUnlockedTowns() {
        foreach (Travel.Towns town in Travel.unlockedTowns) {
            switch (town) {
                case Travel.Towns.WickedGrove:
                    town1.GetComponent<Renderer>().material = unlocked;
                    break;
                case Travel.Towns.Chelm:
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

    //Assign materials to objects and setup UI
    private void Setup() {
        prototypeOver.enabled = false;
        prototypeEndText.enabled = false;
        helperText.enabled = false;
        if (Travel.unlockedTowns.Count == 0) {
            town1.GetComponent<Renderer>().material = locked;
            town2.GetComponent<Renderer>().material = locked;
            town3.GetComponent<Renderer>().material = locked;
            player.SetActive(false);
        }
        else {
            player.transform.position = ReturnTownPosition(Travel.currentTown);
        }
    }

    //Return current town
    private Travel.Towns CurrentTown(GameObject townObject) {
        switch (townObject.name) {
            case "Town1":
                return Travel.Towns.WickedGrove;
            case "Town2":
                return Travel.Towns.Chelm;
            case "Town3":
                return Travel.Towns.Town3;
            default:
                return currentTown;
        }
    }

    private void PrototypeEnd() {
        helperText.enabled = false;
        town1.SetActive(false);
        town2.SetActive(false);
        player.SetActive(false);
        prototypeOver.enabled = true;
        prototypeEndText.enabled = true;
        prototypeEndText.CrossFadeAlpha(255f, 4f, false);

    }

    private void UpdateUI() {
        goldAmount.text = spriteString + " " + Inventory.Instance.goldCount.ToString("N0");
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
}
