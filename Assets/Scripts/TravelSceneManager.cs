using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TravelSceneManager : MonoBehaviour {
    //THIS SCRIPT IS NOW OBSOLETE AND HAS BEEN REPLACED WITH HALL.
    //DO NOT DELETE THIS SCRIPT UNTIL THE HALL HAS BEEN FINALISED
    /*
    //Object used to represent player
    public GameObject player;
    public float playerMoveSpeed;

    //Objects used as representations of towns on map
    public GameObject town1, town2, town3, town4;

    //Materials to represent unlocked towns
    public Material unlocked;
    public Material locked;

    //UI Text
    public TextMeshProUGUI helperText;
    public TextMeshProUGUI goldAmount;
    private string spriteString = "<sprite=0>";

    //Last item clicked
    public GameObject lastTownClicked = null;

    //Current Town
    public Travel.Towns currentTown {
        get { return Inventory.Instance.GetCurrentTown(); }
    }

    //Boolean to keep track of movement and coroutine pause time
    private bool movementFinished = false;
    private float waitTime = 0.05f;
    
    //Default inventories
    public Inventory inventory;
    public ShonkyInventory shonkyInventory;
    public Mine mineInventory;
    
    // Use this for initialization
    void Start() {
        Setup();
        
        CheckUnlockedTowns();
        
        //Load the shop screen in the background as that is the only one which can be travelled to
        StartCoroutine(LoadAsyncScene("Shop"));
    }

    // Update is called once per frame
    void Update() {
        CheckForInput();
        UpdateUI();
    }

    private void CheckForInput() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 30)) {
                //If nothing has been clicked previously and clicked on town object
                if (lastTownClicked == null && hit.transform.gameObject.tag == "Town") {
                    FirstClick(hit);
                }
                //If the player has double clicked on a town
                else if (hit.transform.gameObject == lastTownClicked && lastTownClicked.tag == "Town") {
                    SecondClick(hit);
                } else if (hit.transform.gameObject.tag == "Town" && hit.transform.gameObject != lastTownClicked){
                    //Reset clicked object
                    FirstClick(hit);
                }
            }
        }
    }
    //Used when the player clicks on a gameobject that represents a town the first time
    private void FirstClick(RaycastHit hit) {
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
    private void SecondClick(RaycastHit hit) {
        Travel.Towns selectedTown = CurrentTownObject(lastTownClicked);
        //If not unlocked, attempt to buy
        if (!Travel.unlockedTowns.Contains(selectedTown)) {
            AttemptToBuyTown(selectedTown);
        }
        //If the town has been unlocked, move to selected town
        else {
            movementFinished = false;
            StartCoroutine(MovePlayerToNewTown(selectedTown));
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
        CheckUnlockedTowns();
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

    //Move player to new town using movetowards
    IEnumerator MovePlayerToNewTown(Travel.Towns newTown) {
        while (!movementFinished) {
            player.transform.position = Vector3.MoveTowards(player.transform.position, ReturnTownPosition(newTown), playerMoveSpeed);
            if (player.transform.position == ReturnTownPosition(newTown)) {
                movementFinished = true;
                StopCoroutine("MovePlayerToNewTown");
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

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

    //Assign materials to objects and setup UI
    private void Setup() {
        if(PlayerPrefs.GetInt("FirstStart") == 0) {
            helperText.text = "Select the town where you wish to begin your journey";
            helperText.enabled = true;
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
        town1.GetComponent<Renderer>().material = locked;
        town2.GetComponent<Renderer>().material = locked;
        town3.GetComponent<Renderer>().material = locked;
        town4.GetComponent<Renderer>().material = locked;
        player.SetActive(false);
        player.transform.position = ReturnTownPosition(currentTown);
        
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
    /*
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
    */
}
