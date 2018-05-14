using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening; // Tweening / nice lerping.
//using System;
using UnityEngine.SceneManagement; //To access Minigames

// The toolbox class is how all user inventory interactions should be done.
public class Toolbox : MonoBehaviour {
    public enum Tool {
        Inspector,
        Foreceps,
        Wand,
        None
    }

    public PhysicalInventory physicalInventory;
    public ItemDatabase database;

    //Capture the original positions and gameObjects of each tool
    private Vector3 forcepPos;
    private Vector3 inspectPos;
    private Vector3 wandPos;
    public GameObject forcep;
    public GameObject inspectorObj;
    public GameObject wandObj;

    // A layer mask so that we only hit slots.
    public LayerMask layerMask;

    // Variables for the inspector.
    public GameObject inspectionPanel;
    public TextMeshProUGUI textHeading;
    public TextMeshProUGUI textInfo;

    // Variables for minigames - Not necessary right now
    //public TextMeshProUGUI beginMiniGame

    // Helpers.
    private Inventory inventoryhelper;
    private Tool currentTool;

    //Helpers to capture Items, transfoms, slot indexes, gameobjects, movement stages and instances when using forceps
    private Slot currentSelection = null;
    private Slot secondSelection = null;
    private bool canSelect = true;
    private Transform firstTransform;
    private Transform secondTransform;
    private Vector3 firstSelectionOriginal;
    private Vector3 secondSelectionOriginal;
    private Vector3 firstSelectHigh;
    private Vector3 secondSelectHigh;
    private int slot1SelectionIndex;
    private int slot2SelectionIndex;
    private ItemInstance instanceItem1;
    private ItemInstance instanceItem2;
    private ItemInstance tempHolder;
    private bool inPosition1 = false;
    private bool inPosition2 = false;
    private bool inFinalPosition = false;

    //Two testing instances to showcase resource Pouch. Delete in final implementation once
    //we can create items through code correctly.
    public ItemInstance ore;
    public ItemInstance ruby;

    // Debug.
    private Ray previousRay;

    // Use this for initialization
    void Start() {
        // TODO: Change this to the default tool.
        currentTool = Tool.None;
        inventoryhelper = Inventory.Instance;
        forcepPos = GameObject.FindGameObjectWithTag("forcep").transform.position;
        wandPos = GameObject.FindGameObjectWithTag("wand").transform.position;
        inspectPos = GameObject.FindGameObjectWithTag("inspector").transform.position;
    }

    // Update is called once per frame
    void Update() {
        // Check where we are running the program.
        RuntimePlatform p = Application.platform;
        if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer)
            // Process mouse inputs.
            ProcessMouse();
        else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
            // Process touch inputs.
            ProcessTouch();
    }

    public void SwitchTool(Tool tool) {
        if (currentTool != tool) {
            switch (currentTool) {
                case Tool.Inspector:
                    Transform a = inspectorObj.transform;
                    a.DOMove(inspectPos, 0.7f).SetEase(Ease.OutBack);
                    break;
                case Tool.Foreceps:
                    Transform b = forcep.transform;
                    b.DOMove(forcepPos, 0.7f).SetEase(Ease.OutBack);
                    break;
                case Tool.Wand:
                    Transform c = wandObj.transform;
                    c.DOMove(wandPos, 0.7f).SetEase(Ease.OutBack);
                    break;
            }
            currentTool = tool;
            HideInspector();
        }
    }
    //Updated to raycast if the player selects a tool and switch to that tool if they did
    private void ProcessMouse() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            previousRay = ray;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                Debug.Log("Hit: " + hit.transform.name);
                if (hit.transform.tag == "forcep" || hit.transform.tag == "wand" || hit.transform.tag == "inspector" && canSelect) {
                    switch (hit.transform.tag) {
                        case "forcep":
                            SwitchTool(Tool.Foreceps);
                            Transform a = hit.transform;
                            a.DOMove(a.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                            break;
                        case "wand":
                            SwitchTool(Tool.Wand);
                            Transform b = hit.transform;
                            b.DOMove(b.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                            break;
                        case "inspector":
                            SwitchTool(Tool.Inspector);
                            Transform c = hit.transform;
                            c.DOMove(c.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                            break;
                    }
                }
                else {
                    PickAndUseTool(hit.transform.GetComponent<Slot>());
                }
            }
            else {
                if (currentTool == Tool.Inspector) {
                    HideInspector();
                }
            }
        }
    }

    private void ProcessTouch() {
        if (Input.touchCount == 0) {
            return;
        }

        foreach (Touch touch in Input.touches) {
            if (touch.phase == TouchPhase.Began) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                previousRay = ray;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                    Debug.Log("Hit: " + hit.transform.name);
                    if (hit.transform.tag == "forcep" || hit.transform.tag == "wand" || hit.transform.tag == "inspector" && canSelect) {
                        switch (hit.transform.tag) {
                            case "forcep":
                                SwitchTool(Tool.Foreceps);
                                Transform a = hit.transform;
                                a.DOMove(a.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                                break;
                            case "wand":
                                SwitchTool(Tool.Wand);
                                Transform b = hit.transform;
                                b.DOMove(b.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                                break;
                            case "inspector":
                                SwitchTool(Tool.Inspector);
                                Transform c = hit.transform;
                                c.DOMove(c.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                                break;
                        }
                    }
                    else {
                        PickAndUseTool(hit.transform.GetComponent<Slot>());
                    }
                }
                else {
                    // If there was no hit and we're using the inspector, then we should hide the inspector.
                    if (currentTool == Tool.Inspector) {
                        HideInspector();
                    }
                }
            }
        }
    }

    // Use the right tool on the slot.
    private void PickAndUseTool(Slot slot) {
        switch (currentTool) {
            case Tool.Inspector:
                UseInspector(slot);
                break;
            case Tool.Foreceps:
                UseForceps(slot);
                break;
            case Tool.Wand:
                UseWand(slot);
                break;
        }
    }

    // Inspect an item.
    private void UseInspector(Slot slot) {
        // Can't select 2 items at once.
        if (currentSelection) {
            // Maybe this will cause flickering, might be better to just hide the object.
            HideInspector();
            currentSelection = null;
        }

        // To avoid null errors, always use the x.Get() methods, they check for you.
        Item item;
        ItemInstance instance;
        if (slot.GetItemInstance(out instance)) {
            //Debug.Log(instance.item.GetItemName());
            if (instance.item.GetItemName() == "ResourcePouch") {
                ResourcePouchOpen(instance, slot);
            }
            else {
                if (slot.GetItem(out item)) {
                    this.currentSelection = slot;
                    inspectionPanel.SetActive(true);

                    textHeading.text = item.GetItemName();
                    textInfo.text = item.GetItemInfo();

                    // Animate using tween library -> see https://easings.net/ for some animaions to use.
                    GameObject itemObj;
                    if (slot.GetPrefabInstance(out itemObj)) {
                        Transform t = itemObj.transform;
                        t.DOMove(t.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                    }
                }
            }
        }
    }

    // Hide the inspector and the current item (if one is selected).
    private void HideInspector() {
        inspectionPanel.SetActive(false);
        GameObject gameObj;
        if (currentSelection && currentSelection.GetPrefabInstance(out gameObj)) {
            gameObj.transform.DOMove(currentSelection.transform.position, 1f).SetEase(Ease.OutBounce);
            currentSelection = null;
        }
    }

    //Move an Item to a new slot
    private void UseForceps(Slot slot) {
        Item item;
        if (slot.GetItem(out item) && canSelect) {
            //If first selection
            if (currentSelection == null) {
                this.currentSelection = slot;
                //slot1SelectionIndex = slot.index;
                //ItemInstance instance;
                //if (slot.GetItemInstance(out instance)) {
                    //instanceItem1 = instance;
                //}

                // Animate using tween library -> see https://easings.net/ for some animaions to use.
                GameObject itemObj;
                if (slot.GetPrefabInstance(out itemObj)) {
                    Transform t = itemObj.transform;
                    //firstTransform = t;
                    //firstSelectionOriginal = firstTransform.position;
                    t.DOMove(slot.transform.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                }
            }

            //Else if second selection
            else {//if (secondSelection == null) {
                // If the same item is selected, put it back.
                if (currentSelection == slot) {
                    Debug.Log("Same slot.");
                    GameObject obj;
                    if (currentSelection.GetPrefabInstance(out obj)) {
                        obj.transform.DOMove(currentSelection.transform.position, 1f).SetEase(Ease.OutBounce);
                    }

                    currentSelection = null;
                    secondSelection = null;
                    return;
                }

                // Make things make more sense.
                Slot slot1 = this.currentSelection;
                Slot slot2 = slot;

                //this.secondSelection = slot;
                //slot2SelectionIndex = slot.index;
                //ItemInstance instance;
                //if (slot.GetItemInstance(out instance)) {
                    //instanceItem2 = instance;
                    //tempHolder = instanceItem1;
                //}

                // Move objects to other slot.
                GameObject obj1;
                GameObject obj2;
                if (slot1.GetPrefabInstance(out obj1) && slot2.GetPrefabInstance(out obj2)) {
                    Transform t1 = obj1.transform,
                              t2 = obj2.transform;


                    t1.DOMove(slot1.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
                        .OnComplete(() => t1.DOMove(slot2.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                        .OnComplete(() => t1.DOMove(slot2.transform.position, 1f).SetEase(Ease.OutBounce)));

                    t2.DOMove(slot2.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
                        .OnComplete(() => t2.DOMove(slot1.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                        .OnComplete(() => t2.DOMove(slot1.transform.position, 1f).SetEase(Ease.OutBounce)));

                    ItemInstance inst1, inst2;
                    if (slot1.GetItemInstance(out inst1) && slot2.GetItemInstance(out inst2)) {
                        slot1.SetItemInstantiated(inst2, obj2);
                        slot2.SetItemInstantiated(inst1, obj1);
                    }

                    currentSelection = null;
                    //canSelect = false;

                    //secondTransform = itemObj.transform;
                    //secondSelectionOriginal = secondTransform.position;
                    //secondTransform.DOMove(secondTransform.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
                    //secondSelectHigh = secondTransform.position;
                    //firstSelectHigh = firstTransform.position;
                    //StartCoroutine(Movement(secondTransform, firstTransform, itemObj, origGameObj));
                }

                //If same item selected, put back
                /*
                if (currentSelection == secondSelection && currentSelection.GetPrefabInstance(out gameObj) && canSelect) {
                    Debug.Log("same item");
                    gameObj.transform.DOMove(currentSelection.transform.position, 1f).SetEase(Ease.OutBounce);
                    currentSelection = null;
                    secondSelection = null;
                }
                */
                //Else move the positions
                //else if (canSelect) {
                    //canSelect = false;
                //}
            }
        }
    }

    IEnumerator MoveIfNotAlready(Transform t, Vector3 to) {
        if (DOTween.IsTweening(t)) {
            //Debug.Log("already tweening");
            var tweens = DOTween.TweensById(t);
            yield return tweens[0].WaitForCompletion();
            //yield return new WaitForSeconds(0.1f);
        }

        t.DOMove(to, 0.7f).SetEase(Ease.OutBack);
    }

    /*
    //Coroutine to make the movement look better otherwise the two movements lerp together and go 
    //through the drawer. It is split into the three separate stages of movement
    IEnumerator Movement(Transform t, Transform b, GameObject itemObj, GameObject origGameObj) {
        Debug.Log(firstSelectionOriginal + " first and second " + secondSelectionOriginal);
        while (!canSelect) {
            if (!inPosition1) {
                inPosition1 = true;
                yield return new WaitForSeconds(2.0f);
            }
            secondSelectHigh = secondTransform.position;
            t.DOMove(firstSelectHigh, 3f).SetEase(Ease.OutBack);
            b.DOMove(secondSelectHigh, 3f).SetEase(Ease.OutBack);
            Debug.Log("Moving to other side");
            if (!inPosition2) {
                inPosition2 = true;
                yield return new WaitForSeconds(2.0f);
            }
            Debug.Log("Moving Down");
            t.DOMove(firstSelectionOriginal, 3f).SetEase(Ease.OutBack);
            b.DOMove(secondSelectionOriginal, 3f).SetEase(Ease.OutBack);
            if (!inFinalPosition) {
                inFinalPosition = true;
                canSelect = true;
                Debug.Log("Should be out of coroutine");
            }
            Debug.Log("Out of coroutine");
            inventoryhelper.SwapItem(slot1SelectionIndex, slot2SelectionIndex);
            currentSelection.SetItemInstantiated(instanceItem2, itemObj);
            secondSelection.SetItemInstantiated(tempHolder, origGameObj);
            ResetForNextUse();
            StopCoroutine("Movement");
            yield return new WaitForSeconds(2.0f);
        }
    }

    private void ResetForNextUse() {
        currentSelection = null;
        secondSelection = null;
        canSelect = true;
        firstTransform = null;
        secondTransform = null;
        instanceItem1 = null;
        instanceItem2 = null;
        tempHolder = null;
        inPosition1 = false;
        inPosition2 = false;
        inFinalPosition = false;
    }
    */

    private void UseWand(Slot slot) {
        // Can't select 2 items at once.
        if (currentSelection) {
            // Maybe this will cause flickering, might be better to just hide the object.
            HideInspector();
            currentSelection = null;
        }

        Item item;
        ItemInstance instance;
        if (slot.GetItem(out item)) {
            this.currentSelection = slot;

            if(slot.GetItemInstance(out instance)) {
                Debug.Log(instance.item.GetType());

                switch (instance.item.GetType().ToString()) {
                    case "Gem":
                        SceneManager.LoadScene("Cutting");
                        break;
                    case "Ore":
                        SceneManager.LoadScene("Smelting");
                        break;
                    case "Jewel":
                        SceneManager.LoadScene("Polishing");
                        break;
                    case "Brick":
                        SceneManager.LoadScene("Tracing");
                        break;
                }
            }
        }
    }

    private void ResourcePouchOpen(ItemInstance instance, Slot slot) {
        //ResourcePouchLoot loot = new ResourcePouchLoot();
        //float orePercentage = loot.CalculateDropRates();
        //float gemPercentage = 100f - orePercentage;

        // Hard coded for now.  To do this dynamically, maybe put <names,chances> in a dictionary<string, float>.
        float rubyChance = 0.4f,
              oreChance = 1.00f;
        int numberItems = Random.Range(1, 5);

        var drops = new List<ItemInstance>();
        for (int i = 0; i < numberItems; i++) {
            Item drop;
            float spin = Random.Range(0, 1f);
            if (spin < rubyChance) {
                drop = database.GetActual("Ruby");
                Debug.Log("Added a ruby.");
            } else if (spin < oreChance) {
                drop = database.GetActual("Ore");
                Debug.Log("Added some ore.");
            // To satisfy compiler.
            } else {
                drop = null;
            }

            drops.Add(new ItemInstance(drop, 1, Quality.QualityGrade.Junk));
        }

        Inventory inv = Inventory.Instance;
        slot.RemoveItem();
        inv.RemoveItem(slot.index);

        foreach (ItemInstance drop in drops) {
            int pos = inv.InsertItem(drop);
            Slot toSlot = physicalInventory.GetSlotAtIndex(pos);
            GameObject clone = Instantiate(drop.item.physicalRepresentation, slot.transform.position, slot.transform.rotation);

            // Kind of a placeholder animation.
            // TODO: randomize the Vector3.up a little so that the items separate when they go up.
            clone.transform.DOMove(clone.transform.position + Vector3.up, 0.7f).SetEase(Ease.OutBack)
                .OnComplete(() => clone.transform.DOMove(toSlot.transform.position + Vector3.up, 0.6f).SetEase(Ease.OutBack)
                .OnComplete(() => clone.transform.DOMove(toSlot.transform.position, 1f).SetEase(Ease.OutBounce)));

            toSlot.SetItemInstantiated(drop, clone);
        }

        /*
        //get a copy of inventory for adding to and checking
        //Inventory invInstance = Inventory.Instance;
        //Helper and variables
        //ResourcePouchLoot loot = new ResourcePouchLoot();
        //float orePercentage;
        //float gemPercentage;
        int numOre;
        int numGem;
        float numberItems;

        //Calculate drops and generate list of items
        orePercentage = loot.CalculateDropRates();
        gemPercentage = 100 - orePercentage;
        numberItems = loot.numberItems;
        //Debug.Log(orePercentage + " ore " + gemPercentage + " gem " + numberItems + " items ");
        numOre = loot.CalculateOreDrop(orePercentage, numberItems);
        numGem = loot.CalculateGemDrop(numOre, numberItems);
        List<ItemInstance> possibleDrops = new List<ItemInstance>();
        for (int i = 0; i < numOre; i++) {
            possibleDrops.Add(ore);
        }
        for (int i = 0; i < numGem; i++) {
            possibleDrops.Add(ruby);
        }
        */

        /*
        //Remove Pouch and replace with item
        slot.RemoveItem();
        invInstance.RemoveItem(slot.index);
        int random = UnityEngine.Random.Range(0, possibleDrops.Count);
        invInstance.InsertItem(possibleDrops[random]);
        slot.SetItem(possibleDrops[random]);
        possibleDrops.RemoveAt(random);

        if (possibleDrops.Count > 0) {
            for (int i = 0; i < possibleDrops.Count;) {
                int continueRandom = UnityEngine.Random.Range(0, possibleDrops.Count);
                if (invInstance.InsertItem(possibleDrops[continueRandom])) {
                    Debug.Log("Inserting item");
                    slot.SetItem(possibleDrops[continueRandom]);
                    possibleDrops.RemoveAt(continueRandom);
                }
                else {
                    i = possibleDrops.Count;
                }
            }
        }
        //save.SaveInventory();
        //inventoryPopulaterObject.GetComponent<InventoryPopulator>().PopulateInitial();
        //save.SaveInventory();
        */



    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(previousRay);
    }
}
